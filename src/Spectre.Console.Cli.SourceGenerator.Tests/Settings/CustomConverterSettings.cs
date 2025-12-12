using System.ComponentModel;
using System.Globalization;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Settings;

/// <summary>
/// Settings for testing custom TypeConverter in AOT scenarios.
/// </summary>
public sealed class CustomConverterSettings : CommandSettings
{
    [CommandOption("--point <POINT>")]
    [Description("A point in X,Y format (e.g., 10,20)")]
    [TypeConverter(typeof(PointConverter))]
    public Point Location { get; set; }

    [CommandOption("-v|--verbose")]
    [Description("Enable verbose output")]
    public bool Verbose { get; set; }
}

/// <summary>
/// A simple point structure for testing custom TypeConverter.
/// </summary>
public readonly record struct Point(int X, int Y)
{
    public override string ToString() => $"({X}, {Y})";
}

/// <summary>
/// Custom TypeConverter that converts strings like "10,20" to Point.
/// This tests that custom converters work in AOT scenarios.
/// </summary>
public sealed class PointConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string str)
        {
            var parts = str.Split(',');
            if (parts.Length == 2 &&
                int.TryParse(parts[0].Trim(), out var x) &&
                int.TryParse(parts[1].Trim(), out var y))
            {
                return new Point(x, y);
            }

            throw new FormatException($"Invalid point format: '{str}'. Expected format: X,Y (e.g., 10,20)");
        }

        return base.ConvertFrom(context, culture, value);
    }
}
