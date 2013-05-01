//
//  Win32_Volume.cs
//
//  Author:
//       Fabricio Godoy <skarllot@gmail.com>
//
//  Copyright (c) 2013 Fabricio Godoy
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Management;
using System.IO;

namespace zbxlld.Windows.Supplement
{
	public class Win32_Volume
	{
		const string WQL_VOLUME = "Select * from Win32_Volume";
		const string UNMOUNTED_PREFIX = "\\\\?";

		ManagementObject mgtobj;

		private Win32_Volume(ManagementObject mgtobj)
		{
			this.mgtobj = mgtobj;
		}

		/// <summary>
		/// If true, the volume is mounted to the file system automatically when the first I/O is issued. If false, the
		/// volume is not mounted until explicitly mounted by using the Mount method, or by adding a drive letter or
		/// mount point.
		/// </summary>
		public bool Automount {
			get {
				return (bool)mgtobj["Automount"];
			}
		}

		/// <summary>
		/// Describes the availability and status of the device.
		/// </summary>
		public DriveAvailability Availability {
			get {
				return (DriveAvailability)Enum.ToObject(typeof(DriveAvailability), mgtobj["Availability"] ?? -1);
			}
		}

		/// <summary>
		/// Size of the volume in bytes.
		/// </summary>
		public UInt64 Capacity {
			get {
				return (UInt64)(mgtobj["Capacity"] ?? -1);
			}
		}

		/// <summary>
		/// Unique identifier for the volume on this system.
		/// </summary>
		public string DeviceID {
			get {
				return (string)mgtobj["DeviceID"];
			}
		}

		public Guid DeviceGuid {
			get {
				string volid = DeviceID.TrimEnd('\\');
				int idx = volid.IndexOf('{');
				return new Guid(volid.Substring(idx));
			}
		}

		/// <summary>
		/// Drive letter assigned to a volume. This property is NULL for volumes without drive letters.
		/// </summary>
		public string DriveLetter {
			get {
				return (string)mgtobj["DriveLetter"];
			}
		}

		/// <summary>
		/// Numeric value that corresponds to the type of disk drive that this logical disk represents.
		/// </summary>
		public DriveType DriveType {
			get {
				return (DriveType)Enum.ToObject(typeof(DriveType), mgtobj["DriveType"] ?? -1);
			}
		}

		/// <summary>
		/// File system on the logical disk.
		/// </summary>
		public string FileSystem {
			get {
				return (string)mgtobj["FileSystem"];
			}
		}

		/// <summary>
		/// Number of bytes of available space on the volume.
		/// </summary>
		public UInt64 FreeSpace {
			get {
				return (UInt64)(mgtobj["FreeSpace"] ?? -1);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this volume is mounted.
		/// </summary>
		public bool IsMounted {
			get {
				return !(
					DriveLetter == null &&
					Name.IndexOf(UNMOUNTED_PREFIX, 0) == 0);
			}
		}

		/// <summary>
		/// Volume name of the logical disk. This property is null for volumes without a label. For FAT and FAT32
		/// systems, the maximum length is 11 characters. For NTFS file systems, the maximum length is 32 characters.
		/// </summary>
		public string Label {
			get {
				return (string)mgtobj["Label"];
			}
		}

		/// <summary>
		/// Defines the label by which the object is known. When subclassed, the Name property can be overridden to be
		/// a Key property.
		/// </summary>
		public string Name {
			get {
				return (string)mgtobj["Name"];
			}
		}

		/// <summary>
		/// The PageFilePresent property indicates whether the volume contains a system memory paging file.
		/// </summary>
		public bool PageFilePresent {
			get {
				bool ret = false;
				try {
					ret = (bool)mgtobj["PageFilePresent"];
				}
				catch { }
				return ret;
			}
		}

		/// <summary>
		/// Indicates the state of the logical device.
		/// </summary>
		public DriveStatus StatusInfo {
			get {
				return (DriveStatus)(mgtobj["StatusInfo"] ?? DriveStatus.Unknown);
			}
		}

		public static Win32_Volume[] GetAllVolumes()
		{
			ManagementObjectCollection wmicol = 
				new ManagementObjectSearcher(WQL_VOLUME).Get();

			Win32_Volume[] ret = new Win32_Volume[wmicol.Count];
			int i = 0;
			foreach (ManagementObject item in wmicol) {
				ret[i] = new Win32_Volume(item);
				i++;
			}

			return ret;
		}

		public override string ToString()
		{
			string label = Label;
			string name = Name.TrimEnd(Path.DirectorySeparatorChar);
			if (!IsMounted)
				name = UNMOUNTED_PREFIX + "\\" + name.Substring(11, 6);

			if (label == null)
				return name;
			else
				return string.Format("{0} ({1})", label, name);
		}
	}
}
