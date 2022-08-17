using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zbxlld.Windows.Cli;

public class HelpCommand
{
    private const string UsageTemplate = @$"{ThisAssembly.Info.Product}.

Usage:
{{Items}}
  {ThisAssembly.Info.Title} -h | --help
  {ThisAssembly.Info.Title} --version

Options:
  -h --help Show this screen.
  --version Show version.
  --pretty  Show indented JSON output.";

    private readonly string _usage;

    public HelpCommand(IEnumerable<ICommandProvider> providers)
    {
        var providerItems = new StringBuilder();
        var isFirst = true;
        foreach (var handler in providers.SelectMany(static it => it.GetCommands()))
        {
            if (!isFirst)
                providerItems.AppendLine();

            providerItems.AppendFormat("  {0} {1} [-s <key-suffix>]", ThisAssembly.Info.Title, handler.Key);
            isFirst = false;
        }

        _usage = UsageTemplate.Replace("{Items}", providerItems.ToString());
    }

    public void ShowHelp()
    {
        Console.WriteLine(_usage);
    }

    public void ShowError()
    {
        Console.WriteLine($"{ThisAssembly.Info.Title}: Invalid command. See '{ThisAssembly.Info.Title} --help'.");
    }
}