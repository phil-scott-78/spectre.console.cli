namespace Spectre.Console.Cli.SourceGenerator.Tests.Tests;

public class InheritanceCommandTests : TestBase
{
    [Test]
    public async Task Inherit_DerivedProperties()
    {
        var app = CreateFullApp();
        var result = app.Run("inherit", "TestName", "-v");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Name: TestName");
        await Assert.That(result.Output).Contains("Verbose (inherited): True");
    }

    [Test]
    public async Task Inherit_BaseProperties()
    {
        var app = CreateFullApp();
        var result = app.Run("inherit", "TestName", "--debug");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("Debug (inherited): True");
    }
}
