using Spectre.Console.Cli.SourceGenerator.Tests.Settings;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Commands;

public sealed class BasicCommand(IAnsiConsole console) : Command<BasicSettings>
{
    protected override int Execute(CommandContext context, BasicSettings settings, CancellationToken cancellationToken)
    {
        console.MarkupLine("[green]Basic command executed[/]");

        console.MarkupLine($"[blue]Name:[/] {settings.Name}");

        if (settings.Count.HasValue)
        {
            console.MarkupLine($"[blue]Count:[/] {settings.Count.Value}");
        }

        console.MarkupLine($"[blue]Verbose:[/] {settings.Verbose}");

        if (settings.Tags is not null)
        {
            console.MarkupLine($"[blue]Tags:[/] [[{string.Join(", ", settings.Tags)}]]");
        }

        if (settings.Numbers is not null)
        {
            console.MarkupLine($"[blue]Numbers:[/] [[{string.Join(", ", settings.Numbers)}]]");
        }

        if (settings.Day.HasValue)
        {
            console.MarkupLine($"[blue]Day:[/] {settings.Day.Value}");
        }

        return 0;
    }
}