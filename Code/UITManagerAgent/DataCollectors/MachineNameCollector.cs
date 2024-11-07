using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.DataCollectors;

/// <summary>
/// Class responsible for collecting the Machine Name 
/// </summary>
public class MachineNameCollector : DataCollector {
    /// <summary>
    /// Collect the machine name from the local system
    /// </summary>
    /// <returns>
    /// An <see cref="MachineNameInformation"/> object containing the collected MachineName
    /// </returns>
    [SupportedOSPlatform("windows")]
    public Information Collect() {
        MachineNameInformation machineNameInformation = new();

        string machineName = Environment.MachineName;
        machineNameInformation.MachineName = machineName;
        return machineNameInformation;
    }
}