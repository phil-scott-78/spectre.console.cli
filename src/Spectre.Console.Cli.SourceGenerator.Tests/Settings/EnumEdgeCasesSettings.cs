using System.ComponentModel;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Settings;

/// <summary>
/// Settings for testing enum edge cases in AOT scenarios.
/// Tests case-insensitive parsing, custom enums, and nullable enums.
/// </summary>
public sealed class EnumEdgeCasesSettings : CommandSettings
{
    [CommandOption("--day <DAY>")]
    [Description("Day of week (tests case-insensitive parsing)")]
    public DayOfWeek Day { get; set; }

    [CommandOption("--priority <PRIORITY>")]
    [Description("Priority level")]
    [DefaultValue(Priority.Normal)]
    public Priority Priority { get; set; }

    [CommandOption("--status <STATUS>")]
    [Description("Nullable status (optional)")]
    public Status? Status { get; set; }
}

/// <summary>
/// Custom enum for testing non-system enums in AOT.
/// </summary>
public enum Priority
{
    Low,
    Normal,
    High,
    Critical,
}

/// <summary>
/// Custom nullable enum for testing optional enum values in AOT.
/// </summary>
public enum Status
{
    Pending,
    Active,
    Completed,
    Cancelled,
}
