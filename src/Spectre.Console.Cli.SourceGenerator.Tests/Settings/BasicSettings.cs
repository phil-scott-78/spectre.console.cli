using System.ComponentModel;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Settings;

/// <summary>
/// Tests typical command/option/argument configurations.
/// </summary>
public sealed class BasicSettings : CommandSettings
{
    [CommandArgument(0, "<NAME>")]
    [Description("The name argument")]
    public string Name { get; set; } = string.Empty;

    [CommandArgument(1, "[COUNT]")]
    [Description("Optional count argument")]
    public int? Count { get; set; }

    [CommandOption("-v|--verbose")]
    [Description("Enable verbose output")]
    public bool Verbose { get; set; }

    [CommandOption("-t|--tags <TAGS>")]
    [Description("Tags to apply")]
    public string[]? Tags { get; set; }

    [CommandOption("--numbers <NUMS>")]
    [Description("Numbers to process")]
    public int[]? Numbers { get; set; }

    [CommandOption("--day <DAY>")]
    [Description("Day of the week")]
    public DayOfWeek? Day { get; set; }
}