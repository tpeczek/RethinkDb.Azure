# RethinkDB bindings for Azure Functions

The RethinkDB bindings for Azure Functions supports trigger, input, and output bindings.

## Packages

The RethinkDB bindings for Azure Functions are provided in the [RethinkDb.Azure.WebJobs.Extensions](http://www.nuget.org/packages/RethinkDb.Azure.WebJobs.Extensions) NuGet package.

```
PM>  Install-Package RethinkDb.Azure.WebJobs.Extensions
```

## Trigger

The RethinkDB Trigger uses the [RethinkDB Changefeeds](https://rethinkdb.com/docs/changefeeds/) to listen for inserts, updates and deletes.

### Trigger - language-specific examples

#### Trigger - C# example
In [C# class libraries](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library), use the [`RethinkDbTrigger`](../api/Microsoft.Azure.WebJobs.RethinkDbTriggerAttribute.html) attribute.

The attribute's constructor takes the database name and table name. For information about those settings and other properties that you can configure, see [Trigger - configuration](#trigger---configuration).

The following example shows a [C# function](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library) that is invoked when there are inserts, updates or deletes in the specified database and table.

```cs
using Microsoft.Extensions.Logging;
using RethinkDb.Azure.WebJobs.Extensions.Model;
using Demo.RethinkDb.Azure.Functions.Model;

namespace Demo.RethinkDb.Azure.Functions
{
    public static class RethinkDbTriggeredFunction
    {
        [FunctionName("RethinkDbTriggeredFunction")]
        public static void Run([RethinkDbTrigger(
            databaseName: "ThreadStatsDb",
            tableName: "ThreadStats",
            HostnameSetting = "RethinkDbHostname")]DocumentChange change,
            ILogger log)
        {
            log.LogInformation(Document modified: " + change.GetNewValue());
        }

    }
}
```
### Trigger - configuration

The following table explains the binding configuration properties that you set in the *function.json* file and the [`RethinkDbTrigger`](../api/Microsoft.Azure.WebJobs.RethinkDbTriggerAttribute.html) attribute.

|function.json property | Attribute property |Description|
|---------|---------|----------------------|
|**type** || Must be set to `rethinkDBTrigger`. |
|**direction** || Must be set to `in`. This parameter is set automatically when you create the trigger in the Azure portal. |
|**name** || The variable name used in function code that represents the changed document. | 
|**hostnameSetting**|**HostnameSetting** | The name of an app setting that contains hostname or IP address of the RethinkDB server containing the database and table to monitor. |
|**portSetting**|**PortSetting** | The name of an app setting that contains TCP port of the RethinkDB server containing the database and table to monitor. |
|**databaseName**|**DatabaseName**  | The name of the RethinkDB database with the table to monitor for changes. |
|**tableName** |**TableName** | The name of the table being monitored. |
|**includeTypes** |**IncludeTypes** | The value indicating if change type field should be included. |
|**authorizationKeySetting** |**AuthorizationKeySetting** | The name of an app setting that contains authorization key to the RethinkDB server containing the database and table to monitor. |
|**userSetting** |**UserSetting** | The name of an app setting that contains user account to connect as to the RethinkDB server containing the database and table to monitor. |
|**passwordSetting** |**PasswordSetting** | The name of an app setting that contains user account password to connect as to the RethinkDB server containing the database and table to monitor. |
|**enableSslSetting** |**EnableSslSetting** | The name of an app setting that contains value indicating if SSL/TLS encryption should be enabled for connection to the RethinkDB server containing the database and table to monitor. The underlying driver (RethinkDb.Driver) requires a commercial license for SSL/TLS encryption. |
|**licenseToSetting** |**LicenseToSetting** | The name of an app setting that contains "license to" of underlying driver (RethinkDb.Driver) commercial license. |
|**licenseKeySetting** |**LicenseKeySetting** | The name of an app setting that contains "license key" of underlying driver (RethinkDb.Driver) commercial license. |

### Trigger - usage

By default the trigger doesn't indicate directly whether a document was inserted, updated or deleted, it just provides the old and new value. In order to determine what was the operation one must base logic on old and new value presence/absence:

- If new value is present and old value absent, the operation was an insert.
- If new value is absent and old value present, the operation was a delete.
- If both values are present, the operation was an update.

If this isn't satisfactory, the [`RethinkDbTrigger.IncludeTypes`](../api/Microsoft.Azure.WebJobs.RethinkDbTriggerAttribute.html#Microsoft_Azure_WebJobs_RethinkDbTriggerAttribute_IncludeTypes) property can be set to `true` which will result in [`DocumentChange.Type`](../api/RethinkDb.Azure.WebJobs.Extensions.Model.DocumentChange.html#RethinkDb_Azure_WebJobs_Extensions_Model_DocumentChange_Type) having a value indicating type of change.

## Input

The RethinkDB input binding uses the ReQL API to retrieve RethinkDB document and passes it to the input parameter of the function. The document identifer can be determined based on the trigger that invokes the function. 

### Input - language-specific examples

#### Input - C# examples
In [C# class libraries](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library), use the [`RethinkDb`](../api/Microsoft.Azure.WebJobs.RethinkDbAttribute.html) attribute.

The attribute's constructor takes the database name and table name. For information about those settings and other properties that you can configure, see [Input - configuration](#input---configuration).

When the function exits successfully, any changes made to the input document via named input parameters are automatically persisted.

This section contains the following examples:
- [HTTP trigger, look up ID from query string](#http-trigger-look-up-id-from-query-string)
- [HTTP trigger, look up ID from route data](#http-trigger-look-up-id-from-route-data)

##### HTTP trigger, look up ID from query string
The following example shows a [C# function](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library) that retrieves a single document. The function is triggered by an HTTP request that uses a query string to specify the ID to look up.

```cs
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Demo.RethinkDb.Azure.Functions.Model;

namespace Demo.RethinkDb.Azure.Functions
{
    public static class RethinkDbInputFunctions
    {
        [FunctionName("QueryByIdFromQueryString")]
        public static IActionResult QueryByIdFromQueryString(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")]HttpRequest request,
            [RethinkDb(
                databaseName: "Demo_AspNetCore_Changefeed_RethinkDB",
                tableName: "ThreadStats",
                HostnameSetting = "RethinkDbHostname",
                Id = "{Query.id}")] ThreadStats threadStats,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (threadStats == null)
            {
                log.LogInformation($"Thread stats not found (Query['id']: {request.Query["id"]}).");
                return new NotFoundResult();
            }

            log.LogInformation($"Thread stats found (Query['id']: {request.Query["id"]}).");
            return new ObjectResult(threadStats);
        }
    }
}
```

##### HTTP trigger, look up ID from route data
The following example shows a [C# function](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library) that retrieves a single document. The function is triggered by an HTTP request that uses route data to specify the ID to look up.

```cs
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Demo.RethinkDb.Azure.Functions.Model;

namespace Demo.RethinkDb.Azure.Functions
{
    public static class RethinkDbInputFunctions
    {
        [FunctionName("QueryByIdFromRouteData")]
        public static IActionResult QueryByIdFromRouteData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "QueryByIdFromRouteData/{id}")]HttpRequest request,
            [RethinkDb(
                databaseName: "Demo_AspNetCore_Changefeed_RethinkDB",
                tableName: "ThreadStats",
                HostnameSetting = "RethinkDbHostname",
                Id = "{id}")] ThreadStats threadStats,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (threadStats == null)
            {
                log.LogInformation($"Thread stats not found.");
                return new NotFoundResult();
            }

            log.LogInformation($"Thread stats found.");
            return new ObjectResult(threadStats);
        }
    }
}
```

### Input - configuration

The following table explains the binding configuration properties that you set in the *function.json* file and the [`RethinkDb`](../api/Microsoft.Azure.WebJobs.RethinkDbAttribute.html) attribute.

|function.json property | Attribute property |Description|
|---------|---------|----------------------|
|**type** || Must be set to `rethinkDB`. |
|**direction** || Must be set to `out`. |
|**name** || Name of the binding parameter that represents the document in the function. | 
|**hostnameSetting**|**HostnameSetting** | The name of an app setting that contains hostname or IP address of the RethinkDB server containing the database and table containing the document. |
|**portSetting**|**PortSetting** | The name of an app setting that contains TCP port of the RethinkDB server containing the database and table containing the document. |
|**databaseName**|**DatabaseName**  | The name of the RethinkDB database with the table containing the document. |
|**tableName** |**TableName** | The name of the table containing the document. |
|**id** |**Id** | The ID of the document to retrieve. This property supports binding expressions. |
|**authorizationKeySetting** |**AuthorizationKeySetting** | The name of an app setting that contains authorization key to the RethinkDB server containing the database and table containing the document. |
|**userSetting** |**UserSetting** | The name of an app setting that contains user account to connect as to the RethinkDB server containing the database and table containing the document. |
|**passwordSetting** |**PasswordSetting** | The name of an app setting that contains user account password to connect as to the RethinkDB server containing the database and table containing the document. |
|**enableSslSetting** |**EnableSslSetting** | The name of an app setting that contains value indicating if SSL/TLS encryption should be enabled for connection to the RethinkDB server containing the database and table containing the document. The underlying driver (RethinkDb.Driver) requires a commercial license for SSL/TLS encryption. |
|**licenseToSetting** |**LicenseToSetting** | The name of an app setting that contains "license to" of underlying driver (RethinkDb.Driver) commercial license. |
|**licenseKeySetting** |**LicenseKeySetting** | The name of an app setting that contains "license key" of underlying driver (RethinkDb.Driver) commercial license. |

## Output
The RethinkDB output binding lets you write a new document to a RethinkDB database using the ReQL API.

### Output - language-specific examples

#### Output - C# examples
In [C# class libraries](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library), use the [`RethinkDb`](../api/Microsoft.Azure.WebJobs.RethinkDbAttribute.html) attribute.

The attribute's constructor takes the database name and table name. For information about those settings and other properties that you can configure, see [Output - configuration](#output---configuration).

This section contains the following examples:
- [HTTP trigger, write single document](#http-trigger-write-single-document)
- [HTTP trigger, write multiple documents using IAsyncCollector](#http-trigger-write-multiple-documents-using-iasynccollector)

##### HTTP trigger, write single document
The following example shows a [C# function](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library) that adds a document to a database by using an output parameter.

```cs
using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Demo.RethinkDb.Azure.Functions
{
    public static class RethinkDbOutputFunctions
    {
        [FunctionName("OutputSingleDoc")]
        public static IActionResult OutputSingleDoc(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequest request,
            [RethinkDb(
                databaseName: "Demo_AspNetCore_Changefeed_RethinkDB",
                tableName: "ThreadStats",
                HostnameSetting = "RethinkDbHostname")] out dynamic document,
            ILogger log)
        {
            Guid id = Guid.NewGuid();
            ThreadPool.GetAvailableThreads(out var workerThreads, out var _);
            ThreadPool.GetMinThreads(out var minThreads, out var _);
            ThreadPool.GetMaxThreads(out var maxThreads, out var _);

            document = new
            {
                id,
                WorkerThreads = workerThreads,
                MinThreads = minThreads,
                MaxThreads = maxThreads,
                _source = nameof(RethinkDbOutputFunctions) + "." + nameof(OutputSingleDoc)
            };

            log.LogInformation("C# HTTP trigger function inserted single document");

            return new ObjectResult(document);
        }
    }
}
```

##### HTTP trigger, write multiple documents using IAsyncCollector
The following example shows a [C# function](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library) that adds a collection of documents to a database by using an `IAsyncCollector`.

```cs
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Demo.RethinkDb.Azure.Functions.Model;

namespace Demo.RethinkDb.Azure.Functions
{
    public static class RethinkDbOutputFunctions
    {

        [FunctionName("OutputMultipleDocs")]
        public static async Task<IActionResult> OutputMultipleDocs(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequest request,
            [RethinkDb(
                databaseName: "Demo_AspNetCore_Changefeed_RethinkDB",
                tableName: "ThreadStats",
                HostnameSetting = "RethinkDbHostname")] IAsyncCollector<ThreadStats> threadStatsCollector,
            ILogger log)
        {
            Int32.TryParse(request.Query["count"], out int count);
            Int32.TryParse(request.Query["delay"], out int delay);

            for (int i = 0; i < count; i++)
            {
                ThreadPool.GetAvailableThreads(out var workerThreads, out var _);
                ThreadPool.GetMinThreads(out var minThreads, out var _);
                ThreadPool.GetMaxThreads(out var maxThreads, out var _);

                await threadStatsCollector.AddAsync(new ThreadStats
                {
                    WorkerThreads = workerThreads,
                    MinThreads = minThreads,
                    MaxThreads = maxThreads
                });

                await Task.Delay(delay);
            }

            log.LogInformation($"C# HTTP trigger function inserted {count} documents");

            return new OkResult();
        }
    }
}
```

### Output - configuration

The following table explains the binding configuration properties that you set in the *function.json* file and the [`RethinkDb`](../api/Microsoft.Azure.WebJobs.RethinkDbAttribute.html) attribute.

|function.json property | Attribute property |Description|
|---------|---------|----------------------|
|**type** || Must be set to `rethinkDB`. |
|**direction** || Must be set to `out`. |
|**name** || Name of the binding parameter that represents the document in the function. | 
|**hostnameSetting**|**HostnameSetting** | The name of an app setting that contains hostname or IP address of the RethinkDB server containing the database and table containing the document. |
|**portSetting**|**PortSetting** | The name of an app setting that contains TCP port of the RethinkDB server containing the database and table containing the document. |
|**databaseName**|**DatabaseName**  | The name of the RethinkDB database with the table containing the document. |
|**tableName** |**TableName** | The name of the table containing the document. |
]|**createIfNotExists** |**CreateIfNotExists** | A boolean value to indicate whether the table is created when it doesn't exist. |
|**authorizationKeySetting** |**AuthorizationKeySetting** | The name of an app setting that contains authorization key to the RethinkDB server containing the database and table containing the document. |
|**userSetting** |**UserSetting** | The name of an app setting that contains user account to connect as to the RethinkDB server containing the database and table containing the document. |
|**passwordSetting** |**PasswordSetting** | The name of an app setting that contains user account password to connect as to the RethinkDB server containing the database and table containing the document. |
|**enableSslSetting** |**EnableSslSetting** | The name of an app setting that contains value indicating if SSL/TLS encryption should be enabled for connection to the RethinkDB server containing the database and table containing the document. The underlying driver (RethinkDb.Driver) requires a commercial license for SSL/TLS encryption. |
|**licenseToSetting** |**LicenseToSetting** | The name of an app setting that contains "license to" of underlying driver (RethinkDb.Driver) commercial license. |
|**licenseKeySetting** |**LicenseKeySetting** | The name of an app setting that contains "license key" of underlying driver (RethinkDb.Driver) commercial license. |

### Output - usage
By default, when you write to the output parameter (or use `IAsyncCollector.AddAsync`) in your function, a document is created in your database. This document has an automatically generated identifier. You can specify the document identifier by specifying the id property. When document has identifier, the performed opertion will not be an insert but an upsert.
