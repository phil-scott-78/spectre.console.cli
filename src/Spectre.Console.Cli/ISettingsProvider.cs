namespace Spectre.Console.Cli;

/// <summary>
/// Provides access to bound command settings during command execution.
/// </summary>
/// <remarks>
/// <para>
/// This interface enables services to access command settings that are bound
/// after command-line parsing. Services should inject this provider rather than
/// the settings type directly, since settings are not available until after
/// the command tree is parsed and bound.
/// </para>
/// <para>
/// The settings are populated after command binding during command execution.
/// Accessing settings before they are populated will throw an
/// <see cref="InvalidOperationException"/>.
/// </para>
/// </remarks>
public interface ISettingsProvider
{
    /// <summary>
    /// Gets the current command settings.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if settings have not been populated yet.
    /// </exception>
    CommandSettings Settings { get; }

    /// <summary>
    /// Gets the current command settings as the specified type.
    /// </summary>
    /// <typeparam name="TSettings">The expected settings type.</typeparam>
    /// <returns>The settings cast to the specified type.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if settings have not been populated yet or if the settings
    /// are not of the expected type.
    /// </exception>
    TSettings GetSettings<TSettings>() where TSettings : CommandSettings;

    /// <summary>
    /// Gets a value indicating whether settings have been populated.
    /// </summary>
    bool HasSettings { get; }
}
