#!/usr/local/share/dotnet/dotnet run

/*
GATHER DEBUG PLUGINS

This script is designed to gather debug versions of plugin dlls in the right
folder ready for debugging

How to use:
1. Install .NET 10 SDK or newer
2. Build RanseiLink Solution in Debug mode
3. Open commandline in the folder where gather_plugin_debug.cs is located
4. Run `dotnet run gather_plugin_debug.cs` (on unix platforms the shebang may enable `./gather_plugin_debug.cs`)
 */

Console.WriteLine("Gathering RanseiLink DEBUG plugins...");

// Establish file paths
var rootDirectory = Directory.GetCurrentDirectory();
var pluginsFolderPath = Path.Combine(rootDirectory, "Plugins");
var destination = Path.Combine(rootDirectory, "RanseiLink.Windows", "bin", "Debug", "net10.0-windows", "Plugins");
var destinationXP = Path.Combine(rootDirectory, "RanseiLink.XP", "bin", "Debug", "net10.0", "Plugins");

// Delete existing plugin folders to ensure
// we don't leave any deprecated plugins lingering
if (Directory.Exists(destination))
{
    Directory.Delete(destination, true);
}
if (Directory.Exists(destinationXP))
{
    Directory.Delete(destinationXP, true);
}
Directory.CreateDirectory(destination);
Directory.CreateDirectory(destinationXP);

// Copy plugins to correct folders
// alongside the debug app builds
foreach (var folder in Directory.GetDirectories(pluginsFolderPath))
{
    var pluginName = Path.GetFileName(folder);
    var pluginDllName = pluginName + ".dll";
    Console.WriteLine($"Gathering {pluginName}");

    // Copy dll to output
    var dllToCopy = Path.Combine(folder, "bin", "Debug", "net10.0", pluginDllName);
    var dllDestination = Path.Combine(destination, pluginDllName);
    var dllDestinationXP = Path.Combine(destinationXP, pluginDllName);
    File.Copy(dllToCopy, dllDestination);
    File.Copy(dllToCopy, dllDestinationXP);
}