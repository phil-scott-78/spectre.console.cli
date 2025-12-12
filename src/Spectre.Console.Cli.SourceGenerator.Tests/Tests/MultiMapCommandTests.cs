namespace Spectre.Console.Cli.SourceGenerator.Tests.Tests;

public class MultiMapCommandTests : TestBase
{
    [Test]
    public async Task MultiMap_Dictionary()
    {
        var app = CreateFullApp();
        var result = app.Run("multimap", "--value", "foo=1", "bar=2");

        await Assert.That(result.Output).Contains("foo");
        await Assert.That(result.Output).Contains("1");
        await Assert.That(result.Output).Contains("bar");
        await Assert.That(result.Output).Contains("2");
        await Assert.That(result.ExitCode).IsEqualTo(0);
    }
}
