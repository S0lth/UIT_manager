using System.Net;
using System.Net.Sockets;
using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;
using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.Tests.DataCollectors;

[TestClass]
public class IpsAddressesCollectorTests {
    /// <summary>
    /// Test method to check if it return a IP List if ip are available
    /// </summary>
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public void Collect_ShouldReturnNonEmptyIps_WhenIPsAreAvailable() {
        IpsAddressesCollector collector = new();

        IpsAddressesInformation result = (IpsAddressesInformation)collector.Collect();

        Assert.IsNotNull(result);
        Assert.IsTrue(result.InformationAgents.Count > 0, "Expected at least one IP address in the list.");
    }

    /*
    /// <summary>
    /// Test method to check if loopback address is include in results (should be no)
    /// </summary>
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public void Collect_ShouldNotIncludeLoopbackAddresses() {
        IPAddress loopbackAddress = IPAddress.Loopback;
        IpsAddressesCollector collector = new();

        IpsAddressesInformation result = (IpsAddressesInformation)collector.Collect();

        Assert.IsNotNull(result);
        foreach (string ip in result.Ips) {
            Assert.IsFalse(IPAddress.Parse(ip).Equals(loopbackAddress),
                "The IP list should not contain loopback addresses like 127.0.0.1.");
        }
    }
*/
    /// <summary>
    /// Test method to check if method never throw SocketException (should be handled in Collect)
    /// </summary>
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public void Collect_ShouldHandleSocketExceptionGracefully() {
        IpsAddressesCollector collector = new();

        try {
            IpsAddressesInformation result = (IpsAddressesInformation)collector.Collect();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.InformationAgents.Count >= 0, "The IP list should handle exceptions gracefully.");
        }
        catch (SocketException) {
            Assert.Fail("The method should handle SocketException and not throw it.");
        }
    }
}