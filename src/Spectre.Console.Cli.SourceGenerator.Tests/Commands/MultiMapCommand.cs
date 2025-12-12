using Spectre.Console.Cli.SourceGenerator.Tests.Settings;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Commands;

public sealed class MultiMapCommand : Command<MultiMapSettings>
{
    private readonly IAnsiConsole _console;

    public MultiMapCommand(IAnsiConsole console)
    {
        _console = console;
    }

    protected override int Execute(CommandContext context, MultiMapSettings settings, CancellationToken cancellationToken)
    {
        _console.MarkupLine("[green]MultiMap command executed[/]");

        if (settings.Values is not null)
        {
            _console.MarkupLine($"[blue]Values (IDictionary):[/] {settings.Values.Count} entries");
            foreach (var kvp in settings.Values)
            {
                _console.MarkupLine($"  {kvp.Key} = {kvp.Value}");
            }
        }

        if (settings.Lookups is not null)
        {
            _console.MarkupLine($"[blue]Lookups (ILookup):[/] {settings.Lookups.Count} groups");
            foreach (var group in settings.Lookups)
            {
                _console.MarkupLine($"  {group.Key} = [[{string.Join(", ", group)}]]");
            }
        }

        if (settings.ReadOnlyValues is not null)
        {
            _console.MarkupLine($"[blue]ReadOnlyValues (IReadOnlyDictionary):[/] {settings.ReadOnlyValues.Count} entries");
            foreach (var kvp in settings.ReadOnlyValues)
            {
                _console.MarkupLine($"  {kvp.Key} = {kvp.Value}");
            }
        }

        return 0;
    }
}