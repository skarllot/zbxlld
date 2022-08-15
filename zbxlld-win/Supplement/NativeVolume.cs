using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Win32;

namespace zbxlld.Windows.Supplement
{
	public class NativeVolume : IVolumeInfo
	{
		private const string ClassFullPath = "zbxlld.Windows.Supplement.NativeVolume";
		private const string KeyMountedDevices = @"HKEY_LOCAL_MACHINE\SYSTEM\MountedDevices";
		private const string KeyValuePrefix = @"\DosDevices\";
		private const string KeyValueGuidPrefix = @"\??\Volume";
		private readonly DriveInfo dinfo;
		private Guid? volGuid;

		public NativeVolume(DriveInfo dinfo)
		{
			this.dinfo = dinfo;
		}

		public static NativeVolume[] GetVolumes() {
			DriveInfo[] di = DriveInfo.GetDrives();
			NativeVolume[] ret = new NativeVolume[di.Length];

			for (int i = 0; i < di.Length; i++) {
				ret[i] = new NativeVolume(di[i]);
			}

			return ret;
		}

		[MemberNotNullWhen(true, nameof(Label))]
		public bool Automount {
			get {
				return dinfo.IsReady;
			}
		}

		public string Caption {
			get {
				string? label = Label;
				string name = DriveLetter;
				
				if (label == null)
					return name;
				else
					return $"{label} ({name})";
			}
		}

		public string DriveLetter {
			get {
				return Name.TrimEnd('\\');
			}
		}

		public DriveType DriveType {
			get {
				return dinfo.DriveType;
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
	            return dinfo.IsReady
		            ? dinfo.VolumeLabel
		            : null;
            }
		}

		public string Name {
			get {
				return dinfo.Name;
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
	            return dinfo.IsReady
		            ? dinfo.DriveFormat
		            : null;
            }
		}

		public Guid? VolumeGuid {
			get
			{
				return volGuid ??= TryGetVolumeGuid();
			}
		}

		private Guid? TryGetVolumeGuid()
        {
            if (volGuid.HasValue)
            {
                if (volGuid.Value == Guid.Empty)
                    return null;

                return volGuid.Value;
            }

            using var regKey = Registry.LocalMachine.OpenSubKey(KeyMountedDevices);
            if (regKey == null)
                return null;

            object? keyval;
            try { keyval = regKey.GetValue(KeyValuePrefix + DriveLetter); }
            catch (Exception e)
            {
                if (MainClass.DEBUG)
                {
                    MainClass.WriteLogEntry(string.Format(
                    "{0}.get_VolumeGuid: Could not read value from registry key.", ClassFullPath));
                    MainClass.WriteLogEntry("Exception:");
                    MainClass.WriteLogEntry(e.ToString());
                }
                return null;
            }
            var header = (byte[]?)keyval;
            string[] values = regKey.GetValueNames();
            foreach (string item in values)
            {
                if (item.IndexOf(KeyValueGuidPrefix, StringComparison.Ordinal) == 0)
                {
	                var temp = (byte[]?)regKey.GetValue(item);
	                if (SequenceEqual(header, temp))
                    {
                        return volGuid = Guid.Parse(item.AsSpan(item.IndexOf('{')));
                    }
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

			for (int i = 0; i < a1.Length; i++) {
				if (a1[i].CompareTo(a2[i]) != 0)
					return false;
			}

			return true;
		}
	}
}

