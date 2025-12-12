using System.ComponentModel;

namespace Spectre.Console.Cli.SourceGenerator.Tests.Settings;

/// <summary>
/// Tests FileInfo and DirectoryInfo type conversion.
/// These types use TypeDescriptor.GetConverter() which is reflection-heavy.
/// </summary>
public sealed class FileInfoSettings : CommandSettings
{
    [CommandOption("--file <PATH>")]
    [DefaultValue("default.txt")]
    public FileInfo? File { get; set; }

    [CommandOption("--directory <PATH>")]
    public DirectoryInfo? Directory { get; set; }

    [CommandOption("--output <PATH>")]
    public FileInfo? Output { get; set; }
}