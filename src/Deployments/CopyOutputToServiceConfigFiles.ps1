Param
(
    [Parameter(Mandatory = $true)]
    [System.IO.FileInfo[]] $serviceConfigFiles,

    [Parameter(Mandatory = $true)]
    [hashtable] $appSettings
)

$ErrorActionPreference = "Stop"

if ($appSettings -eq $null)
{
    Write-Error "[appSettings] paremeter cannot be null"
}

foreach ($file in $serviceConfigFiles)
{
    $fileExists = Test-Path -Path $file.FullName -PathType Leaf

    if (!$fileExists)
    {
        Write-Error "Could not find file {0}" -f $file
    }

    [Xml]$xmlServiceConfiguration = Get-Content $file.FullName
    $ns = New-Object Xml.XmlNamespaceManager $xmlServiceConfiguration.NameTable
    $ns.AddNamespace('dns', 'http://schemas.microsoft.com/ServiceHosting/2008/10/ServiceConfiguration')

    foreach ($key in $appSettings.Keys)
    {
       
        $nodes = $xmlServiceConfiguration.SelectNodes("/dns:ServiceConfiguration/dns:Role/dns:ConfigurationSettings/dns:Setting[@name='${key}']", $ns)

        if ($nodes -ne $null)
        {
            foreach ($node in $nodes)
            {
                $node.value = $appSettings.Item($key)
            }
        }
    }

    $xmlServiceConfiguration.Save($file)
	""
    "Service Configuration File (Cloud) updated : {0}" -f $file.FullName
}