namespace Spectre.Console.Cli.SourceGenerator.Tests.Tests;

public class FloatingPointCommandTests : TestBase
{
    [Test]
    public async Task Floats_DefaultValues()
    {
        var app = CreateFullApp();
        var result = app.Run("floats");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("FloatValue: 1.5");
        await Assert.That(result.Output).Contains("DoubleValue: 2.75");
        await Assert.That(result.Output).Contains("DecimalValue: 3.14159");
    }

    [Test]
    public async Task Floats_NegativeAndLargeValues()
    {
        var app = CreateFullApp();
        var result = app.Run("floats");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("NegativeFloat: -0.5");
        await Assert.That(result.Output).Contains("LargeDouble: 1234567.89");
    }
}
