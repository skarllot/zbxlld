using Jab;
using zbxlld.Windows.Cli;
using zbxlld.Windows.DriveDiscovery;
using zbxlld.Windows.Supplement.PerfMon;

namespace zbxlld.Windows.DependencyInjection;

[ServiceProviderModule]
[Singleton(typeof(ICommandProvider), typeof(DriveCommandProvider))]
[Singleton(typeof(Drive))]
[Singleton(typeof(NativeVolumeFactory))]
[Singleton(typeof(LogicalDisk))]
[Singleton(typeof(Localization))]
public interface IDriveDiscoveryModule
{
}