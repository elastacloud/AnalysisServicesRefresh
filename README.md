# AnalysisServicesRefresh

C# .NET Framework console application to refresh an Azure Analysis Services model or [Power BI Premium dataset](https://docs.microsoft.com/en-us/power-bi/admin/service-premium-connect-tools) based on a JSON configuration, using the [Tabular Object Model](https://docs.microsoft.com/en-us/bi-reference/tom/introduction-to-the-tabular-object-model-tom-in-analysis-services-amo) API allowing for:
1. Full refresh of tables.
2. Partitioned refresh of tables, including creation, deletion and refresh of table partitions.
3. Authenticating against OAuth and UsernamePassword data sources.
4. Recaluating model after refresh.

The application can be executed standalone, or the JSON configuration structure allows for execution from Azure Data Factory v2 using a [Custom Activity](https://docs.microsoft.com/en-us/azure/data-factory/transform-data-using-dotnet-custom-activity).

## Transactional Processing

Setting "modelProcessorType" in the JSON configuration:
- To "Transactional": if a failure occurs during processing, no changes are committed to the model and an exception is thrown.
- To "NonTransactional": if a failure occurs during processing a table, the next table will be attempted until all are attempted. If any table fails, an exception is thrown after processing indicating which have failed. The data source is renewed for each table, allowing in the case of OAuth, for longer processing time before token timeout.

## Data Source

Setting "dataSourceType" in the JSON configuration:
- To "Passthrough": Ignore data source modification. Usage for data sources that are preconfigured, such as SQL Auth or PowerBI.
- To "OAuth": Usage for Azure SQL, such as Azure SQL Database, Azure SQL Managed Instance, etc. dataSourceUsername (client id) and dataSourcePassword (client secret) associated with an AD App will authenticate as the AD app's service principal.
- To "UsernamePassword": Usage for Username and Password authentication, such as SQL Auth, Databricks, etc. dataSourceUsername and dataSourcePassword are used for username and password.

## Key Vault

Setting "keyVaultBaseUri" in the JSON configuration:
- To null: analysisServicesClientId, analysisServicesClientSecret, dataSourceUsername and dataSourcePassword are used as entered.
- To "https://*.vault.azure.net/": analysisServicesClientId, analysisServicesClientSecret, dataSourceUsername and dataSourcePassword are retrieved from Azure Key Vault using secret names as entered.

Setting "keyVaultAuthenticationType" in the JSON configuration:
- To "Certificate": keyVaultAuthentication entered is the certificate thumbprint in My.CurrentUser associated with AD App for keyVaultClientId.
- To "ClientSecret": keyVaultAuthentication entered is the client secret associated with AD App for keyVaultClientId.

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

1. Example configuration using Key Vault via Active Directory certificate authentication (with OAuth data source):

```json
{
  "typeProperties": {
    "extendedProperties": {
      "analysisServicesClientId": "AnalysisServicesClientIdInKeyVault",
      "analysisServicesClientSecret": "AnalysisServicesClientSecretInKeyVault",
      "dataSourceName": "SQL/DataSourceServerInModel;DataSourceDatabaseInModel",
      "dataSourceType": "OAuth",
      "dataSourceUsername": "AnalysisServicesDataSourceClientIdInKeyVault",
      "dataSourcePassword": "AnalysisServicesDataSourceClientSecretInKeyVault",
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
	  "keyVaultAuthenticationType": "Certificate",
      "keyVaultBaseUri": "https://keyvault.vault.azure.net/",
      "keyVaultClientId": "ActiveDirectoryClientIdForKeyVault",
      "maxParallelism": 5,
      "modelProcessorType": "Transactional",
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

2. Example configuration using Key Vault via Active Directory client id and secret authentication (with OAuth data source):

```json
{
  "typeProperties": {
    "extendedProperties": {
      "analysisServicesClientId": "AnalysisServicesClientIdInKeyVault",
      "analysisServicesClientSecret": "AnalysisServicesClientSecretInKeyVault",
      "dataSourceName": "SQL/DataSourceServerInModel;DataSourceDatabaseInModel",
      "dataSourceType": "OAuth",
      "dataSourceUsername": "AnalysisServicesDataSourceClientIdInKeyVault",
      "dataSourcePassword": "AnalysisServicesDataSourceClientSecretInKeyVault",
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
	  "keyVaultAuthenticationType": "Secret",
      "keyVaultBaseUri": "https://keyvault.vault.azure.net/",
      "keyVaultClientId": "ActiveDirectoryClientIdForKeyVault",
      "maxParallelism": 5,
      "modelProcessorType": "Transactional",
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

3. Example configuration using Active Directory client id and secret authentication without Key Vault (with OAuth data source):

```json
{
  "typeProperties": {
    "extendedProperties": {
      "analysisServicesClientId": "AnalysisServicesClientId",
      "analysisServicesClientSecret": "AnalysisServicesClientSecret",
      "dataSourceName": "SQL/DataSourceServerInModel;DataSourceDatabaseInModel",
      "dataSourceType": "OAuth",
      "dataSourceUsername": "AnalysisServicesDataSourceClientId",
      "dataSourcePassword": "AnalysisServicesDataSourceClientSecret",
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
      "keyVaultAuthenticationType": "Secret",
      "keyVaultBaseUri": null,
      "keyVaultClientId": null,
      "maxParallelism": 5,
      "modelProcessorType": "Transactional",
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

4. Example configuration using Active Directory client id and secret and Passthrough data source authentication (with Passthrough data source):

```json
{
  "typeProperties": {
    "extendedProperties": {
      "analysisServicesClientId": "AnalysisServicesClientId",
      "analysisServicesClientSecret": "AnalysisServicesClientSecret",
      "dataSourceName": null,
      "dataSourceType": "Passthrough",
      "dataSourceUsername": null,
      "dataSourcePassword": null,
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
	  "keyVaultAuthenticationType": "Secret",
      "keyVaultBaseUri": null,
      "keyVaultClientId": null,
      "maxParallelism": 5,
      "modelProcessorType": "Transactional",
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