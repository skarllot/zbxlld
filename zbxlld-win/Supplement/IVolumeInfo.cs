//
//  IVolumeInfo.cs
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

namespace zbxlld.Windows.Supplement
{
	public interface IVolumeInfo
	{
		bool Automount { get; }
		string Caption { get; }
		string? DriveLetter { get; }
		DriveType DriveType { get; }
		bool IsMounted { get; }
		string? Label { get; }
		string? Name { get; }
		bool? PageFilePresent { get; }
		string? VolumeFormat { get; }
		Guid? VolumeGuid { get; }
	}
}

