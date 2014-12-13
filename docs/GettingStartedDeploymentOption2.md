# Deployment option #2: Development Fabric emulator

This page describes how you can configure and run the RI in the local Development Fabric (compute) emulator.

> Note: There are some common requisite steps for all three deployment scenarios. For more information, see [Getting Started with the Reference Implementation][gettingstarted].

Before you run the RI in the local compute emulator, you must provide some additional configuration data to enable the worker roles to connect to your Event Hub in the cloud and to your local Storage account in the Storage emulator. 

## Adding the configuration data

Before you can deploy the RI to Azure, you must replace the placeholder values in the **Local.cscfg** files for the three worker roles.

You can either edit the **.Local.csfg** files directly, or use the property sheet in Visual Studio for each of the three roles in the **Deployment** folder and make the changes manually. The following table summarizes the recommended values to use for each of the placeholders in the **ServiceConfiguration.Local.csfg** file in the **SimulationDeployment** Cloud Service folder in the Solution:

<table>
<tr>
	<th>Setting name</th><th>Placeholder</th><th>Recommended value</th>
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
	<td>Simulator.EventHubPath</td><td>[YourEventHubName]</td><td>The path you created when you configured your Event Hub.</td>
</tr>
</table>

> Note. If you ran the **ProvisionAssets.ps1** PowerShell script described on [Getting Started with the Reference Implementation][gettingstarted] then you can copy the Event Hub settings from the **ServiceConfiguration.Cloud.csfg** file to the the **ServiceConfiguration.Local.csfg** file.

The following table summarizes the recommended values to use for each of the placeholders in the **ServiceConfiguration.Local.csfg** file in the **ConsumerGroupsDeployment** Cloud Service folder in the Solution:

<table>
<tr>
	<th>Setting name</th><th>Placeholder</th><th>Recommended value</th>
</tr>
<tr>
	<td>Dispatcher.ConsumerGroupName</td><td>Replace the entire <b>value</b></td><td>Dispatcher</td>
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
	<td>Coldstorage.ConsumerGroupName</td><td>Replace the entire <b>value</b></td><td>ColdStorage.Processor</td>
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

> Note. If you ran the **ProvisionAssets.ps1** PowerShell script described on [Getting Started with the Reference Implementation][gettingstarted] then you can copy the Event Hub settings from the **ServiceConfiguration.Cloud.csfg** file to the the **ServiceConfiguration.Local.csfg** file.

## Deploying to the local compute emulator

You must start Visual Studio with Administrator rights to deploy to the local compute emulator because the RI installs custom performance counters on your local machine.

Configure the Visual Studio solution with multiple start projects in the **Solution Property Pages**. You must have the following two projects as your start up projects: **SimulatorDeployment** and **ConsumerGroupsDeployment**. When you start the solution it deploys all three worker roles to the local compute emulator. It uses the Event Hub in your Azure subscription and the local storage emulator.

## Output from the RI running in the local compute emulator

You can view the activities of the three worker roles by monitoring the custom **EventHub ColdStorage**, **EventHubDispatcher**, and **EventHub Sender** performance counters using **Performance Monitor**. For example:

- **EventHub Sender, Total messages sent** shows how many events have been generated by the simulator and sent to Event Hub.
- **EventHub ColdStorage, Total events processed** shows how many events the cold storage processor has received from Event Hub and persisted to blob storage.
- **EventHub Dispatcher, Total processed messages** shows how many events the dispatch processor has received and processed.

You can also view the log files generated by the application in the folder **logs** on your system drive. For example:

- **ApplicationLog_??.txt** contains basic information about the running app.
- **ApiTraceLog.log** contains detailed information about the activities of the app.

For more information about the content of the application log files, view the **NLog.config** file in the Visual Studio solution.


[gettingstarted]: /TBD