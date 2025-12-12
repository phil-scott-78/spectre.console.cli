using Spectre.Console.Cli.Metadata;

namespace Spectre.Console.Cli;

internal sealed class Configurator : IUnsafeConfigurator, IConfigurator, IConfiguration
{
    private readonly ITypeRegistrar _registrar;
    private readonly ICommandMetadataContext _metadataContext;

    public IList<ConfiguredCommand> Commands { get; }
    public CommandAppSettings Settings { get; }
    public ConfiguredCommand? DefaultCommand { get; private set; }
    public IList<string[]> Examples { get; }

    ICommandAppSettings IConfigurator.Settings => Settings;

    public Configurator(ITypeRegistrar registrar, ICommandMetadataContext metadataContext)
    {
        _registrar = registrar;
        _metadataContext = metadataContext;

        Commands = new List<ConfiguredCommand>();
        Settings = new CommandAppSettings(registrar);
        Examples = new List<string[]>();
    }

    public IConfigurator SetHelpProvider(IHelpProvider helpProvider)
    {
        _registrar.RegisterInstance(helpProvider);
        return this;
    }

    public IConfigurator SetHelpProvider<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>()
        where T : IHelpProvider
    {
        _registrar.Register<IHelpProvider, T>();
        return this;
    }

    public IConfigurator AddExample(params string[] args)
    {
        Examples.Add(args);
        return this;
    }

    public ConfiguredCommand SetDefaultCommand<TDefaultCommand>()
        where TDefaultCommand : class, ICommand
    {
        DefaultCommand = ConfiguredCommand.FromType<TDefaultCommand>(
            _metadataContext, CliConstants.DefaultCommandName, isDefaultCommand: true);
        return DefaultCommand;
    }

    public ICommandConfigurator AddCommand<TCommand>(string name)
        where TCommand : class, ICommand
    {
        var command = Commands.AddAndReturn(ConfiguredCommand.FromType<TCommand>(_metadataContext, name, isDefaultCommand: false));
        return new CommandConfigurator(command);
    }

    public ICommandConfigurator AddDelegate<TSettings>(string name, Func<CommandContext, TSettings, CancellationToken, int> func)
        where TSettings : CommandSettings
    {
        var command = Commands.AddAndReturn(ConfiguredCommand.FromDelegate<TSettings>(
            name, (context, settings, cancellationToken) => Task.FromResult(func(context, (TSettings)settings, cancellationToken))));
        return new CommandConfigurator(command);
    }

    public ICommandConfigurator AddAsyncDelegate<TSettings>(string name, Func<CommandContext, TSettings, CancellationToken, Task<int>> func)
        where TSettings : CommandSettings
    {
        var command = Commands.AddAndReturn(ConfiguredCommand.FromDelegate<TSettings>(
            name, (context, settings, cancellationToken) => func(context, (TSettings)settings, cancellationToken)));
        return new CommandConfigurator(command);
    }

    public IBranchConfigurator AddBranch<TSettings>(string name, Action<IConfigurator<TSettings>> action)
        where TSettings : CommandSettings
    {
        var command = ConfiguredCommand.FromBranch<TSettings>(name);
        action(new Configurator<TSettings>(command, _registrar, _metadataContext));
        var added = Commands.AddAndReturn(command);
        return new BranchConfigurator(added);
    }

    /// <inheritdoc/>
    [RequiresDynamicCode("This method uses MakeGenericMethod and is not compatible with AOT compilation.")]
    [RequiresUnreferencedCode("This method uses reflection to discover command types.")]
    ICommandConfigurator IUnsafeConfigurator.AddCommand(string name, Type command)
    {
        var method = GetType().GetMethod("AddCommand");
        if (method == null)
        {
            throw new CommandConfigurationException("Could not find AddCommand by reflection.");
        }

        method = method.MakeGenericMethod(command);

        if (method.Invoke(this, [name]) is not ICommandConfigurator result)
        {
            throw new CommandConfigurationException("Invoking AddCommand returned null.");
        }

        return result;
    }

    /// <inheritdoc/>
    [RequiresDynamicCode("This method uses MakeGenericType and Activator.CreateInstance, and is not compatible with AOT compilation.")]
    [RequiresUnreferencedCode("This method uses reflection to discover settings types.")]
    IBranchConfigurator IUnsafeConfigurator.AddBranch(string name, Type settings, Action<IUnsafeBranchConfigurator> action)
    {
        var command = ConfiguredCommand.FromBranch(settings, name);

        // Create the configurator.
        var configuratorType = typeof(Configurator<>).MakeGenericType(settings);
        if (!(Activator.CreateInstance(configuratorType, new object?[] { command, _registrar, _metadataContext }) is IUnsafeBranchConfigurator configurator))
        {
            throw new CommandConfigurationException("Could not create configurator by reflection.");
        }

        action(configurator);
        var added = Commands.AddAndReturn(command);
        return new BranchConfigurator(added);
    }
}