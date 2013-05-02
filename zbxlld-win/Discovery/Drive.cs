//
//  Drive.cs
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

		static Drive def = new Drive();

		public static Drive Default {
			get {
				return def;
			}
		}

		private Drive()
		{
		}

		#region IArgHandler implementation

		public Supplement.JsonOutput GetOutput(string arg)
		{
			bool mounted = false;
			bool mfolder = false;
			bool mletter = false;
			bool nomount = false;
			bool swap = false;
			bool noswap = false;

			DriveType dtype;
			switch (arg) {
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
				default:
					return null;
			}
			
			Supplement.JsonOutput jout = new Supplement.JsonOutput ();

			Supplement.IVolumeInfo[] vols = null;
			try {
				vols = Supplement.Win32_Volume.GetAllVolumes();
			} catch {
				// Fallback to native method
				vols = Supplement.NativeVolume.GetVolumes();
			}

			bool ismounted, ismletter;
			foreach (Supplement.IVolumeInfo v in vols) {
				ismounted = v.IsMounted;
				ismletter = (v.DriveLetter != null);

				if (v.Automount &&
				    v.DriveType == dtype &&
				    (!mounted || ismounted) &&
				    (!mfolder || (ismounted && !ismletter)) &&
				    (!mletter || (ismounted && ismletter)) &&
				    (!nomount || !ismounted) &&
				    (!swap || v.PageFilePresent) &&
				    (!noswap || !v.PageFilePresent)) {
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

		#endregion

		#region IArgHandler implementation

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
				ARG_DRIVE_NOSWAP
			};
		}

		#endregion
	}
}

