using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
            const string windows_x64 = "win-x64";
            const string mac_x64 = "osx-x64";
            const string linux_x64 = "linux-x64";

            Publish(windows_x64);
            Publish(linux_x64);
        });

    private void Publish(string runtime)
    {
        string output = ArtifactsDirectory / $"{RanseiLink}-{runtime}";

        DotNetTasks.DotNetPublish(_ => _
            .SetProject(Solution.RanseiLink_XP)
            .SetConfiguration(Configuration.Release)
            .SetRuntime(runtime)
            .SetSelfContained(true)
            .SetPublishSingleFile(true)
            .SetOutput(output)
            .SetProperty("DebugType", "None")
            .SetProperty("DebugSymbols", "false")
            .SetProperty("IncludeNativeLibrariesForSelfExtract", "true")
        );

        ZipFile.CreateFromDirectory(output, output + ".zip");
    }

}
