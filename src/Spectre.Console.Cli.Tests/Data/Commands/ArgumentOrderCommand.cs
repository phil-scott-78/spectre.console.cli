namespace Spectre.Console.Tests.Data;

public class ArgumentOrderCommand : Command<ArgumentOrderSettings>
{
    protected override int Execute(CommandContext context, ArgumentOrderSettings settings, CancellationToken cancellationToken)
    {
        return 0;
    }
}
