namespace Spectre.Console.Cli.SourceGenerator.Tests.Settings;

/// <summary>
/// Tests MultiMap type conversion via IDictionary and ILookup.
/// These types trigger Activator.CreateInstance with MakeGenericType.
/// </summary>
public sealed class MultiMapSettings : CommandSettings
{
    [CommandOption("--value <VALUE>")]
    public IDictionary<string, int>? Values { get; set; }

    [CommandOption("--lookup <VALUE>")]
    public ILookup<string, string>? Lookups { get; set; }

    [CommandOption("--readonly <VALUE>")]
    public IReadOnlyDictionary<string, string>? ReadOnlyValues { get; set; }
}