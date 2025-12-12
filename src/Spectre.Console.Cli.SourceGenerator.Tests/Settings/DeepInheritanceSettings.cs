using System.ComponentModel;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Settings;

/// <summary>
/// Level 1 base settings class for testing deep inheritance in AOT scenarios.
/// </summary>
public abstract class Level1Settings : CommandSettings
{
    [CommandOption("--level1")]
    [Description("Level 1 flag (from base)")]
    public bool Level1Flag { get; set; }

    [CommandOption("--shared <VALUE>")]
    [Description("Shared option across all levels")]
    public string SharedOption { get; set; } = string.Empty;
}

/// <summary>
/// Level 2 settings class that inherits from Level1Settings.
/// </summary>
public abstract class Level2Settings : Level1Settings
{
    [CommandOption("--level2")]
    [Description("Level 2 flag (from intermediate)")]
    public bool Level2Flag { get; set; }

    [CommandOption("--count <COUNT>")]
    [Description("A count option at level 2")]
    [DefaultValue(1)]
    public int Count { get; set; } = 1;
}

/// <summary>
/// Level 3 settings class that inherits from Level2Settings.
/// Tests that properties from all inheritance levels are properly discovered.
/// </summary>
public sealed class Level3Settings : Level2Settings
{
    [CommandArgument(0, "<NAME>")]
    [Description("The name argument")]
    public string Name { get; set; } = string.Empty;

    [CommandOption("--level3")]
    [Description("Level 3 flag (from leaf)")]
    public bool Level3Flag { get; set; }
}
