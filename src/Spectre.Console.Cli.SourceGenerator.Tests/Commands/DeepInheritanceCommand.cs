using Spectre.Console.Cli.SourceGenerator.Tests.Settings;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Commands;

/// <summary>
/// Command that tests deep inheritance (3 levels) in AOT scenarios.
/// </summary>
public sealed class DeepInheritanceCommand : Command<Level3Settings>
{
    private readonly IAnsiConsole _console;

    public DeepInheritanceCommand(IAnsiConsole console)
    {
        _console = console;
    }

    protected override int Execute(CommandContext context, Level3Settings settings, CancellationToken cancellationToken)
    {
        _console.MarkupLine("[green]Deep Inheritance Test (3 Levels)[/]");
        _console.WriteLine();

        // Properties from Level 3 (leaf class)
        _console.MarkupLine($"[blue]Name (level 3):[/] {settings.Name}");
        _console.MarkupLine($"[blue]Level3Flag:[/] {settings.Level3Flag}");

        // Properties from Level 2 (intermediate class)
        _console.MarkupLine($"[blue]Level2Flag:[/] {settings.Level2Flag}");
        _console.MarkupLine($"[blue]Count (level 2):[/] {settings.Count}");

        // Properties from Level 1 (base class)
        _console.MarkupLine($"[blue]Level1Flag:[/] {settings.Level1Flag}");
        _console.MarkupLine($"[blue]SharedOption (level 1):[/] {(string.IsNullOrEmpty(settings.SharedOption) ? "(empty)" : settings.SharedOption)}");

        return 0;
    }
}
