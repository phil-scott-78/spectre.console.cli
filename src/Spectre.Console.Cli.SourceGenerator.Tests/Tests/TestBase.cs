using Spectre.Console.Cli.SourceGenerator.Tests.Commands;
using Spectre.Console.Cli.SourceGenerator.Tests.Services;
using Spectre.Console.Cli.Testing;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Tests;

/// <summary>
/// Base class for AOT tests providing common setup functionality.
/// </summary>
public abstract class TestBase
{
    /// <summary>
    /// Creates a CommandAppTester configured with AOT metadata and all commands.
    /// </summary>
    protected static CommandAppTester CreateFullApp()
    {
        var app = new CommandAppTester();
        app.Context = new AotMetadata();
        app.Configure(config =>
        {
            config.SetApplicationName("aottest");

            // Register the test service for DI testing
            config.Settings.Registrar.RegisterInstance<ITestService>(new TestService());

            // Register service that depends on Settings (Settings -> Service DI pattern)
            config.Settings.Registrar.Register<ISettingsAwareService, SettingsAwareService>();

            // Register command that requires DI (has constructor with parameters)
            config.Settings.Registrar.Register<InjectionTestCommand, InjectionTestCommand>();

            // Existing test commands
            config.AddCommand<MultiMapCommand>("multimap")
                .WithDescription("Tests MultiMap (IDictionary/ILookup) type conversion");

            config.AddCommand<FlagValueCommand>("flagvalue")
                .WithDescription("Tests FlagValue<T> type conversion");

            config.AddCommand<FileInfoCommand>("fileinfo")
                .WithDescription("Tests FileInfo/DirectoryInfo type conversion");

            config.AddCommand<BasicCommand>("basic")
                .WithDescription("Tests basic command/option/argument patterns");

            // Stress test commands for RegisterKnownTypes
            config.AddCommand<InjectionTestCommand>("inject")
                .WithDescription("Tests dependency injection via RegisterKnownTypes");

            config.AddCommand<CustomConverterCommand>("converter")
                .WithDescription("Tests custom TypeConverter in AOT scenarios");

            config.AddCommand<CustomDeconstructorCommand>("deconstructor")
                .WithDescription("Tests custom PairDeconstructor in AOT scenarios");

            config.AddCommand<ValidationCommand>("validate")
                .WithDescription("Tests custom validation attributes in AOT scenarios");

            config.AddCommand<InheritedCommand>("inherit")
                .WithDescription("Tests settings inheritance in AOT scenarios");

            // Edge case commands
            config.AddCommand<AsyncTestCommand>("async")
                .WithDescription("Tests async command execution in AOT scenarios");

            config.AddCommand<FloatingPointCommand>("floats")
                .WithDescription("Tests floating-point default values (InvariantCulture fix)");

            config.AddCommand<DeepInheritanceCommand>("deep")
                .WithDescription("Tests deep inheritance (3 levels) in AOT scenarios");

            config.AddCommand<EdgeCasesCommand>("edge")
                .WithDescription("Tests edge cases: hidden/required options, multiple args, array defaults");

            config.AddCommand<TypeConverterCommand>("types")
                .WithDescription("Tests built-in type converters (Uri, Guid, TimeSpan)");

            config.AddCommand<EnumEdgeCasesCommand>("enumtest")
                .WithDescription("Tests enum edge cases (case-insensitive, custom enums, nullable)");

            config.AddCommand<SettingsInjectionCommand>("settingsdi")
                .WithDescription("Tests settings with constructor dependency injection");

            config.AddCommand<ServiceWithSettingsCommand>("servicedi")
                .WithDescription("Tests service with settings dependency injection");

            config.AddCommand<InitOnlyCommand>("initonly")
                .WithDescription("Tests init-only properties with UnsafeAccessor support");

            // Subcommand hierarchy testing
            config.AddBranch("sub", branch =>
            {
                branch.SetDescription("Tests subcommand hierarchy in AOT scenarios");
                branch.AddCommand<SubAddCommand>("add")
                    .WithDescription("Add an item");
                branch.AddCommand<SubRemoveCommand>("remove")
                    .WithDescription("Remove an item");
                branch.AddCommand<SubListCommand>("list")
                    .WithDescription("List items");
            });
        });
        return app;
    }

    /// <summary>
    /// Creates a CommandAppTester with AOT metadata and a single command.
    /// </summary>
    protected static CommandAppTester CreateApp<TCommand>(string name)
        where TCommand : class, ICommand
    {
        var app = new CommandAppTester();
        app.Context = new AotMetadata();
        app.Configure(config =>
        {
            config.SetApplicationName("aottest");
            config.AddCommand<TCommand>(name);
        });
        return app;
    }
}
