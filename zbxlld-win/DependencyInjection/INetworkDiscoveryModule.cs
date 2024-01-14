using Jab;
using zbxlld.Windows.Cli;
using zbxlld.Windows.Discovery;

namespace zbxlld.Windows.DependencyInjection;

[ServiceProviderModule]
[Singleton(typeof(ICommandProvider), typeof(NetworkCommandProvider))]
[Singleton(typeof(Network))]
public interface INetworkDiscoveryModule;