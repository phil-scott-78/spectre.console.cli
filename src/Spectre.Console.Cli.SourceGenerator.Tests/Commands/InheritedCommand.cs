using Spectre.Console.Cli.SourceGenerator.Tests.Settings;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Commands;

/// <summary>
/// Command that tests settings inheritance in AOT scenarios.
/// </summary>
public sealed class InheritedCommand : Command<DerivedSettings>
{
    private readonly IAnsiConsole _console;

    public InheritedCommand(IAnsiConsole console)
    {
        _console = console;
    }

    protected override int Execute(CommandContext context, DerivedSettings settings, CancellationToken cancellationToken)
    {
        _console.MarkupLine("[green]Settings Inheritance Test[/]");

        // Properties from derived class
        _console.MarkupLine($"[blue]Name:[/] {settings.Name}");
        _console.MarkupLine($"[blue]Count:[/] {settings.Count}");

        // Properties from base class (inherited)
        _console.MarkupLine($"[blue]Verbose (inherited):[/] {settings.Verbose}");
        _console.MarkupLine($"[blue]Debug (inherited):[/] {settings.Debug}");

        if (settings.Verbose)
        {
            _console.WriteLine();
            _console.MarkupLine("[grey]Repeating name...[/]");
            for (var i = 0; i < settings.Count; i++)
            {
                _console.MarkupLine($"[yellow]{i + 1}.[/] {settings.Name}");
            }
        }

        return 0;
    }
}
