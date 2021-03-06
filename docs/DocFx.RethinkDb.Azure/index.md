﻿# RethinkDb.Azure

Azure extensions for [RethinkDB](https://www.rethinkdb.com/).

## What's in here

Content available here originates from a lift and shift migration of existing project to Azure. As part of that migration RethinkDB database has been containerized and deployed to Azure Container Instances. This created a need for further integration of RethinkDB with Azure in order to make certain [architectures](articles/azure-rethinkdb-architectures.md) possible.

Currently you can find here:

- [Dockerfile](https://github.com/tpeczek/RethinkDb.Azure/tree/master/docker) for creating basic RethinkDB Docker image
- [RethinkDB bindings for Azure Functions](articles/azure-functions-bindings-rethinkdb.md)

## Demos

The demos projectsares available as part of solution on [GitHub](https://github.com/tpeczek/RethinkDb.Azure).

## Donating

My blog and open source projects are result of my passion for software development, but they require a fair amount of my personal time. If you got value from any of the content I create, then I would appreciate your support by [buying me a coffee](https://www.buymeacoffee.com/tpeczek).

<a href="https://www.buymeacoffee.com/tpeczek"><img src="https://www.buymeacoffee.com/assets/img/custom_images/black_img.png" style="height: 41px !important;width: 174px !important;box-shadow: 0px 3px 2px 0px rgba(190, 190, 190, 0.5) !important;-webkit-box-shadow: 0px 3px 2px 0px rgba(190, 190, 190, 0.5) !important;"  target="_blank"></a>