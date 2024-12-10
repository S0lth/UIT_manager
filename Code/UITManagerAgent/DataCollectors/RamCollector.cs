using System.Management;
using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.DataCollectors;

/// <summary>
/// Collects ram information from the system and returns it as a <see cref="RamInformation"/> instance.
/// </summary>
/// 
[SupportedOSPlatform("windows")]
public class RamCollector : DataCollector {
    private ManagementObjectSearcher _wmiSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");

    /// <summary>
    /// Retrieves the current RAM information from the system.
    /// </summary>
    /// <returns>
    /// An <see cref="Information"/> object with the RAM usage information, 
    /// formatted as Total (GB) and Used (GB).
    /// </returns>
    [SupportedOSPlatform("windows")]
    public Information Collect() {
        RamInformation ramInformation = new RamInformation();

        ManagementObjectCollection moc = _wmiSearcher.Get();
        ManagementObject? memObj = moc.Cast<ManagementObject>().FirstOrDefault();

        if (memObj != null) {
            ramInformation.TotalRam = Convert.ToUInt64(memObj["TotalVisibleMemorySize"]);
            ramInformation.FreeRam = Convert.ToUInt64(memObj["FreePhysicalMemory"]);
            ramInformation.UsedRam = ramInformation.TotalRam - ramInformation.FreeRam;
        }

        return ramInformation;
    }
}