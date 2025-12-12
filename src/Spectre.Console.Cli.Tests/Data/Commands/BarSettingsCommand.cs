namespace Spectre.Console.Tests.Data;

public class BarSettingsCommand : Command<BarCommandSettings>
{
    protected override int Execute(CommandContext context, BarCommandSettings settings, CancellationToken cancellationToken)
    {
        return 0;
    }
}
