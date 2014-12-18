<#
.SYNOPSIS
Storage Account Provisioning - Create a new storage account

.DESCRIPTION
This script creates a new Azure Storage Account in the chosen location, configured for Local Redundancy and hourly monitoring turned on. This storage account will be used for:
 - Event Hubs checkpoint account
 - Storing poison messages
 - Storing cold data

.PARAMETER SubscriptionName
The name of the subscription to use.

.PARAMETER Name
The name of the storage account.

.PARAMETER Location
The location of the storage account
#>

Param
(
    [Parameter (Mandatory = $true)]
    [string] $Name,

    [Parameter (Mandatory = $true)]
    [string] $Location,

    [Parameter (Mandatory = $true)]
    [string] $ColdStorageContainerName,

    [Parameter (Mandatory = $true)]
    [string] $PoisonMessagesContainerName
)

$retenetionDays = 7;

Write-Output ""
Write-Output "This storage account will be used for:
 - Event Hubs checkpoint data
 - Storing poison messages
 - Storing cold data
 ";
Write-Output "";


$storageAccount = Get-AzureStorageAccount -StorageAccountName $Name -ErrorAction SilentlyContinue

if (!$storageAccount)
{
    # Create a new storage account
    Write-Output "";
    Write-Output ("Configuring storage account {0} in location {1}" -f $Name, $Location);

    New-AzureStorageAccount -StorageAccountName $Name -Location $Location -Verbose;
}
else
{
    "Storage account {0} already exists." -f $Name
}

# Get the access key of the storage account
$key = Get-AzureStorageKey -StorageAccountName $Name
$context = New-AzureStorageContext -StorageAccountName $Name -StorageAccountKey $key.Primary;

# Configure monitoring for storage account
Set-AzureStorageServiceMetricsProperty -ServiceType Blob -MetricsType Hour -RetentionDays $retenetionDays -MetricsLevel ServiceAndApi -Context $context -Verbose;
Set-AzureStorageServiceLoggingProperty -ServiceType Blob -LoggingOperations Read,Write -RetentionDays $retenetionDays -Context $context -Verbose;

$coldStorageContainer = Get-AzureStorageContainer -Name $ColdStorageContainerName -ErrorAction SilentlyContinue -Context $context
if (!$coldStorageContainer)
{
    New-AzureStorageContainer -Context $context -Name $ColdStorageContainerName
}
else 
{
    "The storage container {0} already exists." -f $ColdStorageContainerName
}

$poisonMessagesContainer = Get-AzureStorageContainer -Name $PoisonMessagesContainerName -ErrorAction SilentlyContinue -Context $context
if (!$poisonMessagesContainer)
{
    New-AzureStorageContainer -Context $context -Name $PoisonMessagesContainerName
}
else
{
    "The storage container {0} already exists." -f $PoisonMessagesContainerName
}


# Configure options for storage account
Set-AzureStorageAccount -StorageAccountName $Name -Type "Standard_LRS" -Verbose;
Write-Output ("Finished configuring storage account {0} in location {1}" -f $Name, $Location);





