using Spectre.Console.Cli.SourceGenerator.Tests.Settings;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Commands;

public sealed class FlagValueCommand : Command<FlagValueSettings>
{
    private readonly IAnsiConsole _console;

    public FlagValueCommand(IAnsiConsole console)
    {
        _console = console;
    }

    protected override int Execute(CommandContext context, FlagValueSettings settings, CancellationToken cancellationToken)
    {
        _console.MarkupLine("[green]FlagValue command executed[/]");

        if (settings.Port is not null)
        {
            _console.MarkupLine($"[blue]Port:[/] IsSet={settings.Port.IsSet}, Value={settings.Port.Value}");
        }
        else
        {
            _console.MarkupLine("[blue]Port:[/] not provided");
        }

        if (settings.Timeout is not null)
        {
            _console.MarkupLine($"[blue]Timeout:[/] IsSet={settings.Timeout.IsSet}, Value={settings.Timeout.Value}");
        }
        else
        {
            _console.MarkupLine("[blue]Timeout:[/] not provided");
        }

        _console.MarkupLine($"[blue]Verbose:[/] {settings.Verbose}");

        return 0;
    }
}