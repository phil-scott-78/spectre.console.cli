using System.ComponentModel;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Settings;

/// <summary>
/// Settings for testing dependency injection resolution via RegisterKnownTypes.
/// </summary>
public sealed class InjectionTestSettings : CommandSettings
{
    [CommandArgument(0, "<NAME>")]
    [Description("The name to greet")]
    public string Name { get; set; } = string.Empty;
}
