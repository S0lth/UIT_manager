using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.DataCollectors;

/// <summary>
/// Collects disk information from the system and returns it as a <see cref="DiskInformation"/> instance.
/// </summary>
public class DiskCollector : DataCollector {
    /// <summary>
    /// Collects information about all available and ready disk drives on the system.
    /// </summary>
    /// <returns>
    /// An instance of the <see cref="Information"/> class containing details about each disk,
    /// including disk names, total sizes, and available free space.
    /// </returns>
    [SupportedOSPlatform("windows")]
    public Information Collect() {
        DiskInformation.Disk disk = new DiskInformation.Disk();
        DiskInformation diskInformation = new DiskInformation();

        int diskCount = 0;
        try {
            foreach (DriveInfo drive in DriveInfo.GetDrives()) {
                if (drive.IsReady) {
                    diskCount++;
                    disk.DisksName = drive.Name;
                    disk.DiskTotalSize = (drive.TotalSize / (1024 * 1024 * 1024));
                    disk.DiskFreeSize = (drive.TotalFreeSpace / (1024 * 1024 * 1024));
                    diskInformation.Disks.Add(disk);
                }
            }

            diskInformation.NumberDisk = diskCount;
        }
        catch (Exception ex) {
            Console.WriteLine("Error while retrieving users: " + ex.Message);
        }

        diskInformation.NumberDisk = diskCount;

        return diskInformation;
    }
}