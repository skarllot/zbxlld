using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace zbxlld.Windows.DriveDiscovery;

public partial class NativeVolume : IVolumeInfo
{
    private const string KeyMountedDevices = @"HKEY_LOCAL_MACHINE\SYSTEM\MountedDevices";
    private const string KeyValuePrefix = @"\DosDevices\";
    private const string KeyValueGuidPrefix = @"\??\Volume";
    private readonly ILogger<NativeVolume> _logger;
    private readonly DriveInfo _dinfo;
    private Guid? _volGuid;

    public NativeVolume(ILogger<NativeVolume> logger, DriveInfo dinfo)
    {
        _logger = logger;
        _dinfo = dinfo;
    }

    [MemberNotNullWhen(true, nameof(Label))]
    public bool Automount {
        get {
            return _dinfo.IsReady;
        }
    }

    public string Caption {
        get {
            var label = Label;
            var name = DriveLetter;

            return label == null
                ? name
                : $"{label} ({name})";
        }
    }

    public string DriveLetter {
        get {
            return Name.TrimEnd('\\');
        }
    }

    public DriveType DriveType {
        get {
            return _dinfo.DriveType;
        }
    }

    public bool IsMounted {
        get {
            return true;
        }
    }

    public string? Label {
        get
        {
            return _dinfo.IsReady
                ? _dinfo.VolumeLabel
                : null;
        }
    }

    public string Name {
        get {
            return _dinfo.Name;
        }
    }

    public bool? PageFilePresent {
        get {
            return null;
        }
    }

    public string? VolumeFormat {
        get
        {
            return _dinfo.IsReady
                ? _dinfo.DriveFormat
                : null;
        }
    }

    public Guid? VolumeGuid {
        get
        {
            return _volGuid ??= TryGetVolumeGuid();
        }
    }

    private Guid? TryGetVolumeGuid()
    {
        if (_volGuid.HasValue)
        {
            if (_volGuid.Value == Guid.Empty)
                return null;

            return _volGuid.Value;
        }

        using var regKey = Registry.LocalMachine.OpenSubKey(KeyMountedDevices);
        if (regKey == null)
            return null;

        var header = GetVolumeHeader(regKey, DriveLetter);
        if (header == null)
            return null;

        _volGuid = FindVolumeIdFromHeader(regKey, header);
        return _volGuid;
    }

    private byte[]? GetVolumeHeader(RegistryKey regKey, string driveLetter)
    {
        try
        {
            return (byte[]?)regKey.GetValue(KeyValuePrefix + driveLetter);
        }
        catch (Exception e)
        {
            LogKeyReadError(e);
            return null;
        }
    }

    private static Guid? FindVolumeIdFromHeader(RegistryKey regKey, byte[] header)
    {
        var values = regKey.GetValueNames();
        foreach (var item in values)
        {
            if (!item.StartsWith(KeyValueGuidPrefix, StringComparison.Ordinal))
            {
                continue;
            }

            var temp = (byte[]?)regKey.GetValue(item);
            if (SequenceEqual(header, temp))
            {
                return Guid.Parse(item.AsSpan(item.IndexOf('{')));
            }
        }

        return null;
    }

    private static bool SequenceEqual<T>(T[]? a1, T[]? a2) where T : IComparable<T>
    {
        if (a1 is null && a2 is null)
            return true;
        if (a1 is null || a2 is null)
            return false;
        if (a1.Length != a2.Length)
            return false;

        for (var i = 0; i < a1.Length; i++) {
            if (a1[i].CompareTo(a2[i]) != 0)
                return false;
        }

        return true;
    }

    [LoggerMessage(LogLevel.Error, "Error trying to read value from registry key")]
    private partial void LogKeyReadError(Exception exception);
}
