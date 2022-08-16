using System.Collections.Generic;

namespace zbxlld.Windows;

public interface ICommandProvider
{
    IEnumerable<CommandHandler> GetCommands();
}