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
        DiskInformation.Disk disk = new ();
        DiskInformation diskInformation = new ();
        List<InnerValue> innerValues = new();

        int diskCount = 0;
        try {
            foreach (DriveInfo drive in DriveInfo.GetDrives()) {
                if (drive.IsReady) {
                    
                    diskCount++;
                    
                    long diskTot = drive.TotalSize / (1024 * 1024 * 1024);
                    long diskFree = drive.TotalFreeSpace / (1024 * 1024 * 1024);
                    long diskUsed = diskTot - diskFree;
                    
                    disk.DiskTot.Value = diskTot.ToString("F2");
                    disk.DiskFree.Value = diskFree.ToString("F2");
                    disk.DiskUsed.Value = diskUsed.ToString("F2");
                    innerValues.Add(new InnerValue(drive.Name + "\\","null","null",disk.GetList()));
                    
                }
            }
            diskInformation.InformationAgents.Add(new InnerValue("Disks","null","null",innerValues));

            diskInformation.InformationAgents.Add(new InnerValue("Number disks","TEXT",diskCount.ToString()));
        }
        catch (Exception ex) {
            Console.WriteLine("Error while retrieving users: " + ex.Message);
        }
        
        return diskInformation;
    }
}