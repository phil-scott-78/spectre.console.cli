namespace Spectre.Console.Tests.Data;

public class FooSettingsCommand : Command<FooCommandSettings>
{
    protected override int Execute(CommandContext context, FooCommandSettings settings, CancellationToken cancellationToken)
    {
        return 0;
    }
}
