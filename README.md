# RethinkDb.Azure
[![NuGet Version](https://img.shields.io/nuget/v/RethinkDb.Azure.WebJobs.Extensions?label=RethinkDb.Azure.WebJobs.Extensions&logo=nuget)](https://www.nuget.org/packages/RethinkDb.Azure.WebJobs.Extensions/)
[![NuGet Version](https://img.shields.io/nuget/v/RethinkDb.Azure.Functions.Worker.Extensions?label=RethinkDb.Azure.Functions.Worker.Extensions&logo=nuget)](https://www.nuget.org/packages/RethinkDb.Azure.Functions.Worker.Extensions/)

Azure extensions for [RethinkDB](https://www.rethinkdb.com/).

## RethinkDB Trigger and Bindings for Azure Functions

The RethinkDB extension for Azure Functions supports trigger, input and output bindings.

The extension NuGet package you need to install depends on both the runtime version and C# execution mode you're using in your function app.

### Functions 2.x+  In-Process Model

Install the [RethinkDb.Azure.WebJobs.Extensions](https://www.nuget.org/packages/RethinkDb.Azure.WebJobs.Extensions) NuGet package.

```
>  dotnet add package RethinkDb.Azure.WebJobs.Extensions
```

### Functions 2.x+  Isolated Worker Model

Install the [RethinkDb.Azure.Functions.Worker.Extensions](https://www.nuget.org/packages/RethinkDb.Azure.Functions.Worker.Extensions) NuGet package.

```
>  dotnet add package RethinkDb.Azure.Functions.Worker.Extensions
```

## Documentation

The documentation is available [here](https://tpeczek.github.io/RethinkDb.Azure/).

## Demos

The project repository contains the following demos:
- [In-Process Azure Functions](https://github.com/tpeczek/RethinkDb.Azure/tree/main/samples/Demo.RethinkDb.Azure.Functions)

## Donating

My blog and open source projects are result of my passion for software development, but they require a fair amount of my personal time. If you got value from any of the content I create, then I would appreciate your support by [sponsoring me](https://github.com/sponsors/tpeczek) (either monthly or one-time).

## Copyright and License

Copyright © 2018 - 2024 Tomasz Pęczek

Licensed under the [MIT License](https://github.com/tpeczek/RethinkDb.Azure/blob/master/LICENSE.md)