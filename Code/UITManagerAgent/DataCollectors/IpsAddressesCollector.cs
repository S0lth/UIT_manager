using System.Net;
using System.Net.Sockets;
using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.DataCollectors;

public class IpsAddressesCollector : DataCollector
{
    public Information Collect()
    {
        IpsAddressesInformation ipsAddressesInformation = new();
        string hostname = Dns.GetHostName();
        IPHostEntry ipEntry = Dns.GetHostEntry(hostname);
        foreach (IPAddress ip in ipEntry.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                ipsAddressesInformation.GetIpsList().Add(ip.ToString());
            }
        }
        return ipsAddressesInformation;
    }
}