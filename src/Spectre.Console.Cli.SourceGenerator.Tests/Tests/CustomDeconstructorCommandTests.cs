namespace Spectre.Console.Cli.SourceGenerator.Tests.Tests;

public class CustomDeconstructorCommandTests : TestBase
{
    [Test]
    public async Task Deconstructor_ColonPair()
    {
        var app = CreateFullApp();
        var result = app.Run("deconstructor", "--dict", "key:value");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("key");
        await Assert.That(result.Output).Contains("value");
    }
}
