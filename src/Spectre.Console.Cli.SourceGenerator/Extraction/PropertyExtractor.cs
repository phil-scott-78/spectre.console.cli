using System.Globalization;
using Microsoft.CodeAnalysis;
using Spectre.Console.Cli.SourceGenerator.Model;

namespace Spectre.Console.Cli.SourceGenerator.Extraction;

/// <summary>
/// Extracts metadata from properties including attribute data.
/// </summary>
internal static class PropertyExtractor
{
    private const string CommandOptionAttributeName = "Spectre.Console.Cli.CommandOptionAttribute";
    private const string CommandArgumentAttributeName = "Spectre.Console.Cli.CommandArgumentAttribute";
    private const string DescriptionAttributeName = "System.ComponentModel.DescriptionAttribute";
    private const string TypeConverterAttributeName = "System.ComponentModel.TypeConverterAttribute";
    private const string DefaultValueAttributeName = "System.ComponentModel.DefaultValueAttribute";
    private const string PairDeconstructorAttributeName = "Spectre.Console.Cli.PairDeconstructorAttribute";
    private const string ParameterValueProviderAttributeName = "Spectre.Console.Cli.ParameterValueProviderAttribute";
    private const string ParameterValidationAttributeName = "Spectre.Console.Cli.ParameterValidationAttribute";

    private const string FlagValueTypeName = "Spectre.Console.Cli.FlagValue";
    private const string IDictionaryTypeName = "System.Collections.Generic.IDictionary";
    private const string ILookupTypeName = "System.Linq.ILookup";
    private const string IReadOnlyDictionaryTypeName = "System.Collections.Generic.IReadOnlyDictionary";

    /// <summary>
    /// Extracts a property model with all attribute data.
    /// </summary>
    /// <param name="property">The property symbol to extract.</param>
    /// <param name="settingsType">The root settings type being extracted (to detect inherited properties).</param>
    public static PropertyModel Extract(IPropertySymbol property, INamedTypeSymbol? settingsType = null)
    {
        var name = property.Name;
        var propertyTypeName = property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var declaringTypeName = property.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var isNullable = property.Type.NullableAnnotation == NullableAnnotation.Annotated ||
                        (property.Type is INamedTypeSymbol namedType &&
                         namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T);
        var isValueType = property.Type.IsValueType;
        var isInitOnly = property.SetMethod?.IsInitOnly ?? false;
        var hasSetter = property.SetMethod is not null;

        // An init-only property is "inherited" if it's declared in a different type than the settings type being extracted
        var isInheritedInit = isInitOnly && settingsType != null &&
            !SymbolEqualityComparer.Default.Equals(property.ContainingType, settingsType);

        // Extract generic type information
        var (arrayElementTypeName, flagValueInnerTypeName, dictionaryKeyTypeName, dictionaryValueTypeName) =
            ExtractGenericTypeInfo(property.Type);

        // Extract attribute data
        CommandOptionAttributeModel? optionAttribute = null;
        CommandArgumentAttributeModel? argumentAttribute = null;
        string? description = null;
        string? converterTypeName = null;
        DefaultValueAttributeModel? defaultValue = null;
        string? pairDeconstructorTypeName = null;
        ValidationAttributeModel? valueProviderAttribute = null;
        var validationAttributes = new List<ValidationAttributeModel>();

        foreach (var attribute in property.GetAttributes())
        {
            var attrClass = attribute.AttributeClass;
            if (attrClass is null)
            {
                continue;
            }

            var attrName = attrClass.ToDisplayString();

            switch (attrName)
            {
                case CommandOptionAttributeName:
                    optionAttribute = ExtractCommandOption(attribute);
                    break;

                case CommandArgumentAttributeName:
                    argumentAttribute = ExtractCommandArgument(attribute);
                    break;

                case DescriptionAttributeName:
                    description = ExtractDescription(attribute);
                    break;

                case TypeConverterAttributeName:
                    converterTypeName = ExtractTypeConverter(attribute);
                    break;

                case DefaultValueAttributeName:
                    defaultValue = ExtractDefaultValue(attribute);
                    break;

                case PairDeconstructorAttributeName:
                    pairDeconstructorTypeName = ExtractPairDeconstructor(attribute);
                    break;

                default:
                    // Check if it's a value provider attribute (inherits from ParameterValueProviderAttribute)
                    if (InheritsFrom(attrClass, ParameterValueProviderAttributeName))
                    {
                        // Extract full attribute data to recreate it with constructor args
                        valueProviderAttribute = ExtractValidationAttribute(attribute);
                    }
                    // Check if it's a validation attribute (inherits from ParameterValidationAttribute)
                    else if (InheritsFrom(attrClass, ParameterValidationAttributeName))
                    {
                        validationAttributes.Add(ExtractValidationAttribute(attribute));
                    }
                    break;
            }
        }

        return new PropertyModel(
            name,
            propertyTypeName,
            declaringTypeName,
            isNullable,
            isValueType,
            isInitOnly,
            hasSetter,
            isInheritedInit,
            optionAttribute,
            argumentAttribute,
            description,
            converterTypeName,
            defaultValue,
            pairDeconstructorTypeName,
            valueProviderAttribute,
            new EquatableArray<ValidationAttributeModel>(validationAttributes.ToArray()),
            arrayElementTypeName,
            flagValueInnerTypeName,
            dictionaryKeyTypeName,
            dictionaryValueTypeName);
    }

    private static (string? arrayElement, string? flagValueInner, string? dictKey, string? dictValue) ExtractGenericTypeInfo(ITypeSymbol type)
    {
        string? arrayElementTypeName = null;
        string? flagValueInnerTypeName = null;
        string? dictionaryKeyTypeName = null;
        string? dictionaryValueTypeName = null;

        // Handle nullable value types - unwrap Nullable<T>
        if (type is INamedTypeSymbol nullableType &&
            nullableType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
        {
            type = nullableType.TypeArguments[0];
        }

        // Check for arrays
        if (type is IArrayTypeSymbol arrayType)
        {
            arrayElementTypeName = arrayType.ElementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            return (arrayElementTypeName, null, null, null);
        }

        // Check for generic types
        if (type is INamedTypeSymbol genericType && genericType.IsGenericType)
        {
            var originalDef = genericType.OriginalDefinition.ToDisplayString();

            // Check for FlagValue<T>
            if (originalDef.StartsWith(FlagValueTypeName))
            {
                flagValueInnerTypeName = genericType.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                return (null, flagValueInnerTypeName, null, null);
            }

            // Check for pair-deconstructable types (IDictionary, ILookup, IReadOnlyDictionary)
            if (originalDef.StartsWith(IDictionaryTypeName) ||
                originalDef.StartsWith(ILookupTypeName) ||
                originalDef.StartsWith(IReadOnlyDictionaryTypeName))
            {
                if (genericType.TypeArguments.Length >= 2)
                {
                    dictionaryKeyTypeName = genericType.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    dictionaryValueTypeName = genericType.TypeArguments[1].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    return (null, null, dictionaryKeyTypeName, dictionaryValueTypeName);
                }
            }
        }

        return (null, null, null, null);
    }

    private static CommandOptionAttributeModel? ExtractCommandOption(AttributeData attribute)
    {
        if (attribute.ConstructorArguments.Length > 0 &&
            attribute.ConstructorArguments[0].Value is string template)
        {
            // Extract isRequired from second constructor argument (defaults to false)
            var isRequired = attribute.ConstructorArguments.Length > 1 &&
                             attribute.ConstructorArguments[1].Value is bool req && req;

            // Extract IsHidden from named arguments
            var isHidden = false;
            foreach (var namedArg in attribute.NamedArguments)
            {
                if (namedArg.Key == "IsHidden" && namedArg.Value.Value is bool hidden)
                {
                    isHidden = hidden;
                    break;
                }
            }

            return new CommandOptionAttributeModel(template, isRequired, isHidden);
        }
        return null;
    }

    private static CommandArgumentAttributeModel? ExtractCommandArgument(AttributeData attribute)
    {
        if (attribute.ConstructorArguments.Length > 0 &&
            attribute.ConstructorArguments[0].Value is int position)
        {
            string? name = null;
            if (attribute.ConstructorArguments.Length > 1 &&
                attribute.ConstructorArguments[1].Value is string n)
            {
                name = n;
            }
            return new CommandArgumentAttributeModel(position, name);
        }
        return null;
    }

    private static string? ExtractDescription(AttributeData attribute)
    {
        if (attribute.ConstructorArguments.Length > 0 &&
            attribute.ConstructorArguments[0].Value is string description)
        {
            return description;
        }
        return null;
    }

    private static string? ExtractTypeConverter(AttributeData attribute)
    {
        // TypeConverterAttribute can take a Type or a string
        if (attribute.ConstructorArguments.Length > 0)
        {
            var arg = attribute.ConstructorArguments[0];
            if (arg.Value is INamedTypeSymbol typeSymbol)
            {
                return typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            }
            if (arg.Value is string typeName)
            {
                return typeName;
            }
        }
        return null;
    }

    private static DefaultValueAttributeModel? ExtractDefaultValue(AttributeData attribute)
    {
        if (attribute.ConstructorArguments.Length > 0)
        {
            // DefaultValueAttribute has two constructor overloads:
            // 1. DefaultValueAttribute(object value) - single value
            // 2. DefaultValueAttribute(Type type, string value) - type + string for complex types
            if (attribute.ConstructorArguments.Length >= 2 &&
                attribute.ConstructorArguments[0].Kind == TypedConstantKind.Type)
            {
                // Two-argument form: DefaultValueAttribute(typeof(T), "value")
                var typeArg = FormatTypedConstant(attribute.ConstructorArguments[0]);
                var valueArg = FormatTypedConstant(attribute.ConstructorArguments[1]);
                return new DefaultValueAttributeModel($"{typeArg}, {valueArg}");
            }
            else
            {
                // Single-argument form: DefaultValueAttribute(value)
                var arg = attribute.ConstructorArguments[0];
                var valueExpression = FormatTypedConstant(arg);
                return new DefaultValueAttributeModel(valueExpression);
            }
        }
        return null;
    }

    private static string? ExtractPairDeconstructor(AttributeData attribute)
    {
        if (attribute.ConstructorArguments.Length > 0 &&
            attribute.ConstructorArguments[0].Value is INamedTypeSymbol typeSymbol)
        {
            return typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }
        return null;
    }

    private static ValidationAttributeModel ExtractValidationAttribute(AttributeData attribute)
    {
        var typeName = attribute.AttributeClass!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        // Extract constructor arguments
        var constructorArgs = new List<AttributeArgumentModel>();
        foreach (var arg in attribute.ConstructorArguments)
        {
            constructorArgs.Add(new AttributeArgumentModel(FormatTypedConstant(arg)));
        }

        // Extract named arguments
        var namedArgs = new List<AttributeNamedArgumentModel>();
        foreach (var namedArg in attribute.NamedArguments)
        {
            namedArgs.Add(new AttributeNamedArgumentModel(
                namedArg.Key,
                FormatTypedConstant(namedArg.Value)));
        }

        return new ValidationAttributeModel(
            typeName,
            new EquatableArray<AttributeArgumentModel>(constructorArgs.ToArray()),
            new EquatableArray<AttributeNamedArgumentModel>(namedArgs.ToArray()));
    }

    private static string FormatTypedConstant(TypedConstant constant)
    {
        if (constant.IsNull)
        {
            return "null";
        }

        switch (constant.Kind)
        {
            case TypedConstantKind.Primitive:
                return FormatPrimitiveValue(constant.Value);

            case TypedConstantKind.Enum:
                var enumType = constant.Type!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                return $"({enumType}){constant.Value}";

            case TypedConstantKind.Type:
                var typeValue = (INamedTypeSymbol)constant.Value!;
                return $"typeof({typeValue.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)})";

            case TypedConstantKind.Array:
                var elements = constant.Values.Select(FormatTypedConstant);
                var elementType = ((IArrayTypeSymbol)constant.Type!).ElementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                return $"new {elementType}[] {{ {string.Join(", ", elements)} }}";

            default:
                return constant.Value?.ToString() ?? "null";
        }
    }

    private static string FormatPrimitiveValue(object? value)
    {
        if (value is null)
        {
            return "null";
        }

        if (value is string s)
        {
            return $"\"{EscapeString(s)}\"";
        }

        if (value is char c)
        {
            return $"'{EscapeChar(c)}'";
        }

        if (value is bool b)
        {
            return b ? "true" : "false";
        }

        if (value is float f)
        {
            return $"{f.ToString(CultureInfo.InvariantCulture)}f";
        }

        if (value is double d)
        {
            return $"{d.ToString(CultureInfo.InvariantCulture)}d";
        }

        if (value is decimal m)
        {
            return $"{m.ToString(CultureInfo.InvariantCulture)}m";
        }

        if (value is long l)
        {
            return $"{l}L";
        }

        if (value is ulong ul)
        {
            return $"{ul}UL";
        }

        if (value is uint ui)
        {
            return $"{ui}U";
        }

        return value.ToString()!;
    }

    private static string EscapeString(string s)
    {
        return s
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }

    private static string EscapeChar(char c)
    {
        return c switch
        {
            '\\' => "\\\\",
            '\'' => "\\'",
            '\n' => "\\n",
            '\r' => "\\r",
            '\t' => "\\t",
            _ => c.ToString()
        };
    }

    private static bool InheritsFrom(INamedTypeSymbol type, string baseTypeName)
    {
        var current = type.BaseType;
        while (current is not null)
        {
            if (current.ToDisplayString() == baseTypeName)
            {
                return true;
            }
            current = current.BaseType;
        }
        return false;
    }
}
