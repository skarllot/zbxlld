using Jab;
using Microsoft.Extensions.Logging;
using zbxlld.Windows.Cli;

namespace zbxlld.Windows.DependencyInjection;

[ServiceProvider]
[Import(typeof(IEventLogLoggingModule))]
[Import(typeof(IDriveDiscoveryModule))]
[Import(typeof(INetworkDiscoveryModule))]
[Import(typeof(IServiceDiscoveryModule))]
[Singleton(typeof(HelpCommand))]
[Singleton(typeof(VersionCommand))]
[Singleton(typeof(CommandApp))]
[Transient(typeof(LogLevel), Factory = nameof(GetLogLevel))]
public sealed partial class ZbxlldContainer
{
    private readonly LogLevel _logLevel;

    public ZbxlldContainer(LogLevel logLevel)
    {
        _logLevel = logLevel;
    }

    private LogLevel GetLogLevel() => _logLevel;
}
