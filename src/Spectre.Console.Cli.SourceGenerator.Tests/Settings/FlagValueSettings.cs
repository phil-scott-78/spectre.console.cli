namespace Spectre.Console.Cli.SourceGenerator.Tests.Settings;

/// <summary>
/// Tests FlagValue type conversion.
/// FlagValue triggers Activator.CreateInstance for generic instantiation.
/// </summary>
public sealed class FlagValueSettings : CommandSettings
{
    [CommandOption("--port [PORT]")]
    public FlagValue<int>? Port { get; set; }

    [CommandOption("--timeout [SECONDS]")]
    public FlagValue<int?>? Timeout { get; set; }

    [CommandOption("--verbose")]
    public bool Verbose { get; set; }
}