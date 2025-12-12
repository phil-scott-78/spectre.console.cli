using System.ComponentModel;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Settings;

/// <summary>
/// Settings for testing custom PairDeconstructor in AOT scenarios.
/// </summary>
public sealed class CustomDeconstructorSettings : CommandSettings
{
    [CommandOption("--kv <KV>")]
    [Description("Key:value pairs using colon separator (e.g., name:value)")]
    [PairDeconstructor(typeof(ColonPairDeconstructor))]
    public IDictionary<string, string>? KeyValues { get; set; }

    [CommandOption("-v|--verbose")]
    [Description("Enable verbose output")]
    public bool Verbose { get; set; }
}

/// <summary>
/// Custom PairDeconstructor that uses colon as the separator instead of the default equals sign.
/// This tests that custom deconstructors work in AOT scenarios.
/// </summary>
public sealed class ColonPairDeconstructor : PairDeconstructor<string, string>
{
    protected override (string Key, string Value) Deconstruct(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new FormatException("Value cannot be null or empty.");
        }

        var colonIndex = value.IndexOf(':');
        if (colonIndex < 0)
        {
            throw new FormatException($"Invalid key:value format: '{value}'. Expected format: key:value");
        }

        var key = value.Substring(0, colonIndex);
        var val = value.Substring(colonIndex + 1);

        return (key, val);
    }
}
