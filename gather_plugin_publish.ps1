
<#
    GATHER PLUGINS
    This script is designed to gather release versions of plugin dlls after publishing the app via visual studio
    First build the RanseiLink solution for release, then run this script
#>

param([string] $Destination="$pwd\GatheredPlugins")


# Establish file paths
$rlRootDir = "${env:USERPROFILE}\source\repos\RanseiLink"
$pluginsFolderPath = "$rlRootDir\Plugins"

# Find plugins
$pluginFolders = Get-ChildItem $pluginsFolderPath -Directory

New-Item -ItemType Directory -Path $Destination -Force

foreach ($folder in $pluginFolders) 
{
    $pluginFolderName = $folder.FullName
    $pluginName = $folder.Name
    Write-Host "Gathering $pluginName"

    # Get version number from cs file attribute
    $pluginCsFile = "$pluginFolderName\$pluginName.cs"
    $version = (Select-String -Path $pluginCsFile -Pattern '\[Plugin\(".*?", ".*?", "(.*?)"\)]').Matches.Groups[1].Value

    # Copy dll to output
    $dllToCopy = "$pluginFolderName\bin\Release\net8.0\$pluginName.dll"
    $dllDestination = "$Destination\$pluginName-$version.dll"
    Copy-Item -Path $dllToCopy -Destination $dllDestination
}