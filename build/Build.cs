using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using Serilog;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[ShutdownDotNetAfterServerBuild]
public class Build : NukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [GitVersion] readonly GitVersion GitVersion;

    [Parameter("NuGet API Key", Name = "NUGET_API_KEY")] readonly string NuGetApiKey;

    [Solution] readonly Solution Solution;

    AbsolutePath SourceDirectory => RootDirectory / "src";

    AbsolutePath BuildDirectory => RootDirectory / "build";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(dir => Directory.Delete(dir, true));
            BuildDirectory.GlobDirectories("**/.output").ForEach(dir => Directory.Delete(dir, true));
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Clean)
        .DependsOn(Restore)
        .Executes(() =>
        {
            if (GitVersion == null)
            {
                Log.Warning(
                    "GitVersion appears to be null. Have a look at it! Versions are defaulting to 0.1.0 for now...");
            }

            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion?.AssemblySemVer ?? "0.1.0")
                .SetFileVersion(GitVersion?.AssemblySemFileVer ?? "0.1.0")
                .SetInformationalVersion(GitVersion?.InformationalVersion ?? "0.1.0")
                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetConfiguration(Configuration)
                .SetProjectFile(Solution)
                .EnableNoRestore()
                .EnableNoBuild());
        });

    Target Pack => _ => _
        .DependsOn(Test)
        .DependsOn(Compile)
        .Executes(() =>
        {
            var packProjects = Solution
                .AllProjects
                .Where(p => !p.Name.EndsWith(".Tests"))
                .Where(p => !p.Name.EndsWith(".Build"));

            foreach (var packProject in packProjects)
            {
                Log.Information("Packing {ProjectName}...", packProject.Name);
                DotNetPack(s => s
                    .SetProject(packProject.Path)
                    .SetVersion(GitVersion?.NuGetVersionV2 ?? "0.1.0")
                    .SetOutputDirectory(BuildDirectory / ".output/Packages")
                    .SetConfiguration(Configuration)
                    .EnableIncludeSource()
                    .EnableIncludeSymbols()
                    .EnableNoBuild()
                    .EnableNoRestore());
            }
        });

    Target Publish => _ => _
        .DependsOn(Pack)
        .Requires(() => !string.IsNullOrWhiteSpace(NuGetApiKey))
        .Executes(() =>
        {
            const string nugetSource = "https://api.nuget.org/v3/index.json";

            var packages = (BuildDirectory / ".output/Packages").GlobFiles("*.nupkg");

            foreach (var package in packages)
            {
                Log.Information("Publishing {PackageName}...", package);
                DotNetNuGetPush(s => s
                    .SetApiKey(NuGetApiKey)
                    .SetSymbolApiKey(NuGetApiKey)
                    .SetTargetPath(package)
                    .SetSource(nugetSource)
                    .SetSymbolSource(nugetSource));
            }
        });

    public static int Main() => Execute<Build>(x => x.Test);
}