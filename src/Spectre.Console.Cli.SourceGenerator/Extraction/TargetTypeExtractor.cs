using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Spectre.Console.Cli.SourceGenerator.Model;

namespace Spectre.Console.Cli.SourceGenerator.Extraction;

/// <summary>
/// Extracts metadata from a class marked with [SpectreMetadata].
/// </summary>
internal static class TargetTypeExtractor
{
    /// <summary>
    /// Extracts a target type model from the attribute syntax context.
    /// </summary>
    public static TargetTypeModel? Extract(GeneratorAttributeSyntaxContext context)
    {
        if (context.TargetSymbol is not INamedTypeSymbol typeSymbol)
        {
            return null;
        }

        // Check if the class is declared as partial
        var isPartial = false;
        if (context.TargetNode is ClassDeclarationSyntax classDecl)
        {
            isPartial = classDecl.Modifiers.Any(SyntaxKind.PartialKeyword);
        }

        var ns = typeSymbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : typeSymbol.ContainingNamespace.ToDisplayString();

        return new TargetTypeModel(
            name: typeSymbol.Name,
            ns: ns,
            fullyQualifiedName: typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            isPartial: isPartial,
            accessibility: typeSymbol.DeclaredAccessibility);
    }
}
