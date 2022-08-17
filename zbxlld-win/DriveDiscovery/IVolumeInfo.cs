using System;
using System.IO;

namespace zbxlld.Windows.DriveDiscovery
{
	public interface IVolumeInfo
	{
		bool Automount { get; }
		string Caption { get; }
		string? DriveLetter { get; }
		DriveType DriveType { get; }
		bool IsMounted { get; }
		string? Label { get; }
		string? Name { get; }
		bool? PageFilePresent { get; }
		string? VolumeFormat { get; }
		Guid? VolumeGuid { get; }
	}
}

