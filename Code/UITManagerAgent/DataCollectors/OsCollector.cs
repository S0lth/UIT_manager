using Microsoft.Win32;
using System.Management;
using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.DataCollectors;

/// <summary>
/// Collects OS information.
/// </summary>
public class OsCollector : DataCollector {
    [SupportedOSPlatform("windows")]
    private ManagementObjectSearcher _searcher = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
    
    /// <summary>
    /// Retrieves the current os information from the system.
    /// </summary>
    /// <returns>
    /// An <see cref="Information"/> object with the os name, version and build
    /// </returns>
    [SupportedOSPlatform("windows")]
    public Information Collect() {
        OsInformation osInformation = new();

        try {
            ManagementObject? queryObj =
                _searcher.Get().OfType<ManagementObject>().FirstOrDefault();

            if (queryObj != null) {
                string? osName = queryObj["Caption"].ToString();
                string? osVersion = queryObj["Version"].ToString();
                string? osBuild = "";
                using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion"))
                {
                    if (key != null)
                    {
                        osBuild = key.GetValue("CurrentBuild")?.ToString() ?? "Inconnu";
                    }
                }

                if (osName != null && osVersion != null && osBuild != null) {
                    osInformation.InformationAgents[0].Value = osName;
                    osInformation.InformationAgents[1].Value = osVersion;
                    osInformation.InformationAgents[2].Value = osBuild;
                }
                else {
                    osInformation.InformationAgents[0].Value = "Unknown OS";
                    osInformation.InformationAgents[1].Value = "Unknown Version";
                    osInformation.InformationAgents[2].Value = "Unknown Build Number";
                }
            }
        }
        catch (Exception ex) {
            osInformation.InformationAgents[0].Value = "Unknown OS";
            osInformation.InformationAgents[1].Value = "Unknown Version";
            osInformation.InformationAgents[2].Value = "Unknown Build Number";
            Console.WriteLine(ex.Message);
        }

        return osInformation;
    }
    
}