namespace Spectre.Console.Tests.Data;

public class ArgumentVectorCommand : Command<ArgumentVectorSettings>
{
    protected override int Execute(CommandContext context, ArgumentVectorSettings settings, CancellationToken cancellationToken)
    {
        return 0;
    }
}
