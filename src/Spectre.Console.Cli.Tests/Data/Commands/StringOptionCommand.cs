namespace Spectre.Console.Tests.Data;

public class StringOptionCommand : Command<StringOptionSettings>
{
    protected override int Execute(CommandContext context, StringOptionSettings settings, CancellationToken cancellationToken)
    {
        return 0;
    }
}
