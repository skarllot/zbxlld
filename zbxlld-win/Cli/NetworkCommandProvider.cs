using zbxlld.Windows.Discovery;

namespace zbxlld.Windows.Cli;

public class NetworkCommandProvider : ICommandProvider
{
    private readonly Network _network;

    public NetworkCommandProvider(Network network)
    {
        _network = network;
    }

    public IEnumerable<CommandHandler> GetCommands()
    {
        yield return new("network.discovery", "", _network.GetAll);
    }
}
