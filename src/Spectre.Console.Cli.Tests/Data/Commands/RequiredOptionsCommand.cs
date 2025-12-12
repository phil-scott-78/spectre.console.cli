namespace Spectre.Console.Tests.Data;

public class RequiredOptionsCommand : Command<RequiredOptionsSettings>
{
    protected override int Execute(CommandContext context, RequiredOptionsSettings settings, CancellationToken cancellationToken)
    {
        return 0;
    }
}
