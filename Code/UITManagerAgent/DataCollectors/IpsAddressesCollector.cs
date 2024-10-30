using System.Net;
using System.Net.Sockets;
using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.DataCollectors;
/// <summary>
/// Class responsible for collecting IP addresses.
/// </summary>
public class IpsAddressesCollector : DataCollector {
    /// <summary>
    /// Collects the IP addresses from the local system.
    /// </summary>
    /// <returns>
    /// An <see cref="IpsAddressesInformation"/> object containing the collected IP addresses.</returns>
    public Information Collect() {
        IpsAddressesInformation ipsAddressesInformation = new();
        string hostname = Dns.GetHostName();
        IPHostEntry ipEntry = Dns.GetHostEntry(hostname);
        foreach (IPAddress ip in ipEntry.AddressList) {
            if (ip.AddressFamily == AddressFamily.InterNetwork) {
                ipsAddressesInformation.GetIpsList().Add(ip.ToString());
            }
        }
        return ipsAddressesInformation;
    }
}
