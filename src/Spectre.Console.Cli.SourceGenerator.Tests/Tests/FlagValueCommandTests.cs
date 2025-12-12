namespace Spectre.Console.Cli.SourceGenerator.Tests.Tests;

public class FlagValueCommandTests : TestBase
{
    [Test]
    public async Task FlagValue_PortFlagOnly()
    {
        var app = CreateFullApp();
        var result = app.Run("flagvalue", "--port");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("IsSet=True");
    }

    [Test]
    public async Task FlagValue_PortWithValue()
    {
        var app = CreateFullApp();
        var result = app.Run("flagvalue", "--port", "8080");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Value=8080");
    }

    [Test]
    public async Task FlagValue_TimeoutWithValue()
    {
        var app = CreateFullApp();
        var result = app.Run("flagvalue", "--timeout", "30");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Value=30");
    }
}
