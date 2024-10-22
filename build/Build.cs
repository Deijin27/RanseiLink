using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Microsoft.Build.Tasks;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Solution(GenerateProjects = true)] 
    readonly Solution Solution;

    const string RanseiLink = nameof(RanseiLink);

    AbsolutePath SourceDirectory => RootDirectory / "RanseiLink.XP";
    //AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(x => x.DeleteDirectory());
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetTasks.DotNetRestore(_ => _
               .SetProjectFile(Solution)
               );
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
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
        });


    /// <summary>
    /// Note: this actually seems to be unnecessary when I tested with wsl the file already has this permission
    /// But let's do it just in case anyway.
    /// </summary>
    private void GiveUnixExecutePermission(AbsolutePath path)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // TODO: should validate that wsl is installed somehow
            using (var wd = path.Parent.SwitchWorkingDirectory())
            {
                // Switch working directory because the commands don't seem to like drive identifiers
                // (may just be a result of the funky way we're running this command)
                // Run the wsl command from outside of the linux file system using -e
                var p = Process.Start("wsl", $"-e chmod +x \"{path.Name}\"");
                p.WaitForExit();
            }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var p = Process.Start("/bin/bash/", $"chmod +x \"{path}\"");
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
    private void ZipPreservePermissions(AbsolutePath dirPath)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            using (var wd = dirPath.Parent.SwitchWorkingDirectory())
            {
                var p = Process.Start("wsl", $"-e zip -r {dirPath.Name}.zip {dirPath.Name}");
                p.WaitForExit();
            }
        }
        else
        {
            // Assuming this works properly in unix systems, haven't actually tested it yet
            ZipFile.CreateFromDirectory(dirPath, dirPath + ".zip");
        }
    }

    private void GetHashes()
    {
        using var sw = new StreamWriter(ArtifactsDirectory / "hashes.md");
        sw.WriteLine("## SHA256 Hashes");
        sw.WriteLine();
        foreach (var zip in ArtifactsDirectory.GetFiles("*.zip"))
        {
            var hash = GetSha256Hash(zip);
            sw.WriteLine($"- {zip.Name}: `{hash}`");
        }
    }

    private string GetSha256Hash(AbsolutePath path)
    {
        Assert.True(path.FileExists());

        using var md5 = SHA256.Create();
        using var stream = File.OpenRead(path);
        var hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    AbsolutePath GetOutput(string version, string runtime)
    {
        return ArtifactsDirectory / $"{RanseiLink}-XP-{version}-{runtime}";
    }

    private void PublishConsole()
    {
        var proj = Solution._App.RanseiLink_Console;
        var version = GetVersion(proj);
        var output = ArtifactsDirectory / $"{RanseiLink}-Console-{version}";

        DotNetTasks.DotNetPublish(_ => _
            .SetProject(proj)
            .SetConfiguration(Configuration.Release)
            .EnableNoRestore()
            .SetSelfContained(false)
            .SetOutput(output)
            .SetProperty("DebugType", "None")
            .SetProperty("DebugSymbols", "false")
        );
        ZipFile.CreateFromDirectory(output, output + ".zip");
    }

    private void PublishWpfWindows(string runtime)
    {
        var proj = Solution._App.RanseiLink_Windows;
        var version = GetVersion(proj);
        var output = ArtifactsDirectory / $"{RanseiLink}-Windows-{version}";

        DotNetTasks.DotNetPublish(_ => _
            .SetProject(proj)
            .SetConfiguration(Configuration.Release)
            .EnableNoRestore()
            .SetRuntime(runtime)
            .SetSelfContained(true)
            .SetPublishSingleFile(true)
            .SetOutput(output)
            .SetProperty("DebugType", "None")
            .SetProperty("DebugSymbols", "false")
            .SetProperty("IncludeNativeLibrariesForSelfExtract", "true")
        );
        var exe = output / $"{RanseiLink}.Windows.exe";
        exe.RenameWithoutExtension($"{RanseiLink}-{version}");
        ZipFile.CreateFromDirectory(output, output + ".zip");
    }

    private void PublishWindows(string runtime)
    {
        var proj = Solution._App.RanseiLink_XP;
        var version = GetVersion(proj);
        var output = GetOutput(version, runtime);

        DotNetTasks.DotNetPublish(_ => _
            .SetProject(proj)
            .SetConfiguration(Configuration.Release)
            .SetRuntime(runtime)
            .SetSelfContained(true)
            .SetPublishSingleFile(true)
            .SetOutput(output)
            .SetProperty("DebugType", "None")
            .SetProperty("DebugSymbols", "false")
            .SetProperty("IncludeNativeLibrariesForSelfExtract", "true")
        );
        var exe = output / $"{RanseiLink}.exe";
        exe.RenameWithoutExtension($"{RanseiLink}-{version}");
        ZipFile.CreateFromDirectory(output, output + ".zip");
    }

    private void PublishLinux(string runtime)
    {
        var proj = Solution._App.RanseiLink_XP;
        var version = GetVersion(proj);
        var output = GetOutput(version, runtime);

        DotNetTasks.DotNetPublish(_ => _
            .SetProject(proj)
            .SetConfiguration(Configuration.Release)
            .SetRuntime(runtime)
            .SetSelfContained(true)
            .SetPublishSingleFile(true)
            .SetOutput(output)
            .SetProperty("DebugType", "None")
            .SetProperty("DebugSymbols", "false")
            .SetProperty("IncludeNativeLibrariesForSelfExtract", "true")
        );
        var exe = output / RanseiLink;
        var newExe = output / $"{RanseiLink}-{version}";
        exe.RenameWithoutExtension($"{RanseiLink}-{version}");

        // Give execute permission
        GiveUnixExecutePermission(newExe);

        ZipPreservePermissions(output);
    }

    private void PublishMac(string runtime)
    {
        var proj = Solution._App.RanseiLink_XP;
        var version = GetVersion(proj);
        var output = GetOutput(version, runtime);
        var appFolder = output / $"{RanseiLink}.app";

        var buildResultOutput = appFolder / "Contents" / "MacOS";

        DotNetTasks.DotNetPublish(_ => _
            .SetProject(proj)
            .SetConfiguration(Configuration.Release)
            .SetRuntime(runtime)
            .SetSelfContained(true)
            .SetOutput(buildResultOutput)
            .SetProperty("DebugType", "None")
            .SetProperty("DebugSymbols", "false")
        );

        // ICNS
        var resources = appFolder / "Contents" / "Resources";
        Directory.CreateDirectory(resources);
        SaveResourceToFile("Resources.RanseiLink.icns", resources / "RanseiLink.icns");

        // INFO.PLIST
        var str = ReadResourceText("Resources.Info.plist");
        str = str.Replace("{version}", version);
        File.WriteAllText(appFolder / "Info.plist", str);

        // Give execute permission
        var exe = buildResultOutput / RanseiLink;
        GiveUnixExecutePermission(exe);

        ZipPreservePermissions(output);
    }

    private static string GetVersion(Project project)
    {
        var versionPrefix = project.GetProperty("VersionPrefix");
        var versionSuffix = project.GetProperty("VersionSuffix");
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

}
