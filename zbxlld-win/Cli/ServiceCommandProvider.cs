using System.Collections.Generic;
using zbxlld.Windows.Discovery;

namespace zbxlld.Windows.Cli;

public class ServiceCommandProvider : ICommandProvider
{
    private readonly Service _service;

    public ServiceCommandProvider(Service service)
    {
        _service = service;
    }

    public IEnumerable<CommandHandler> GetCommands()
    {
        const string prefix = "service.discovery";
        yield return new(prefix + "", "", _service.GetAny);
        yield return new(prefix + ".any", "", _service.GetAny);
        yield return new(prefix + ".auto", "", _service.GetAuto);
        yield return new(prefix + ".demand", "", _service.GetManual);
        yield return new(prefix + ".manual", "", _service.GetManual);
        yield return new(prefix + ".disabled", "", _service.GetDisabled);
    }
}