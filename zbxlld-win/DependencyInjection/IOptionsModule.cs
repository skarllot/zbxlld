using Jab;
using Microsoft.Extensions.Options;

namespace zbxlld.Windows.DependencyInjection;

[ServiceProviderModule]
[Singleton(typeof(IOptions<>), typeof(OptionsManager<>))]
[Scoped(typeof(IOptionsSnapshot<>), typeof(OptionsManager<>))]
[Singleton(typeof(IOptionsMonitor<>), typeof(OptionsMonitor<>))]
[Transient(typeof(IOptionsFactory<>), typeof(OptionsFactory<>))]
[Singleton(typeof(IOptionsMonitorCache<>), typeof(OptionsCache<>))]
public interface IOptionsModule
{
    public static IConfigureOptions<TOptions> Configure<TOptions>(Action<TOptions> configure)
        where TOptions : class =>
        Configure(Options.DefaultName, configure);

    public static IConfigureOptions<TOptions> Configure<TOptions>(string name, Action<TOptions> configure)
        where TOptions : class =>
        new ConfigureNamedOptions<TOptions>(name, configure);
}
