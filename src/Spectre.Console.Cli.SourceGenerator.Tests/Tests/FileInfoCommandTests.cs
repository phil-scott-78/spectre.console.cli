namespace Spectre.Console.Cli.SourceGenerator.Tests.Tests;

public class FileInfoCommandTests : TestBase
{
    [Test]
    public async Task FileInfo_DefaultFilePath()
    {
        var app = CreateFullApp();
        var result = app.Run("fileinfo");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Output).Contains("FileInfo command executed");
    }
}
