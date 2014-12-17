# Deployment option #1: Console apps

This page describes how you can configure and run the RI locally as a set of console apps.

> Note: There are some common requisite steps for all three deployment scenarios. For more information, see [Getting Started with the Reference Implementation][gettingstarted].

Before you run the RI console applications, you must provide some additional configuration data to enable the console apps to connect to the Event Hub service and the Development Storage emulator.

## Adding the configuration data

You add the configuration data for the RI console apps in a file named **mysettings.config** in the **RunFromConsole** folder in the solution. If you ran the **ProvisionAssets.ps1** PowerShell script described on [Getting Started with the Reference Implementation][gettingstarted] then it will have replaced all the placeholder values in the **mysettings.config** file for you.

Alternatively, you can edit the **mysettings.config** file manually. To help you, there is a template file named **my-settings-template.config** that you can copy as **mysettings.config** and then edit to add your configuration values.

> Note. You will see build errors in the solution if you do not create the **mysettings.config** file.

The template file, **my-settings-template.config**, contains placeholders for all the settings that you must change before running the RI. The file is organized into three main sections (**Simulator**, **Dispatcher**, and **Cold Storage**). The following table summarizes the recommended values to add in the **mysettings.config** file for this deployment option:

<table>
<tr>
	<th>Key</th><th>Placeholder</th><th>Recommended value</th>
</tr>
<tr>
	<td>Simulator.EventHubConnectionString</td><td>[YourServiceNamespace]</td><td>The Service Namespace you created when you configured your Event Hub.</td>
</tr>
<tr>
	<td>Simulator.EventHubConnectionString</td><td>[YourAccessKeyName]</td><td>RootManageSharedAccessKey</td>
</tr>
<tr>
	<td>Simulator.EventHubConnectionString</td><td>[YourAccessKey]</td><td>The primary key of the Service Bus RootManageSharedAccessKey policy from your Azure Subscription.</td>
</tr>
<tr>
	<td>Simulator.EventHubPath</td><td>[YourEventHubPath]</td><td>The path you created when you configured your Event Hub.</td>
</tr>
<tr>
	<td>Dispatcher.CheckpointStorageAccount</td><td>Replace the entire <b>value</b></td><td>UseDevelopmentStorage=true</td>
</tr>
<tr>
	<td>Dispatcher.EventHubConnectionString</td><td>[YourServiceNamespace]</td><td>The Service Namespace you created when you configured your Event Hub.</td>
</tr>
<tr>
	<td>Dispatcher.EventHubConnectionString</td><td>[YourAccessKeyName]</td><td>RootManageSharedAccessKey</td>
</tr>
<tr>
	<td>Dispatcher.EventHubConnectionString</td><td>[YourAccessKey]</td><td>The primary key of the Service Bus RootManageSharedAccessKey policy from your Azure Subscription.</td>
</tr>
<tr>
	<td>Dispatcher.EventHubName</td><td>[YourEventHubName]</td><td>The path you created when you configured your Event Hub.</td>
</tr>
<tr>
	<td>Dispatcher.PoisonMessageStorageAccount</td><td>Replace the entire <b>value</b></td><td>UseDevelopmentStorage=true</td>
</tr>
<tr>
	<td>Coldstorage.CheckpointStorageAccount</td><td>Replace the entire <b>value</b></td><td>UseDevelopmentStorage=true</td>
</tr>
<tr>
	<td>Coldstorage.EventHubConnectionString</td><td>[YourServiceNamespace]</td><td>The Service Namespace you created when you configured your Event Hub.</td>
</tr>
<tr>
	<td>Coldstorage.EventHubConnectionString</td><td>[YourAccessKeyName]</td><td>RootManageSharedAccessKey</td>
</tr>
<tr>
	<td>Coldstorage.EventHubConnectionString</td><td>[YourAccessKey]</td><td>The primary key of the Service Bus RootManageSharedAccessKey policy from your Azure Subscription.</td>
</tr>
<tr>
	<td>Coldstorage.EventHubName</td><td>[YourEventHubName]</td><td>The path you created when you configured your Event Hub.</td>
</tr>
<tr>
	<td>Coldstorage.BlobWriterStorageAccount</td><td>Replace the entire <b>value</b></td><td>UseDevelopmentStorage=true</td>
</tr>
</table>

## Running the console apps

Before you run the RI console apps for the first time, ensure that the Azure Storage Emulator is running. For more information, see [Using the Azure Storage Emulator for Development and Testing][usingstorageemulator].

You can find the three console apps in the folder **RunFromConsole** in the solution.
<font color=red> They are:

- Cars.Simulator.ConsoleHost 
- ColdStorage.ConsoleHost 
- DispatchingProcessor.ConsoleHost
</font>

> Note. The first time you run the console apps you should run them with *administrator* permissions in order to install the custom performance counters on your local machine. The console apps dispaly error messages if they cannot create the custom performance counters.

Run the **Cars.Simulator.ConsoleHost** app to start generating a stream of sample events that the app sends to your Event Hub. The app prompts you to run a simulation scenario:

    Cars.Simulator.ConsoleHost

    [1] Run NoErrorsExpected
    [2] Run MessagesWithNoHandler
    [3] Run MalformedMessages
    [4] Run LongRunningMessages
    [5] Send 1 message from NoErrorsExpected
    [6] Send 1 message from MessagesWithNoHandler
    [7] Send 1 message from MalformedMessages
    [8] Send 1 message from LongRunningMessages
    [9] Exit

    Select an option:

Select the scenario you want to run.

Run the **ColdStorage.ConsoleHost** app to receive events from your Event Hub and persist them to blob storage. The first time you run the app you should choose option 1 to create the Event Hub consumer group **ColdStorage** and blob containers in your storage account before you choose option 2 to start consuming events:

    ColdStorage.Tests.ConsoleHost

    [1] Provision Resources
    [2] Run Cold Storage Consumer
    [3] Exit

    Select an option:

When you finish running the Cold Storage Consumer, you can find the persisted events in the **coldstorage** blob container in the Azure Storage emulator.

Run the **DispatchingProcessor.ConsoleHost** app the receive events from Event Hub and dispatch them to the handlers. The first time you run the app you should choose option 1 to create the Event Hub consumer group **DispatchingProcessor** and blob containers in your storage account before you choose option 2 to start consuming events:

    DispatchingProcessor.ConsoleHost

    [1] Provision Resources
    [2] Run Dispatching Processor
    [3] Exit

    Select an option:

## Output from the console apps

To view the activity of the **Cars.Simulator.ConsoleHost**, **ColdStorage.ConsoleHost**, and **DispatchingProcessor.ConsoleHost** apps, in addition to the messages in the console windows you can view the custom performance counters and log data.

You can find the custom performance monitor counters in the following performance counter categories:

- EventHub ColdStorage
- EventHub Dispatcher
- EventHub Sender

You can find the log files from the RI in the **logs** folder on your system drive. For example:

- **ApplicationLog_*.txt** contains basic information about the running app.
- **ApiTraceLog.log** contains detailed information about the activities of the app.

You can find the log configuration in the **NLog.config** file in the Visual Studio solution.


[gettingstarted]: ../GettingStarted.md

[usingstorageemulator]: http://msdn.microsoft.com/en-us/library/azure/hh403989.aspx
