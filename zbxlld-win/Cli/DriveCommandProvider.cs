using System.Collections.Generic;
using zbxlld.Windows.DriveDiscovery;

namespace zbxlld.Windows.Cli;

public class DriveCommandProvider : ICommandProvider
{
    private readonly Drive _drive;

    public DriveCommandProvider(Drive drive)
    {
        _drive = drive;
    }

    public IEnumerable<CommandHandler> GetCommands()
    {
        const string prefix = "drive.discovery.";
        yield return new(prefix + "fixed", "", _drive.GetFixed);
        yield return new(prefix + "removable", "", _drive.GetRemovable);
        yield return new(prefix + "mounted", "", _drive.GetMounted);
        yield return new(prefix + "mountfolder", "", _drive.GetMountedFolder);
        yield return new(prefix + "mountletter", "", _drive.GetMountedLetter);
        yield return new(prefix + "nomount", "", _drive.GetNotMounted);
        yield return new(prefix + "swap", "", _drive.GetSwap);
        yield return new(prefix + "noswap", "", _drive.GetNoSwap);
        yield return new(prefix + "network", "", _drive.GetNetwork);
    }
}