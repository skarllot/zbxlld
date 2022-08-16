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
using zbxlld.Windows.DriveDiscovery;

namespace zbxlld.Windows.Supplement.PerfMon
{
	public class LogicalDisk
	{
		private const string CounterLogicalDisk = "236";
		private const string CounterFreeMb = "410";
		private const long MbMult = 1048576;

		// Relates Performance Monitor instance name to volume GUID.
		private readonly Dictionary<Guid, string> _perfMonGuid;

		public LogicalDisk(Localization localization)
		{
			_perfMonGuid = CreateVolumeDictionary(localization);
		}

		// Try to discover GUID from buggy Performance Monitor instance names.
		// Note: Discover drive GUID comparing free space is ugly, but MS gave me no choice.
		private static Dictionary<Guid, string> CreateVolumeDictionary(Localization localization)
		{
			// =====         WMI         =====
			var vols = Win32Volume.GetAllVolumes();
			// Free megabytes and volume GUID relation
			var wmiFree = new Dictionary<ulong, Guid>(vols.Length);
			// Volume name and volume GUID relation
			var wmiName = new Dictionary<string, Guid>(vols.Length);

			foreach (Win32Volume v in vols) {
				if (v.Automount &&
				    v.DriveType == System.IO.DriveType.Fixed) {
					if (v.IsMounted) {
						wmiName.Add(v.Name.TrimEnd('\\'), v.DeviceGuid);
					} else {
						wmiFree.Add(v.FreeSpace / MbMult, v.DeviceGuid);
					}
				}
			}

			var result = new Dictionary<Guid, string>(wmiFree.Count + wmiName.Count);

			// ===== PERFORMANCE MONITOR ======
			var perfCat = new PerformanceCounterCategory(localization.GetName(CounterLogicalDisk));
			// TODO: Find a faster way to get instance names.
			string[] instances = perfCat.GetInstanceNames();
			// Free megabytes and Performance Monitor instance name
			var perfFree = new Dictionary<ulong, string>(instances.Length);

			foreach (string item in instances) {
				if (item == "_Total")
					continue;

				if (wmiName.TryGetValue(item, out var volId)) {
					result.Add(volId, item);
				} else {
					var p = new PerformanceCounter(
						localization.GetName(CounterLogicalDisk),
						localization.GetName(CounterFreeMb),
						item);
					perfFree.Add((ulong)p.RawValue, item);
					p.Close();
					p.Dispose();
				}
			}

			foreach (var pf in perfFree)
			{
				if (wmiFree.TryGetValue(pf.Key, out var guid))
					result.Add(guid, pf.Value);
			}

			return result;
		}

		public string? GetInstanceName(Guid guid)
		{
			_perfMonGuid.TryGetValue(guid, out string? ret);
			return ret;
		}
	}
}

