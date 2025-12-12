using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Spectre.Console.Cli.SourceGenerator.Diagnostics;
using Spectre.Console.Cli.SourceGenerator.Emitters;
using Spectre.Console.Cli.SourceGenerator.Extraction;
using Spectre.Console.Cli.SourceGenerator.Model;

namespace Spectre.Console.Cli.SourceGenerator;

/// <summary>
/// Source generator that produces AOT-safe metadata implementations for Spectre.Console.Cli.
/// </summary>
[Generator]
public class SpectreCliMetadataGenerator : IIncrementalGenerator
{
    private const string CommandSettingsFullName = "Spectre.Console.Cli.CommandSettings";
    private const string ICommandFullName = "Spectre.Console.Cli.ICommand";

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Step 1: Always emit the marker attribute via post-initialization
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource("SpectreMetadataAttribute.g.cs",
                SourceText.From(AttributeEmitter.Emit(), Encoding.UTF8));
        });

        // Step 2: Find types marked with [SpectreMetadata] using ForAttributeWithMetadataName
        var targetTypes = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                AttributeEmitter.AttributeFullName,
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: static (ctx, _) => TargetTypeExtractor.Extract(ctx))
            .Where(static m => m is not null)
            .Select(static (m, _) => m!);

        // Step 3: Find all candidate settings/commands
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsCandidateClass(s),
                transform: static (ctx, _) => GetClassSymbol(ctx))
            .Where(static m => m is not null);

        // Step 4: Combine target types (collect first to handle multiple)
        var allTargetTypes = targetTypes.Collect();

        // Step 5: Combine everything
        var combined = allTargetTypes
            .Combine(context.CompilationProvider)
            .Combine(classDeclarations.Collect());

        // Step 6: Register source output - only generates when target type exists
        context.RegisterSourceOutput(combined,
            static (spc, source) =>
            {
                var targetTypes = source.Left.Left;
                var compilation = source.Left.Right;
                var classes = source.Right!;
                Execute(targetTypes, compilation, classes, spc);
            });
    }

    private static bool IsCandidateClass(SyntaxNode node)
    {
        return node switch
        {
            ClassDeclarationSyntax { BaseList: not null } => true,
            RecordDeclarationSyntax { BaseList: not null } => true,
            _ => false
        };
    }

    private static INamedTypeSymbol? GetClassSymbol(GeneratorSyntaxContext context)
    {
        return context.Node switch
        {
            ClassDeclarationSyntax classDecl =>
                context.SemanticModel.GetDeclaredSymbol(classDecl) as INamedTypeSymbol,
            RecordDeclarationSyntax recordDecl =>
                context.SemanticModel.GetDeclaredSymbol(recordDecl) as INamedTypeSymbol,
            _ => null
        };
    }

    private static void Execute(
        ImmutableArray<TargetTypeModel> targetTypes,
        Compilation compilation,
        ImmutableArray<INamedTypeSymbol?> classes,
        SourceProductionContext context)
    {
        // No target type marked with [SpectreMetadata] - don't generate anything
        if (targetTypes.IsEmpty)
        {
            return;
        }

        // Get the first target type
        var targetType = targetTypes[0];

        // Report warning if multiple target types
        if (targetTypes.Length > 1)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.MultipleTargetTypes,
                Location.None,
                targetType.Name));
        }

        // Report error if not partial
        if (!targetType.IsPartial)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.TargetTypeMustBePartial,
                Location.None,
                targetType.Name));
            return;
        }

        // Get the base types we're looking for
        var commandSettingsType = compilation.GetTypeByMetadataName(CommandSettingsFullName);
        var iCommandType = compilation.GetTypeByMetadataName(ICommandFullName);

        if (commandSettingsType is null)
        {
            // Spectre.Console.Cli is not referenced
            return;
        }

        // Find all settings types
        var settingsTypes = new List<SettingsTypeModel>();
        var commandTypes = new List<CommandTypeModel>();

        // Deduplicate and filter
        var seenTypes = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        foreach (var classSymbol in classes)
        {
            if (classSymbol is null)
            {
                continue;
            }

            if (!seenTypes.Add(classSymbol))
            {
                continue;
            }

            // Check if it's a generic type (open or closed)
            var isGenericType = classSymbol is { IsGenericType: true, TypeParameters.Length: > 0 };

            // Check if it's a settings type
            if (InheritsFrom(classSymbol, commandSettingsType))
            {
                // Report error for generic settings types (AOT cannot generate valid code for unbound generics)
                if (isGenericType)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptors.GenericSettingsNotSupported,
                        classSymbol.Locations.FirstOrDefault() ?? Location.None,
                        classSymbol.ToDisplayString()));
                    continue;
                }

                // Report error for inaccessible settings types (AOT requires accessible types)
                if (!IsAccessible(classSymbol))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptors.SettingsTypeMustBeAccessible,
                        classSymbol.Locations.FirstOrDefault() ?? Location.None,
                        classSymbol.ToDisplayString()));
                    continue;
                }

                var model = SettingsTypeExtractor.Extract(classSymbol);
                settingsTypes.Add(model);
            }

            // Check if it's a command type
            if (iCommandType is not null && ImplementsInterface(classSymbol, iCommandType))
            {
                // Report error for generic command types (AOT cannot discover closed generic usages)
                if (isGenericType)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptors.GenericCommandNotSupported,
                        classSymbol.Locations.FirstOrDefault() ?? Location.None,
                        classSymbol.ToDisplayString()));
                    continue;
                }

                // Report error for inaccessible command types (AOT requires accessible types)
                if (!IsAccessible(classSymbol))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptors.CommandTypeMustBeAccessible,
                        classSymbol.Locations.FirstOrDefault() ?? Location.None,
                        classSymbol.ToDisplayString()));
                    continue;
                }

                var model = CommandTypeExtractor.Extract(classSymbol);
                commandTypes.Add(model);
            }
        }

        // Collect generic type usages
        var genericTypes = GenericTypeCollector.Collect(settingsTypes);

        // Generate the source as a partial class augmentation
        var source = MetadataContextEmitter.EmitPartial(targetType, settingsTypes, commandTypes, genericTypes);
        context.AddSource($"{targetType.Name}.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    private static bool InheritsFrom(INamedTypeSymbol type, INamedTypeSymbol baseType)
    {
        var current = type.BaseType;
        while (current is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType))
            {
                return true;
            }
            current = current.BaseType;
        }
        return false;
    }

    private static bool ImplementsInterface(INamedTypeSymbol type, INamedTypeSymbol interfaceType)
    {
        foreach (var iface in type.AllInterfaces)
        {
            if (SymbolEqualityComparer.Default.Equals(iface, interfaceType))
            {
                return true;
            }
        }
        return false;
    }

    private static bool IsAccessible(INamedTypeSymbol type)
    {
        // Check if the type and all containing types are accessible (public or internal)
        var current = type;
        while (current is not null)
        {
            if (current.DeclaredAccessibility != Accessibility.Public &&
                current.DeclaredAccessibility != Accessibility.Internal)
            {
                return false;
            }
            current = current.ContainingType;
        }
        return true;
    }

}
