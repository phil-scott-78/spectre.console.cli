using Spectre.Console.Cli.SourceGenerator.Tests.Services;
using Spectre.Console.Cli.SourceGenerator.Tests.Settings;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Commands;

/// <summary>
/// Command that uses a service which depends on Settings via DI.
/// This verifies that the AOT generator correctly handles the case
/// where a service receives Settings as a constructor dependency.
/// </summary>
public sealed class ServiceWithSettingsCommand : Command<ServiceWithSettingsSettings>
{
    private readonly IAnsiConsole _console;
    private readonly ISettingsAwareService _service;

    public ServiceWithSettingsCommand(IAnsiConsole console, ISettingsAwareService service)
    {
        _console = console;
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    protected override int Execute(CommandContext context, ServiceWithSettingsSettings settings, CancellationToken cancellationToken)
    {
        _console.MarkupLine("[green]Service with Settings DI Test[/]");
        _console.WriteLine();
        _console.MarkupLine($"[blue]Message:[/] {settings.Message}");
        _console.MarkupLine($"[blue]Prefix:[/] {settings.Prefix ?? "(none)"}");
        _console.MarkupLine($"[blue]Formatted:[/] {_service.FormatMessage()}");

        return 0;
    }
}
