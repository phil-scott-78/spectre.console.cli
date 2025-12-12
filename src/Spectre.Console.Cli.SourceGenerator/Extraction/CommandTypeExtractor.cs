using Microsoft.CodeAnalysis;
using Spectre.Console.Cli.SourceGenerator.Model;

namespace Spectre.Console.Cli.SourceGenerator.Extraction;

/// <summary>
/// Extracts metadata from command types.
/// </summary>
internal static class CommandTypeExtractor
{
    private const string DescriptionAttributeName = "System.ComponentModel.DescriptionAttribute";
    private const string ICommandGenericName = "Spectre.Console.Cli.ICommand<TSettings>";

    /// <summary>
    /// Extracts a command type model.
    /// </summary>
    public static CommandTypeModel Extract(INamedTypeSymbol typeSymbol)
    {
        var fullyQualifiedName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var safeName = CommandTypeModel.GetSafeName(fullyQualifiedName);
        string? description = null;

        // Look for DescriptionAttribute
        foreach (var attribute in typeSymbol.GetAttributes())
        {
            var attrName = attribute.AttributeClass?.ToDisplayString();
            if (attrName == DescriptionAttributeName)
            {
                if (attribute.ConstructorArguments.Length > 0 &&
                    attribute.ConstructorArguments[0].Value is string desc)
                {
                    description = desc;
                }
                break;
            }
        }

        var hasParameterlessConstructor = typeSymbol.Constructors.Any(c =>
            !c.IsStatic &&
            c.DeclaredAccessibility == Accessibility.Public &&
            c.Parameters.Length == 0);

        // Extract the settings type from ICommand<TSettings>
        var settingsTypeName = ExtractSettingsType(typeSymbol);

        return new CommandTypeModel(
            fullyQualifiedName,
            safeName,
            description,
            hasParameterlessConstructor,
            settingsTypeName);
    }

    /// <summary>
    /// Extracts the TSettings type argument from ICommand&lt;TSettings&gt;.
    /// </summary>
    private static string? ExtractSettingsType(INamedTypeSymbol typeSymbol)
    {
        // Look through all interfaces for ICommand<TSettings>
        foreach (var @interface in typeSymbol.AllInterfaces)
        {
            if (!@interface.IsGenericType)
            {
                continue;
            }

            var originalDefinition = @interface.OriginalDefinition.ToDisplayString();
            if (originalDefinition == ICommandGenericName)
            {
                // Found ICommand<TSettings>, extract the type argument
                if (@interface.TypeArguments.Length > 0)
                {
                    return @interface.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                }
            }
        }

        return null;
    }
}
