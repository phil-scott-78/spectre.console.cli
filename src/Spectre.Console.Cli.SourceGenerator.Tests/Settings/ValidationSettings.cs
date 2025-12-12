using System.ComponentModel;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Settings;

/// <summary>
/// Settings for testing custom validation attributes in AOT scenarios.
/// </summary>
public sealed class ValidationSettings : CommandSettings
{
    [CommandOption("--age <AGE>")]
    [Description("Age (must be between 0 and 150)")]
    [ValidateRange(0, 150)]
    public int Age { get; set; }

    [CommandOption("--email <EMAIL>")]
    [Description("Email address (must contain @)")]
    [ValidateEmail]
    public string? Email { get; set; }

    [CommandOption("--name <NAME>")]
    [Description("Name (must not be empty)")]
    [ValidateNotEmpty]
    public string? Name { get; set; }
}

/// <summary>
/// Validates that a numeric value is within a specified range.
/// </summary>
public sealed class ValidateRangeAttribute : ParameterValidationAttribute
{
    private readonly int _min;
    private readonly int _max;

    public ValidateRangeAttribute(int min, int max) : base($"Value must be between {min} and {max}")
    {
        _min = min;
        _max = max;
    }

    public override ValidationResult Validate(CommandParameterContext context)
    {
        if (context.Value is int value)
        {
            if (value >= _min && value <= _max)
            {
                return ValidationResult.Success();
            }

            return ValidationResult.Error($"Value {value} is out of range. Must be between {_min} and {_max}.");
        }

        return ValidationResult.Success();
    }
}

/// <summary>
/// Validates that a string contains an @ symbol (basic email validation).
/// </summary>
public sealed class ValidateEmailAttribute : ParameterValidationAttribute
{
    public ValidateEmailAttribute() : base("Value must be a valid email address")
    {
    }

    public override ValidationResult Validate(CommandParameterContext context)
    {
        if (context.Value is string str && !string.IsNullOrEmpty(str))
        {
            if (str.Contains('@') && str.Contains('.'))
            {
                return ValidationResult.Success();
            }

            return ValidationResult.Error($"'{str}' is not a valid email address.");
        }

        return ValidationResult.Success();
    }
}

/// <summary>
/// Validates that a string is not empty or whitespace.
/// </summary>
public sealed class ValidateNotEmptyAttribute : ParameterValidationAttribute
{
    public ValidateNotEmptyAttribute() : base("Value must not be empty")
    {
    }

    public override ValidationResult Validate(CommandParameterContext context)
    {
        if (context.Value is string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                return ValidationResult.Success();
            }

            return ValidationResult.Error("Value must not be empty or whitespace.");
        }

        return ValidationResult.Success();
    }
}
