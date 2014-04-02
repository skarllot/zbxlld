//
//  NativeVolume.cs
//
//  Author:
//       Fabricio Godoy <skarllot@gmail.com>
//
//  Copyright (c) 2014 Fabricio Godoy
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
using System.IO;
using Microsoft.Win32;
using System.Collections;

namespace zbxlld.Windows.Supplement
{
	public class NativeVolume : IVolumeInfo
	{
        const string CLASS_FULL_PATH = "zbxlld.Windows.Supplement.NativeVolume";
		const string KEY_MOUNTED_DEVICES = @"HKEY_LOCAL_MACHINE\SYSTEM\MountedDevices";
		const string KEY_VALUE_PREFIX = @"\DosDevices\";
		const string KEY_VALUE_GUID_PREFIX = @"\??\Volume";
		DriveInfo dinfo;
        Guid? volGuid;

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

		#region IVolumeInfo implementation

		public bool Automount {
			get {
				return dinfo.IsReady;
			}
		}

		public string Caption {
			get {
				string label = Label;
				string name = DriveLetter;
				
				if (label == null)
					return name;
				else
					return string.Format("{0} ({1})", label, name);
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

		public string Label {
            get
            {
                if (dinfo.IsReady)
                    return dinfo.VolumeLabel;
                else
                    return null;
            }
		}

		public string Name {
			get {
				return dinfo.Name;
			}
		}

		public bool PageFilePresent {
			get {
				return false;
			}
		}

		public string VolumeFormat {
            get
            {
                if (dinfo.IsReady)
                    return dinfo.DriveFormat;
                else
                    return null;
            }
		}

		public Guid VolumeGuid {
			get {
                Guid guid;
                TryGetVolumeGuid(out guid);
                return guid;
			}
		}

		#endregion

        private bool TryGetVolumeGuid(out Guid volGuid)
        {
            volGuid = Guid.Empty;

            if (this.volGuid.HasValue)
            {
                if (this.volGuid.Value == Guid.Empty)
                    return false;

                volGuid = this.volGuid.Value;
                return true;
            }

            this.volGuid = Guid.Empty;
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey(KEY_MOUNTED_DEVICES);
            if (regKey == null)
                return false;

            object keyval;
            try { keyval = regKey.GetValue(KEY_VALUE_PREFIX + DriveLetter); }
            catch (Exception e)
            {
                if (MainClass.DEBUG)
                {
                    MainClass.WriteLogEntry(string.Format(
                    "{0}.get_VolumeGuid: Could not read value from registry key.", CLASS_FULL_PATH));
                    MainClass.WriteLogEntry("Exception:");
                    MainClass.WriteLogEntry(e.ToString());
                }
                return false;
            }
            byte[] header = (byte[])keyval;
            string[] values = regKey.GetValueNames();
            byte[] temp;
            foreach (string item in values)
            {
                if (item.IndexOf(KEY_VALUE_GUID_PREFIX) == 0)
                {
                    temp = (byte[])regKey.GetValue(item);
                    if (SequenceEqual<byte>(header, temp))
                    {
                        regKey.Close();
                        volGuid = new Guid(item.Substring(item.IndexOf('{')));
                        this.volGuid = volGuid;
                        return true;
                    }
                }
            };

            regKey.Close();
            return false;
        }

		private static bool SequenceEqual<T>(T[] a1, T[] a2) where T : IComparable<T>
		{
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

