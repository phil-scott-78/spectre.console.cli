namespace Spectre.Console.Cli;

/// <summary>
/// Default implementation of <see cref="ISettingsProvider"/> that holds
/// bound settings and makes them available to injected services.
/// </summary>
internal sealed class SettingsProvider : ISettingsProvider
{
    private CommandSettings? _settings;

    /// <inheritdoc />
    public CommandSettings Settings
    {
        get
        {
            if (_settings == null)
            {
                throw new InvalidOperationException(
                    "Settings have not been populated yet. " +
                    "Ensure you access settings only during or after command execution.");
            }

            return _settings;
        }
    }

    /// <inheritdoc />
    public bool HasSettings => _settings != null;

    /// <inheritdoc />
    public TSettings GetSettings<TSettings>() where TSettings : CommandSettings
    {
        var settings = Settings;
        if (settings is TSettings typed)
        {
            return typed;
        }

        throw new InvalidOperationException(
            $"Settings are of type '{settings.GetType().FullName}', " +
            $"not the expected type '{typeof(TSettings).FullName}'.");
    }

    /// <summary>
    /// Sets the bound settings. Called by the command executor after binding.
    /// </summary>
    /// <param name="settings">The bound settings instance.</param>
    internal void SetSettings(CommandSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }
}
