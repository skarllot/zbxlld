using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using zbxlld.Windows.Supplement.PerfMon;

namespace zbxlld.Windows.DriveDiscovery
{
	public class Drive
	{
		private readonly ILogger<Drive> _logger;
		private readonly NativeVolumeFactory _nativeVolumeFactory;
		private readonly LogicalDisk _logicalDisk;

		public Drive(ILogger<Drive> logger, NativeVolumeFactory nativeVolumeFactory, LogicalDisk logicalDisk)
		{
			_logger = logger;
			_nativeVolumeFactory = nativeVolumeFactory;
			_logicalDisk = logicalDisk;
		}

        private bool TryGetVolumesViaWmi([NotNullWhen(true)] out IReadOnlyList<IVolumeInfo>? vols)
        {
            vols = null;

            try
            {
                vols = Win32Volume.GetAllVolumes();
                return true;
            }
            catch (OutOfMemoryException e)
            {
	            _logger.LogCritical(e, "Not enough memory to get all volumes information");

                // TODO: Make a proper exit
                Console.WriteLine("You have insufficient permissions or insufficient memory.");
                Environment.Exit(ErrorId.GetAllVolumesOutOfMemory);
                return false;
            }
            catch (Exception e)
            {
	            _logger.LogError(e, "Unexpected error trying to get all volumes information");
                return false;
            }
        }

        public void GetFixed(Utf8JsonWriter writer, string? keySuffix)
        {
	        Get(writer, DriveType.Fixed, keySuffix);
        }

        public void GetRemovable(Utf8JsonWriter writer, string? keySuffix)
        {
	        Get(writer, DriveType.Removable, keySuffix);
        }

        public void GetMounted(Utf8JsonWriter writer, string? keySuffix)
        {
	        Get(writer, DriveType.Fixed, keySuffix, mounted: true);
        }

        public void GetMountedFolder(Utf8JsonWriter writer, string? keySuffix)
        {
	        Get(writer, DriveType.Fixed, keySuffix, mfolder: true);
        }

        public void GetMountedLetter(Utf8JsonWriter writer, string? keySuffix)
        {
	        Get(writer, DriveType.Fixed, keySuffix, mletter: true);
        }

        public void GetNotMounted(Utf8JsonWriter writer, string? keySuffix)
        {
	        Get(writer, DriveType.Fixed, keySuffix, nomount: true);
        }

        public void GetSwap(Utf8JsonWriter writer, string? keySuffix)
        {
	        Get(writer, DriveType.Fixed, keySuffix, swap: true);
        }

        public void GetNoSwap(Utf8JsonWriter writer, string? keySuffix)
        {
	        Get(writer, DriveType.Fixed, keySuffix, noswap: true);
        }

        public void GetNetwork(Utf8JsonWriter writer, string? keySuffix)
        {
	        Get(writer, DriveType.Network, keySuffix, onlyNative: true);
        }

        private void Get(
	        Utf8JsonWriter writer,
	        DriveType dtype,
	        string? keySuffix,
	        bool mounted = false,
	        bool mfolder = false,
	        bool mletter = false,
	        bool nomount = false,
	        bool swap = false,
	        bool noswap = false,
	        bool onlyNative = false)
        {
			IReadOnlyList<IVolumeInfo>? vols;

            if (onlyNative)
            {
                vols = _nativeVolumeFactory.GetVolumes();
            }
            else
            {
                if (!TryGetVolumesViaWmi(out vols))
                {
	                _logger.LogInformation("Failed to use WMI falling back to framework implementation");

                    // Fallback to native method
                    vols = _nativeVolumeFactory.GetVolumes();
                }
            }

            writer.WriteStartArray();

            bool isMounted, ismletter;
			foreach (var v in vols) {
				isMounted = v.IsMounted;
				ismletter = v.DriveLetter != null;

				if (v.Automount &&                      // Match to drives mounted automatically
				    v.DriveType == dtype &&                     // Match drive type
				    (!mounted || isMounted) &&                  // If defined to mounted drives, then match mounted drives
				    (!mfolder || (isMounted && !ismletter)) &&  // If defined to folder mounted drives, then match mounted and not has letter
				    (!mletter || (isMounted && ismletter)) &&   // If defined to letter mounted drives, then match mounted and has letter
				    (!nomount || !isMounted) &&                 // If defined to not mounted drives, then match not mounted drives
				    (!swap || v.PageFilePresent == true) &&             // If defined to drives that has page files, then match to drives that has page file
				    (!noswap || v.PageFilePresent != true)) {          // If defined to drives that no page files, then match to drives that has no page file

					string? pmInstName = v switch
					{
						{ IsMounted: true, Name: not null } => v.Name?.TrimEnd('\\'),
						{ VolumeGuid: not null } => _logicalDisk.GetInstanceName(v.VolumeGuid.Value),
						_ => null
					};

					writer.WriteStartObject();
					writer.WriteZabbixMacro("FSNAME", keySuffix, v.Name);
					writer.WriteZabbixMacro("FSPERFMON", keySuffix, pmInstName);
					writer.WriteZabbixMacro("FSLABEL", keySuffix, v.Label);
					writer.WriteZabbixMacro("FSFORMAT", keySuffix, v.VolumeFormat);
					writer.WriteZabbixMacro("FSCAPTION", keySuffix, v.Caption);
					writer.WriteEndObject();
				}
			}

			writer.WriteEndArray();
        }
	}
}

