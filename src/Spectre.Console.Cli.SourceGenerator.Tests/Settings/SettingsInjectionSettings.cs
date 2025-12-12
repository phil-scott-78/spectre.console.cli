using System.ComponentModel;
using Spectre.Console.Cli.SourceGenerator.Tests.Services;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Settings;

/// <summary>
/// Settings with constructor dependency injection.
/// Tests that the source generator correctly handles settings classes
/// with constructor parameters that should be resolved via DI.
/// </summary>
public sealed class SettingsInjectionSettings : CommandSettings
{
    private readonly ITestService _service;

    public SettingsInjectionSettings(ITestService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    [CommandArgument(0, "<MESSAGE>")]
    [Description("The message to process")]
    public string Message { get; set; } = string.Empty;

    [CommandOption("-u|--uppercase")]
    [Description("Convert output to uppercase")]
    public bool Uppercase { get; set; }

    /// <summary>
    /// Gets the message processed by the injected service.
    /// </summary>
    public string GetProcessedMessage()
    {
        var result = _service.GetMessage(Message);
        return Uppercase ? result.ToUpperInvariant() : result;
    }
}
