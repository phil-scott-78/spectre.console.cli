using Spectre.Console.Cli.Metadata;

namespace Spectre.Console.Cli;

internal sealed class CommandArgument : CommandParameter, ICommandArgument
{
    public string Value { get; }
    public int Position { get; set; }

    public CommandArgument(
        Type parameterType, ParameterKind parameterKind, IPropertyAccessor accessor, string? description,
        TypeConverterAttribute? converter, DefaultValueAttribute? defaultValue,
        CommandArgumentAttribute argument, ParameterValueProviderAttribute? valueProvider,
        IEnumerable<ParameterValidationAttribute> validators)
            : base(parameterType, parameterKind, accessor, description, converter, defaultValue,
                  null, valueProvider, validators, argument.IsRequired, false)
    {
        Value = argument.ValueName;
        Position = argument.Position;
    }
}