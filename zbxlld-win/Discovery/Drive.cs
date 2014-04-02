//
//  Drive.cs
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
using System.Collections.Generic;
using System.IO;
using zbxlld.Windows;

namespace zbxlld.Windows.Discovery
{
	public class Drive : IArgHandler
	{
		const string ARG_DRIVE = "drive.discovery";
		const string ARG_DRIVE_FIXED = ARG_DRIVE + ".fixed";
		const string ARG_DRIVE_REMOVABLE = ARG_DRIVE + ".removable";
		const string ARG_DRIVE_MOUNTED = ARG_DRIVE + ".mounted";
		const string ARG_DRIVE_MFOLDER = ARG_DRIVE + ".mountfolder";
		const string ARG_DRIVE_MLETTER = ARG_DRIVE + ".mountletter";
		const string ARG_DRIVE_NOMOUNT = ARG_DRIVE + ".nomount";
		const string ARG_DRIVE_SWAP = ARG_DRIVE + ".swap";
		const string ARG_DRIVE_NOSWAP = ARG_DRIVE + ".noswap";
        const string ARG_DRIVE_NETWORK = ARG_DRIVE + ".network";
        const string CLASS_FULL_PATH = "zbxlld.Windows.Discovery.Drive";

		static Drive def = new Drive();

		public static Drive Default {
			get {
				return def;
			}
		}

		private Drive()
		{
		}

        private bool TryGetVolumesViaWmi(out Supplement.IVolumeInfo[] vols)
        {
            vols = null;

            try
            {
                vols = Supplement.Win32_Volume.GetAllVolumes();
            }
            catch (System.OutOfMemoryException e)
            {
                if (MainClass.DEBUG)
                {
                    MainClass.WriteLogEntry(string.Format("{0}.GetOutput: Out of memory exception.", CLASS_FULL_PATH));
                    MainClass.WriteLogEntry("Exception:");
                    MainClass.WriteLogEntry(e.ToString());
                }
                // TODO: Make a proper exit
                Console.WriteLine("You have insufficient permissions or insufficient memory.");
                Environment.Exit((int)ErrorId.GetAllVolumesOutOfMemory);
            }
            catch (Exception e)
            {
                if (MainClass.DEBUG)
                {
                    MainClass.WriteLogEntry(string.Format(
                        "{0}.GetOutput: Unexpected exception.", CLASS_FULL_PATH));
                    MainClass.WriteLogEntry("Exception:");
                    MainClass.WriteLogEntry(e.ToString());
                }
                vols = null;
                return false;
            }

            return true;
        }

		#region IArgHandler implementation

		public Supplement.JsonOutput GetOutput(string key)
		{
			bool mounted = false;
			bool mfolder = false;
			bool mletter = false;
			bool nomount = false;
			bool swap = false;
			bool noswap = false;
            bool onlyNative = false;

			DriveType dtype;
			switch (key) {
				case ARG_DRIVE_FIXED:
					dtype = DriveType.Fixed;
					break;
				case ARG_DRIVE_REMOVABLE:
					dtype = DriveType.Removable;
					break;
				case ARG_DRIVE_MOUNTED:
					dtype = DriveType.Fixed;
					mounted = true;
					break;
				case ARG_DRIVE_MFOLDER:
					dtype = DriveType.Fixed;
					mfolder = true;
					break;
				case ARG_DRIVE_MLETTER:
					dtype = DriveType.Fixed;
					mletter = true;
					break;
				case ARG_DRIVE_NOMOUNT:
					dtype = DriveType.Fixed;
					nomount = true;
					break;
				case ARG_DRIVE_SWAP:
					dtype = DriveType.Fixed;
					swap = true;
					break;
				case ARG_DRIVE_NOSWAP:
					dtype = DriveType.Fixed;
					noswap = true;
					break;
                case ARG_DRIVE_NETWORK:
                    dtype = DriveType.Network;
                    onlyNative = true;
                    break;
				default:
					return null;
			}
			
			Supplement.JsonOutput jout = new Supplement.JsonOutput ();

			Supplement.IVolumeInfo[] vols = null;

            if (onlyNative)
            {
                vols = Supplement.NativeVolume.GetVolumes();
            }
            else
            {
                if (!TryGetVolumesViaWmi(out vols))
                {
                    if (MainClass.DEBUG)
                        MainClass.WriteLogEntry(string.Format("{0}.GetOutput: Fallback to native method.", CLASS_FULL_PATH));

                    // Fallback to native method
                    vols = Supplement.NativeVolume.GetVolumes();
                }
            }

            if (MainClass.DEBUG)
            {
                MainClass.WriteLogEntry(string.Format("{0}.GetOutput: got volumes. " +
                    "vols.Length: {1}", CLASS_FULL_PATH, vols.Length));
                for (int i = 0; i < vols.Length; i++)
                {
                    MainClass.WriteLogEntry(string.Format("{0}.GetOutput: vol[{1}] {{ " +
                        "Automount={2}, Caption={3}, DriveLetter={4}, DriveType={5}, " +
                        "IsMounted={6}, Label={7}, Name={8}, PageFilePresent={9}, " +
                        "VolumeFormat={10}, VolumeGuid={11} }}.", CLASS_FULL_PATH, i,
                        vols[i].Automount.ToString(), vols[i].Caption, vols[i].DriveLetter,
                        vols[i].DriveType.ToString(), vols[i].IsMounted.ToString(),
                        vols[i].Label, vols[i].Name, vols[i].PageFilePresent.ToString(),
                        vols[i].VolumeFormat, vols[i].VolumeGuid.ToString()));
                }
            }

			bool ismounted, ismletter;
			foreach (Supplement.IVolumeInfo v in vols) {
				ismounted = v.IsMounted;
				ismletter = (v.DriveLetter != null);

				if (v.Automount &&                              // Match to drives mounted automatically
				    v.DriveType == dtype &&                     // Match drive type
				    (!mounted || ismounted) &&                  // If defined to mounted drives, then match mounted drives
				    (!mfolder || (ismounted && !ismletter)) &&  // If defined to folder mounted drives, then match mounted and not has letter
				    (!mletter || (ismounted && ismletter)) &&   // If defined to letter mounted drives, then match mounted and has letter
				    (!nomount || !ismounted) &&                 // If defined to not mounted drives, then match not mounted drives
				    (!swap || v.PageFilePresent) &&             // If defined to drives that has page files, then match to drives that has page file
				    (!noswap || !v.PageFilePresent)) {          // If defined to drives that no page files, then match to drives that has no page file
					Dictionary<string, string> item =
						new Dictionary<string, string> (2);

					string pmInstName = string.Empty;
					if (ismounted) {
						pmInstName = v.Name.TrimEnd('\\');
					} else {
						pmInstName = Supplement.PerfMon.LogicalDisk.GetInstanceName(v.VolumeGuid);
					}

					item.Add ("FSNAME", v.Name);
					item.Add ("FSPERFMON", pmInstName);
					item.Add ("FSLABEL", v.Label ?? "");
					item.Add ("FSFORMAT", v.VolumeFormat);
					item.Add ("FSCAPTION", v.Caption);
					jout.Add (item);
				}
			}

			return jout;
		}

		string[] IArgHandler.GetAllowedArgs()
		{
			return new string[] {
				ARG_DRIVE_FIXED,
				ARG_DRIVE_REMOVABLE,
				ARG_DRIVE_MOUNTED,
				ARG_DRIVE_MFOLDER,
				ARG_DRIVE_MLETTER,
				ARG_DRIVE_NOMOUNT,
				ARG_DRIVE_SWAP,
				ARG_DRIVE_NOSWAP,
                ARG_DRIVE_NETWORK
			};
		}

		#endregion
	}
}

