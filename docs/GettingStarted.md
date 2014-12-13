# Getting Started with the Reference Implementation

This page summarizes the three different options you have for deploying and running the Reference Implementation (RI). You can find links to detailed information about each deployment option at the end of this page, but you should read the prerequisite information here first. The three options are:

1. Run a collection of console apps on your local machine. This is the simplest and easiest option and is ideal if you want to study the code. Note: you will still need an Azure subscription to provision and run an Event Hub service in this scenario and you will need the Azure Development Storage emulator running on your local machine.
2. Deploy the RI to your local Azure Development Fabric (Compute) emulator. This option is useful if you want to understand how the RI makes use of worker roles in a Cloud Service. Note: you will still need an Azure subscription to provision and run an Event Hub service in this scenario and you will also need the Azure Storage Emulator running on your local machine.
3. Deploy the complete RI to your Azure subscription. This option enables you to explore how the RI behaves at scale in a production environment.

> Note: Other deployment configurations are possible (such as using an Azure Storage account with options 1 and 2) but this documentation only covers the three recommended deployment options.

For more information about the overall architecture of the RI, see the page "[Architecture Overview][architectureoverviewpage]."

## Prerequisites for all three options

For all three deployment options you must complete the following prerequisites:

### 1. Install Visual Studio and the Microsoft Azure SDK for .NET

To build and run the RI you will need Visual Studio 2013 Professional or above and the Microsoft Azure SDK for .NET version 2.5 or above. The Microsoft Azure SDK for .NET includes the Development Storage and Development Fabric emulators. Deployment options 1 and 2 make use of the Development Storage emulator and deployment option 2 makes use of the Development Fabric (compute) emulator.

### 2. Download and build the source

Download the source and verify that you can build it in your local development environment. The solution uses the NuGet package manager to download and install its dependencies automatically.

You can download the source code as a ZIP file here [RI Source][rizipdownload].

### 3. Provision your environment

All three deployment options use an Event Hub which you must configure before you run the RI. You can configure an Event Hub manually using the Azure Management portal but we recommend you use the **ProvisionAssets.ps1** PowerShell script located in the **Deployment** folder included in the Visual Studio solution you downloaded. This script also provisions the Azure Storage account that deployment option #3 requires and modifies the configuration files in your Visual Studio solution to include details of the Event Hub and Storage account.

This script prompts prompts you for the following information:

- **SubscriptionName**: The name of your Azure subscription. You will need your credentials to sign in.
- **ServiceBusNamespace**: The unique name of the Service Bus Namespace to create  or use. For example, *mysbnamespace*.
- **ServiceBusEventHubPath**: The path for your Event Hub. For example, *myeventhubpath*.
- **StorageAccountName**: The unique name of the Azure Storage account to create or use. For example, *mystorageaccount*.
- **Location**: The Azure region to deploy to. For example, *West US*.

The script provides default values for:

- **ColdStorageConsumerGroupName**: The name of the Event Hub consumer group the cold storage processor uses.
- **ColdStorageContainerName**: The blob container the cold storage processor uses to persist event data.
- **PoisonMessagesContainerName**: The blob container the dispatch processor uses to persist poison messages.
- **DispatcherConsumerGroupName**: The name of the Event Hub consumer group the dispatch processor uses.
- **PartitionCount**: The number of Event Hub partitions to configure.
- **MessageRetentionInDays**: The number of days the Event Hub should persist messages for.

> Note: Before you run this script, install and configure Azure PowerShell and use the **Add-AzureAccount** cmdlet to connect your subscription. For more information, see [How to install and configure Azure PowerShell][installazurepowershell].

This script updates the following files in your Visual Studio solution:

- **mysettings.config**: This file holds the configuration settings used by deployment option #1 when you run the console apps locally.
- The **ServiceConfiguration.Cloud.cscfg** files in the two cloud services (**SimulatorDeployment** and **ConsumerGroupsDeployment**) in your Visual Studio solution: These files hold the configuration settings used by deployment option #3 when you deploy the RI to the cloud.

## Using the different deployment options

For more information about how to run the RI as a set of console apps, see [Deployment option #1: Console apps][deploymentconsole].

For more information about how to run the RI in the local Development Fabric (Compute) emulator, see [Deployment option #2: Development Fabric emulator][deploymentemulator].

For more information about how to run the RI as a set of console apps, see [Deployment option #3: Cloud][deploymentcloud].




[architectureoverviewpage]: /TBD
[rizipdownload]: /TBD
[deploymentconsole]: /TBD
[deploymentemulator]: /TBD
[deploymentcloud]: /TBD

[installazurepowershell]: http://azure.microsoft.com/en-us/documentation/articles/install-configure-powershell/
