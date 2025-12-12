namespace Spectre.Console.Cli.SourceGenerator.Tests.Tests;

public class InjectionCommandTests : TestBase
{
    [Test]
    public async Task Inject_DIServiceResolution()
    {
        var app = CreateFullApp();
        var result = app.Run("inject", "World");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Hello, World!");
    }
}
