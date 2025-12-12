namespace Spectre.Console.Cli.SourceGenerator.Tests.Tests;

public class TypeConverterCommandTests : TestBase
{
    [Test]
    public async Task Types_UriDefaultValue()
    {
        var app = CreateFullApp();
        var result = app.Run("types");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Uri: https://example.com");
    }

    [Test]
    public async Task Types_UriParsing()
    {
        var app = CreateFullApp();
        var result = app.Run("types", "--uri", "https://github.com/spectreconsole");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Uri: https://github.com/spectreconsole");
    }

    [Test]
    public async Task Types_GuidParsing()
    {
        var app = CreateFullApp();
        var result = app.Run("types", "--guid", "12345678-1234-1234-1234-123456789abc");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Guid: 12345678-1234-1234-1234-123456789abc");
    }

    [Test]
    public async Task Types_TimeSpanDefaultValue()
    {
        var app = CreateFullApp();
        var result = app.Run("types");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Duration: 00:30:00");
    }

    [Test]
    public async Task Types_TimeSpanParsing()
    {
        var app = CreateFullApp();
        var result = app.Run("types", "--duration", "02:30:45");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Duration: 02:30:45");
    }
}
