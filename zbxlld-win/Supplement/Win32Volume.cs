using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Management;
using System.IO;
using System.Linq;

namespace zbxlld.Windows.Supplement
{
	public class Win32Volume : IVolumeInfo
	{
		private const string WqlVolume = "Select * from Win32_Volume";
		private const string UnmountedPrefix = @"\\?";

		private readonly Dictionary<string, object?> properties;

		private Win32Volume(ManagementObject mgtobj)
		{
			Automount = (bool)mgtobj.Properties["Automount"].Value;
			Availability = (DriveAvailability)Enum.ToObject(typeof(DriveAvailability),mgtobj.Properties["Availability"].Value);
			Capacity = (ulong)mgtobj.Properties["Capacity"].Value;
			DeviceId = (string)mgtobj.Properties["DeviceID"].Value;
			DriveLetter = (string?)mgtobj.Properties["DriveLetter"].Value;
			DriveType = (DriveType)Enum.ToObject(typeof(DriveType), mgtobj.Properties["DriveType"].Value);
			FileSystem = (string)mgtobj.Properties["FileSystem"].Value;
			FreeSpace = (ulong)mgtobj.Properties["FreeSpace"].Value;
			Label = (string?)mgtobj.Properties["Label"].Value;
			Name = (string)mgtobj.Properties["Name"].Value;
			StatusInfo = (DriveStatus)Enum.ToObject(typeof(DriveStatus), mgtobj.Properties["StatusInfo"].Value);

            properties = new Dictionary<string, object?>(mgtobj.Properties.Count);
            foreach (var item in mgtobj.Properties)
                properties.Add(item.Name, item.Value);
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
		public DriveAvailability Availability { get; }

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
				int idx = DeviceId.IndexOf('{');
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
				return DriveLetter is not null || Name.IndexOf(UnmountedPrefix, StringComparison.Ordinal) != 0;
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
				if (!properties.TryGetValue("PageFilePresent", out object? val))
                    return null;
                return val as bool?;
			}
		}

		/// <summary>
		/// Indicates the state of the logical device.
		/// </summary>
		public DriveStatus StatusInfo { get; }

		public static Win32Volume[] GetAllVolumes()
		{
			var wmicol = new ManagementObjectSearcher(WqlVolume).Get();

			var ret = new Win32Volume[wmicol.Count];
			int i = 0;
			foreach (var item in wmicol.OfType<ManagementObject>()) {
				ret[i] = new Win32Volume(item);
				i++;
			}

			return ret;
		}

		public override string ToString()
		{
			string? label = Label;
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
}
