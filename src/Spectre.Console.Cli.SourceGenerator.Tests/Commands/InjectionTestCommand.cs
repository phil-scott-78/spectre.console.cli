using Spectre.Console.Cli.SourceGenerator.Tests.Services;
using Spectre.Console.Cli.SourceGenerator.Tests.Settings;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Commands;

/// <summary>
/// Command that tests dependency injection via constructor injection.
/// This verifies that types registered via RegisterKnownTypes can be resolved.
/// </summary>
public sealed class InjectionTestCommand : Command<InjectionTestSettings>
{
    private readonly IAnsiConsole _console;
    private readonly ITestService _service;

    public InjectionTestCommand(IAnsiConsole console, ITestService service)
    {
        _console = console;
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    protected override int Execute(CommandContext context, InjectionTestSettings settings, CancellationToken cancellationToken)
    {
        _console.MarkupLine("[green]Dependency Injection Test[/]");
        _console.MarkupLine($"[blue]Service result:[/] {_service.GetMessage(settings.Name)}");
        return 0;
    }
}
