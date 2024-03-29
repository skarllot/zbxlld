using System.Diagnostics.CodeAnalysis;
using System.Management;

namespace zbxlld.Windows.DriveDiscovery;

public class Win32Volume : IVolumeInfo
{
    private const string WqlVolume = "Select * from Win32_Volume";
    private const string UnmountedPrefix = @"\\?";

    private readonly Dictionary<string, object?> _properties;

    [SuppressMessage("Maintainability", "CA1507:Use nameof in place of string")]
    private Win32Volume(ManagementObject mgtobj)
    {
        Automount = (bool)mgtobj.Properties["Automount"].Value;
        Capacity = (ulong)mgtobj.Properties["Capacity"].Value;
        DeviceId = (string)mgtobj.Properties["DeviceID"].Value;
        DriveLetter = (string?)mgtobj.Properties["DriveLetter"].Value;
        DriveType = (DriveType)Enum.ToObject(typeof(DriveType), mgtobj.Properties["DriveType"].Value);
        FileSystem = (string)mgtobj.Properties["FileSystem"].Value;
        FreeSpace = (ulong)mgtobj.Properties["FreeSpace"].Value;
        Label = (string?)mgtobj.Properties["Label"].Value;
        Name = (string)mgtobj.Properties["Name"].Value;

        _properties = new Dictionary<string, object?>(mgtobj.Properties.Count);
        foreach (var item in mgtobj.Properties)
            _properties.Add(item.Name, item.Value);
    }

    /// <summary>
    /// If true, the volume is mounted to the file system automatically when the first I/O is issued. If false, the
    /// volume is not mounted until explicitly mounted by using the Mount method, or by adding a drive letter or
    /// mount point.
    /// </summary>
    public bool Automount { get; }

    /// <summary>
    /// Describes the availability and status of the device.
    /// </summary>
    public DriveAvailability? Availability {
        get {
            if (!_properties.TryGetValue("Availability", out var val))
                return null;
            return val is not null ? (DriveAvailability)Enum.ToObject(typeof(DriveAvailability), val) : null;
        }
    }

    /// <summary>
    /// Size of the volume in bytes.
    /// </summary>
    public ulong Capacity { get; }

    /// <summary>
    /// Unique identifier for the volume on this system.
    /// </summary>
    public string DeviceId { get; }

    public Guid DeviceGuid {
        get {
            var idx = DeviceId.IndexOf('{');
            return Guid.Parse(DeviceId.AsSpan(idx, DeviceId.IndexOf('}') - idx + 1));
        }
    }

    /// <summary>
    /// Drive letter assigned to a volume. This property is NULL for volumes without drive letters.
    /// </summary>
    public string? DriveLetter { get; }

    /// <summary>
    /// Numeric value that corresponds to the type of disk drive that this logical disk represents.
    /// </summary>
    public DriveType DriveType { get; }

    /// <summary>
    /// File system on the logical disk.
    /// </summary>
    public string FileSystem { get; }

    /// <summary>
    /// Number of bytes of available space on the volume.
    /// </summary>
    public ulong FreeSpace { get; }

    /// <summary>
    /// Gets a value indicating whether this volume is mounted.
    /// </summary>
    public bool IsMounted {
        get {
            return DriveLetter is not null || !Name.StartsWith(UnmountedPrefix, StringComparison.Ordinal);
        }
    }

    /// <summary>
    /// Volume name of the logical disk. This property is null for volumes without a label. For FAT and FAT32
    /// systems, the maximum length is 11 characters. For NTFS file systems, the maximum length is 32 characters.
    /// </summary>
    public string? Label { get; }

    /// <summary>
    /// Defines the label by which the object is known. When subclassed, the Name property can be overridden to be
    /// a Key property.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The PageFilePresent property indicates whether the volume contains a system memory paging file.
    /// </summary>
    public bool? PageFilePresent {
        get {
            if (!_properties.TryGetValue("PageFilePresent", out var val))
                return null;
            return val as bool?;
        }
    }

    /// <summary>
    /// Indicates the state of the logical device.
    /// </summary>
    public DriveStatus? StatusInfo {
        get {
            if (!_properties.TryGetValue("StatusInfo", out var val))
                return null;
            return val is not null ? (DriveStatus)Enum.ToObject(typeof(DriveStatus), val) : null;
        }
    }

    public static Win32Volume[] GetAllVolumes()
    {
        var wmicol = new ManagementObjectSearcher(WqlVolume).Get();

        var ret = new Win32Volume[wmicol.Count];
        var i = 0;
        foreach (var item in wmicol.OfType<ManagementObject>()) {
            ret[i] = new Win32Volume(item);
            i++;
        }

        return ret;
    }

    public override string ToString()
    {
        var label = Label;
        var name = Name.AsSpan().TrimEnd(Path.DirectorySeparatorChar);
        if (!IsMounted)
            name = string.Concat(UnmountedPrefix, "\\", name.Slice(11, 6));

        if (label == null)
            return name.ToString();
        else
            return $"{label} ({name})";
    }

    string IVolumeInfo.Caption => ToString();

    string IVolumeInfo.VolumeFormat => FileSystem;

    Guid? IVolumeInfo.VolumeGuid => DeviceGuid;
}
