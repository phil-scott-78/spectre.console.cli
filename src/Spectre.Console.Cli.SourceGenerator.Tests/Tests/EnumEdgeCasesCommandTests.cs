namespace Spectre.Console.Cli.SourceGenerator.Tests.Tests;

public class EnumEdgeCasesCommandTests : TestBase
{
    [Test]
    public async Task Enum_LowercaseParsing()
    {
        var app = CreateFullApp();
        var result = app.Run("enumtest", "--day", "monday");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Day: Monday");
    }

    [Test]
    public async Task Enum_UppercaseParsing()
    {
        var app = CreateFullApp();
        var result = app.Run("enumtest", "--day", "WEDNESDAY");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Day: Wednesday");
    }

    [Test]
    public async Task Enum_MixedCaseParsing()
    {
        var app = CreateFullApp();
        var result = app.Run("enumtest", "--day", "FrIdAy");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Day: Friday");
    }

    [Test]
    public async Task Enum_CustomEnumWithDefault()
    {
        var app = CreateFullApp();
        var result = app.Run("enumtest", "--day", "Monday");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Priority: Normal");
    }

    [Test]
    public async Task Enum_CustomEnumParsing()
    {
        var app = CreateFullApp();
        var result = app.Run("enumtest", "--day", "Monday", "--priority", "Critical");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Priority: Critical");
    }

    [Test]
    public async Task Enum_NullableStatusProvided()
    {
        var app = CreateFullApp();
        var result = app.Run("enumtest", "--day", "Monday", "--status", "Active");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Status: Active");
    }

    [Test]
    public async Task Enum_NullableStatusNull()
    {
        var app = CreateFullApp();
        var result = app.Run("enumtest", "--day", "Monday");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Status: (null)");
    }

    [Test]
    public async Task Enum_InvalidValue_Fails()
    {
        var app = CreateFullApp();
        var result = app.Run("enumtest", "--day", "NotADay");

        await Assert.That(result.ExitCode).IsEqualTo(-1);
    }
}
