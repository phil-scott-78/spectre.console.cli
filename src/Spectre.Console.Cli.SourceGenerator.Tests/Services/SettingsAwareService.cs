using Spectre.Console.Cli.SourceGenerator.Tests.Settings;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Services;

/// <summary>
/// Service that accesses Settings via ISettingsProvider.
/// This pattern works because the provider is populated after settings
/// are bound, and services access settings lazily during execution.
/// </summary>
public sealed class SettingsAwareService : ISettingsAwareService
{
    private readonly ISettingsProvider _settingsProvider;

    public SettingsAwareService(ISettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
    }

    public string FormatMessage()
    {
        // Settings are accessed at call time, after they've been bound
        var settings = _settingsProvider.GetSettings<ServiceWithSettingsSettings>();
        return string.IsNullOrEmpty(settings.Prefix)
            ? settings.Message
            : $"{settings.Prefix}: {settings.Message}";
    }
}
