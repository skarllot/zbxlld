//
//  _SCManager.cs
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
using System.Runtime.InteropServices;

namespace zbxlld.Windows.Supplement
{
	public class SCManager
	{
		private static IntPtr scmHandle = IntPtr.Zero;

		private QueryServiceConfigStruct qscs;

		public SCManager(string serviceName)
		{
			if (scmHandle == IntPtr.Zero) {
				scmHandle = OpenSCManager(null, null, ACCESS_MASK.GENERIC_READ);

				if (scmHandle == IntPtr.Zero)
					throw new ExternalException("Open Service Manager Error");
			}

			IntPtr svcHandle = OpenService(scmHandle, serviceName,
			                               SERVICE_ACCESS.SERVICE_QUERY_CONFIG |
			                               SERVICE_ACCESS.SERVICE_QUERY_STATUS);
			if (svcHandle == IntPtr.Zero)
				throw new ExternalException("Open Service Error");


			uint bufSize = 0;
			QueryServiceConfig(svcHandle, IntPtr.Zero, 0, out bufSize);
			IntPtr qscsHandle = Marshal.AllocHGlobal((int)bufSize);
			QueryServiceConfig(svcHandle, qscsHandle, bufSize, out bufSize);
			qscs = (QueryServiceConfigStruct)Marshal.PtrToStructure(
				qscsHandle, typeof(QueryServiceConfigStruct));

			Marshal.FreeHGlobal(qscsHandle);
			CloseServiceHandle(svcHandle);
		}

		public ServiceStartType StartType {
			get {
				switch (qscs.startType) {
					case START_TYPE.SERVICE_AUTO_START:
						return ServiceStartType.Auto;
					case START_TYPE.SERVICE_DEMAND_START:
						return ServiceStartType.Demand;
					case START_TYPE.SERVICE_DISABLED:
						return ServiceStartType.Disabled;
					default:
						return ServiceStartType.Auto |
							ServiceStartType.Demand |
							ServiceStartType.Disabled;
				}
			}
		}

		[DllImport("advapi32.dll", EntryPoint="OpenSCManagerW", ExactSpelling=true, CharSet=CharSet.Unicode, SetLastError=true)]
		private static extern IntPtr OpenSCManager(string machineName, string databaseName, ACCESS_MASK dwAccess);
		
		[DllImport("advapi32.dll", SetLastError=true, CharSet=CharSet.Auto)]
		private static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, SERVICE_ACCESS dwDesiredAccess);
		
		[DllImport("advapi32.dll", CharSet=CharSet.Unicode, SetLastError=true)]
		private static extern bool QueryServiceConfig(IntPtr hService, IntPtr intPtrQueryConfig,
		                                                 uint cbBufSize, out uint pcbBytesNeeded);

		[DllImport("advapi32.dll", SetLastError=true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseServiceHandle( IntPtr hSCObject );

		[Flags]
		private enum ACCESS_MASK : uint
		{
			SC_MANAGER_ALL_ACCESS = 0xF003F,
			SC_MANAGER_CREATE_SERVICE = 0x0002,
			SC_MANAGER_CONNECT = 0x0001,
			SC_MANAGER_ENUMERATE_SERVICE = 0x0004,
			SC_MANAGER_LOCK = 0x0008,
			SC_MANAGER_MODIFY_BOOT_CONFIG = 0x0020,
			SC_MANAGER_QUERY_LOCK_STATUS = 0x0010,
			DELETE = 0x00010000,
			READ_CONTROL = 0x00020000,
			WRITE_DAC = 0x00040000,
			WRITE_OWNER = 0x00080000,
			SYNCHRONIZE = 0x00100000,
			STANDARD_RIGHTS_REQUIRED = 0x000F0000,
			STANDARD_RIGHTS_READ = READ_CONTROL,
			STANDARD_RIGHTS_WRITE = READ_CONTROL,
			STANDARD_RIGHTS_EXECUTE = READ_CONTROL,
			STANDARD_RIGHTS_ALL = 0x001F0000,
			SPECIFIC_RIGHTS_ALL = 0x0000FFFF,
			GENERIC_READ = STANDARD_RIGHTS_READ | SC_MANAGER_ENUMERATE_SERVICE | SC_MANAGER_QUERY_LOCK_STATUS,
			GENERIC_WRITE = STANDARD_RIGHTS_WRITE | SC_MANAGER_CREATE_SERVICE | SC_MANAGER_MODIFY_BOOT_CONFIG,
			GENERIC_EXECUTE = STANDARD_RIGHTS_EXECUTE | SC_MANAGER_CONNECT | SC_MANAGER_LOCK,
			GENERIC_ALL = SC_MANAGER_ALL_ACCESS
		}
		
		[Flags]
		private enum SERVICE_ACCESS : uint
		{
			STANDARD_RIGHTS_REQUIRED       = 0xF0000,
			SERVICE_QUERY_CONFIG       = 0x00001,
			SERVICE_CHANGE_CONFIG      = 0x00002,
			SERVICE_QUERY_STATUS       = 0x00004,
			SERVICE_ENUMERATE_DEPENDENTS   = 0x00008,
			SERVICE_START          = 0x00010,
			SERVICE_STOP           = 0x00020,
			SERVICE_PAUSE_CONTINUE     = 0x00040,
			SERVICE_INTERROGATE        = 0x00080,
			SERVICE_USER_DEFINED_CONTROL   = 0x00100,
			SERVICE_ALL_ACCESS         = (STANDARD_RIGHTS_REQUIRED     |
			                              SERVICE_QUERY_CONFIG     |
			                              SERVICE_CHANGE_CONFIG    |
			                              SERVICE_QUERY_STATUS     |
			                              SERVICE_ENUMERATE_DEPENDENTS |
			                              SERVICE_START        |
			                              SERVICE_STOP         |
			                              SERVICE_PAUSE_CONTINUE       |
			                              SERVICE_INTERROGATE      |
			                              SERVICE_USER_DEFINED_CONTROL)
		}
		
		[StructLayout(LayoutKind.Sequential)]
		private struct QueryServiceConfigStruct
		{
			public int serviceType;
			public START_TYPE startType;
			public int errorControl;
			public IntPtr binaryPathName;
			public IntPtr loadOrderGroup;
			public int tagID;
			public IntPtr dependencies;
			public IntPtr startName;
			public IntPtr displayName;
		}
		
		[Flags]
		private enum START_TYPE : int
		{
			// A service started automatically by the service control manager during system startup.
			SERVICE_AUTO_START = 0x00000002,
			
			// A device driver started by the system loader. This value is valid only for driver services.
			SERVICE_BOOT_START = 0x00000000,
			
			// A service started by the service control manager when a process calls the StartService function.
			SERVICE_DEMAND_START = 0x00000003,
			
			// A service that cannot be started. Attempts to start the service result in the error code ERROR_SERVICE_DISABLED.
			SERVICE_DISABLED = 0x00000004,
			
			// A device driver started by the IoInitSystem function. This value is valid only for driver services.
			SERVICE_SYSTEM_START = 0x00000001
		}
	}
}

