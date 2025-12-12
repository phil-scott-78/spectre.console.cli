using Microsoft.CodeAnalysis;
using Spectre.Console.Cli.SourceGenerator.Model;

namespace Spectre.Console.Cli.SourceGenerator.Extraction;

/// <summary>
/// Extracts metadata from CommandSettings types.
/// </summary>
internal static class SettingsTypeExtractor
{
    private const string DescriptionAttributeName = "System.ComponentModel.DescriptionAttribute";

    /// <summary>
    /// Extracts a complete model from a settings type symbol.
    /// </summary>
    public static SettingsTypeModel Extract(INamedTypeSymbol typeSymbol)
    {
        var fullyQualifiedName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var safeName = SettingsTypeModel.GetSafeName(fullyQualifiedName);
        var @namespace = typeSymbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : typeSymbol.ContainingNamespace.ToDisplayString();
        var isAbstract = typeSymbol.IsAbstract;

        // Extract description from DescriptionAttribute if present
        var description = ExtractDescription(typeSymbol);

        // Extract properties (walking inheritance hierarchy)
        var properties = ExtractProperties(typeSymbol);

        // Extract constructors
        var constructors = ExtractConstructors(typeSymbol);

        return new SettingsTypeModel(
            fullyQualifiedName,
            safeName,
            @namespace,
            new EquatableArray<PropertyModel>(properties.ToArray()),
            new EquatableArray<ConstructorModel>(constructors.ToArray()),
            description,
            isAbstract);
    }

    private static string? ExtractDescription(INamedTypeSymbol typeSymbol)
    {
        foreach (var attribute in typeSymbol.GetAttributes())
        {
            var attrName = attribute.AttributeClass?.ToDisplayString();
            if (attrName == DescriptionAttributeName)
            {
                if (attribute.ConstructorArguments.Length > 0 &&
                    attribute.ConstructorArguments[0].Value is string description)
                {
                    return description;
                }
                break;
            }
        }
        return null;
    }

    private static List<PropertyModel> ExtractProperties(INamedTypeSymbol typeSymbol)
    {
        var properties = new List<PropertyModel>();

        // Walk the inheritance chain to collect all properties
        // Process from derived to base so overridden property attributes take precedence
        var typeChain = new List<INamedTypeSymbol>();
        var current = typeSymbol;

        while (current is not null && current.SpecialType != SpecialType.System_Object)
        {
            // Stop when we hit CommandSettings base class
            if (current.ToDisplayString() == "Spectre.Console.Cli.CommandSettings")
            {
                break;
            }

            typeChain.Add(current);
            current = current.BaseType;
        }

        // Don't reverse - process derived classes first so overridden attributes win
        var seenProperties = new HashSet<string>();

        foreach (var type in typeChain)
        {
            foreach (var member in type.GetMembers())
            {
                if (member is IPropertySymbol property &&
                    property.DeclaredAccessibility == Accessibility.Public &&
                    !property.IsStatic &&
                    !property.IsIndexer &&
                    (property.SetMethod is not null || property.GetMethod is not null)) // Include get-only for constructor binding
                {
                    // Skip if we've already seen this property (override)
                    if (seenProperties.Contains(property.Name))
                    {
                        continue;
                    }

                    seenProperties.Add(property.Name);

                    // Pass the root settings type to detect inherited init-only properties
                    var propertyModel = PropertyExtractor.Extract(property, typeSymbol);
                    properties.Add(propertyModel);
                }
            }
        }

        return properties;
    }

    private static List<ConstructorModel> ExtractConstructors(INamedTypeSymbol typeSymbol)
    {
        var constructors = new List<ConstructorModel>();
        var index = 0;

        foreach (var constructor in typeSymbol.Constructors)
        {
            // Skip static constructors
            if (constructor.IsStatic)
            {
                continue;
            }

            // Skip implicitly declared constructors that are not public
            if (constructor.DeclaredAccessibility != Accessibility.Public)
            {
                continue;
            }

            var parameters = new List<ConstructorParameterModel>();
            foreach (var parameter in constructor.Parameters)
            {
                var paramName = parameter.Name;
                var paramTypeName = parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                var hasDefaultValue = parameter.HasExplicitDefaultValue;
                string? defaultValueExpression = null;

                if (hasDefaultValue)
                {
                    defaultValueExpression = FormatDefaultValue(parameter);
                }

                parameters.Add(new ConstructorParameterModel(
                    paramName,
                    paramTypeName,
                    hasDefaultValue,
                    defaultValueExpression));
            }

            constructors.Add(new ConstructorModel(
                index++,
                new EquatableArray<ConstructorParameterModel>(parameters.ToArray())));
        }

        return constructors;
    }

    private static string? FormatDefaultValue(IParameterSymbol parameter)
    {
        if (!parameter.HasExplicitDefaultValue)
        {
            return null;
        }

        var defaultValue = parameter.ExplicitDefaultValue;

        if (defaultValue is null)
        {
            // Check if it's a nullable value type with null default
            if (parameter.Type.IsValueType)
            {
                return "default";
            }
            return "null";
        }

        if (defaultValue is string s)
        {
            return $"\"{EscapeString(s)}\"";
        }

        if (defaultValue is char c)
        {
            return $"'{EscapeChar(c)}'";
        }

        if (defaultValue is bool b)
        {
            return b ? "true" : "false";
        }

        if (defaultValue is float f)
        {
            return $"{f}f";
        }

        if (defaultValue is double d)
        {
            return $"{d}d";
        }

        if (defaultValue is decimal m)
        {
            return $"{m}m";
        }

        if (defaultValue is long l)
        {
            return $"{l}L";
        }

        if (defaultValue is ulong ul)
        {
            return $"{ul}UL";
        }

        if (defaultValue is uint ui)
        {
            return $"{ui}U";
        }

        // For enums, we need to cast
        if (parameter.Type.TypeKind == TypeKind.Enum)
        {
            var enumTypeName = parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            return $"({enumTypeName}){defaultValue}";
        }

        return defaultValue.ToString();
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
}
