using Spectre.Console.Cli.SourceGenerator.Tests.Settings;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Commands;

public sealed class FileInfoCommand : Command<FileInfoSettings>
{
    private readonly IAnsiConsole _console;

    public FileInfoCommand(IAnsiConsole console)
    {
        _console = console;
    }

    protected override int Execute(CommandContext context, FileInfoSettings settings, CancellationToken cancellationToken)
    {
        _console.MarkupLine("[green]FileInfo command executed[/]");

        if (settings.File is not null)
        {
            _console.MarkupLine($"[blue]File:[/] {settings.File.FullName}");
        }

        if (settings.Directory is not null)
        {
            _console.MarkupLine($"[blue]Directory:[/] {settings.Directory.FullName}");
        }

        if (settings.Output is not null)
        {
            _console.MarkupLine($"[blue]Output:[/] {settings.Output.FullName}");
        }

        return 0;
    }
}