namespace Spectre.Console.Cli.SourceGenerator.Tests.Tests;

public class ServiceWithSettingsCommandTests : TestBase
{
    [Test]
    public async Task ServiceWithSettings_BasicExecution()
    {
        var app = CreateFullApp();
        var result = app.Run("servicedi", "Hello");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Service with Settings DI Test");
        await Assert.That(result.Output).Contains("Message: Hello");
        await Assert.That(result.Output).Contains("Formatted: Hello");
    }

    [Test]
    public async Task ServiceWithSettings_WithPrefix()
    {
        var app = CreateFullApp();
        var result = app.Run("servicedi", "World", "--prefix", "Greeting");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Prefix: Greeting");
        await Assert.That(result.Output).Contains("Formatted: Greeting: World");
    }
}
