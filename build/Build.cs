using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace RanseiLink.Build;

public class Build
{
    private readonly string _rootDirectory;
    private readonly string _windowsCsproj;
    private readonly string _consoleCsproj;
    private readonly string _avaloniaCsproj;
    private readonly string _artifactsDirectory;
    const string RanseiLink = nameof(RanseiLink);

    public Build()
    {
        _rootDirectory = "";
        _windowsCsproj = Path.Combine(_rootDirectory, "RanseiLink.Windows", "RanseiLink.Windows.csproj");
        _consoleCsproj = Path.Combine(_rootDirectory, "RanseiLink.Console", "RanseiLink.Console.csproj");
        _avaloniaCsproj = Path.Combine(_rootDirectory, "RanseiLink.XP", "RanseiLink.XP.csproj");
        _artifactsDirectory = Path.Combine(_rootDirectory, "artifacts");
    }

    public void Run(string[] args)
    {
        Clean();
        Restore();
        Compile();
        Publish();
    }

    void Clean()
    {
        // Clean bin and obj dirs
        var projectDirs = Directory.EnumerateFiles(_rootDirectory, "*.csproj", SearchOption.AllDirectories)
            .Where(x => !x.Contains("RanseiLink.Build"))
            .Select(Path.GetDirectoryName)
            .ToArray();
        Log($"Cleaning {projectDirs.Length} projects");
        foreach (var projDir in projectDirs)
        {
            Log($"Cleaning Project Directory: {projDir}");
            foreach (var dir in Directory.EnumerateDirectories(projDir, "**/bin;**/obj"))
            {
                Log($"Deleting: {dir}");
                Directory.Delete(dir, true);
            }
        }
        // Clean artifcats dir
        if (Directory.Exists(_artifactsDirectory))
        {
            Directory.Delete(_artifactsDirectory, recursive: true);
        }
        Directory.CreateDirectory(_artifactsDirectory);
    }

    void RunCommand(string command, string args)
    {
        var startInfo = new ProcessStartInfo(command, args)
        {
            Arguments = args,
            WorkingDirectory = _artifactsDirectory
        };
        using var process = Process.Start(startInfo);
        process.WaitForExit();
    }

    void Restore()
    {
        RunCommand("dotnet", $"restore \"{_windowsCsproj}\"");
    }

    void Compile()
    {
        // Not sure how I should build to be able to run tests, because the wpf is published as single exe so needs special treatment

        //DotNetTasks.DotNetBuild(_ => _
        //    .SetProjectFile(Solution)
        //    .EnableNoRestore()
        //    .SetProperty("DebugType", "None")
        //    .SetProperty("DebugSymbols", "false")
        //    .SetConfiguration(Configuration.Release)
        //    );

        //DotNetTasks.DotNetBuild(_ => _
        //    .SetProjectFile(Solution._App.RanseiLink_Windows)
        //    .EnableNoRestore()
        //    .SetProperty("DebugType", "None")
        //    .SetProperty("DebugSymbols", "false")
        //    .SetSelfContained(true)
        //    .SetPublishSingleFile(true)
        //    .SetConfiguration(Configuration.Release)
        //    );

    }


    void Publish()
    {
        const string win_x64 = "win-x64";
        //const string win_arm64 = "win-arm64";
        //const string mac_x64 = "osx-x64";
        //const string mac_arm64 = "osx-arm64";
        //const string linux_x64 = "linux-x64";
        //const string linux_arm64 = "linux-arm64";

        PublishConsole();
        PublishWpfWindows(win_x64);
        //PublishWindows(win_x64);
        //PublishWindows(win_arm64);
        //PublishLinux(linux_x64);
        //PublishLinux(linux_arm64);
        //PublishMac(mac_x64);
        //PublishMac(mac_arm64);
        //GetHashes();

    }

    private void PublishConsole()
    {
        var proj = _consoleCsproj;
        var version = GetVersion(proj);
        var output = Path.Combine(_artifactsDirectory, $"{RanseiLink}-Console-{version}");

        RunCommand("dotnet",
            $"publish {proj}"
            + " --configuration Release"
            + " --no-restore"
            + " --self-contained false"
            +$" --artifacts-path \"{output}\""
            + " -p:DebugType=None"
            + " -p:DebugSymbols=false"
            );

        ZipFile.CreateFromDirectory(output, output + ".zip");
    }

    private void PublishWpfWindows(string runtime)
    {
        var proj = _windowsCsproj;
        var version = GetVersion(proj);
        var output = Path.Combine(_artifactsDirectory, $"{RanseiLink}-{version}-{runtime}");

        RunCommand("dotnet",
            $"publish {proj}"
            + " --configuration Release"
            + " --no-restore"
            +$" --runtime {runtime}"
            + " --self-contained"
            + " -p:PublishSingleFile=true"
            +$" --artifacts-path \"{output}\""
            + " -p:DebugType=None"
            + " -p:DebugSymbols=false"
            + " -p:IncludeNativeLibrariesForSelfExtract=true"
            );

        var exe = Path.Combine(output, $"{RanseiLink}.Windows.exe");
        var renamedExe = Path.Combine(Path.GetDirectoryName(exe), $"{RanseiLink}-{version}.exe");
        File.Move(exe, renamedExe);

        ZipFile.CreateFromDirectory(output, output + ".zip");
    }

    private string GetAvaloniaOutput(string version, string runtime)
    {
        return Path.Combine(_artifactsDirectory, $"{RanseiLink}-XP-{version}-{runtime}");
    }

    private void PublishAvaloniaWindows(string runtime)
    {
        var proj = _avaloniaCsproj;
        var version = GetVersion(proj);
        var output = GetAvaloniaOutput(version, runtime);

        RunCommand("dotnet",
            $"publish {proj}"
            + " --configuration Release"
            +$" --runtime {runtime}"
            + " --self-contained"
            + " -p:PublishSingleFile=true"
            +$" --artifacts-path \"{output}\""
            + " -p:DebugType=None"
            + " -p:DebugSymbols=false"
            + " -p:IncludeNativeLibrariesForSelfExtract=true"

            );

        var exe = Path.Combine(output, $"{RanseiLink}.exe");
        var renamedExe = Path.Combine(Path.GetDirectoryName(exe), $"{RanseiLink}-{version}.exe");
        File.Move(exe, renamedExe);

        ZipFile.CreateFromDirectory(output, output + ".zip");
    }

    private void PublishAvaloniaLinux(string runtime)
    {
        var proj = _avaloniaCsproj;
        var version = GetVersion(proj);
        var output = GetAvaloniaOutput (version, runtime);

        RunCommand("dotnet",
            $"publish {proj}"
            + " --configuration Release"
            +$" --runtime {runtime}"
            + " --self-contained"
            + " -p:PublishSingleFile=true"
            +$" --artifacts-path \"{output}\""
            + " -p:DebugType=None"
            + " -p:DebugSymbols=false"
            + " -p:IncludeNativeLibrariesForSelfExtract=true"

            );

        var exe = Path.Combine(output, RanseiLink);
        var renamedExe = Path.Combine(Path.GetDirectoryName(exe), $"{RanseiLink}-{version}.exe");

        // Give execute permission
        GiveUnixExecutePermission(renamedExe);

        ZipPreservePermissions(output);
    }

    private void PublishAvaloniaMac(string runtime)
    {
        var proj = _avaloniaCsproj;
        var version = GetVersion(proj);
        var output = GetAvaloniaOutput(version, runtime);
        var appFolder = Path.Combine(output, $"{RanseiLink}.app");

        var buildResultOutput = Path.Combine(appFolder, "Contents", "MacOS");

        RunCommand("dotnet",
            $"publish {_avaloniaCsproj}"
            + " --configuration Release"
            +$" --runtime {runtime}"
            + " --self-contained"
            +$" --artifacts-path \"{buildResultOutput}\""
            + " -p:DebugType=None"
            + " -p:DebugSymbols=false"
            );

        // ICNS
        var resources = Path.Combine(appFolder, "Contents", "Resources");
        Directory.CreateDirectory(resources);
        SaveResourceToFile("Resources.RanseiLink.icns", Path.Combine(resources, "RanseiLink.icns"));

        // INFO.PLIST
        var str = ReadResourceText("Resources.Info.plist");
        str = str.Replace("{version}", version);
        File.WriteAllText(Path.Combine(appFolder, "Info.plist"), str);

        // Give execute permission
        var exe = Path.Combine(buildResultOutput, RanseiLink);
        GiveUnixExecutePermission(exe);

        ZipPreservePermissions(output);
    }

    private static string GetVersion(string project)
    {
        var doc = XDocument.Load(project);
        var rootPropertyGroup = doc.Root?.Element("PropertyGroup");
        if (rootPropertyGroup == null)
        {
            throw new Exception($"Missing required element 'PropertyGroup' in project '{project}'");
        }
        var versionPrefix = rootPropertyGroup.Element("VersionPrefix")?.Value;
        if (versionPrefix == null)
        {
            throw new Exception($"Missing required element 'VersionPrefix' in project '{project}'");
        }
        var versionSuffix = rootPropertyGroup.Element("VersionSuffix")?.Value;
        if (versionPrefix == null)
        {
            throw new Exception($"Missing required element 'VersionSuffix' in project '{project}'");
        }
        string version = versionPrefix;
        if (!string.IsNullOrEmpty(versionSuffix))
        {
            version += "-" + versionSuffix;
        }
        return version;
    }

    /// <summary>
    /// Gets the resource from this assembly. Throws a useful excepiton if it fails.
    /// </summary>
    private Stream GetResourceStream(string resource)
    {
        var resourceStream = GetType().Assembly.GetManifestResourceStream(resource);
        if (resourceStream == null)
        {
            string resources = string.Join(",", GetType().Assembly.GetManifestResourceNames());
            throw new Exception($"Cannot find resource '{resource}' in list of resources '{resources}'");
        }
        return resourceStream;
    }

    private void SaveResourceToFile(string resource, string saveFile)
    {
        using var targetStream = File.Create(saveFile);
        using var sourceStream = GetResourceStream(resource);
        sourceStream.CopyTo(targetStream);
    }

    private string ReadResourceText(string resource)
    {
        using var sourceStream = new StreamReader(GetResourceStream(resource));
        return sourceStream.ReadToEnd();
    }


    /// <summary>
    /// Note: this actually seems to be unnecessary when I tested with wsl the file already has this permission
    /// But let's do it just in case anyway.
    /// </summary>
    private static void GiveUnixExecutePermission(string path)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // TODO: should validate that wsl is installed somehow
            try
            {
                // Switch working directory because the commands don't seem to like drive identifiers
                // (may just be a result of the funky way we're running this command)
                // Run the wsl command from outside of the linux file system using -e
                var startInfo = new ProcessStartInfo("wsl")
                {
                    Arguments = $"-e chmod +x \"{Path.GetFileName(path)}\"",
                    WorkingDirectory = Path.GetDirectoryName(path)
                };
                using var p = Process.Start(startInfo);
                p.WaitForExit();
            }
            finally
            {

            }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            using var p = Process.Start("/bin/bash/", $"chmod +x \"{path}\"");
            p.WaitForExit();
        }
        else
        {
            throw new Exception("I don't know how to chmod+x on this platform yet :(");
        }

    }

    /// <summary>
    /// If running on windows. Recursively zip directory using wsl to preserve permissions
    /// c# zipping only preserves the execute permissions if ran from the linux environment it seems
    /// therefore this is necessary on windows.
    /// 
    /// If not on windows this does a regular zipfile
    /// </summary>
    /// <remarks>
    /// Requires running in wsl: sudo apt install zip
    /// </remarks>
    private static void ZipPreservePermissions(string dirPath)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var dirName = Path.GetFileName(dirPath);
            var startInfo = new ProcessStartInfo("wsl")
            {
                Arguments = $"-e zip -r {dirName}.zip {dirName}",
                WorkingDirectory = Path.GetDirectoryName(dirPath)
            };
            using var p = Process.Start(startInfo);
            p.WaitForExit();
        }
        else
        {
            // Assuming this works properly in unix systems, haven't actually tested it yet
            ZipFile.CreateFromDirectory(dirPath, dirPath + ".zip");
        }
    }

    private void GetHashes()
    {
        using var sw = new StreamWriter(Path.Combine(_artifactsDirectory, "hashes.md"));
        sw.WriteLine("## SHA256 Hashes");
        sw.WriteLine();
        foreach (var zip in Directory.GetFiles(_artifactsDirectory, "*.zip"))
        {
            var hash = GetSha256Hash(zip);
            sw.WriteLine($"- {Path.GetFileName(zip)}: `{hash}`");
        }
    }

    private static string GetSha256Hash(string path)
    {
        using var md5 = SHA256.Create();
        using var stream = File.OpenRead(path);
        var hash = md5.ComputeHash(stream);
        return Convert.ToHexStringLower(hash);
    }

    private void Log(string message)
    {
        Console.WriteLine(message);
    }

}
