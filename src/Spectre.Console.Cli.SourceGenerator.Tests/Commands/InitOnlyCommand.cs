using Spectre.Console.Cli.SourceGenerator.Tests.Settings;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Commands;

public sealed class InitOnlyCommand(IAnsiConsole console) : Command<InitOnlySettings>
{
    protected override int Execute(CommandContext context, InitOnlySettings settings, CancellationToken cancellationToken)
    {
        console.MarkupLine("[green]Init-only command executed[/]");

        console.MarkupLine($"[blue]Name:[/] {settings.Name}");
        console.MarkupLine($"[blue]Age:[/] {settings.Age}");

        if (settings.Title is not null)
        {
            console.MarkupLine($"[blue]Title:[/] {settings.Title}");
        }

        console.MarkupLine($"[blue]Verbose:[/] {settings.Verbose}");

        return 0;
    }
}
