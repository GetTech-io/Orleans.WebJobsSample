# Microsoft Orleans + Azure WebJob Sample

[Microsoft Orleans](https://github.com/dotnet/orleans) is a cross-platform framework for building robust, scalable distributed applications.  
Orleans builds on the developer productivity of .NET and brings it to the world of distributed applications, such as cloud services. Orleans scales from a single on-premises server to globally distributed, highly-available applications in the cloud.

[Azure WebJob](https://docs.microsoft.com/en-us/azure/app-service/webjobs-create) is a feature of [Azure App Service](https://azure.microsoft.com/en-us/services/app-service/) that enables you to run a program or script in the same context as a web app, API app, or mobile app. There is no additional cost to use WebJobs.

The [Azure WebJobs SDK](https://github.com/Azure/azure-webjobs-sdk) is a framework that simplifies the task of writing background processing code that runs in Azure. The Azure WebJobs SDK includes a declarative binding and trigger system that works with Azure Storage Blobs, Queues and Tables as well as Service Bus. The binding system makes it incredibly easy to write code that reads or writes Azure Storage objects. The trigger system automatically invokes a function in your code whenever any new data is received in a queue or blob.

Also Azure WebJobs SDK is the base building block of [Azure Functions](https://docs.microsoft.com/en-us/azure/azure-functions/functions-overview), but the hosting and scalablity are managed by Azure

## This sample is made for [this blog post]()

This sample want to demonstrate how you can integrate Orleans and WebJob. Luckly WebJob sdk 3.x and Orleans 3.x support .Net Core generic Host (Microsoft.Extensions.Hosting) and you can Co-Host these two services in the same process very easily. Of course WebJob can be hosted in different process or service if you design requires.  

The point is that WebJob can extend Orleans with very powerful Azure integration like triggers for Timer, Storage Blob and Queue, CosmosDB, etc.


# Prerequisite

- [.Net Core 3.1](https://www.microsoft.com/net/download/dotnet-core/3.1)
- [Visual Studio 2019 16.4](https://visualstudio.microsoft.com/)
- [Windows Azure Storage Emulator](https://docs.microsoft.com/hu-hu/azure/storage/common/storage-use-emulator) by default it is installed by Visual Studio Azure Tools

## Build

Run `dotnet build` to build the Orleans.WebJobsSample.Server project.

## Run tests

- Run  `C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator>AzureStorageEmulator.exe start` or start from start menu
- Run `dotnet run"` in the Orleans.WebJobsSample.Server project directory or start from Visual Studio.

## Thanks

[.Net Boxed Orleans Template](https://github.com/Dotnet-Boxed/Templates)
