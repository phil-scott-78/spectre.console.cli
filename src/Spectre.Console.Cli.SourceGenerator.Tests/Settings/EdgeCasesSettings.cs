using System.ComponentModel;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Settings;

/// <summary>
/// Settings for testing edge cases in AOT scenarios:
/// - Multiple positioned arguments
/// - Hidden options
/// - Required options
/// - Array default values
/// </summary>
public sealed class EdgeCasesSettings : CommandSettings
{
    [CommandArgument(0, "<FIRST>")]
    [Description("First required argument")]
    public string First { get; set; } = string.Empty;

    [CommandArgument(1, "<SECOND>")]
    [Description("Second required argument (integer)")]
    public int Second { get; set; }

    [CommandArgument(2, "[THIRD]")]
    [Description("Third optional argument")]
    public string? Third { get; set; }

    [CommandArgument(3, "[FOURTH]")]
    [Description("Fourth optional argument")]
    public int? Fourth { get; set; }

    [CommandOption("--hidden", IsHidden = true)]
    [Description("A hidden option (not shown in help)")]
    public bool Hidden { get; set; }

    [CommandOption("--required <VALUE>", true)]
    [Description("A required option")]
    public string Required { get; set; } = string.Empty;

    [CommandOption("--days <DAYS>")]
    [Description("Days of the week")]
    [DefaultValue(new[] { DayOfWeek.Monday, DayOfWeek.Friday })]
    public DayOfWeek[]? Days { get; set; }

    [CommandOption("--tags <TAGS>")]
    [Description("Tags with default values")]
    [DefaultValue(new[] { "default", "tags" })]
    public string[]? Tags { get; set; }
}
