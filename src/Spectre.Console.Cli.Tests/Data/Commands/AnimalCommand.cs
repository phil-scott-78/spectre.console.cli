namespace Spectre.Console.Tests.Data;

// Suppress SCCLI004: This generic command is used for testing purposes only.
// In production, users should create concrete non-generic command classes.
#pragma warning disable SCCLI004
public abstract class AnimalCommand<TSettings> : Command<TSettings>
    where TSettings : CommandSettings
{
}