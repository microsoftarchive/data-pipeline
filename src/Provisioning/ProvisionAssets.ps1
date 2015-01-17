<#
.SYNOPSIS
Provision an Azure Storage Account and a Service Bus Event Hub.

.DESCRIPTION
This script will create and configure the required Event Hub And Azure storage assets to be able to run the sample. The script outputs the configuration keys and values that need to be replaced in the mysettings.config configuration file.
Important: Make sure you use these values in the mysettings.config and the service configuration files before running the sample with the Azure Emulator or deploying the sample to Azure.

.PARAMETER SubscriptionName
The name of the subscription to use.

.PARAMETER ServiceBusNamespace
The name of Service Bus namespace.

.PARAMETER ServiceBusEventHubPath
The name of the event hub.

.PARAMETER Location
The location of the storage account.

.PARAMETER PartitionCount
The number of partitions to use. Defaults to 16.

.PARAMETER MessageRetentionInDays
The number of days the messages will be retained.

.PARAMETER ColdStorageCounsumerGroupName
The name of the consumer group for the Cold Storage processor.

.PARAMETER ColdStorageContainerName
the name of the blob container where cold data will be stored.

.PARAMETER PoisonMessagesContainerName
The name of the blob container where poison messages will be stored.

.PARAMETER DispatcherConsumerGroupName
The name of the consumer group used by the dispatcher.

.PARAMETER StorageAccountName
The name of the storage account used by the sample.

#>
Param
(
	[Parameter (Mandatory = $true)]
	[string] $SubscriptionName,

    [Parameter (Mandatory = $true)]
    [ValidatePattern("^[A-Za-z][-A-Za-z0-9]*[A-Za-z0-9]$")] 
    [string] $ServiceBusNamespace,

    [Parameter (Mandatory = $true)]
    [ValidatePattern("^[A-Za-z0-9]$|^[A-Za-z0-9][\w-\.\/]*[A-Za-z0-9]$")] 
    [string] $ServiceBusEventHubPath,

    [Parameter (Mandatory = $true)]
    [String]$StorageAccountName,

    [Parameter (Mandatory = $true)]
    [string] $Location,

    [String]$ColdStorageConsumerGroupName = "ColdStorage.Processor",

    [String]$ColdStorageContainerName = "coldstorage",

    [String]$PoisonMessagesContainerName = "poison-messages",

    [String]$DispatcherConsumerGroupName = "Dispatcher",

    [Int]$PartitionCount = 16,

    [Int]$MessageRetentionInDays = 7    
)

# Make the script stop on error
$ErrorActionPreference = "Stop"

# Check the azure module is installed
if(-not(Get-Module -name "Azure")) 
{ 
    if(Get-Module -ListAvailable | Where-Object { $_.name -eq "Azure" }) 
    { 
        Import-Module Azure
    }
    else
    {
        "Windows Azure Powershell has not been installed."
        Exit
    }
}

Add-AzureAccount
Select-AzureSubscription -SubscriptionName $SubscriptionName

# Provision Event Hub
.\CreateEventHub.ps1 -Namespace $ServiceBusNamespace -Path $ServiceBusEventHubPath -ColdStorageConsumerGroupName $ColdStorageConsumerGroupName -DispatcherConsumerGroupName $DispatcherConsumerGroupName -Location $Location -PartitionCount $PartitionCount -MessageRetentionInDays $MessageRetentionInDays

# Provision Storage Account
.\ProvisionStorageAccount.ps1 -Name $StorageAccountName -Location $Location -ColdStorageContainerName $ColdStorageContainerName -PoisonMessagesContainerName $PoisonMessagesContainerName


# Get output
$storageAccountKey = Get-AzureStorageKey -StorageAccountName $StorageAccountName
$saConnectionString = "DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}" -f $StorageAccountName, $storageAccountKey.Primary;

$serviceBus = Get-AzureSBNamespace -Name $ServiceBusNamespace
$sbConnectionString = $serviceBus.ConnectionString + ";TransportType=Amqp"

""
# copy config settings to mysettings file.
$settings = @{
    'Simulator.EventHubConnectionString'=$sbConnectionString;
    'Simulator.EventHubPath'=$ServiceBusEventHubPath;
    'Dispatcher.EventHubConnectionString'=$sbConnectionString;
    'Dispatcher.ConsumerGroupName'=$DispatcherConsumerGroupName;
    'Dispatcher.CheckpointStorageAccount'=$saConnectionString;
    'Dispatcher.EventHubName'=$ServiceBusEventHubPath;
    'Dispatcher.PoisonMessageStorageAccount'=$saConnectionString;
    'Dispatcher.PoisonMessageContainer'=$PoisonMessagesContainerName;
    'Coldstorage.ConsumerGroupName'=$ColdStorageConsumerGroupName;
    'Coldstorage.CheckpointStorageAccount'=$saConnectionString;
    'Coldstorage.EventHubConnectionString'=$sbConnectionString;
    'Coldstorage.EventHubName'=$ServiceBusEventHubPath;
    'Coldstorage.BlobWriterStorageAccount'=$saConnectionString;
    'Coldstorage.ContainerName'=$ColdStorageContainerName
}

$scriptPath = Split-Path (Get-Variable MyInvocation -Scope 0).Value.MyCommand.Path

.\CopyOutputToConfigFile.ps1 -configurationFile "..\RunFromConsole\mysettings.config" -appSettings $settings

# get Cloud service configuration files 
$serviceConfigFiles = Get-ChildItem -Include "ServiceConfiguration.Cloud.cscfg" -Path "$($scriptPath)\.." -Recurse
.\CopyOutputToServiceConfigFiles.ps1 -serviceConfigFiles $serviceConfigFiles -appSettings $settings

""
"Provision and configuration complete. Please review your mysettings.config and the ServiceConfiguration.Cloud.cscfg files with the latest configuration settings."