//
//  Main.cs
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

namespace zbxlld.Windows
{
	class MainClass
	{
        public const string PROGRAM_NAME = "zbxlld-win";
        // Latest release: 0.6.0.20
        // Major.Minor.Maintenance.Build
        public const string PROGRAM_VERSION = "0.6.1.22";
        public const string PROGRAM_VERSION_SIMPLE = "0.6.1";
        public const string PROGRAM_TITLE = PROGRAM_NAME + " " + PROGRAM_VERSION_SIMPLE;

        public static readonly bool DEBUG = System.Diagnostics.Debugger.IsAttached;

		static IArgHandler[] ARG_HANDLERS = new IArgHandler[] {
			Discovery.Drive.Default, Discovery.Network.Default, Discovery.Service.Default };

		public static void Main(string[] args)
		{
			string key, keySuffix = null;

			if (args.Length < 1) {
				Console.WriteLine("At least one parameter should be provided.");
				return;
			} else if (args.Length > 1) {
				if (args [1] != "NULL")
					keySuffix = args [1];
			}

			key = args [0];

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

			Supplement.JsonOutput jout = val.GetOutput(key);
			if (jout == null) {
				Console.WriteLine("Invalid argument");
				return;
			}

			Console.Write(jout.GetOutput(keySuffix));
		}
	}
}
