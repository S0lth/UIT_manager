using System.Management;
using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.DataCollectors;

/// <summary>
/// Collects ram information from the system and returns it as a <see cref="RamInformation"/> instance.
/// </summary>
public class RamCollector : DataCollector
{
    /// <summary>
    /// Retrieves the current RAM information from the system.
    /// </summary>
    /// <returns>
    /// An <see cref="Information"/> object with the RAM usage information, 
    /// formatted as Total (GB) and Used (GB).
    /// </returns>
    public Information Collect()
    {
        RamInformation ramInformation = new RamInformation();

        ManagementObjectCollection moc = ramInformation.GetWmiSearcher().Get();
        ManagementObject memObj = moc.Cast<ManagementObject>().FirstOrDefault();

        if (memObj != null)
        {
            ramInformation.SetTotalMemory(Convert.ToUInt64(memObj["TotalVisibleMemorySize"]));
            ramInformation.SetFreeMemory(Convert.ToUInt64(memObj["FreePhysicalMemory"]));
            ramInformation.SetUsedMemory(ramInformation.GetTotalMemory() - ramInformation.GetFreeMemory());
        }

        return ramInformation;
    }
}