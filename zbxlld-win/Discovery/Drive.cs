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

namespace zbxlld.Windows.Discovery
{
	public class Drive : IArgHandler
	{
		const string ARG_DRIVE_FIXED = "drive.discovery.fixed";
		const string ARG_DRIVE_REMOVABLE = "drive.discovery.removable";

		static Drive def = new Drive();

		public static Drive Default {
			get {
				return def;
			}
		}

		#region IArgHandler implementation

		public Supplement.JsonOutput GetOutput(string arg)
		{
			DriveType dtype;
			switch (arg) {
				case ARG_DRIVE_FIXED:
					dtype = DriveType.Fixed;
					break;
				case ARG_DRIVE_REMOVABLE:
					dtype = DriveType.Removable;
					break;
				default:
					return null;
			}
			
			Supplement.JsonOutput jout = new Supplement.JsonOutput ();
			
			DriveInfo[] drives = DriveInfo.GetDrives ();
			foreach (DriveInfo d in drives) {
				if (d.IsReady && d.DriveType == dtype) {
					Dictionary<string, string> item =
						new Dictionary<string, string> (2);
					
					item.Add ("FSNAME", d.Name.Replace ("\\", ""));
					item.Add ("FSLABEL", d.VolumeLabel);
					item.Add ("FSFORMAT", d.DriveFormat);
					jout.Add (item);
				}
			}

			return jout;
		}

		#endregion

		#region IArgHandler implementation

		string[] IArgHandler.GetAllowedArgs()
		{
			return new string[] { ARG_DRIVE_FIXED, ARG_DRIVE_REMOVABLE };
		}

		#endregion
	}
}

