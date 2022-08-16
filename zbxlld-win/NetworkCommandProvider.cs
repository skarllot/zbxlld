using System.Collections.Generic;
using zbxlld.Windows.Discovery;
using zbxlld.Windows.DriveDiscovery;

namespace zbxlld.Windows;

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