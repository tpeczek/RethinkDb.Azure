# Azure RethinkDB Architectures

Below architectures represent scenarios, which are available thanks to extensions provided by this project.

## Serverless web application with additional post-processing

This architecture shows a serverless web application. The application uses Azure Functions based API to store and retrieve documents from containerized RethinkDB, deployed to Azure Container Instances. Additionally, a second Azure Functions application is listening to RethinkDB changefeed and performs post-processing.

<center>
	<object type="image/svg+xml" data="../resources/svg/azure-rethinkdb-function-apps-architecture.svg" style="max-width:600px;max-height:310px" >
		Azure RethinkDB Function Apps Architecture
	</object>
<center>