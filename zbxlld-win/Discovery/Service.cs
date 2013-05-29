//
//  Service.cs
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
using System.ServiceProcess;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace zbxlld.Windows.Discovery
{
	public class Service : IArgHandler
	{
		private const string ARG_SERVICE = "service.discovery";
		private const string ARG_SERVICE_ANY = "service.discovery.any";
		private const string ARG_SERVICE_AUTO = "service.discovery.auto";
		private const string ARG_SERVICE_DEMAND = "service.discovery.demand";
		private const string ARG_SERVICE_DISABLED = "service.discovery.disabled";

		static Service def = new Service();

		public static Service Default {
			get {
				return def;
			}
		}

		#region IArgHandler implementation

		public Supplement.JsonOutput GetOutput(string arg, string suffix)
		{
			Supplement.ServiceStartType filter;
			switch (arg) {
				case ARG_SERVICE:
				case ARG_SERVICE_ANY:
					filter = Supplement.ServiceStartType.Auto |
						Supplement.ServiceStartType.Demand |
						Supplement.ServiceStartType.Disabled;
					break;
				case ARG_SERVICE_AUTO:
					filter = Supplement.ServiceStartType.Auto;
					break;
				case ARG_SERVICE_DEMAND:
					filter = Supplement.ServiceStartType.Demand;
					break;
				case ARG_SERVICE_DISABLED:
					filter = Supplement.ServiceStartType.Disabled;
					break;
				default:
					return null;
			}

			Supplement.JsonOutput jout = new Supplement.JsonOutput(suffix);
			
			foreach (ServiceController sc in ServiceController.GetServices()) {
				Supplement.SCManager scm = new Supplement.SCManager(sc.ServiceName);
				if ((scm.StartType & filter) != scm.StartType)
					continue;

				Dictionary<string, string> item =
					new Dictionary<string, string>(4);

				item.Add("SVCNAME", sc.ServiceName);
				item.Add("SVCDESC", sc.DisplayName);
				item.Add("SVCSTATUS", sc.Status.ToString());
				item.Add("SVCSTARTTYPE", scm.StartType.ToString());
				jout.Add(item);
			}

			return jout;
		}

		#endregion

		#region IArgHandler implementation

		string[] IArgHandler.GetAllowedArgs()
		{
			return new string[] { ARG_SERVICE, ARG_SERVICE_ANY, ARG_SERVICE_AUTO,
				ARG_SERVICE_DEMAND, ARG_SERVICE_DISABLED };
		}

		#endregion



	}
}

