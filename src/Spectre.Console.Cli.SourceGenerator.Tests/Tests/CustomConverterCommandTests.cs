namespace Spectre.Console.Cli.SourceGenerator.Tests.Tests;

public class CustomConverterCommandTests : TestBase
{
    [Test]
    public async Task Converter_PointConversion()
    {
        var app = CreateFullApp();
        var result = app.Run("converter", "--point", "10,20");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("(10, 20)");
    }
}
