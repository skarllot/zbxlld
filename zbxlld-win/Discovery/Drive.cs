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
using System.IO;
using System.Collections.Generic;
using zbxlld.Windows.Supplement;

namespace zbxlld.Windows.Discovery
{
	public class Drive : IArgHandler
	{
		const string ARG_DRIVE_FIXED = "drive.discovery.fixed";
		const string ARG_DRIVE_REMOVABLE = "drive.discovery.removable";
		const string ARG_DRIVE_MPOINT = "drive.discovery.mountpoint";
		const string ARG_DRIVE_NOMPOINT = "drive.discovery.nomountpoint";
		const string ARG_DRIVE_SWAP = "drive.discovery.swap";
		const string ARG_DRIVE_NOSWAP = "drive.discovery.noswap";

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
			bool mpoint = false;
			bool nompoint = false;
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
				case ARG_DRIVE_MPOINT:
					dtype = DriveType.Fixed;
					mpoint = true;
					break;
				case ARG_DRIVE_NOMPOINT:
					dtype = DriveType.Fixed;
					nompoint = true;
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

			Win32_Volume[] vols = Win32_Volume.GetAllVolumes();
			bool ismpoint;
			foreach (Win32_Volume v in vols) {
				ismpoint = (v.DriveLetter == null);

				if (v.Automount &&
				    v.DriveType == dtype &&
				    (!mpoint || ismpoint) &&
				    (!nompoint || !ismpoint) &&
				    (!swap || v.PageFilePresent) &&
				    (!noswap || !v.PageFilePresent)) {
					Dictionary<string, string> item =
						new Dictionary<string, string> (2);
					
					item.Add ("FSNAME", v.Name.TrimEnd('\\').Replace('\\', '/'));
					item.Add ("FSLABEL", v.Label);
					item.Add ("FSFORMAT", v.FileSystem);
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
				ARG_DRIVE_MPOINT,
				ARG_DRIVE_NOMPOINT,
				ARG_DRIVE_SWAP,
				ARG_DRIVE_NOSWAP
			};
		}

		#endregion
	}
}

