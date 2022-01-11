# RethinkDb.Azure
[![NuGet version](https://badge.fury.io/nu/RethinkDb.Azure.WebJobs.Extensions.svg)](http://badge.fury.io/nu/RethinkDb.Azure.WebJobs.Extensions)

Azure extensions for [RethinkDB](https://www.rethinkdb.com/).

## What's in here

Content available here originates from a lift and shift migration of existing project to Azure. As part of that migration RethinkDB database has been containerized and deployed to Azure Container Instances. This created a need for further integration of RethinkDB with Azure in order to make certain architectures possible.

Currently you can find here:

- [Dockerfile](https://github.com/tpeczek/RethinkDb.Azure/tree/master/docker) for creating basic RethinkDB Docker image
- RethinkDB bindings for Azure Functions

## Documentation

The documentation is available [here](https://tpeczek.github.io/RethinkDb.Azure/).

## Demos

The demos projects are available as part of solution.

## Donating

My blog and open source projects are result of my passion for software development, but they require a fair amount of my personal time. If you got value from any of the content I create, then I would appreciate your support by [sponsoring me](https://github.com/sponsors/tpeczek) (either monthly or one-time).

## Copyright and License

Copyright © 2018 - 2022 Tomasz Pęczek

Licensed under the [MIT License](https://github.com/tpeczek/RethinkDb.Azure/blob/master/LICENSE.md)