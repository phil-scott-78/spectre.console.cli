using Spectre.Console.Cli.SourceGenerator.Tests.Settings;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Commands;

/// <summary>
/// Command that tests custom PairDeconstructor in AOT scenarios.
/// </summary>
public sealed class CustomDeconstructorCommand : Command<CustomDeconstructorSettings>
{
    private readonly IAnsiConsole _console;

    public CustomDeconstructorCommand(IAnsiConsole console)
    {
        _console = console;
    }

    protected override int Execute(CommandContext context, CustomDeconstructorSettings settings, CancellationToken cancellationToken)
    {
        _console.MarkupLine("[green]Custom PairDeconstructor Test[/]");

        if (settings.KeyValues is null || settings.KeyValues.Count == 0)
        {
            _console.MarkupLine("[yellow]No key:value pairs provided[/]");
            return 0;
        }

        var table = new Table();
        table.AddColumn("Key");
        table.AddColumn("Value");

        foreach (var kvp in settings.KeyValues)
        {
            table.AddRow(kvp.Key, kvp.Value);
        }

        _console.Write(table);

        if (settings.Verbose)
        {
            _console.MarkupLine($"[grey]Total pairs: {settings.KeyValues.Count}[/]");
        }

        return 0;
    }
}
