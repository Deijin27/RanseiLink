
<#
    PUBLISH RANSEILINK CONSOLE
    This script is designed to gather files after publishing the app via visual studio
#>

param([string] $Destination=$pwd)

# Establish file paths
$rlRootDir = "${env:USERPROFILE}\source\repos\RanseiLink"
$rlReleaseDir = "${rlRootDir}\RanseiLink.Console\bin\Release\net6.0\publish"
$csproj = "${rlRootDir}\RanseiLink.Console\RanseiLink.Console.csproj"

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

$rlExportDir = "${Destination}\RanseiLink-Console-${version}"
Write-Host "Publishing to directory '${rlExportDir}'"

# Copy the published directory
Copy-Item -Path $RlReleaseDir -Destination $RlExportDir -Exclude *.pdb -Recurse