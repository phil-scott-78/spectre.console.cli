using Spectre.Console.Cli.SourceGenerator.Tests.Settings;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Commands;

/// <summary>
/// Subcommand that lists items - tests command hierarchy in AOT scenarios.
/// </summary>
public sealed class SubListCommand : Command<SubListSettings>
{
    private readonly IAnsiConsole _console;

    public SubListCommand(IAnsiConsole console)
    {
        _console = console;
    }

    protected override int Execute(CommandContext context, SubListSettings settings, CancellationToken cancellationToken)
    {
        _console.MarkupLine("[green]Sub List Command[/]");

        if (settings.Verbose)
        {
            _console.MarkupLine("[grey]Verbose mode enabled[/]");
        }

        if (!string.IsNullOrEmpty(settings.Filter))
        {
            _console.MarkupLine($"[blue]Filter:[/] {settings.Filter}");
        }

        // Simulate listing items
        _console.MarkupLine("[blue]Items:[/]");
        _console.MarkupLine("  - item1");
        _console.MarkupLine("  - item2");
        _console.MarkupLine("  - item3");

        return 0;
    }
}
