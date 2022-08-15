using System;
using System.Runtime.InteropServices;

namespace zbxlld.Windows.Supplement
{
	public class ServiceInfo
	{
		private ServiceConfigInfo qscs;

		public ServiceInfo(nint managerHandle, string serviceName)
		{
			var svcHandle = OpenService(managerHandle, serviceName,
				SERVICE_ACCESS.SERVICE_QUERY_CONFIG |
				SERVICE_ACCESS.SERVICE_QUERY_STATUS);
			if (svcHandle == 0)
				throw new ExternalException("Open Service Error");

			QueryServiceConfig(svcHandle, IntPtr.Zero, 0, out uint bufSize);
			var qscsHandle = Marshal.AllocHGlobal((int)bufSize);
			QueryServiceConfig(svcHandle, qscsHandle, bufSize, out bufSize);
			qscs = Marshal.PtrToStructure<ServiceConfigInfo>(qscsHandle);

			Marshal.FreeHGlobal(qscsHandle);
			CloseServiceHandle(svcHandle);
		}

		public ServiceStartType StartType {
			get
			{
				return qscs.startType switch
				{
					START_TYPE.SERVICE_AUTO_START => ServiceStartType.Auto,
					START_TYPE.SERVICE_DEMAND_START => ServiceStartType.Demand,
					START_TYPE.SERVICE_DISABLED => ServiceStartType.Disabled,
					_ => ServiceStartType.Auto | ServiceStartType.Demand | ServiceStartType.Disabled
				};
			}
		}

		[DllImport("advapi32.dll", SetLastError=true, CharSet=CharSet.Auto)]
		private static extern nint OpenService(nint hSCManager, string lpServiceName, SERVICE_ACCESS dwDesiredAccess);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool QueryServiceConfig(
			nint hService,
			nint intPtrQueryConfig,
			uint cbBufSize,
			out uint pcbBytesNeeded);

		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseServiceHandle(IntPtr hSCObject);

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
		
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct ServiceConfigInfo
		{
			public int serviceType;
			public START_TYPE startType;
			public int errorControl;
			public string binaryPathName;
			public string loadOrderGroup;
			public int tagID;
			public IntPtr dependencies;
			public string startName;
			public string displayName;
		}
		
		[Flags]
		private enum START_TYPE
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