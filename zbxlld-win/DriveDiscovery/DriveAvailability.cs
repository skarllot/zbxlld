namespace zbxlld.Windows.DriveDiscovery
{
	public enum DriveAvailability : ushort
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
