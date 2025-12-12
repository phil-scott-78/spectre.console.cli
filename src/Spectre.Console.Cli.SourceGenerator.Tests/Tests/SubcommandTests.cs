namespace Spectre.Console.Cli.SourceGenerator.Tests.Tests;

public class SubcommandTests : TestBase
{
    [Test]
    public async Task SubAdd_Basic()
    {
        var app = CreateFullApp();
        var result = app.Run("sub", "add", "myitem");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Successfully added: myitem");
    }

    [Test]
    public async Task SubAdd_WithForce()
    {
        var app = CreateFullApp();
        var result = app.Run("sub", "add", "myitem", "--force");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Force mode enabled");
    }

    [Test]
    public async Task SubRemove_Basic()
    {
        var app = CreateFullApp();
        var result = app.Run("sub", "remove", "myitem");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Successfully removed: myitem");
    }

    [Test]
    public async Task SubRemove_WithAllFlag()
    {
        var app = CreateFullApp();
        var result = app.Run("sub", "remove", "myitem", "--all");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Removing all matching items");
    }

    [Test]
    public async Task SubList_Basic()
    {
        var app = CreateFullApp();
        var result = app.Run("sub", "list");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("item1");
        await Assert.That(result.Output).Contains("item2");
        await Assert.That(result.Output).Contains("item3");
    }

    [Test]
    public async Task SubList_WithFilter()
    {
        var app = CreateFullApp();
        var result = app.Run("sub", "list", "--filter", "pattern");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Filter: pattern");
    }
}
