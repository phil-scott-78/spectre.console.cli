using Spectre.Console.Cli.Metadata;

namespace Spectre.Console.Cli;

/// <summary>
/// Default implementation of <see cref="IPairDeconstructor"/> for parsing key=value pairs.
/// </summary>
[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes")]
public sealed class DefaultPairDeconstructor : IPairDeconstructor
{
    /// <inheritdoc/>
    (object? Key, object? Value) IPairDeconstructor.Deconstruct(
        ITypeResolver resolver,
        ICommandMetadataContext metadataContext,
        Type keyType,
        Type valueType,
        string? value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var parts = value.Split(['='], StringSplitOptions.None);
        if (parts.Length < 1 || parts.Length > 2)
        {
            throw CommandParseException.ValueIsNotInValidFormat(value);
        }

        var stringkey = parts[0];
        var stringValue = parts.Length == 2 ? parts[1] : null;
        if (stringValue == null)
        {
            // Got a default constructor?
            if (valueType.IsValueType)
            {
                // Get the string variant of a default instance.
                // Should not get null here, but compiler doesn't know that.
                stringValue = metadataContext.CreateDefaultValue(valueType)?.ToString() ?? string.Empty;
            }
            else
            {
                // Try with an empty string.
                // Hopefully, the type converter knows how to convert it.
                stringValue = string.Empty;
            }
        }

        return (Parse(metadataContext, stringkey, keyType),
            Parse(metadataContext, stringValue, valueType));
    }

    private static object? Parse(ICommandMetadataContext metadataContext, string value, Type targetType)
    {
        try
        {
            var converter = metadataContext.GetTypeConverter(targetType);
            return converter.ConvertFrom(value);
        }
        catch
        {
            // Can't convert something. Just give up and tell the user.
            throw CommandParseException.ValueIsNotInValidFormat(value);
        }
    }
}