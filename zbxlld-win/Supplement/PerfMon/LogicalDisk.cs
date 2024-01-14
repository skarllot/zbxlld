using System.Diagnostics;
using zbxlld.Windows.DriveDiscovery;

namespace zbxlld.Windows.Supplement.PerfMon;

public class LogicalDisk
{
    private const string CounterLogicalDisk = "236";
    private const string CounterFreeMb = "410";
    private const long MbMult = 1048576;

    private readonly Localization _localization;

    // Relates Performance Monitor instance name to volume GUID.
    private Dictionary<Guid, string>? _perfMonGuid;

    public LogicalDisk(Localization localization)
    {
        _localization = localization;
    }

    // Try to discover GUID from buggy Performance Monitor instance names.
    // Note: Discover drive GUID comparing free space is ugly, but MS gave me no choice.
    private static Dictionary<Guid, string> CreateVolumeDictionary(Localization localization)
    {
        // =====         WMI         =====
        var vols = Win32Volume.GetAllVolumes();
        // Free megabytes and volume GUID relation
        var wmiFree = new Dictionary<ulong, Guid>(vols.Length);
        // Volume name and volume GUID relation
        var wmiName = new Dictionary<string, Guid>(vols.Length);

        foreach (var v in vols) {
            if (v.Automount &&
                v.DriveType == DriveType.Fixed) {
                if (v.IsMounted) {
                    wmiName.Add(v.Name.TrimEnd('\\'), v.DeviceGuid);
                } else {
                    wmiFree.Add(v.FreeSpace / MbMult, v.DeviceGuid);
                }
            }
        }

        var result = new Dictionary<Guid, string>(wmiFree.Count + wmiName.Count);

        // ===== PERFORMANCE MONITOR ======
        var perfCat = new PerformanceCounterCategory(localization.GetName(CounterLogicalDisk));
        // TODO: Find a faster way to get instance names.
        string[] instances = perfCat.GetInstanceNames();
        // Free megabytes and Performance Monitor instance name
        var perfFree = new Dictionary<ulong, string>(instances.Length);

        foreach (var item in instances) {
            if (item == "_Total")
                continue;

            if (wmiName.TryGetValue(item, out var volId)) {
                result.Add(volId, item);
            } else {
                var p = new PerformanceCounter(
                    localization.GetName(CounterLogicalDisk),
                    localization.GetName(CounterFreeMb),
                    item);
                perfFree.Add((ulong)p.RawValue, item);
                p.Close();
                p.Dispose();
            }
        }

        foreach (var pf in perfFree)
        {
            if (wmiFree.TryGetValue(pf.Key, out var guid))
                result.Add(guid, pf.Value);
        }

        return result;
    }

    public string? GetInstanceName(Guid volumeId)
    {
        _perfMonGuid ??= CreateVolumeDictionary(_localization);
        _perfMonGuid.TryGetValue(volumeId, out var ret);
        return ret;
    }
}
