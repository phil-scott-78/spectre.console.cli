using Spectre.Console.Cli.SourceGenerator.Tests.Settings;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Commands;

/// <summary>
/// Command that tests edge cases in AOT scenarios:
/// - Multiple positioned arguments
/// - Hidden options
/// - Required options
/// - Array default values
/// </summary>
public sealed class EdgeCasesCommand : Command<EdgeCasesSettings>
{
    private readonly IAnsiConsole _console;

    public EdgeCasesCommand(IAnsiConsole console)
    {
        _console = console;
    }

    protected override int Execute(CommandContext context, EdgeCasesSettings settings, CancellationToken cancellationToken)
    {
        _console.MarkupLine("[green]Edge Cases Test[/]");
        _console.WriteLine();

        // Multiple positioned arguments
        _console.MarkupLine("[yellow]Arguments:[/]");
        _console.MarkupLine($"  [blue]First:[/] {settings.First}");
        _console.MarkupLine($"  [blue]Second:[/] {settings.Second}");
        _console.MarkupLine($"  [blue]Third:[/] {settings.Third ?? "(not provided)"}");
        _console.MarkupLine($"  [blue]Fourth:[/] {(settings.Fourth.HasValue ? settings.Fourth.Value.ToString() : "(not provided)")}");
        _console.WriteLine();

        // Required option
        _console.MarkupLine("[yellow]Options:[/]");
        _console.MarkupLine($"  [blue]Required:[/] {settings.Required}");
        _console.MarkupLine($"  [blue]Hidden:[/] {settings.Hidden}");
        _console.WriteLine();

        // Array default values
        _console.MarkupLine("[yellow]Array defaults:[/]");
        if (settings.Days is not null)
        {
            _console.MarkupLine($"  [blue]Days:[/] [[{string.Join(", ", settings.Days)}]]");
        }

        if (settings.Tags is not null)
        {
            _console.MarkupLine($"  [blue]Tags:[/] [[{string.Join(", ", settings.Tags)}]]");
        }

        return 0;
    }
}
