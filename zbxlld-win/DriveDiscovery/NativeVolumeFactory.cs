using Microsoft.Extensions.Logging;

namespace zbxlld.Windows.DriveDiscovery;

public class NativeVolumeFactory
{
    private readonly ILoggerFactory _loggerFactory;

    public NativeVolumeFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public NativeVolume[] GetVolumes()
    {
        var di = DriveInfo.GetDrives();
        var ret = new NativeVolume[di.Length];
        var logger = _loggerFactory.CreateLogger<NativeVolume>();

        for (var i = 0; i < di.Length; i++)
        {
            ret[i] = new NativeVolume(logger, di[i]);
        }

        return ret;
    }
}
