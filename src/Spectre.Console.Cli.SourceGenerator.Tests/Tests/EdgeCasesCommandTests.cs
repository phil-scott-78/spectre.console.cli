namespace Spectre.Console.Cli.SourceGenerator.Tests.Tests;

public class EdgeCasesCommandTests : TestBase
{
    [Test]
    public async Task Edge_MultipleArguments()
    {
        var app = CreateFullApp();
        var result = app.Run("edge", "first", "42", "--required", "reqval");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("First: first");
        await Assert.That(result.Output).Contains("Second: 42");
        await Assert.That(result.Output).Contains("Required: reqval");
    }

    [Test]
    public async Task Edge_OptionalThirdArgument()
    {
        var app = CreateFullApp();
        var result = app.Run("edge", "first", "42", "third", "--required", "reqval");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Third: third");
    }

    [Test]
    public async Task Edge_OptionalFourthArgument()
    {
        var app = CreateFullApp();
        var result = app.Run("edge", "first", "42", "third", "99", "--required", "reqval");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Fourth: 99");
    }

    [Test]
    public async Task Edge_HiddenOptionWorks()
    {
        var app = CreateFullApp();
        var result = app.Run("edge", "first", "42", "--required", "reqval", "--hidden");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Hidden: True");
    }

    [Test]
    public async Task Edge_ArrayDefaultValues()
    {
        var app = CreateFullApp();
        var result = app.Run("edge", "first", "42", "--required", "reqval");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Monday");
        await Assert.That(result.Output).Contains("Friday");
        await Assert.That(result.Output).Contains("default");
        await Assert.That(result.Output).Contains("tags");
    }

    [Test]
    public async Task Edge_MissingRequiredOption_Fails()
    {
        var app = CreateFullApp();
        var result = app.Run("edge", "first", "42");

        await Assert.That(result.ExitCode).IsEqualTo(-1);
    }
}
