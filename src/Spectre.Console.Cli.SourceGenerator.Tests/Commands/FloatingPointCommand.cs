using System.Globalization;
using Spectre.Console.Cli.SourceGenerator.Tests.Settings;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Commands;

/// <summary>
/// Command that tests floating-point default values in AOT scenarios.
/// This verifies the InvariantCulture fix for float/double/decimal formatting.
/// </summary>
public sealed class FloatingPointCommand : Command<FloatingPointSettings>
{
    private readonly IAnsiConsole _console;

    public FloatingPointCommand(IAnsiConsole console)
    {
        _console = console;
    }

    protected override int Execute(CommandContext context, FloatingPointSettings settings, CancellationToken cancellationToken)
    {
        _console.MarkupLine("[green]Floating-Point Default Values Test[/]");
        _console.WriteLine();

        // Use InvariantCulture to ensure consistent output regardless of system locale
        _console.MarkupLine($"[blue]FloatValue:[/] {settings.FloatValue.ToString(CultureInfo.InvariantCulture)}");
        _console.MarkupLine($"[blue]DoubleValue:[/] {settings.DoubleValue.ToString(CultureInfo.InvariantCulture)}");
        _console.MarkupLine($"[blue]DecimalValue:[/] {settings.DecimalValue.ToString(CultureInfo.InvariantCulture)}");
        _console.MarkupLine($"[blue]NegativeFloat:[/] {settings.NegativeFloat.ToString(CultureInfo.InvariantCulture)}");
        _console.MarkupLine($"[blue]LargeDouble:[/] {settings.LargeDouble.ToString(CultureInfo.InvariantCulture)}");

        return 0;
    }
}
