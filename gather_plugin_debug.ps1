
<#
    GATHER DEBUG PLUGINS
    This script is designed to gather debug versions of plugin dlls in the right
	folder ready for debugging
    First build the RanseiLink solution for debug, then run this script
#>

# Establish file paths
$rlRootDir = "${env:USERPROFILE}\source\repos\RanseiLink"
$pluginsFolderPath = "$rlRootDir\Plugins"
$Destination = "$rlRootDir\RanseiLink.Windows\bin\Debug\net8.0-windows\Plugins"
$DestinationXP = "$rlRootDir\RanseiLink.XP\bin\Debug\net8.0\Plugins"

# Find plugins
$pluginFolders = Get-ChildItem $pluginsFolderPath -Directory

New-Item -ItemType Directory -Path $Destination -Force

foreach ($folder in $pluginFolders) 
{
    $pluginFolderName = $folder.FullName
    $pluginName = $folder.Name
    Write-Host "Gathering $pluginName"

    # Copy dll to output
    $dllToCopy = "$pluginFolderName\bin\Debug\net8.0\$pluginName.dll"
    $dllDestination = "$Destination\$pluginName.dll"
	$dllDestinationXP = "$DestinationXP\$pluginName.dll"
    Copy-Item -Path $dllToCopy -Destination $dllDestination -Force
    Copy-Item -Path $dllToCopy -Destination $dllDestinationXP -Force
}