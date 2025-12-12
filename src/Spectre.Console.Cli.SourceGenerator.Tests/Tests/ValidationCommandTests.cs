namespace Spectre.Console.Cli.SourceGenerator.Tests.Tests;

public class ValidationCommandTests : TestBase
{
    [Test]
    public async Task Validate_ValidAge()
    {
        var app = CreateFullApp();
        var result = app.Run("validate", "--age", "25");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Age: 25");
    }

    [Test]
    public async Task Validate_InvalidAgeTooHigh_Fails()
    {
        var app = CreateFullApp();
        var result = app.Run("validate", "--age", "200");

        await Assert.That(result.ExitCode).IsEqualTo(-1);
    }

    [Test]
    public async Task Validate_ValidEmail()
    {
        var app = CreateFullApp();
        var result = app.Run("validate", "--age", "25", "--email", "test@example.com");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Email: test@example.com");
    }

    [Test]
    public async Task Validate_InvalidEmail_Fails()
    {
        var app = CreateFullApp();
        var result = app.Run("validate", "--age", "25", "--email", "invalid");

        await Assert.That(result.ExitCode).IsEqualTo(-1);
    }
}
