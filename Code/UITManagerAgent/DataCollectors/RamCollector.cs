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
            
            ulong ramTot = Convert.ToUInt64(memObj["TotalVisibleMemorySize"]);
            ulong ramFree = Convert.ToUInt64(memObj["FreePhysicalMemory"]);
            ulong ramUsed = ramTot - ramFree;
            
            ramInformation.InformationAgents[0].Value = (ramTot / (float)(1024 * 1024)).ToString("F2");
            ramInformation.InformationAgents[1].Value = (ramUsed / (float)(1024 * 1024)).ToString("F2");
            ramInformation.InformationAgents[2].Value = (ramFree / (float)(1024 * 1024)).ToString("F2");
        }

        return ramInformation;
    }
}