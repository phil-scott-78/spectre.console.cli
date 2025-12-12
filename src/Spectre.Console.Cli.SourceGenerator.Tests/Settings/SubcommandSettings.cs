using System.ComponentModel;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Settings;

/// <summary>
/// Settings for the sub add command.
/// </summary>
public sealed class SubAddSettings : CommandSettings
{
    [CommandArgument(0, "<ITEM>")]
    [Description("The item to add")]
    public string Item { get; set; } = string.Empty;

    [CommandOption("-f|--force")]
    [Description("Force add even if item exists")]
    public bool Force { get; set; }

    [CommandOption("-v|--verbose")]
    [Description("Enable verbose output")]
    public bool Verbose { get; set; }
}

/// <summary>
/// Settings for the sub remove command.
/// </summary>
public sealed class SubRemoveSettings : CommandSettings
{
    [CommandArgument(0, "<ITEM>")]
    [Description("The item to remove")]
    public string Item { get; set; } = string.Empty;

    [CommandOption("--all")]
    [Description("Remove all matching items")]
    public bool All { get; set; }

    [CommandOption("-v|--verbose")]
    [Description("Enable verbose output")]
    public bool Verbose { get; set; }
}

/// <summary>
/// Settings for the sub list command.
/// </summary>
public sealed class SubListSettings : CommandSettings
{
    [CommandOption("--filter <PATTERN>")]
    [Description("Filter items by pattern")]
    public string? Filter { get; set; }

    [CommandOption("-v|--verbose")]
    [Description("Enable verbose output")]
    public bool Verbose { get; set; }
}
