using System.Management;
using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.DataCollectors;

/// <summary>
/// Collects OS information.
/// </summary>
public class OsCollector : DataCollector {
    [SupportedOSPlatform("windows")]
    public Information Collect() {
        OsInformation osInformation = new();

        try {
            ManagementObject? queryObj =
                osInformation.WmiSearcher.Get().OfType<ManagementObject>().FirstOrDefault();

            if (queryObj != null) {
                string? osName = queryObj["Caption"].ToString();
                string? osVersion = queryObj["Version"].ToString();

                if (osName != null && osVersion != null) {
                    osInformation.OsName = osName;
                    osInformation.OsVersion = osVersion;
                }
                else {
                    osInformation.OsName = "Unknown OS";
                    osInformation.OsVersion = "Unknown Version";
                }
            }
        }
        catch (Exception ex) {
            osInformation.OsName = "Unknown OS";
            osInformation.OsVersion = "Unknown Version";
            Console.WriteLine(ex.Message);
        }

        return osInformation;
    }
}