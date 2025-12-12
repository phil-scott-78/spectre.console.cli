namespace Spectre.Console.Cli.SourceGenerator.Tests.Tests;

public class AsyncCommandTests : TestBase
{
    [Test]
    public async Task Async_BasicExecution()
    {
        var app = CreateFullApp();
        var result = await app.RunAsync(["async", "HelloAsync", "-d", "10"]);

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Message: HelloAsync");
        await Assert.That(result.Output).Contains("Async operation completed");
    }
}
