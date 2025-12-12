namespace Spectre.Console.Cli.SourceGenerator.Tests.Tests;

public class DeepInheritanceCommandTests : TestBase
{
    [Test]
    public async Task Deep_AllThreeLevels()
    {
        var app = CreateFullApp();
        var result = app.Run("deep", "TestName", "--level1", "--level2", "--level3");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Level1Flag: True");
        await Assert.That(result.Output).Contains("Level2Flag: True");
        await Assert.That(result.Output).Contains("Level3Flag: True");
    }

    [Test]
    public async Task Deep_Level1Only()
    {
        var app = CreateFullApp();
        var result = app.Run("deep", "TestName", "--level1");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Level1Flag: True");
        await Assert.That(result.Output).Contains("Level2Flag: False");
        await Assert.That(result.Output).Contains("Level3Flag: False");
    }

    [Test]
    public async Task Deep_SharedOptionFromLevel1()
    {
        var app = CreateFullApp();
        var result = app.Run("deep", "TestName", "--shared", "SharedValue");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("SharedOption (level 1): SharedValue");
    }
}
