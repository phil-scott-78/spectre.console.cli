using Spectre.Console.Cli.SourceGenerator.Tests.Settings;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Commands;

/// <summary>
/// Command that tests custom TypeConverter in AOT scenarios.
/// </summary>
public sealed class CustomConverterCommand : Command<CustomConverterSettings>
{
    private readonly IAnsiConsole _console;

    public CustomConverterCommand(IAnsiConsole console)
    {
        _console = console;
    }

    protected override int Execute(CommandContext context, CustomConverterSettings settings, CancellationToken cancellationToken)
    {
        _console.MarkupLine("[green]Custom TypeConverter Test[/]");
        _console.MarkupLine($"[blue]Point:[/] {settings.Location}");
        _console.MarkupLine($"[blue]X coordinate:[/] {settings.Location.X}");
        _console.MarkupLine($"[blue]Y coordinate:[/] {settings.Location.Y}");

        if (settings.Verbose)
        {
            _console.MarkupLine("[grey]Verbose mode enabled[/]");
        }

        return 0;
    }
}
