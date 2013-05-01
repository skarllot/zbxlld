//
//  LogicalDisk.cs
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
using System.Diagnostics;
using System.Collections.Generic;
using zbxlld.Windows.Supplement.PerfMon;

namespace zbxlld.Windows.Supplement.PerfMon
{
	public class LogicalDisk
	{
		const string MSG_EXCEPTION = "Unmounted volumes count and Performance Monitor invalid instances name count" +
			"don't match";
		const string COUNTER_LOGICAL_DISK = "236";
		const string COUNTER_FREE_MB = "410";
		const long MB_MULT = 1048576;

		// Relates Performance Monitor instance name to volume GUID.
		static Dictionary<Guid, string> perfMonGuid;

		// Try to discover GUID from buggy Performance Monitor instance names.
		// Note: Discover drive GUID comparing free space is uggly, but MS gave me no choice.
		static LogicalDisk()
		{
			// =====         WMI         =====
			Win32_Volume[] vols = Win32_Volume.GetAllVolumes();
			// Free megabytes and volume GUID relation
			Dictionary<ulong, Guid> wmiFree = new Dictionary<ulong, Guid>(vols.Length);
			// Volume name and volume GUID relation
			Dictionary<string, Guid> wmiName = new Dictionary<string, Guid>(vols.Length);

			foreach (Win32_Volume v in vols) {
				if (v.Automount &&
				    v.DriveType == System.IO.DriveType.Fixed) {
					if (v.IsMounted) {
						wmiName.Add(v.Name.TrimEnd('\\'), v.DeviceGuid);
					} else {
						wmiFree.Add(v.FreeSpace / MB_MULT, v.DeviceGuid);
					}
				}
			}
			perfMonGuid = new Dictionary<Guid, string>(wmiFree.Count + wmiName.Count);

			// ===== PERFORMANCE MONITOR ======
			PerformanceCounterCategory perfCat = new PerformanceCounterCategory(
				Localization.GetName(COUNTER_LOGICAL_DISK));
			// TODO: Find a faster way to get instance names.
			string[] instances = perfCat.GetInstanceNames();
			// Free megabytes and Performance Monitor instance name
			Dictionary<ulong, string> perfFree = new Dictionary<ulong, string>(instances.Length);

			foreach (string item in instances) {
				if (item == "_Total")
					continue;

				Guid volId = Guid.Empty;
				if (wmiName.TryGetValue(item, out volId)) {
					perfMonGuid.Add(volId, item);
				} else {
					PerformanceCounter p = new PerformanceCounter(
						Localization.GetName(COUNTER_LOGICAL_DISK),
						Localization.GetName(COUNTER_FREE_MB),
						item);
					perfFree.Add((ulong)p.RawValue, item);
					p.Close();
					p.Dispose();
				}
			}

			ulong[] warray = new ulong[wmiFree.Count];
			ulong[] pmarray = new ulong[perfFree.Count];
			if (warray.Length != pmarray.Length)
				throw new NotSupportedException(MSG_EXCEPTION);
			wmiFree.Keys.CopyTo(warray, 0);
			perfFree.Keys.CopyTo(pmarray, 0);
			Array.Sort<ulong>(warray);
			Array.Sort<ulong>(pmarray);

			for (int i = 0; i < warray.Length; i++) {
				perfMonGuid.Add(wmiFree[warray[i]], perfFree[pmarray[i]]);
			}
		}

		public static string GetInstanceName(Guid guid)
		{
			string ret = null;
			perfMonGuid.TryGetValue(guid, out ret);
			return ret;
		}
	}
}

