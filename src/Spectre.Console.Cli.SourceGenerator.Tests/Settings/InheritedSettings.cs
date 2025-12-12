using System.ComponentModel;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Settings;

/// <summary>
/// Base settings class for testing inheritance in AOT scenarios.
/// </summary>
public abstract class BaseSettings : CommandSettings
{
    [CommandOption("-v|--verbose")]
    [Description("Enable verbose output")]
    public bool Verbose { get; set; }

    [CommandOption("--debug")]
    [Description("Enable debug mode")]
    public bool Debug { get; set; }
}

/// <summary>
/// Derived settings class that inherits from BaseSettings.
/// Tests that inherited properties are properly discovered in AOT scenarios.
/// </summary>
public sealed class DerivedSettings : BaseSettings
{
    [CommandArgument(0, "<NAME>")]
    [Description("The name to process")]
    public string Name { get; set; } = string.Empty;

    [CommandOption("-c|--count <COUNT>")]
    [Description("Number of times to repeat")]
    [DefaultValue(1)]
    public int Count { get; set; } = 1;
}
