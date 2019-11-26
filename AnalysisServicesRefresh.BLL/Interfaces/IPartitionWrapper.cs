﻿using Microsoft.AnalysisServices.Tabular;

namespace AnalysisServicesRefresh.BLL.Interfaces
{
    public interface IPartitionWrapper
    {
        string Name { get; set; }
        PartitionSource Source { get; set; }
        Partition Partition { get; }
        void RequestRefresh(RefreshType type);
        void CopyTo(IPartitionWrapper partition);
    }
}