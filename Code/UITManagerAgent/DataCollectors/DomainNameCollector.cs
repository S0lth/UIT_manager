using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.DataCollectors;

public class DomainNameCollector : DataCollector
{
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

