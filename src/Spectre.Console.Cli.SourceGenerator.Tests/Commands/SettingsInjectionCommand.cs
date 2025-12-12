using Spectre.Console.Cli.SourceGenerator.Tests.Settings;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Commands;

/// <summary>
/// Command that uses settings with constructor dependency injection.
/// This verifies that settings classes with constructor parameters
/// can be resolved correctly in AOT scenarios.
/// </summary>
public sealed class SettingsInjectionCommand : Command<SettingsInjectionSettings>
{
    private readonly IAnsiConsole _console;

    public SettingsInjectionCommand(IAnsiConsole console)
    {
        _console = console;
    }

    protected override int Execute(CommandContext context, SettingsInjectionSettings settings, CancellationToken cancellationToken)
    {
        _console.MarkupLine("[green]Settings DI Test[/]");
        _console.WriteLine();
        _console.MarkupLine($"[blue]Original message:[/] {settings.Message}");
        _console.MarkupLine($"[blue]Uppercase:[/] {settings.Uppercase}");
        _console.MarkupLine($"[blue]Processed result:[/] {settings.GetProcessedMessage()}");

        return 0;
    }
}
