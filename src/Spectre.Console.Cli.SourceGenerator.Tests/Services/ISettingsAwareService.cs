namespace Spectre.Console.Cli.SourceGenerator.Tests.Services;

/// <summary>
/// Service that accesses Settings via ISettingsProvider.
/// This pattern works because the provider is populated after settings
/// are bound, and services access settings lazily during execution.
/// </summary>
public interface ISettingsAwareService
{
    string FormatMessage();
}
