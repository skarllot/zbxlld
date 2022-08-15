using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace zbxlld.Windows.Supplement
{
	public sealed class SCManager : CriticalFinalizerObject, IDisposable
	{
		private readonly nint scmHandle;

		public SCManager()
		{
			scmHandle = OpenSCManager(null, null, ACCESS_MASK.GENERIC_READ);
			if (scmHandle == 0)
				throw new ExternalException("Open Service Manager Error");
		}

		~SCManager()
		{
			ReleaseUnmanagedResources();
		}

		public ServiceInfo GetServiceInfo(string serviceName)
		{
			return new ServiceInfo(scmHandle, serviceName);
		}

		public void Dispose()
		{
			ReleaseUnmanagedResources();
			GC.SuppressFinalize(this);
		}

		private void ReleaseUnmanagedResources()
		{
			CloseServiceHandle(scmHandle);
		}

		[DllImport("advapi32.dll", EntryPoint="OpenSCManagerW", ExactSpelling=true, CharSet=CharSet.Unicode, SetLastError=true)]
		private static extern nint OpenSCManager(string? machineName, string? databaseName, ACCESS_MASK dwAccess);

		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseServiceHandle(IntPtr hSCObject);

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
	}
}

