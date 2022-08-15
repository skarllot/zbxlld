//
//  Win32_Volume.cs
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

namespace zbxlld.Windows.Supplement
{
	public enum DriveAvailability
	{
		Other = 1,
		Unknown = 2,
		Running = 3,
		Warning = 4,
		InTest = 5,
		NotApplicable = 6,
		PowerOff = 7,
		Offline = 8,
		OffDuty = 9,
		Degraded = 10,
		NotInstalled = 11,
		InstallError = 12,
		PowerSaveUnknown = 13,
		PowerSaveLowPowerMode = 14,
		PowerSaveStandby = 15,
		PowerCycle = 16,
		PowerSaveWarning = 17,
		Paused = 18,
		NotReady = 19,
		NotConfigured = 20,
		Quiesced = 21
	}

}
