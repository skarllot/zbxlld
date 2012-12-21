//
//  Main.cs
//
//  Author:
//       Fabricio Godoy <skarllot@gmail.com>
//
//  Copyright (c) 2012 Fabricio Godoy
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
using System.Net.NetworkInformation;

namespace zbxlld.windows
{
	class MainClass
	{
		private const string ARG_DRIVE_FIXED = "drive.discovery.fixed";
		private const string ARG_DRIVE_REMOVABLE = "drive.discovery.removable";
		private const string ARG_NETWORK = "network.discovery";

		public static void Main (string[] args)
		{
			if (args.Length < 1) {
				Console.WriteLine ("At least one parameter should be provided.");
				return;
			}

			JsonOutput jout;

			switch (args [0]) {
			case ARG_DRIVE_FIXED:
			case ARG_DRIVE_REMOVABLE:
				DriveType dtype = DriveType.Fixed;
				if (args [0] == ARG_DRIVE_REMOVABLE)
					dtype = DriveType.Removable;

				jout = new JsonOutput ();

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

				Console.Write (jout.ToString ());
				break;
			case ARG_NETWORK:
				jout = new JsonOutput ();

				NetworkInterface[] netifs = NetworkInterface.GetAllNetworkInterfaces ();
				foreach (NetworkInterface n in netifs) {
					if (n.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
						n.NetworkInterfaceType != NetworkInterfaceType.Tunnel) {
						Dictionary<string, string> item =
							new Dictionary<string, string> (3);

						item.Add ("IFDESC", n.Description);
						item.Add ("IFNAME", n.Name);
						item.Add ("IFADDR", n.GetPhysicalAddress ().ToString ());
						jout.Add (item);
					}
				}

				Console.Write (jout.ToString ());
				break;
			default:
				Console.WriteLine ("Invalid argument");
				break;
			}
		}
	}
}
