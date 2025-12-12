using Spectre.Console.Cli.SourceGenerator.Tests.Settings;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Commands;

/// <summary>
/// Subcommand that adds an item - tests command hierarchy in AOT scenarios.
/// </summary>
public sealed class SubAddCommand : Command<SubAddSettings>
{
    private readonly IAnsiConsole _console;

    public SubAddCommand(IAnsiConsole console)
    {
        _console = console;
    }

    protected override int Execute(CommandContext context, SubAddSettings settings, CancellationToken cancellationToken)
    {
        _console.MarkupLine("[green]Sub Add Command[/]");

        if (settings.Verbose)
        {
            _console.MarkupLine("[grey]Verbose mode enabled[/]");
        }

        _console.MarkupLine($"[blue]Adding item:[/] {settings.Item}");

        if (settings.Force)
        {
            _console.MarkupLine("[yellow]Force mode enabled - will overwrite if exists[/]");
        }

        _console.MarkupLine($"[green]Successfully added:[/] {settings.Item}");

        return 0;
    }
}
