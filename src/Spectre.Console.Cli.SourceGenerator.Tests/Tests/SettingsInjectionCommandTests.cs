namespace Spectre.Console.Cli.SourceGenerator.Tests.Tests;

public class SettingsInjectionCommandTests : TestBase
{
    [Test]
    public async Task SettingsDI_BasicExecutionWithDI()
    {
        var app = CreateFullApp();
        var result = app.Run("settingsdi", "TestMessage");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Settings DI Test");
        await Assert.That(result.Output).Contains("Original message: TestMessage");
        await Assert.That(result.Output).Contains("Hello, TestMessage!");
    }

    [Test]
    public async Task SettingsDI_WithUppercaseOption()
    {
        var app = CreateFullApp();
        var result = app.Run("settingsdi", "World", "-u");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Uppercase: True");
        await Assert.That(result.Output).Contains("HELLO, WORLD!");
    }

    [Test]
    public async Task SettingsDI_LongOptionForm()
    {
        var app = CreateFullApp();
        var result = app.Run("settingsdi", "Spectre", "--uppercase");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Uppercase: True");
        await Assert.That(result.Output).Contains("HELLO, SPECTRE!");
    }
}
