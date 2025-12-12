namespace Spectre.Console.Cli.SourceGenerator.Tests.Tests;

public class HelpDisplayTests : TestBase
{
    [Test]
    public async Task Help_RootHelpListsCommands()
    {
        var app = CreateFullApp();
        var result = app.Run("--help");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("basic");
        await Assert.That(result.Output).Contains("multimap");
        await Assert.That(result.Output).Contains("flagvalue");
        await Assert.That(result.Output).Contains("async");
        await Assert.That(result.Output).Contains("floats");
        await Assert.That(result.Output).Contains("deep");
        await Assert.That(result.Output).Contains("edge");
        await Assert.That(result.Output).Contains("types");
        await Assert.That(result.Output).Contains("enumtest");
        await Assert.That(result.Output).Contains("settingsdi");
        await Assert.That(result.Output).Contains("sub");
    }

    [Test]
    public async Task Help_BasicCommandHelp()
    {
        var app = CreateFullApp();
        var result = app.Run("basic", "--help");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("NAME");
        await Assert.That(result.Output).Contains("ARGUMENTS");
        await Assert.That(result.Output).Contains("OPTIONS");
    }

    [Test]
    public async Task Help_SubBranchHelp()
    {
        var app = CreateFullApp();
        var result = app.Run("sub", "--help");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("add");
        await Assert.That(result.Output).Contains("remove");
        await Assert.That(result.Output).Contains("list");
    }

    [Test]
    public async Task Help_HiddenOptionNotShown()
    {
        var app = CreateFullApp();
        var result = app.Run("edge", "--help");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).DoesNotContain("--hidden");
    }
}
