using Spectre.Console.Cli.SourceGenerator.Tests.Settings;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Commands;

/// <summary>
/// Command that tests async command execution in AOT scenarios.
/// </summary>
public sealed class AsyncTestCommand(IAnsiConsole console) : AsyncCommand<AsyncSettings>
{
    protected override async Task<int> ExecuteAsync(CommandContext context, AsyncSettings settings, CancellationToken cancellationToken)
    {
        console.MarkupLine("[blue]Starting async operation...[/]");
        await Task.Delay(settings.DelayMs, cancellationToken);
        console.MarkupLine($"[green]Message:[/] {settings.Message}");
        console.MarkupLine("[blue]Async operation completed.[/]");

        return 0;
    }
}
