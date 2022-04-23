
<#
    PUBLISH RANSEILINK GUI
    This script is designed to gather files after publishing the app via visual studio
#>

param([string] $Destination=$pwd)

# Establish file paths
$rlRootDir = "${env:USERPROFILE}\source\repos\RanseiLink"
$rlReleaseDir = "$rlRootDir\RanseiLink\bin\Release\net6.0-windows\publish\win-x64"
$csproj = "$rlRootDir\RanseiLink\RanseiLink.csproj"

# Read version from csproj
$csprojXml = [xml](Get-Content $csproj)
$versionPrefix = $csprojXml.Project.PropertyGroup.VersionPrefix
$versionSuffix = $csprojXml.Project.PropertyGroup.VersionSuffix
if ($versionSuffix)
{
    $version = "${versionPrefix}-${versionSuffix}"
}
else
{
	$version = "${versionPrefix}"
}

$rlExportDir = "${Destination}\RanseiLink-${version}"
Write-Host "Publishing to directory '${rlExportDir}'"

# Copy the published directory
Copy-Item -Path $RlReleaseDir -Destination $RlExportDir -Exclude *.pdb -Recurse

# Rename the exe to contain the version number
Rename-Item -Path "$rlExportDir\RanseiLink.exe" -NewName "RanseiLink-${version}.exe" 

# Copy latest plugins
#Copy-Item -Path "$Destination\LatestPlugins" -Destination "$rlExportDir\Plugins" -Recurse

read-host