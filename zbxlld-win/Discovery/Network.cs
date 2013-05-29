//
//  Network.cs
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
using System.Net.NetworkInformation;
using System.Collections.Generic;

namespace zbxlld.Windows.Discovery
{
	public class Network : IArgHandler
	{
		private const string ARG_NETWORK = "network.discovery";

		static Network def = new Network();

		public static Network Default {
			get {
				return def;
			}
		}

		#region IArgHandler implementation

		public Supplement.JsonOutput GetOutput(string arg)
		{
			if (arg != ARG_NETWORK)
				return null;

			Supplement.JsonOutput jout = new Supplement.JsonOutput ();
			
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

			return jout;
		}

		#endregion

		#region IArgHandler implementation

		string[] IArgHandler.GetAllowedArgs()
		{
			return new string[] { ARG_NETWORK };
		}

		#endregion
	}
}

