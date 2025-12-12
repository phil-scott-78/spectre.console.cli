#:sdk Cake.Sdk@6.0.0

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

////////////////////////////////////////////////////////////////
// Tasks

Task("Clean")
    .Does(context =>
{
    context.CleanDirectory("./.artifacts");
});

Task("Lint")
    .Does(context =>
{
    DotNetFormatStyle("./src/Spectre.Console.Cli.slnx",
        new DotNetFormatSettings
        {
            VerifyNoChanges = true,
        });
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Lint")
    .Does(context =>
{
    DotNetBuild("./src/Spectre.Console.Cli.slnx", new DotNetBuildSettings
    {
        Configuration = configuration,
        Verbosity = DotNetVerbosity.Minimal,
        NoLogo = true,
        NoIncremental = context.HasArgument("rebuild"),
        MSBuildSettings = new DotNetMSBuildSettings()
            .TreatAllWarningsAs(MSBuildTreatAllWarningsAs.Error)
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(context =>
{
    DotNetTest("./src/Spectre.Console.Cli.Tests/Spectre.Console.Cli.Tests.csproj", new DotNetTestSettings
    {
        Configuration = configuration,
        Verbosity = DotNetVerbosity.Minimal,
        PathType = DotNetTestPathType.Project,
        NoRestore = true,
        NoBuild = true,

    });
});

Task("Test-SourceGenerator")
    .IsDependentOn("Test")
    .Does(context =>
{
    var projectPath = "./src/Spectre.Console.Cli.SourceGenerator.Tests/Spectre.Console.Cli.SourceGenerator.Tests.csproj";
    var publishDir = "./.artifacts/sourcegen-tests";

    DotNetPublish(projectPath, new DotNetPublishSettings
    {
        Configuration = configuration,
        OutputDirectory = publishDir,
        Verbosity = DotNetVerbosity.Minimal,
        Framework = "net10.0",
        NoLogo = true,
    });

    var exeName = context.IsRunningOnWindows()
        ? "Spectre.Console.Cli.SourceGenerator.Tests.exe"
        : "Spectre.Console.Cli.SourceGenerator.Tests";

    var exePath = System.IO.Path.Combine(publishDir, exeName);

    var exitCode = StartProcess(exePath);
    if (exitCode != 0)
    {
        throw new CakeException($"Source generator tests failed with exit code {exitCode}");
    }
});

Task("Package")
    .IsDependentOn("Test-SourceGenerator")
    .Does(context =>
{
    context.DotNetPack($"./src/Spectre.Console.Cli.slnx", new DotNetPackSettings
    {
        Configuration = configuration,
        Verbosity = DotNetVerbosity.Minimal,
        NoLogo = true,
        NoRestore = true,
        NoBuild = true,
        OutputDirectory = "./.artifacts",
        MSBuildSettings = new DotNetMSBuildSettings()
            .TreatAllWarningsAs(MSBuildTreatAllWarningsAs.Error)
    });
});

Task("Publish-NuGet")
    .WithCriteria(ctx => BuildSystem.IsRunningOnGitHubActions, "Not running on GitHub Actions")
    .IsDependentOn("Package")
    .Does(context =>
{
    var apiKey = Argument<string?>("nuget-key", null);
    if (string.IsNullOrWhiteSpace(apiKey))
    {
        throw new CakeException("No NuGet API key was provided.");
    }

    // Publish to GitHub Packages
    foreach (var file in context.GetFiles("./.artifacts/*.nupkg"))
    {
        context.Information("Publishing {0}...", file.GetFilename().FullPath);
        DotNetNuGetPush(file.FullPath, new DotNetNuGetPushSettings
        {
            Source = "https://api.nuget.org/v3/index.json",
            ApiKey = apiKey,
        });
    }
});

////////////////////////////////////////////////////////////////
// Targets

Task("Publish")
    .IsDependentOn("Publish-NuGet");

Task("Default")
    .IsDependentOn("Package");

////////////////////////////////////////////////////////////////
// Execution

RunTarget(target);