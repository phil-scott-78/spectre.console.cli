namespace Spectre.Console.Cli.SourceGenerator.Tests.Tests;

/// <summary>
/// Tests init-only properties with UnsafeAccessor support in AOT scenarios.
/// </summary>
public class InitOnlyCommandTests : TestBase
{
    [Test]
    public async Task InitOnly_BasicExecution()
    {
        var app = CreateFullApp();
        var result = app.Run("initonly", "TestName");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Name: TestName");
        await Assert.That(result.Output).Contains("Age: 0");
        await Assert.That(result.Output).Contains("Verbose: False");
    }

    [Test]
    public async Task InitOnly_WithAgeOption()
    {
        var app = CreateFullApp();
        var result = app.Run("initonly", "TestName", "--age", "30");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Name: TestName");
        await Assert.That(result.Output).Contains("Age: 30");
    }

    [Test]
    public async Task InitOnly_WithTitleOption()
    {
        var app = CreateFullApp();
        var result = app.Run("initonly", "TestName", "--title", "Mr.");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Name: TestName");
        await Assert.That(result.Output).Contains("Title: Mr.");
    }

    [Test]
    public async Task InitOnly_WithAllOptions()
    {
        var app = CreateFullApp();
        var result = app.Run("initonly", "TestName", "-a", "25", "-t", "Dr.", "-v");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Name: TestName");
        await Assert.That(result.Output).Contains("Age: 25");
        await Assert.That(result.Output).Contains("Title: Dr.");
        await Assert.That(result.Output).Contains("Verbose: True");
    }

    [Test]
    public async Task InitOnly_MissingRequiredArg_Fails()
    {
        var app = CreateFullApp();
        var result = app.Run("initonly");

        await Assert.That(result.ExitCode).IsEqualTo(-1);
    }
}
