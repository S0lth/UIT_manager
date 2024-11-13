using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;
using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.Tests.BasicInformation;

/// <summary>
/// This class contains the Unit Tests for method <see cref="IpsAddressesInformation.ToJson"/>
/// </summary>
[TestClass]
public class IpsAddressesTest {
    private IpsAddressesInformation? _ipsAddressesInformation;
    /// <summary>
    /// Initialize a new instance of the <see cref="IpsAddressesInformation"/> class before each test.
    /// </summary>
    [TestInitialize]
    public void Setup() {
        _ipsAddressesInformation = new IpsAddressesInformation();
    }

    /// <summary>
    /// Test method to check if <see cref="IpsAddressesInformation.ToJson"/>
    /// return a correct JSON format with manually set IPS
    /// </summary>
    [TestMethod]
    public void ToJson_ShouldReturnValidJson_WhenIPSAddressesAreManuallySet() {
        if (_ipsAddressesInformation != null) {
            _ipsAddressesInformation.IpsList = new() {
                "192.168.1.64",
                "172.16.25.1",
                "192.168.2.1",
            };
            List<string> ipsList = _ipsAddressesInformation.IpsList;
            string expectedJson = $"{{\"IpsList\":[\"{ipsList[0]}\",\"{ipsList[1]}\",\"{ipsList[2]}\"]}}";

            Assert.AreEqual(expectedJson, _ipsAddressesInformation.ToJson());
        }
        else {
            Assert.Fail();
        }
    }

    /// <summary>
    /// Test method to check if <see cref="IpsAddressesInformation.ToJson"/>
    /// return a json with no values if IPsList is empty
    /// </summary>
    [TestMethod]
    public void ToJson_ShouldReturnValidJson_WhenIPSAddressesAreEmpty() {
        string expectedJson = "{\"IpsList\":[]}";
        if (_ipsAddressesInformation != null) {
            _ipsAddressesInformation.IpsList = new();
            Assert.AreEqual(expectedJson, _ipsAddressesInformation.ToJson());
        }
        else {
            Assert.Fail();
        }
    }

    /// <summary>
    /// Test method to check if <see cref="IpsAddressesInformation.ToJson"/>
    /// return the correct format with real values
    /// </summary>
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public void ToJson_ShouldReturnValidJson_WithActualIPSAddresses() {
        IpsAddressesCollector ipsAddressesCollector = new();
        _ipsAddressesInformation = (IpsAddressesInformation)ipsAddressesCollector.Collect();

        List<string> actualIPsList = _ipsAddressesInformation.IpsList;

        string expected = "{\"IpsList\":[";
        if (actualIPsList.Count > 0) {
            foreach (string ip in actualIPsList) {
                expected += $"\"{ip}\",";
            }
            expected = expected.Remove(expected.Length - 1) + "]}";
        }
        Assert.AreEqual(expected, _ipsAddressesInformation.ToJson());
    }
}