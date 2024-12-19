using System.Management;
using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.DataCollectors;

/// <summary>
/// A data collector that retrieves the domain name of the computer system and retruns it as an <see cref="DomainNameInformation"/>.
/// </summary>
public class DomainNameCollector : DataCollector {
    /// <summary>
    /// Collects domain name form the system.
    /// </summary>
    /// <returns>An <see cref="DomainNameInformation"/> object containing the system's domain name.</returns>
    [SupportedOSPlatform("windows")]
    public Information Collect() {
        DomainNameInformation domainNameInformation = new DomainNameInformation();

        try {
            var searcher = new ManagementObjectSearcher("select * from Win32_ComputerSystem");

            var query = searcher.Get().OfType<ManagementObject>().FirstOrDefault();

            domainNameInformation.DomaineName.Value = query?["Domain"]?.ToString();
        }
        catch (Exception ex) {
            Console.WriteLine("Error: " + ex.Message);
        }

        return domainNameInformation;
    }
}