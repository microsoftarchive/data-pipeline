# Deployment option #3: Cloud

This page describes how you can configure and run the RI in the Cloud using your Azure subscription.

> Note: There are some common requisite steps for all three deployment scenarios. For more information, see [Getting Started with the Reference Implementation][gettingstarted].

Before you run the RI in the cloud, you must provide some additional configuration data to enable the worker roles to connect to your Event Hub and Azure Storage accounts. 

## Adding the configuration data

Before you can deploy the RI to Azure, you must replace the placeholder values in the **Cloud.cscfg** files for the three worker roles. If you ran the **ProvisionAssets.ps1** PowerShell script described on [Getting Started with the Reference Implementation][gettingstarted] then it will have replaced all the placeholder values for you.

Alternatively, you can either edit the **.Cloud.csfg** files directly, or use the property sheet in Visual Studio for each of the three roles in the **Deployment** folder and make the changes manually. The following table summarizes the recommended values to use for each of the placeholders in the **ServiceConfiguration.Cloud.csfg** file in the **SimulationDeployment** Cloud Service folder in the Solution:

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

The following table summarizes the recommended values to use for each of the placeholders in the **ServiceConfiguration.Cloud.csfg** file in the **ConsumerGroupsDeployment** Cloud Service folder in the Solution:

<table>
<tr>
	<th>Setting name</th><th>Placeholder</th><th>Recommended value</th>
</tr>
<tr>
	<td>Dispatcher.CheckpointStorageAccount</td><td>[YourStorageAccount]</b></td><td>The name of the storage account you created.</td>
</tr>
<tr>
	<td>Dispatcher.CheckpointStorageAccount</td><td>[YourAccountKey]</b></td><td>The primary key of the storage account you created.</td>
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
	<td>Dispatcher.PoisonMessageStorageAccount</td><td>[YourStorageAccount]</b></td><td>The name of the storage account you created.</td>
</tr>
<tr>
	<td>Dispatcher.PoisonMessageStorageAccount</td><td>[YourAccountKey]</b></td><td>The primary key of the storage account you created.</td>
</tr>
<tr>
	<td>Coldstorage.CheckpointStorageAccount</td><td>[YourStorageAccount]</b></td><td>The name of the storage account you created.</td>
</tr>
<tr>
	<td>Coldstorage.CheckpointStorageAccount</td><td>[YourAccountKey]</b></td><td>The primary key of the storage account you created.</td>
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
	<td>Coldstorage.BlobWriterStorageAccount</td><td>[YourStorageAccount]</b></td><td>The name of the storage account you created.</td>
</tr>
<tr>
	<td>Coldstorage.BlobWriterStorageAccount</td><td>[YourAccountKey]</b></td><td>The primary key of the storage account you created.</td>
</tr>
</table>

## Configuring diagnostics

We also recommend that you create a separate Storage account to store your diagnostics information (including the custom performance counter values from the three worker roles). To configure the three worker roles to use your diagnostics storage account, either you can use the property sheet for each worker role and specify the details of your storage account when you click on the **Configure** button below the **Enable Diagnostics** checkbox, or you can edit each of the three **diagnostics.wadcfgx** files in the solution manually. For example, if the name of the storage account you are using for diagnostics is **myridiagnostics**, then edit the three **diagnostics.wadcfgx** files in the solution as shown:

    <?xml version="1.0" encoding="utf-8"?>
    <DiagnosticsConfiguration xmlns="http://schemas.microsoft.com/ServiceHosting/2010/10/DiagnosticsConfiguration">
      <PublicConfig xmlns="http://schemas.microsoft.com/ServiceHosting/2010/10/DiagnosticsConfiguration">
        <WadCfg>
          ...
        </WadCfg>
        <StorageAccount>myridiagnostics</StorageAccount>
      </PublicConfig>
      <PrivateConfig xmlns="http://schemas.microsoft.com/ServiceHosting/2010/10/DiagnosticsConfiguration">
        <StorageAccount name="myridiagnostics" endpoint="https://core.windows.net/" />
      </PrivateConfig>
      <IsEnabled>true</IsEnabled>
    </DiagnosticsConfiguration>


## Deploying to Windows Azure

To deploy the RI to Azure using your subscription, you can use the **Publish** wizard in Visual Studio. To deploy the simulator that generates events, right-click on the **SimulatorDeployment** cloud service in Visual Studio and select **Publish**. In the wizard:

- Connect to your Azure subscription.
- Create a new Cloud Service in a suitable region.
- Ensure that you choose the **Production** environment.
- Ensure that you choose the **Release** build configuration.
- Ensure that you choose the **Cloud** service configuration.
- You may want to enable Remote Desktop for troubleshooting the deployment.
- Save the profile for future deployments.
- Click **Publish** to start the publishing process.

To deploy the worker roles that consume the events, right-click on the **ConsumerGroupsDeployment** cloud service in Visual Studio and select **Publish**. In the wizard:

- Connect to your Azure subscription.
- Create a new Cloud Service in a suitable region.
- Ensure that you choose the **Production** environment.
- Ensure that you choose the **Release** build configuration.
- Ensure that you choose the **Cloud** service configuration.
- You may want to enable Remote Desktop for troubleshooting the deployment.
- Save the profile for future deployments.
- Click **Publish** to start the publishing process.

## Output from the RI running in the cloud

The **diagnostics.wadcfgx** files include configuration settings to copy the custom performance counter values periodically to the **WADPerformanceCountersTable** in your diagnostics storage account. You can view the content of this table using **Server Explorer** in Visual Studio.You can monitor the activity of the three worker roles by examining the performance counter values such as:

- \EventHub ColdStorage(WaWorkerHost)\Average write time
- \EventHub ColdStorage(WaWorkerHost)\Total blocks written
- \EventHub ColdStorage(WaWorkerHost)\Total concurrent writes failed
- \EventHub ColdStorage(WaWorkerHost)\Total events processed
- \EventHub Dispatcher(WaWorkerHost)\Current task count
- \EventHub Dispatcher(WaWorkerHost)\Total processed messages
- \EventHub Dispatcher(WaWorkerHost)\Total timeouts
- \EventHub Dispatcher(WaWorkerHost)\Total timeouts
- \EventHub Sender(WaWorkerHost)\Avg. message sending time
- \EventHub Sender(WaWorkerHost)\Messages sent per sec
- \EventHub Sender(WaWorkerHost)\Total messages sent

Alternatively, you can use Remote Desktop to connect to the worker role instances and use Performance Monitor to view the custom performance counters. If you use Remote Desktop, you can also view the log files generated by the application in the folder **D:\logs**. For example:

- **ApplicationLog_??.txt** contains basic information about the running app.
- **ApiTraceLog.log** contains detailed information about the activities of the app.

The log files are archived every hour and copied to the **telemetry-archive** blob container in your diagnostics storage account.

For more information about the content of the application log files, view the **NLog.config** file in the Visual Studio solution.


[gettingstarted]: /TBD