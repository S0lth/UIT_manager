using Microsoft.Win32;
using System.Management;
using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.DataCollectors;

/// <summary>
/// Collects OS information.
/// </summary>
public class OsCollector : DataCollector {
    
    
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
                osInformation.WmiSearcher.Get().OfType<ManagementObject>().FirstOrDefault();

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
                    osInformation.OsName = osName;
                    osInformation.OsVersion = osVersion;
                    osInformation.OsBuild = osBuild;
                }
                else {
                    osInformation.OsName = "Unknown OS";
                    osInformation.OsVersion = "Unknown Version";
                    osInformation.OsBuild = "Unknown Build Number";
                }
            }
        }
        catch (Exception ex) {
            osInformation.OsName = "Unknown OS";
            osInformation.OsVersion = "Unknown Version";
            osInformation.OsBuild = "Unknown Build Number";
            Console.WriteLine(ex.Message);
        }

        return osInformation;
    }
    
}