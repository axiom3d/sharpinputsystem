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
var artifactsDirectory = MakeAbsolute(Directory("./BuildArtifacts"));
var solutionFile = "./src/SharpInputSystem.sln";

Func<MSBuildSettings,MSBuildSettings> commonSettings = settings => settings
    .SetConfiguration(configuration)
    .WithProperty("PackageOutputPath", artifactsDirectory.FullPath);

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
                            wyamPublishDirectoryPath: MakeAbsolute(Directory("./BuildArtifacts/gh-pages")),
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
        MSBuild(solutionFile,
            settings => commonSettings(settings)
                        .WithTarget("Clean"));
    });

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        NuGetRestore(solutionFile);
    });

Task("Build-Product")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        MSBuild(solutionFile, settings =>
            settings.SetConfiguration(configuration));
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
        NUnit3("./src/**/bin/" + configuration + "/*.Tests.dll", new NUnit3Settings {
            NoResults = true
            });
    });

Task("Package")
    .IsDependentOn("Test")
    .Does(() => 
    {
        if (!configuration.Contains("OSX")) {
            GenerateReleaseNotes();
        }

        MSBuild(solutionFile,
            settings => commonSettings(settings)
                        .WithTarget("Pack")
                        .WithProperty("NoBuild","true")
                        .WithProperty("IncludeSymbols","true"));
    });

private void GenerateReleaseNotes()
{
    var releaseNotesExitCode = StartProcess(
        @"tools\GitReleaseNotes.Portable.0.7.1\tools\GitReleaseNotes.exe", 
        new ProcessSettings { Arguments = ". /o BuildArtifacts/releasenotes.md" });
    if (string.IsNullOrEmpty(System.IO.File.ReadAllText("./BuildArtifacts/releasenotes.md")))
        System.IO.File.WriteAllText("./BuildArtifacts/releasenotes.md", "No issues closed since last release");

    if (releaseNotesExitCode != 0) throw new Exception("Failed to generate release notes");
}

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.CleanDocumentationTask
    .IsDependentOn("Clean");

BuildParameters.Tasks.AppVeyorTask
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