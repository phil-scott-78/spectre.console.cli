namespace Spectre.Console.Cli.SourceGenerator.Tests.Tests;

public class BasicCommandTests : TestBase
{
    [Test]
    public async Task Basic_SimpleArgument()
    {
        var app = CreateFullApp();
        var result = app.Run("basic", "TestName");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Name: TestName");
        await Assert.That(result.Output).Contains("Verbose: False");
    }

    [Test]
    public async Task Basic_WithVerboseFlag()
    {
        var app = CreateFullApp();
        var result = app.Run("basic", "TestName", "-v");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Name: TestName");
        await Assert.That(result.Output).Contains("Verbose: True");
    }

    [Test]
    public async Task Basic_WithCountArgument()
    {
        var app = CreateFullApp();
        var result = app.Run("basic", "TestName", "5");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Name: TestName");
        await Assert.That(result.Output).Contains("Count: 5");
    }

    [Test]
    public async Task Basic_WithTagsOption()
    {
        var app = CreateFullApp();
        var result = app.Run("basic", "TestName", "--tags", "a", "b", "c");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Tags: [a, b, c]");
    }

    [Test]
    public async Task Basic_WithNumbersOption()
    {
        var app = CreateFullApp();
        var result = app.Run("basic", "TestName", "--numbers", "1", "2", "3");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Numbers: [1, 2, 3]");
    }

    [Test]
    public async Task Basic_WithDayEnum()
    {
        var app = CreateFullApp();
        var result = app.Run("basic", "TestName", "--day", "Monday");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Day: Monday");
    }

    [Test]
    public async Task Basic_MissingRequiredArg_Fails()
    {
        var app = CreateFullApp();
        var result = app.Run("basic");

        await Assert.That(result.ExitCode).IsEqualTo(-1);
    }
}
