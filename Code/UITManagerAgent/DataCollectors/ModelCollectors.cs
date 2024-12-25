using UITManagerAgent.BasicInformation;
using System.Management;
using System.Runtime.Versioning;

namespace UITManagerAgent.DataCollectors;

public class ModelCollectors : DataCollector {
    [SupportedOSPlatform("windows")]
    private ManagementObjectSearcher _searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
    
    /// <summary>
    /// Retrieves the model of the machine
    /// </summary>
    /// <returns>
    /// An <see cref="Information"/> object with the model of the machine
    /// </returns>
    [SupportedOSPlatform("windows")]
    public Information Collect() {
        ModelInformation modelInformation = new ModelInformation();
        ManagementObjectCollection collection = _searcher.Get();
        ManagementObject? managementObject = collection.OfType<ManagementObject>().FirstOrDefault();

        if (managementObject != null) {
            modelInformation.Model.Value = managementObject["Model"]?.ToString() ?? "Unknown Model";
        }

        return modelInformation;
    }
}
