using System.ComponentModel;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Settings;

/// <summary>
/// Settings for testing async command execution in AOT scenarios.
/// </summary>
public sealed class AsyncSettings : CommandSettings
{
    [CommandArgument(0, "<MESSAGE>")]
    [Description("The message to display")]
    public string Message { get; set; } = string.Empty;

    [CommandOption("-d|--delay <MS>")]
    [Description("Delay in milliseconds before completing")]
    [DefaultValue(100)]
    public int DelayMs { get; set; } = 100;
}
