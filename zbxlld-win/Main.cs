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

namespace zbxlld.Windows
{
	class MainClass
	{
		static IArgHandler[] ARG_HANDLERS = new IArgHandler[] {
			Discovery.Drive.Default, Discovery.Network.Default, Discovery.Service.Default };

		public static void Main(string[] args)
		{
			if (args.Length < 1) {
				Console.WriteLine("At least one parameter should be provided.");
				return;
			}

			Dictionary<string, IArgHandler> hList =
				new Dictionary<string, IArgHandler>();
			foreach (IArgHandler i in ARG_HANDLERS) {
				foreach (string j in i.GetAllowedArgs()) {
					hList.Add(j, i);
				}
			}

			IArgHandler val;
			if (!hList.TryGetValue(args [0], out val)) {
				Console.WriteLine("Invalid argument");
				return;
			}

			Supplement.JsonOutput jout = val.GetOutput(args[0]);
			if (jout == null) {
				Console.WriteLine("Invalid argument");
				return;
			}

			Console.Write(jout.ToString());
		}
	}
}
