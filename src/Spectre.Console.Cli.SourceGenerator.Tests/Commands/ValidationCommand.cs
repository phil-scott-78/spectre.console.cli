using Spectre.Console.Cli.SourceGenerator.Tests.Settings;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Commands;

/// <summary>
/// Command that tests custom validation attributes in AOT scenarios.
/// </summary>
public sealed class ValidationCommand : Command<ValidationSettings>
{
    private readonly IAnsiConsole _console;

    public ValidationCommand(IAnsiConsole console)
    {
        _console = console;
    }

    protected override int Execute(CommandContext context, ValidationSettings settings, CancellationToken cancellationToken)
    {
        _console.MarkupLine("[green]Validation Attributes Test[/]");
        _console.MarkupLine("[grey]All validations passed![/]");
        _console.WriteLine();

        _console.MarkupLine($"[blue]Age:[/] {settings.Age}");

        if (!string.IsNullOrEmpty(settings.Email))
        {
            _console.MarkupLine($"[blue]Email:[/] {settings.Email}");
        }

        if (!string.IsNullOrEmpty(settings.Name))
        {
            _console.MarkupLine($"[blue]Name:[/] {settings.Name}");
        }

        return 0;
    }
}
