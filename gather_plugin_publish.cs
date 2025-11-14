#!/usr/local/share/dotnet/dotnet run

/*
GATHER PUBLISH PLUGINS

This script is designed to gather release versions of plugin dlls 
and put them in the target folder

How to use:
1. Install .NET 10 SDK or newer
2. Build RanseiLink Solution in Release mode
3. Open commandline in the folder where gather_plugin_publish.cs is located
4. Run `dotnet run gather_plugin_publish.cs -- "c:\Target\Folder"` (on unix platforms the shebang may enable `./gather_plugin_debug.cs -- "c:\Target\Folder"`)

 */
using System.Text.RegularExpressions;

Console.WriteLine("Gathering RanseiLink PUBLISH plugins...");

// Establish file paths
var rootDirectory = Directory.GetCurrentDirectory();
var pluginsFolderPath = Path.Combine(rootDirectory, "Plugins");

var destination = args.FirstOrDefault();
if (destination == null)
{
    destination = Path.Combine(rootDirectory, "artifacts", "GatheredPlugins");
}

// delete existing plugin target folder
if (Directory.Exists(destination))
{
    Directory.Delete(destination, true); 
}
Directory.CreateDirectory(destination);

// Copy plugins to correct folder
var versionRegex = new Regex(@"\[Plugin\("".*?"", "".*?"", ""(.*?)""\)]");
foreach (var folder in Directory.GetDirectories(pluginsFolderPath))
{
    var pluginName = Path.GetFileName(folder);
    Console.WriteLine($"Gathering {pluginName}");

    // Get version number from cs file attribute
    var pluginCsFile = Path.Combine(folder, pluginName + ".cs");
    var version = versionRegex.Match(File.ReadAllText(pluginCsFile)).Groups[1].Value;

    // Copy dll to output
    var dllToCopy = Path.Combine(folder, "bin", "Debug", "net10.0", pluginName + ".dll");
    var dllDestination = Path.Combine(destination, $"{pluginName}-{version}.dll");
    File.Copy(dllToCopy, dllDestination);
}

