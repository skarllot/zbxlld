using Jab;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.Extensions.Options;

namespace zbxlld.Windows.DependencyInjection;

[ServiceProviderModule]
[Import(typeof(IOptionsModule))]
[Singleton(typeof(ILoggerFactory), typeof(LoggerFactory))]
[Singleton(typeof(ILogger<>), typeof(Logger<>))]
[Singleton(typeof(IConfigureOptions<LoggerFilterOptions>), Factory = nameof(CreateFilterOptions))]
public interface ILoggingModule
{
    public static IConfigureOptions<LoggerFilterOptions> CreateFilterOptions(LogLevel logLevel) =>
        IOptionsModule.Configure<LoggerFilterOptions>(options => options.MinLevel = logLevel);
}

[ServiceProviderModule]
[Import(typeof(ILoggingModule))]
[Singleton(typeof(ILoggerProvider), typeof(EventLogLoggerProvider))]
public interface IEventLogLoggingModule
{
}