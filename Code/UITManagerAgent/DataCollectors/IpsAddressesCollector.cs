using System.Net;
using System.Net.Sockets;
using System.Runtime.Versioning;
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
    /// An <see cref="IpsAddressesInformation"/> object containing the collected IP addresses.
    /// If no IP addresses are found or an error occurs, the list may be empty.
    /// </returns>
    /// <remarks>
    /// This method handles potential network-related exceptions
    /// such as <see cref="SocketException"/> and errors.
    /// In case of error, the method returns an empty
    /// <see cref="IpsAddressesInformation"/> object.
    /// </remarks>
    [SupportedOSPlatform("windows")]
    public Information Collect() {
        IpsAddressesInformation ipsAddressesInformation = new();
        try {
            string hostname = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(hostname);
            foreach (IPAddress ip in ipEntry.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    ipsAddressesInformation.InformationAgents.Add(new InnerValue("Ip Address","TEXT",ip.ToString()));
                }
            }
        }
        catch (SocketException socketException) {
            Console.WriteLine($"Network error while retrieving IP addresses: {socketException.Message}");
        }
        catch (Exception exception) {
            Console.WriteLine($"An unexpected error occured: {exception.Message}");
        }

        return ipsAddressesInformation;
    }
}