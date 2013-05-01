//
//  Localization.cs
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
using Microsoft.Win32;
using System.Collections.Generic;

namespace zbxlld.Windows.Supplement.PerfMon
{
	public class Localization
	{
		const string KEY_PERFLIB_CURLANG = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Perflib\CurrentLanguage";
		const string KEY_PERFLIB_DEFAULT = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Perflib\009";
		static Dictionary<string, string> counterList;

		static Localization()
		{
			RegistryKey regKey;

			// TODO: Heavy test both keys
			/*try {
				//regKey = Registry.LocalMachine.OpenSubKey(KEY_PERFLIB_CURLANG);
			} catch (Exception) {
				regKey = Registry.LocalMachine.OpenSubKey(KEY_PERFLIB_DEFAULT);
			}*/
			regKey = Registry.LocalMachine.OpenSubKey(KEY_PERFLIB_DEFAULT);

			string[] strCounter = regKey.GetValue("Counter") as string[];
			regKey.Close();

			counterList = new Dictionary<string, string>(strCounter.Length / 2);
			for (int i = 0; i < strCounter.Length; i += 2) {
				if (string.IsNullOrEmpty(strCounter[i]) ||
				    string.IsNullOrEmpty(strCounter[i+1])) {
				    continue;
				}
				counterList.Add(strCounter[i], strCounter[i+1]);
			}
		}

		public static string GetName(string id) {
			return counterList[id];
		}
	}
}

