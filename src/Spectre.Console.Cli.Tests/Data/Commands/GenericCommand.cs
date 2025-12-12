namespace Spectre.Console.Tests.Data;

// Suppress SCCLI004: This generic command is used for testing purposes only.
// In production, users should create concrete non-generic command classes.
#pragma warning disable SCCLI004
public sealed class GenericCommand<TSettings> : Command<TSettings>
    where TSettings : CommandSettings
{
    protected override int Execute(CommandContext context, TSettings settings, CancellationToken cancellationToken)
    {
        if (settings is ArgumentOrderSettings argumentOrderSettings)
        {
            System.Console.WriteLine(argumentOrderSettings.Bar);
        }
        return 0;
    }
}