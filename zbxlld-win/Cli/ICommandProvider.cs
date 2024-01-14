namespace zbxlld.Windows.Cli;

public interface ICommandProvider
{
    IEnumerable<CommandHandler> GetCommands();
}
