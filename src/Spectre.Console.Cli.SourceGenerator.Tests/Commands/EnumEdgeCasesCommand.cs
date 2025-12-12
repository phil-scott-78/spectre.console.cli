using Spectre.Console.Cli.SourceGenerator.Tests.Settings;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Commands;

/// <summary>
/// Command that tests enum edge cases in AOT scenarios.
/// Tests case-insensitive parsing, custom enums, and nullable enums.
/// </summary>
public sealed class EnumEdgeCasesCommand : Command<EnumEdgeCasesSettings>
{
    private readonly IAnsiConsole _console;

    public EnumEdgeCasesCommand(IAnsiConsole console)
    {
        _console = console;
    }

    protected override int Execute(CommandContext context, EnumEdgeCasesSettings settings, CancellationToken cancellationToken)
    {
        _console.MarkupLine("[green]Enum Edge Cases Test[/]");
        _console.WriteLine();
        _console.MarkupLine($"[blue]Day:[/] {settings.Day}");
        _console.MarkupLine($"[blue]Priority:[/] {settings.Priority}");
        _console.MarkupLine($"[blue]Status:[/] {settings.Status?.ToString() ?? "(null)"}");

        return 0;
    }
}
