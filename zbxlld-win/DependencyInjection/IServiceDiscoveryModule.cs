using Jab;
using zbxlld.Windows.Discovery;

namespace zbxlld.Windows.DependencyInjection;

[ServiceProviderModule]
[Singleton(typeof(ICommandProvider), typeof(ServiceCommandProvider))]
[Singleton(typeof(Service))]
public interface IServiceDiscoveryModule
{
}