using Microsoft.CodeAnalysis;

namespace Spectre.Console.Cli.SourceGenerator.Diagnostics;

/// <summary>
/// Diagnostic descriptors for the source generator.
/// </summary>
internal static class DiagnosticDescriptors
{
    /// <summary>
    /// Error when the target type is not declared as partial.
    /// </summary>
    public static readonly DiagnosticDescriptor TargetTypeMustBePartial = new(
        id: "SCCLI001",
        title: "Target type must be partial",
        messageFormat: "The type '{0}' marked with [SpectreMetadata] must be declared as partial",
        category: "Spectre.Console.Cli.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Classes marked with [SpectreMetadata] must be declared as partial so the source generator can augment them with the metadata context implementation.");

    /// <summary>
    /// Warning when multiple types are marked with [SpectreMetadata].
    /// </summary>
    public static readonly DiagnosticDescriptor MultipleTargetTypes = new(
        id: "SCCLI002",
        title: "Multiple SpectreMetadata targets",
        messageFormat: "Multiple types are marked with [SpectreMetadata], only '{0}' will be used",
        category: "Spectre.Console.Cli.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Only one class should be marked with [SpectreMetadata] per compilation. Additional marked classes will be ignored.");

    /// <summary>
    /// Error when a settings type is not accessible (private or protected).
    /// </summary>
    public static readonly DiagnosticDescriptor SettingsTypeMustBeAccessible = new(
        id: "SCCLI003",
        title: "Settings type must be accessible",
        messageFormat: "Settings type '{0}' must be public or internal for AOT compatibility. Private and protected settings types cannot be used with Native AOT.",
        category: "Spectre.Console.Cli.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "CommandSettings types must be declared as public or internal so the source generator can reference them in the generated metadata. Private and protected types cannot be used with Native AOT compilation.");

    /// <summary>
    /// Error when a generic command type is encountered.
    /// </summary>
    public static readonly DiagnosticDescriptor GenericCommandNotSupported = new(
        id: "SCCLI004",
        title: "Generic command types are not supported in AOT",
        messageFormat: "Command type '{0}' is a generic type. Generic command types cannot be used with Native AOT compilation. Create a non-generic derived type instead.",
        category: "Spectre.Console.Cli.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Generic command types like 'MyCommand<T>' cannot be discovered by the source generator at compile time when used as closed generics like 'MyCommand<MySettings>'. Create a concrete non-generic command class that inherits from the appropriate base type instead.");

    /// <summary>
    /// Error when a generic settings type is encountered.
    /// </summary>
    public static readonly DiagnosticDescriptor GenericSettingsNotSupported = new(
        id: "SCCLI005",
        title: "Generic settings types are not supported in AOT",
        messageFormat: "Settings type '{0}' is a generic type. Generic settings types cannot be used with Native AOT compilation. Use a non-generic settings class.",
        category: "Spectre.Console.Cli.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Generic settings types like 'MySettings<T>' cannot be used with Native AOT compilation because the source generator cannot generate valid code for unbound type parameters.");

    /// <summary>
    /// Error when a command type is not accessible (private or protected).
    /// </summary>
    public static readonly DiagnosticDescriptor CommandTypeMustBeAccessible = new(
        id: "SCCLI006",
        title: "Command type must be accessible",
        messageFormat: "Command type '{0}' must be public or internal for AOT compatibility. Private and protected command types cannot be used with Native AOT.",
        category: "Spectre.Console.Cli.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Command types must be declared as public or internal so the source generator can reference them in the generated metadata. Private and protected types cannot be used with Native AOT compilation.");

}
