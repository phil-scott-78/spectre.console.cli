using System.ComponentModel;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Settings;

public sealed class ServiceWithSettingsSettings : CommandSettings
{
    [CommandArgument(0, "<MESSAGE>")]
    [Description("The message to display")]
    public string Message { get; set; } = string.Empty;

    [CommandOption("-p|--prefix")]
    [Description("Prefix to add")]
    public string? Prefix { get; set; }
}
