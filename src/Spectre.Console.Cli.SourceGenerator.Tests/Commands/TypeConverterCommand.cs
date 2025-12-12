using Spectre.Console.Cli.SourceGenerator.Tests.Settings;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Commands;

/// <summary>
/// Command that tests built-in type converters (Uri, Guid, TimeSpan) in AOT scenarios.
/// </summary>
public sealed class TypeConverterCommand : Command<TypeConverterSettings>
{
    private readonly IAnsiConsole _console;

    public TypeConverterCommand(IAnsiConsole console)
    {
        _console = console;
    }

    protected override int Execute(CommandContext context, TypeConverterSettings settings, CancellationToken cancellationToken)
    {
        _console.MarkupLine("[green]Type Converter Test[/]");
        _console.WriteLine();
        _console.MarkupLine($"[blue]Uri:[/] {settings.Uri}");
        _console.MarkupLine($"[blue]Guid:[/] {settings.Id?.ToString() ?? "(null)"}");
        _console.MarkupLine($"[blue]Duration:[/] {settings.Duration}");

        return 0;
    }
}
