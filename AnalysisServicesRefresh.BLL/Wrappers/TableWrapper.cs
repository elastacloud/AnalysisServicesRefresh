﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.AnalysisServices.Tabular;

namespace AnalysisServicesRefresh.BLL.Wrappers
{
    [ExcludeFromCodeCoverage]
    internal class TableWrapper : ITableWrapper
    {
        private readonly Table _table;

        public TableWrapper(Table table)
        {
            _table = table;
        }

        public IPartitionCollectionWrapper Partitions => new PartitionCollectionWrapper(_table.Partitions);

        public void RequestRefresh(RefreshType type)
        {
            _table.RequestRefresh(type);
        }

        public string Name => _table.Name;
    }
}