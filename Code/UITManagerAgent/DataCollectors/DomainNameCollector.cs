using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using UITManagerAgent.BasicInformation;

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
    public Information Collect()
    {
        DomainNameInformation domainNameInformation = new DomainNameInformation();

        try
        {
            var searcher = new ManagementObjectSearcher("select * from Win32_ComputerSystem");

            var querry = searcher.Get().OfType<ManagementObject>().FirstOrDefault();

            domainNameInformation.SetDomainName(querry["Domain"].ToString());
        }
        catch (ManagementException ex)
        {
            Console.WriteLine(ex.Message);
        }



        return domainNameInformation;
    }
}

