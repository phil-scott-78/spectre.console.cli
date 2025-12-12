using System.ComponentModel;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Settings;

/// <summary>
/// Settings for testing floating-point default values in AOT scenarios.
/// This verifies the InvariantCulture fix for float/double/decimal formatting.
/// </summary>
public sealed class FloatingPointSettings : CommandSettings
{
    [CommandOption("--float <VALUE>")]
    [Description("A float value")]
    [DefaultValue(1.5f)]
    public float FloatValue { get; set; } = 1.5f;

    [CommandOption("--double <VALUE>")]
    [Description("A double value")]
    [DefaultValue(2.75d)]
    public double DoubleValue { get; set; } = 2.75d;

    [CommandOption("--decimal <VALUE>")]
    [Description("A decimal value")]
    // Note: Using string-based DefaultValue for decimal as DefaultValueAttribute doesn't support decimal literals directly
    // We rely on the property initializer for the actual default value
    public decimal DecimalValue { get; set; } = 3.14159m;

    [CommandOption("--negative <VALUE>")]
    [Description("A negative float value")]
    [DefaultValue(-0.5f)]
    public float NegativeFloat { get; set; } = -0.5f;

    [CommandOption("--large <VALUE>")]
    [Description("A large double value")]
    [DefaultValue(1234567.89d)]
    public double LargeDouble { get; set; } = 1234567.89d;
}
