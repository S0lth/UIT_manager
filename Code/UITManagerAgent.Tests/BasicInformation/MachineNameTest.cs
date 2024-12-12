using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;
using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.Tests.BasicInformation;


/// <summary>
/// This class contains the Unit Tests for method <see cref="MachineNameInformation.ToJson"/>
/// </summary>
[TestClass]
public class MachineNameTest {

    private MachineNameInformation? _machineNameInformation;

    /// <summary>
    /// Initialize a new instance of the <see cref="MachineNameInformation"/> class before each test.
    /// </summary>
    [TestInitialize]
    public void Setup() {
        _machineNameInformation = new();
    }

    /// <summary>
    /// Test method to check if the method <see cref="MachineNameInformation.ToJson"/>
    /// returns the correct format with define value
    /// </summary>
    [TestMethod]
    public void ToJson_ShouldReturnValidJson_WhenMachineNameIsManuallySet() {
        if (_machineNameInformation != null) {
            _machineNameInformation.MachineName = "DESKTOP_AAAAAA";
            string json = _machineNameInformation.ToJson();
            string expected = "{\"MachineName\":\"DESKTOP_AAAAAA\",\"Format\":\"TEXT\"}";
            StringAssert.Contains(expected, json);
        }
        else {
            Assert.Fail("MachineName cannot be null");
        }
    }

    /// <summary>
    /// Test method to check if the method <see cref="MachineNameInformation.ToJson"/>
    /// returns the correct format with real value
    /// </summary>
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public void ToJson_ShouldReturnValidJson_WithActualMachineName() {
        if (_machineNameInformation != null) {
            MachineNameCollector machineNameCollector = new();
            _machineNameInformation = (MachineNameInformation)machineNameCollector.Collect();
            string json = _machineNameInformation.ToJson();
            string expected = $"{{\"MachineName\":\"{_machineNameInformation.MachineName}\",\"Format\":\"TEXT\"}}";
            StringAssert.Contains(json, expected);
        }
    }
}