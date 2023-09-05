using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
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
            // This doesn't actually work atm, the project fails to build with EnableNoRestore
            // So just commenting this out until I can figure out what the issue is
            //DotNetTasks.DotNetRestore(_ => _
            //   .SetProjectFile(Solution)
            //   );
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            const string windows_x64 = "win-x64";
            const string mac_x64 = "osx-x64";
            const string linux_x64 = "linux-x64";

            PublishWindows(windows_x64);
            PublishLinux(linux_x64);
            PublishMac(mac_x64);
        });

    AbsolutePath GetOutput(string version, string runtime)
    {
        return ArtifactsDirectory / $"{RanseiLink}-{version}-{runtime}";;
}

    private void PublishWindows(string runtime)
    {
        var proj = Solution.RanseiLink_XP;
        var version = GetVersion(Solution.RanseiLink_XP);
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
        exe.RenameWithoutExtension($"{RanseiLink}-5.5");
        ZipFile.CreateFromDirectory(output, output + ".zip");
    }

    private void PublishLinux(string runtime)
    {
        var proj = Solution.RanseiLink_XP;
        var version = GetVersion(Solution.RanseiLink_XP);
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
        exe.RenameWithoutExtension($"{RanseiLink}-5.5");
        ZipFile.CreateFromDirectory(output, output + ".zip");
    }

    private void PublishMac(string runtime)
    {
        var proj = Solution.RanseiLink_XP;
        var version = GetVersion(Solution.RanseiLink_XP);
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
        ZipFile.CreateFromDirectory(output, output + ".zip");
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
