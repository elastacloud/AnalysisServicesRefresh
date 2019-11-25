﻿using System.Collections.Generic;
using AnalysisServicesRefresh.BLL.Enums;
using AnalysisServicesRefresh.BLL.Models;

namespace AnalysisServicesRefresh.Models
{
    internal class ExtendedProperties
    {
        public string AnalysisServicesClientIdName { get; set; }
        public string AnalysisServicesClientSecretName { get; set; }
        public AuthenticationType AuthenticationType { get; set; }
        public string DataSourceClientIdName { get; set; }
        public string DataSourceClientSecretName { get; set; }
        public string DataSourceName { get; set; }
        public DataSourceType DataSourceType { get; set; }
        public string DatabaseName { get; set; }
        public string DirectoryId { get; set; }
        public List<FullTableConfiguration> FullTables { get; set; }
        public string KeyVaultAuthentication { get; set; }
        public string KeyVaultBaseUri { get; set; }
        public string KeyVaultClientId { get; set; }
        public List<PartitionedTableConfiguration> PartitionedTables { get; set; }
        public string ServerName { get; set; }
    }
}