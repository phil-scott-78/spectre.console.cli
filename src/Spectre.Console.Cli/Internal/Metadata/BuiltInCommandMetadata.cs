namespace Spectre.Console.Cli.Internal.Metadata;

/// <summary>
/// Hand-written metadata for built-in command settings types.
/// These are registered into the metadata context by CommandApp.
/// </summary>
internal static class BuiltInCommandMetadata
{
    /// <summary>
    /// Registers all built-in command metadata into the provided context.
    /// </summary>
    /// <param name="typeRegistrar">The type registrar to register command types with for DI resolution.</param>
    public static void Register(ITypeRegistrar typeRegistrar)
    {
        // Register all built-in command types for DI resolution
        typeRegistrar.Register<VersionCommand>();
        typeRegistrar.Register<XmlDocCommand>();
        typeRegistrar.Register<ExplainCommand>();
        typeRegistrar.Register<OpenCliGeneratorCommand>();

        // Register built-in settings types for DI resolution
        typeRegistrar.Register<EmptyCommandSettings>();
        typeRegistrar.Register<ExplainCommand.Settings>();
    }
}
