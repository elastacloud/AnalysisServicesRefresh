# AnalysisServicesRefresh

C# .NET Framework console application to refresh an Azure Analysis Services model based on a JSON configuration, using the [Tabular Object Model](https://docs.microsoft.com/en-us/bi-reference/tom/introduction-to-the-tabular-object-model-tom-in-analysis-services-amo) API allowing for:
1. Full refresh of tables.
2. Creation, deletion and refresh of table partitions.
3. Refreshing an OAuth SQL data source.
4. Recaluating model after refresh.

The application can be executed standalone, or the JSON configuration structure allows for execution from Azure Data Factory v2 using a [Custom Activity](https://docs.microsoft.com/en-us/azure/data-factory/transform-data-using-dotnet-custom-activity).

## Full Refresh

Any tables listed in "fullTables" in the JSON configuration will be fully processed.

## Partitioned Refresh

Any table listed in "partitionedTables" in the JSON configuration processed as follows:
- Any partitions defined in the table configuration that is not defined on the server will be created.
- Any partitions defined on the server that is not defined in the table configuration will be deleted.
- Any partitions marked "refresh": 1 will be fully processed.

A partitioned refresh will duplicate and rename a template M query partition named *Template* for each new partition. The template partition must have *-1* and *-2* placeholders, which will be replaced with the partition's Minimum and Maximum from the JSON configuration respectively.

```
let
    Source = #"SQL/DataSourceServerInModel;DataSourceDatabaseInModel",
    #"SourceTable" = Source{[Schema="schema",Item="SourceTable"]}[Data],
    #"Filtered Rows" = Table.SelectRows(#"SourceTable", each [DateKey] >= -1 and [DateKey] <= -2 )
in
    #"Filtered Rows"
```

*Partitions named DevPartition will be ignored for development purposes.*

## Example Configurations

1. Example configuration using Key Vault via Active Directory certificate and OAuth data source authentication:

```json
{
  "typeProperties": {
    "extendedProperties": {
      "authenticationType": "Certificate",
      "analysisServicesClientIdName": "AnalysisServicesClientIdInKeyVault",
      "analysisServicesClientSecretName": "AnalysisServicesClientSecretInKeyVault",
      "dataSourceName": "SQL/DataSourceServerInModel;DataSourceDatabaseInModel",
      "dataSourceType": "OAuth",
      "dataSourceClientIdName": "AnalysisServicesDataSourceClientIdInKeyVault",
      "dataSourceClientSecretName": "AnalysisServicesDataSourceClientSecretInKeyVault",
      "databaseName": "AnalysisServicesModelName",
      "directoryId": "ActiveDirectoryDirectoryId",
      "fullTables": [
        {
          "name": "D Calendar"
        },
        {
          "name": "D Item"
        }
      ],
      "keyVaultAuthentication": "ActiveDirectoryClientCertificateThumbprintForKeyVault",
      "keyVaultBaseUri": "https://keyvault.vault.azure.net/",
      "keyVaultClientId": "ActiveDirectoryClientIdForKeyVault",
      "partitionedTables": [
        {
          "name": "F Stock",
          "partitions": [
            {
              "name": "W201827",
              "minimum": 20180729,
              "maximum": 20180804,
              "refresh": 0
            },
            {
              "name": "W201828",
              "minimum": 20180805,
              "maximum": 20180811,
              "refresh": 1
            },
            {
              "name": "W201829",
              "minimum": 20180812,
              "maximum": 20180818,
              "refresh": 1
            }
          ]
        },
        {
          "name": "F Sales",
          "partitions": [
            {
              "name": "W201827",
              "minimum": 20180729,
              "maximum": 20180804,
              "refresh": 0
            },
            {
              "name": "W201828",
              "minimum": 20180805,
              "maximum": 20180811,
              "refresh": 1
            },
            {
              "name": "W201829",
              "minimum": 20180812,
              "maximum": 20180818,
              "refresh": 1
            }
          ]
        }
      ],
      "serverName": "asazure://AnalysisServicesServerName"
    }
  }
}
```

2. Example configuration using Key Vault via Active Directory client id and secret and OAuth data source authentication:

```json
{
  "typeProperties": {
    "extendedProperties": {
      "authenticationType": "Secret",
      "analysisServicesClientIdName": "AnalysisServicesClientIdInKeyVault",
      "analysisServicesClientSecretName": "AnalysisServicesClientSecretInKeyVault",
      "dataSourceName": "SQL/DataSourceServerInModel;DataSourceDatabaseInModel",
      "dataSourceType": "OAuth",
      "dataSourceClientIdName": "AnalysisServicesDataSourceClientIdInKeyVault",
      "dataSourceClientSecretName": "AnalysisServicesDataSourceClientSecretInKeyVault",
      "databaseName": "AnalysisServicesModelName",
      "directoryId": "ActiveDirectoryDirectoryId",
      "fullTables": [
        {
          "name": "D Calendar"
        },
        {
          "name": "D Item"
        }
      ],
      "keyVaultAuthentication": "ActiveDirectoryClientSecretForKeyVault",
      "keyVaultBaseUri": "https://keyvault.vault.azure.net/",
      "keyVaultClientId": "ActiveDirectoryClientIdForKeyVault",
      "partitionedTables": [
        {
          "name": "F Stock",
          "partitions": [
            {
              "name": "W201827",
              "minimum": 20180729,
              "maximum": 20180804,
              "refresh": 0
            },
            {
              "name": "W201828",
              "minimum": 20180805,
              "maximum": 20180811,
              "refresh": 1
            },
            {
              "name": "W201829",
              "minimum": 20180812,
              "maximum": 20180818,
              "refresh": 1
            }
          ]
        },
        {
          "name": "F Sales",
          "partitions": [
            {
              "name": "W201827",
              "minimum": 20180729,
              "maximum": 20180804,
              "refresh": 0
            },
            {
              "name": "W201828",
              "minimum": 20180805,
              "maximum": 20180811,
              "refresh": 1
            },
            {
              "name": "W201829",
              "minimum": 20180812,
              "maximum": 20180818,
              "refresh": 1
            }
          ]
        }
      ],
      "serverName": "asazure://AnalysisServicesServerName"
    }
  }
}
```

3. Example configuration using Active Directory client id and secret and OAuth data source authentication:

```json
{
  "typeProperties": {
    "extendedProperties": {
      "authenticationType": "Secret",
      "analysisServicesClientIdName": "AnalysisServicesClientId",
      "analysisServicesClientSecretName": "AnalysisServicesClientSecret",
      "dataSourceName": "SQL/DataSourceServerInModel;DataSourceDatabaseInModel",
      "dataSourceType": "OAuth",
      "dataSourceClientIdName": "AnalysisServicesDataSourceClientId",
      "dataSourceClientSecretName": "AnalysisServicesDataSourceClientSecret",
      "databaseName": "AnalysisServicesModelName",
      "directoryId": "ActiveDirectoryDirectoryId",
      "fullTables": [
        {
          "name": "D Calendar"
        },
        {
          "name": "D Item"
        }
      ],
      "keyVaultAuthentication": null,
      "keyVaultBaseUri": null,
      "keyVaultClientId": null,
      "partitionedTables": [
        {
          "name": "F Stock",
          "partitions": [
            {
              "name": "W201827",
              "minimum": 20180729,
              "maximum": 20180804,
              "refresh": 0
            },
            {
              "name": "W201828",
              "minimum": 20180805,
              "maximum": 20180811,
              "refresh": 1
            },
            {
              "name": "W201829",
              "minimum": 20180812,
              "maximum": 20180818,
              "refresh": 1
            }
          ]
        },
        {
          "name": "F Sales",
          "partitions": [
            {
              "name": "W201827",
              "minimum": 20180729,
              "maximum": 20180804,
              "refresh": 0
            },
            {
              "name": "W201828",
              "minimum": 20180805,
              "maximum": 20180811,
              "refresh": 1
            },
            {
              "name": "W201829",
              "minimum": 20180812,
              "maximum": 20180818,
              "refresh": 1
            }
          ]
        }
      ],
      "serverName": "asazure://AnalysisServicesServerName"
    }
  }
}
```

4. Example configuration using Active Directory client id and secret and SQL data source authentication:

```json
{
  "typeProperties": {
    "extendedProperties": {
      "authenticationType": "Secret",
      "analysisServicesClientIdName": "AnalysisServicesClientId",
      "analysisServicesClientSecretName": "AnalysisServicesClientSecret",
      "dataSourceName": null,
      "dataSourceType": "Passthrough",
      "dataSourceClientIdName": null,
      "dataSourceClientSecretName": null,
      "databaseName": "AnalysisServicesModelName",
      "directoryId": "ActiveDirectoryDirectoryId",
      "fullTables": [
        {
          "name": "D Calendar"
        },
        {
          "name": "D Item"
        }
      ],
      "keyVaultAuthentication": null,
      "keyVaultBaseUri": null,
      "keyVaultClientId": null,
      "partitionedTables": [
        {
          "name": "F Stock",
          "partitions": [
            {
              "name": "W201827",
              "minimum": 20180729,
              "maximum": 20180804,
              "refresh": 0
            },
            {
              "name": "W201828",
              "minimum": 20180805,
              "maximum": 20180811,
              "refresh": 1
            },
            {
              "name": "W201829",
              "minimum": 20180812,
              "maximum": 20180818,
              "refresh": 1
            }
          ]
        },
        {
          "name": "F Sales",
          "partitions": [
            {
              "name": "W201827",
              "minimum": 20180729,
              "maximum": 20180804,
              "refresh": 0
            },
            {
              "name": "W201828",
              "minimum": 20180805,
              "maximum": 20180811,
              "refresh": 1
            },
            {
              "name": "W201829",
              "minimum": 20180812,
              "maximum": 20180818,
              "refresh": 1
            }
          ]
        }
      ],
      "serverName": "asazure://AnalysisServicesServerName"
    }
  }
}
```

## Getting Started
### Requirements
- [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472)
- [Analysis Services MSOLAP (x86) Redistributable](https://docs.microsoft.com/en-us/azure/analysis-services/analysis-services-data-providers)

### Run
Create an "activity.json" configuration file using the templates provided above in the same directory as the executable and run `AnalysisServicesRefresh.exe`.

## Build and Test
Created using Visual Studio 2019. 
Unit tests are implemented using MSTest.

## Contribute
Please feel free to raise any issues or pull requests to help improve this project.