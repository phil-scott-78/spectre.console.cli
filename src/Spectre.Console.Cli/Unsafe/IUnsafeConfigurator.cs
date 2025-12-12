namespace Spectre.Console.Cli.Unsafe;

/// <summary>
/// Represents an unsafe configurator that uses reflection-based type discovery.
/// </summary>
/// <remarks>
/// Methods on this interface are not compatible with AOT compilation.
/// For AOT scenarios, use the generic methods on <see cref="IConfigurator"/> instead.
/// </remarks>
public interface IUnsafeConfigurator
{
    /// <summary>
    /// Adds a command.
    /// </summary>
    /// <param name="name">The name of the command.</param>
    /// <param name="command">The command type.</param>
    /// <returns>A command configurator that can be used to configure the command further.</returns>
    /// <remarks>
    /// This method uses reflection and is not compatible with AOT compilation.
    /// For AOT scenarios, use <see cref="IConfigurator.AddCommand{TCommand}(string)"/> instead.
    /// </remarks>
    [RequiresDynamicCode("This method uses MakeGenericMethod and is not compatible with AOT compilation.")]
    [RequiresUnreferencedCode("This method uses reflection to discover command types.")]
    ICommandConfigurator AddCommand(string name, Type command);

    /// <summary>
    /// Adds a command branch.
    /// </summary>
    /// <param name="name">The name of the command branch.</param>
    /// <param name="settings">The command setting type.</param>
    /// <param name="action">The command branch configurator.</param>
    /// <returns>A branch configurator that can be used to configure the branch further.</returns>
    /// <remarks>
    /// This method uses reflection and is not compatible with AOT compilation.
    /// For AOT scenarios, use <see cref="IConfigurator.AddBranch{TSettings}(string, Action{IConfigurator{TSettings}})"/> instead.
    /// </remarks>
    [RequiresDynamicCode("This method uses MakeGenericType and Activator.CreateInstance, and is not compatible with AOT compilation.")]
    [RequiresUnreferencedCode("This method uses reflection to discover settings types.")]
    IBranchConfigurator AddBranch(string name, Type settings, Action<IUnsafeBranchConfigurator> action);
}