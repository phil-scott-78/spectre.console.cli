using Spectre.Console.Cli.SourceGenerator.Tests.Settings;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Commands;

/// <summary>
/// Subcommand that removes an item - tests command hierarchy in AOT scenarios.
/// </summary>
public sealed class SubRemoveCommand : Command<SubRemoveSettings>
{
    private readonly IAnsiConsole _console;

    public SubRemoveCommand(IAnsiConsole console)
    {
        _console = console;
    }

    protected override int Execute(CommandContext context, SubRemoveSettings settings, CancellationToken cancellationToken)
    {
        _console.MarkupLine("[green]Sub Remove Command[/]");

        if (settings.Verbose)
        {
            _console.MarkupLine("[grey]Verbose mode enabled[/]");
        }

        _console.MarkupLine($"[blue]Removing item:[/] {settings.Item}");

        if (settings.All)
        {
            _console.MarkupLine("[yellow]Removing all matching items[/]");
        }

        _console.MarkupLine($"[green]Successfully removed:[/] {settings.Item}");

        return 0;
    }
}
