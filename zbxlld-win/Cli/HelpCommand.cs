using System.Globalization;
using System.Text;

namespace zbxlld.Windows.Cli;

public class HelpCommand
{
    private const string UsageTemplate =
        $$"""
          {{ThisAssembly.Info.Product}}.

          Usage:
          {Items}
            {{ThisAssembly.Info.Title}} -h | --help
            {{ThisAssembly.Info.Title}} --version

          Options:
            -h --help Show this screen.
            --version Show version.
            --pretty  Show indented JSON output.
          """;

    private readonly string _usage;

    public HelpCommand(IEnumerable<ICommandProvider> providers)
    {
        var providerItems = new StringBuilder();
        var isFirst = true;
        foreach (var handler in providers.SelectMany(static it => it.GetCommands()))
        {
            if (!isFirst)
                providerItems.AppendLine();

            providerItems.Append(
                CultureInfo.InvariantCulture,
                $"  {ThisAssembly.Info.Title} {handler.Key} [-s <key-suffix>]");
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
