using System.ComponentModel;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Settings;

/// <summary>
/// Settings for testing built-in type converters in AOT scenarios.
/// Tests Uri, Guid, and TimeSpan conversions.
/// </summary>
public sealed class TypeConverterSettings : CommandSettings
{
    [CommandOption("--uri <URI>")]
    [Description("A URI value")]
    [DefaultValue("https://example.com")]
    public Uri? Uri { get; set; }

    [CommandOption("--guid <GUID>")]
    [Description("A GUID value")]
    public Guid? Id { get; set; }

    [CommandOption("--duration <DURATION>")]
    [Description("A duration/timespan value")]
    [DefaultValue("00:30:00")]
    public TimeSpan Duration { get; set; }
}
