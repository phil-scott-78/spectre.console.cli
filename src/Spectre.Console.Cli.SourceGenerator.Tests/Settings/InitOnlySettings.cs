using System.ComponentModel;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Settings;

/// <summary>
/// Tests init-only properties with UnsafeAccessor support.
/// </summary>
public sealed class InitOnlySettings : CommandSettings
{
    [CommandArgument(0, "<NAME>")]
    [Description("The name argument (init-only)")]
    public string Name { get; init; } = string.Empty;

    [CommandOption("-a|--age <AGE>")]
    [Description("The age option (init-only)")]
    public int Age { get; init; }

    [CommandOption("-t|--title <TITLE>")]
    [Description("Optional title (init-only)")]
    public string? Title { get; init; }

    [CommandOption("-v|--verbose")]
    [Description("Enable verbose output (regular setter for comparison)")]
    public bool Verbose { get; set; }
}
