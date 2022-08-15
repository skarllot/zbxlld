using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace zbxlld.Windows.Discovery
{
	public class Drive : IArgHandler
	{
		private const string ArgDrive = "drive.discovery";
		private const string ArgDriveFixed = ArgDrive + ".fixed";
		private const string ArgDriveRemovable = ArgDrive + ".removable";
		private const string ArgDriveMounted = ArgDrive + ".mounted";
		private const string ArgDriveMfolder = ArgDrive + ".mountfolder";
		private const string ArgDriveMletter = ArgDrive + ".mountletter";
		private const string ArgDriveNomount = ArgDrive + ".nomount";
		private const string ArgDriveSwap = ArgDrive + ".swap";
		private const string ArgDriveNoswap = ArgDrive + ".noswap";
		private const string ArgDriveNetwork = ArgDrive + ".network";
		private const string ClassFullPath = "zbxlld.Windows.Discovery.Drive";

		private static Drive def = new Drive();

		public static Drive Default {
			get {
				return def;
			}
		}

		private Drive()
		{
		}

        private bool TryGetVolumesViaWmi([NotNullWhen(true)] out IReadOnlyList<Supplement.IVolumeInfo>? vols)
        {
            vols = null;

            try
            {
                vols = Supplement.Win32Volume.GetAllVolumes();
                return true;
            }
            catch (OutOfMemoryException e)
            {
                if (MainClass.DEBUG)
                {
                    MainClass.WriteLogEntry(string.Format("{0}.GetOutput: Out of memory exception.", ClassFullPath));
                    MainClass.WriteLogEntry("Exception:");
                    MainClass.WriteLogEntry(e.ToString());
                }
                // TODO: Make a proper exit
                Console.WriteLine("You have insufficient permissions or insufficient memory.");
                Environment.Exit((int)ErrorId.GetAllVolumesOutOfMemory);
                return false;
            }
            catch (Exception e)
            {
                if (MainClass.DEBUG)
                {
                    MainClass.WriteLogEntry(string.Format(
                        "{0}.GetOutput: Unexpected exception.", ClassFullPath));
                    MainClass.WriteLogEntry("Exception:");
                    MainClass.WriteLogEntry(e.ToString());
                }
                return false;
            }
        }

        public Supplement.JsonOutput GetOutput(string key)
		{
			bool mounted = false;
			bool mfolder = false;
			bool mletter = false;
			bool nomount = false;
			bool swap = false;
			bool noswap = false;
            bool onlyNative = false;

			DriveType dtype;
			switch (key) {
				case ArgDriveFixed:
					dtype = DriveType.Fixed;
					break;
				case ArgDriveRemovable:
					dtype = DriveType.Removable;
					break;
				case ArgDriveMounted:
					dtype = DriveType.Fixed;
					mounted = true;
					break;
				case ArgDriveMfolder:
					dtype = DriveType.Fixed;
					mfolder = true;
					break;
				case ArgDriveMletter:
					dtype = DriveType.Fixed;
					mletter = true;
					break;
				case ArgDriveNomount:
					dtype = DriveType.Fixed;
					nomount = true;
					break;
				case ArgDriveSwap:
					dtype = DriveType.Fixed;
					swap = true;
					break;
				case ArgDriveNoswap:
					dtype = DriveType.Fixed;
					noswap = true;
					break;
                case ArgDriveNetwork:
                    dtype = DriveType.Network;
                    onlyNative = true;
                    break;
				default:
					return new Supplement.JsonOutput();
			}
			
			Supplement.JsonOutput jout = new Supplement.JsonOutput ();

			IReadOnlyList<Supplement.IVolumeInfo>? vols;

            if (onlyNative)
            {
                vols = Supplement.NativeVolume.GetVolumes();
            }
            else
            {
                if (!TryGetVolumesViaWmi(out vols))
                {
                    if (MainClass.DEBUG)
                        MainClass.WriteLogEntry(string.Format("{0}.GetOutput: Fallback to native method.", ClassFullPath));

                    // Fallback to native method
                    vols = Supplement.NativeVolume.GetVolumes();
                }
            }

            if (MainClass.DEBUG)
            {
                MainClass.WriteLogEntry(string.Format("{0}.GetOutput: got volumes. " +
                    "vols.Length: {1}", ClassFullPath, vols.Count));
                for (int i = 0; i < vols.Count; i++)
                {
                    MainClass.WriteLogEntry(string.Format("{0}.GetOutput: vol[{1}] {{ " +
                        "Automount={2}, Caption={3}, DriveLetter={4}, DriveType={5}, " +
                        "IsMounted={6}, Label={7}, Name={8}, PageFilePresent={9}, " +
                        "VolumeFormat={10}, VolumeGuid={11} }}.", ClassFullPath, i,
                        vols[i].Automount.ToString(), vols[i].Caption, vols[i].DriveLetter,
                        vols[i].DriveType.ToString(), vols[i].IsMounted.ToString(),
                        vols[i].Label, vols[i].Name, vols[i].PageFilePresent.ToString(),
                        vols[i].VolumeFormat, vols[i].VolumeGuid.ToString()));
                }
            }

			bool isMounted, ismletter;
			foreach (var v in vols) {
				isMounted = v.IsMounted == true;
				ismletter = v.DriveLetter != null;

				if (v.Automount == true &&                      // Match to drives mounted automatically
				    v.DriveType == dtype &&                     // Match drive type
				    (!mounted || isMounted) &&                  // If defined to mounted drives, then match mounted drives
				    (!mfolder || (isMounted && !ismletter)) &&  // If defined to folder mounted drives, then match mounted and not has letter
				    (!mletter || (isMounted && ismletter)) &&   // If defined to letter mounted drives, then match mounted and has letter
				    (!nomount || !isMounted) &&                 // If defined to not mounted drives, then match not mounted drives
				    (!swap || v.PageFilePresent == true) &&             // If defined to drives that has page files, then match to drives that has page file
				    (!noswap || v.PageFilePresent != true)) {          // If defined to drives that no page files, then match to drives that has no page file
					var item = new Dictionary<string, string>(5);

					string? pmInstName = v switch
					{
						{ IsMounted: true, Name: not null } => v.Name?.TrimEnd('\\'),
						{ VolumeGuid: not null } => Supplement.PerfMon.LogicalDisk.GetInstanceName(v.VolumeGuid.Value),
						_ => null
					};

					item.Add ("FSNAME", v.Name ?? string.Empty);
					item.Add ("FSPERFMON", pmInstName ?? string.Empty);
					item.Add ("FSLABEL", v.Label ?? string.Empty);
					item.Add ("FSFORMAT", v.VolumeFormat ?? string.Empty);
					item.Add ("FSCAPTION", v.Caption);
					jout.Add (item);
				}
			}

			return jout;
		}

		string[] IArgHandler.GetAllowedArgs()
		{
			return new[] {
				ArgDriveFixed,
				ArgDriveRemovable,
				ArgDriveMounted,
				ArgDriveMfolder,
				ArgDriveMletter,
				ArgDriveNomount,
				ArgDriveSwap,
				ArgDriveNoswap,
                ArgDriveNetwork
			};
		}
	}
}

