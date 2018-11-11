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
|**hostnameSetting**|**HostnameSetting** | The name of an app setting that contains hostname or IP address of the RethinkDB server containing the database and table to monitor.. |
|**databaseName**|**DatabaseName**  | The name of the RethinkDB database with the table to monitor for changes. |
|**tableName** |**TableName** | The name of the table being monitored. |
|**includeTypes** |**IncludeTypes** | The value indicating if change type field should be included. |

### Trigger - usage

By default the trigger doesn't indicate directly whether a document was inserted, updated or deleted, it just provides the old and new value. In order to determine what was the operation one must base logic on old and new value presence/absence:

- If new value is present and old value absent, the operation was an insert.
- If new value is absent and old value present, the operation was a delete.
- If both values are present, the operation was an update.

If this isn't satisfactory, the [`RethinkDbTrigger.IncludeTypes`](../api/Microsoft.Azure.WebJobs.RethinkDbTriggerAttribute.html#Microsoft_Azure_WebJobs_RethinkDbTriggerAttribute_IncludeTypes) property can be set to `true` which will result in [`DocumentChange.Type`](../api/RethinkDb.Azure.WebJobs.Extensions.Model.DocumentChange.html#RethinkDb_Azure_WebJobs_Extensions_Model_DocumentChange_Type) having a value indicating type of change.