# Architecture Overview

The following diagram shows the key architectural elements of the solution and some of the relationships between them. This page provides an overview of each of the elements and is intended to provide a high-level view of the architecture; other pages will describe the elements in detail.

![Architecture overview][architectureoverview]

Note: This description assumes a "real-world" deployment to Microsoft Azure. For information about alternative test deployments on a local workstation, see the page "[Getting Started with the Reference Implementation][gettingstartedpage]."

## Cars.Simulator.WorkerRole

The solution includes the **Cars.Simulator.WorkerRole** worker role that acts as an event producer to enable you to generate a stream of events that are sent to your Event Hub from a set of simulated devices (cars). You can configure the simulation using parameters such as the number of simulated devices to use and the duration of the simulation. You can also select a simulation scenario that determines what types of event the simulator generates, such long-running or malformed events. The **Cars.Dispatcher.WorkerRole** processor delivers different event types to different handlers.

Each car simulator instance uses a unique partition id when it sends events to the Event Hub. Partition ids do not map directly to partitions in Event Hub. You can have many more partition ids than partitions; Event Hub uses a hash of the partition id to determine which partition should process events with a particular partition id.

## Event Hub

Microsoft Azure Service Bus Event Hub is an event ingestor service that provides event and telemetry ingress to the cloud at massive scale, with low latency and high reliability. In this solution, the **Cars.Simulator.WorkerRole**  worker role instances are event producers that generate events and deliver them to the Event Hub service. This solution includes two downstream services that act as event consumers: the **Cars.Dispatcher.WorkerRole** worker role and the **ColdStorage.WorkerRole** worker role that process the events ingested by the Event Hub service.

When you configure the Event Hub service, you specify a namespace and the number of partitions to use. For more information about how this solution uses Event Hub, see the page [Event Hub][eventhubpage].

## Cars.Dispatcher.WorkerRole

**Cars.Dispatcher.WorkerRole** worker role instances are event consumers that receive events from the Event Hub service. Within the worker role instances at any given time there is a single **EventProcessor** instance active for each partition in the Event Hub. A reader examines each incoming event to determine which handler(s) it should invoke to process the event. If it cannot process an event, it stores it in a blob in the **Dispatcher.PoisonMessages** storage account. If it does not process the event within the expected time period, then the performance counter that measures timeouts is incremented. However, an event that times out may still be processed successfully.

The **Cars.Dispatcher.WorkerRole** worker role instances use the **Dispatcher** consumer group to receive messages from the Event Hub service.

## Handlers

The solution includes four sample handlers for the events that the **Cars.Dispatcher.WorkerRole** worker role instances receive from the Event Hub.

- The **LongRunningHandler** deliberately waits when it is invoked to simulate a slow downstream service.
- The **ThrowsExceptionHandler** deliberately throws an exception in order to test how the solution handles this scenario.
- The **UpdateEngineNotificationHandler** deserializes the event message, but does nothing with it in this sample implementation. This simulates a handler that is functioning without errors.
- The **UpdateLocationHandler** deserializes the event message, but does nothing with it in this sample implementation. This simulates a handler that is functioning without errors.

All the handlers implement the **IMessageHandler** interface, and the **Cars.Dispatcher.WorkerRole** worker role scans for classes that implement this type when it initializes. The **Cars.Dispatcher.WorkerRole** worker role uses a custom attribute on the handler class to identify the event type(s) it should handle.

There are also two built-in handlers (not shown on the diagram): **AzureBlobPoisonMessageHandler** and **UnknownTypeMessageHandler**.

## ColdStorage.WorkerRole

**ColdStorage.WorkerRole** worker role instances are event consumers that persist the events they receive from the Event Hub to blobs in the **ColdStorage.BlobWriterStorageAccount** for later analysis. Within the worker role instances, at any given time there is a single **ColdStorageProcessor** instance active for each partition in the Event Hub.

The **ColdStorage.WorkerRole** worker role instances use the **ColdStorage.Processor** consumer group to receive messages from the Event Hub service.

## RollingBlobWriter

Each **ColdStorageProcessor** instance uses a **RollingBlobWriter** instance to persist events from the Event Hub service to blob storage. The **RollingBlobWriter** tries to maximize the amount of data that it can store in a single blob by filling each block in the block blob as close to its maximum capacity as is feasible.

## Instrumentation

The code in the  **Cars.DispatcherHost** worker role, the **Cars.DispatcherHost** worker role, and the **Cars.SimulatorHost** worker role uses custom performance counters that are automatically installed when you deploy the solution to Azure. These custom performance counters enable you to monitor the activities of the three worker roles. For example, you can monitor the number of processed events and the number of timeouts while the worker roles are processing the events. This configuration in the **diagnostics.wadcfgx** files ensures that the custom performance monitor counter values are copied to the  **WADPerformanceCountersTable** in your diagnostics storage account.

The three worker roles also include configuration to collect standard Azure performance data. For more details, see the **diagnostics.wadcfgx** files in the projects.

In addition, the three worker roles also include configuration to collect standard Service Bus Event Hub performance data. See the **Microsoft.ServiceBus.MessagingPerformanceCounters.man** files in the projects.

## Logging

The three worker roles use the same logging infrastructure to create log data. See the **NLog.config** files in the projects. Application log files are written to the **logs** folder on your system drive. The log files are archived every hour and copied to the **telemetry-archive** blob container in your diagnostics storage account.

## Configuration

The projects in the solution all use the standard **.cscfg** files. The Visual Studio solution includes a PowerShell script that will provision the required assets in your Azure subscription and modify the **.cscfg** files accordingly.

## ColdStorage.BlobWriterStorageAccount

**RollingBlobWriter** instances persist event data from the Event Hub service in containers under a container named **coldstorage** in this storage account. It is possible to use multiple storage accounts in very high volume scenarios.

## ColdStorage.CheckPointStorageAccount

The **ColdStorage.WorkerRole** worker role instances use this storage account to write checkpoint data as they process events from the Event Hub service. It uses one blob per Event Hub partition in a container named **[your event hub path]/ColdStorage.Processor**. This enables the worker role instances to pick up where they left off processing messages in a particular Event Hub partition if the worker role instance restarts for any reason.

## Dispatcher.CheckPointStorageAccount

The **Cars.Dispatcher.WorkerRole** worker role instances use this storage account to write checkpoint data as they process events from the Event Hub service. It uses one blob per Event Hub partition in a contained named **[your event hub path]/Dispatcher**. This enables the worker role instances to pick up where they left off processing messages in a particular Event Hub partition if the worker role instance restarts for any reason.

## Dispatcher.PoisonMessagesStorageAccount

The **Cars.Dispatcher.WorkerRole** worker role instances use this storage account to record any messages that it cannot process in a container named **poison-messages**.

## HDInsight cluster

An HDInsight cluster enables you to run Hive queries to analyze the event data that the solution persists in the **ColdStorage.BlobWriterStorageAccount**. You must configure your HDInsight cluster in your Azure subscription before you can run a Hive query. The solution includes a sample Hive query in the PowerShell script file **hivequery.ps1**. You can run this query from your on premises environment to query the data in cold storage.




[gettingstartedpage]: GettingStarted.md
[eventhubpage]: EventHubService.md

[architectureoverview]: Figures/02-architectureoverview.png
