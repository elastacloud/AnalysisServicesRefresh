using AnalysisServicesRefresh.BLL.Factories;
using AnalysisServicesRefresh.BLL.Interfaces;
using AnalysisServicesRefresh.BLL.Models;
using Microsoft.AnalysisServices.Tabular;
using NLog;
using System;
using System.Linq;

namespace AnalysisServicesRefresh.BLL.BLL
{
    public class PartitionedRefresh : IRefresh
    {
        private const string TemplatePartitionName = "Template";
        private const string DevPartition = "DevPartition";
        private readonly ILogger _logger;
        private readonly IPartitionWrapperFactory _partitionFactory;
        private ITableWrapper _table;

        public PartitionedRefresh(PartitionedTableConfiguration partitionedTable)
            : this(partitionedTable, new PartitionWrapperFactory(), LogManager.GetCurrentClassLogger())
        {
        }

        public PartitionedRefresh(PartitionedTableConfiguration partitionedTable,
            IPartitionWrapperFactory partitionFactory,
            ILogger logger)
        {
            _partitionFactory = partitionFactory;
            _logger = logger;
            PartitionedTable = partitionedTable;
        }

        public PartitionedTableConfiguration PartitionedTable { get; }

        public void Refresh(ITableWrapper table)
        {
            _table = table;

            _logger.Info($"Requesting partitioned refresh for table {table.Name}.");

            CheckPartitionOverlaps();
            CheckExistingPartitionDefinitions();
            RemoveOldPartitions();
            AddNewPartitions();
            RefreshPartitions();
        }

        private void CheckPartitionOverlaps()
        {
            var ordered = PartitionedTable.Partitions.OrderBy(x => x.Minimum);
            var zip = ordered
                .Zip(ordered.Skip(1), (c, n) => new
                {
                    Current = c,
                    Next = n
                });

            if (zip.Any(x => x.Current.Maximum >= x.Next.Minimum))
            {
                throw new ArgumentException("Partitions contain overlapping bounds.");
            }
        }

        private void CheckExistingPartitionDefinitions()
        {
            var template = _table.Partitions.FirstOrDefault(x => x.Name == TemplatePartitionName);

            if (template == null)
            {
                throw new ArgumentException(
                    $"Table {_table.Name} is missing {TemplatePartitionName} partition.");
            }

            var templateQuery = GetPartitionQuery(template);

            var sourcePartitions = PartitionedTable.Partitions.Select(x => new
            {
                x.Name,
                Query = templateQuery
                    .Replace("-1", x.Minimum.ToString())
                    .Replace("-2", x.Maximum.ToString())
            });

            var destinationPartitions = _table.Partitions
                .Where(x => x.Name != TemplatePartitionName && x.Name != DevPartition)
                .Select(x => new
                {
                    x.Name,
                    Query = GetPartitionQuery(x)
                });

            var changedPartitions = (from sp in sourcePartitions
                                     join dp in destinationPartitions
                                         on sp.Name equals dp.Name
                                     where sp.Query != dp.Query
                                     select sp.Name).ToList();

            if (changedPartitions.Any())
            {
                throw new ArgumentException(
                    $"Partition query definitions changed for existing partitions: {string.Join(", ", changedPartitions)}");
            }
        }

        private void RemoveOldPartitions()
        {
            var oldPartitions =
                (from dp in _table.Partitions.Where(x => x.Name != TemplatePartitionName && x.Name != DevPartition)
                 join sp in PartitionedTable.Partitions
                     on dp.Name equals sp.Name into lj
                 from sp in lj.DefaultIfEmpty()
                 where sp == null
                 orderby dp.Name
                 select dp).ToList();

            oldPartitions.ForEach(x => _table.Partitions.Remove(x));
            _logger.Info($"Removing partitions: {string.Join(", ", oldPartitions.Select(x => x.Name))}.");
        }

        private void AddNewPartitions()
        {
            var template = _table.Partitions.Find(TemplatePartitionName);

            var newPartitions = (from sp in PartitionedTable.Partitions
                                 join dp in _table.Partitions.Where(x => x.Name != TemplatePartitionName && x.Name != DevPartition)
                                     on sp.Name equals dp.Name into lj
                                 from dp in lj.DefaultIfEmpty()
                                 where dp == null
                                 orderby sp.Name
                                 select sp).ToList();

            foreach (var p in newPartitions)
            {
                var partition = _partitionFactory.Create();
                template.CopyTo(partition);

                partition.Name = p.Name;

                switch (partition.Source)
                {
                    case MPartitionSource mPartitionSource:
                        mPartitionSource.Expression =
                            mPartitionSource.Expression
                                .Replace("-1", p.Minimum.ToString())
                                .Replace("-2", p.Maximum.ToString());
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"Partition {partition.Name} in {_table.Name} is an unknown partition type.");
                }

                _table.Partitions.Add(partition);
            }

            _logger.Info($"Adding partitions: {string.Join(", ", newPartitions.Select(x => x.Name))}.");
        }

        private void RefreshPartitions()
        {
            var partitions =
                (from dp in _table.Partitions
                 join sp in PartitionedTable.Partitions
                     on dp.Name equals sp.Name
                 where sp.Refresh
                 orderby dp.Name
                 select dp).ToList();

            partitions.ForEach(x => x.RequestRefresh(RefreshType.Full));
            _logger.Info($"Refreshing partitions: {string.Join(", ", partitions.Select(x => x.Name))}.");
        }

        private string GetPartitionQuery(IPartitionWrapper partition)
        {
            switch (partition.Source)
            {
                case MPartitionSource mPartitionSource:
                    return mPartitionSource.Expression;
                default:
                    throw new InvalidOperationException(
                        $"Partition {partition.Name} in {_table.Name} is an unknown partition type.");
            }
        }
    }
}