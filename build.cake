#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#tool nuget:?package=GitReleaseNotes.Portable&version=0.7.1
#tool nuget:?package=Wyam&version=2.1.1
#tool nuget:https://www.nuget.org/api/v2?package=JetBrains.ReSharper.CommandLineTools&version=2018.1.0

#addin nuget:?package=Cake.Wyam&version=2.1.1

#load nuget:https://www.nuget.org/api/v2?package=Cake.Wyam.Recipe&version=0.6.0
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Build");

// 1. If command line parameter parameter passed, use that.
// 2. Otherwise if an Environment variable exists, use that.
var configuration =
    HasArgument("Configuration") ? Argument<string>("Configuration") :
    EnvironmentVariable("Configuration") != null ? EnvironmentVariable("Configuration") : "Release";

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// The build number to use in the version number of the built NuGet packages.
// There are multiple ways this value can be passed, this is a common pattern.
// 1. If command line parameter parameter passed, use that.
// 2. Otherwise if running on AppVeyor, get it's build number.
// 3. Otherwise if running on Travis CI, get it's build number.
// 4. Otherwise if an Environment variable exists, use that.
// 5. Otherwise default the build number to 0.
var buildNumber =
    HasArgument("BuildNumber") ? Argument<int>("BuildNumber") :
    AppVeyor.IsRunningOnAppVeyor ? AppVeyor.Environment.Build.Number :
    TravisCI.IsRunningOnTravisCI ? TravisCI.Environment.Build.BuildNumber :
    EnvironmentVariable("BuildNumber") != null ? int.Parse(EnvironmentVariable("BuildNumber")) : 0;

// Define directories.
var artifactsDirectory = MakeAbsolute(Directory("./artifacts"));
var solutionFile = "./src/SharpInputSystem.sln";

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            title: "SharpInputSystem",
                            repositoryOwner: "Axiom3D",
                            repositoryName: "sharpinputsystem",
                            appVeyorAccountName: "borrillis",
                            webHost: "axiom3d.github.io",
                            wyamRecipe: "Docs",
                            wyamTheme: "Samson",
                            wyamSourceFiles: MakeAbsolute(Directory("./")).FullPath + "/**/{!bin,!obj,!packages,!*.Tests,}/**/*.cs",
                            wyamPublishDirectoryPath: Directory($"{artifactsDirectory}/gh-pages"),
                            webLinkRoot: "/sharpinputsystem",
                            webBaseEditUrl: "https://github.com/axiom3d/sharpinputsystem/tree/master/",
                            shouldPublishDocumentation: true,
                            shouldPurgeCloudflareCache: false);

BuildParameters.PrintParameters(Context);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(artifactsDirectory);
        DotNetCoreClean(solutionFile, new DotNetCoreCleanSettings {
            Configuration = configuration
        });
    });

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetCoreRestore(solutionFile, new DotNetCoreRestoreSettings {
            
        });
    });

Task("Build-Product")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        DotNetCoreBuild(solutionFile, new DotNetCoreBuildSettings {
            Configuration = configuration,
            NoRestore = true
        });
    });

Task("InspectCode")
    .Description("Inspect the code using Resharper's rule set")
    .Does(() =>
{
    var settings = new InspectCodeSettings() {
        SolutionWideAnalysis = true,
        OutputFile = $"{artifactsDirectory}/inspectcode.xml",
        ThrowExceptionOnFindingViolations = true
    };
    InspectCode(solutionFile, settings);
});
   
Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        DotNetCoreTest(solutionFile, new DotNetCoreTestSettings {
            Configuration = configuration
            });
    });

Task("Package")
    .IsDependentOn("Test")
    .Does(() => 
    {
        DotNetCorePack(solutionFile, new DotNetCorePackSettings {
            Configuration = configuration,
            OutputDirectory = artifactsDirectory.FullPath
        });
    });

Task("GenerateReleaseNotes")
    .Does(() => 
    {
        var releaseNotesExitCode = StartProcess(
            @"tools\GitReleaseNotes.Portable.0.7.1\tools\GitReleaseNotes.exe", 
            new ProcessSettings { Arguments = $". /o ./artifacts/releasenotes.md" });
        if (string.IsNullOrEmpty(System.IO.File.ReadAllText($"{artifactsDirectory}/releasenotes.md")))
            System.IO.File.WriteAllText($"{artifactsDirectory}/releasenotes.md", "No issues closed since last release");

        if (releaseNotesExitCode != 0) throw new Exception("Failed to generate release notes");
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.CleanDocumentationTask
    .IsDependentOn("Clean");

BuildParameters.Tasks.AppVeyorTask
    .IsDependentOn("GenerateReleaseNotes")
    .IsDependentOn("Package");

BuildParameters.Tasks.BuildDocumentationTask
    .IsDependentOn("Build-Product");

BuildParameters.Tasks.PreviewDocumentationTask
    .IsDependentOn("Build-Product");

Task("Build")
    .IsDependentOn("Build-Product")
    .IsDependentOn("Build-Documentation");

Task("Validate")
    .Description("Validate code quality using Resharper CLI. tools.")
    //.IsDependentOn("Analyse-Dependencies")
    //.IsDependentOn("DupFinder")
    .IsDependentOn("InspectCode");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);