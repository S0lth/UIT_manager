using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using UITManagerAgent.BasicInformation;
using System.Runtime.Versioning;

namespace UITManagerAgent.DataCollectors;

/// <summary>
/// A data collector that retrieves the domain name of the computer system and retruns it as an <see cref="DomainNameInformation"/>.
/// </summary>
public class DomainNameCollector : DataCollector
{
    /// <summary>
    /// Collects domain name form the system.
    /// </summary>
    /// <returns>An <see cref="DomainNameInformation"/> object containing the system's domain name.</returns>
    [SupportedOSPlatform("windows")]
    public Information Collect()
    {
        DomainNameInformation domainNameInformation = new DomainNameInformation();

        try
        {
            var searcher = new ManagementObjectSearcher("select * from Win32_ComputerSystem");

            var query = searcher.Get().OfType<ManagementObject>().FirstOrDefault();

            // If a machine doesn't have a domain name, No domain is assigned.
            domainNameInformation.DomainName = query?["Domain"]?.ToString() ?? "No domain";
        }
        catch (ManagementException ex)
        {
            Console.WriteLine("WMI error: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("General error: " + ex.Message);
        }

        return domainNameInformation;
    }
}

