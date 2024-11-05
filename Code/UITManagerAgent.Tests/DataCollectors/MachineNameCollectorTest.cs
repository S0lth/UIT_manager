using System.Runtime.Versioning;
using UITManagerAgent.DataCollectors;
using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.Tests.DataCollectors;

/// <summary>
/// Contains unit tests for the <see cref="MachineNameCollector"/> class.
/// </summary>
[TestClass]
public class MachineNameCollectorTest {
    private MachineNameCollector? _machineNameCollector;

    /// <summary>
    /// Initialize a new instance of <see cref="MachineNameCollector"/> class before each tests.
    /// </summary>
    [TestInitialize]
    public void Setup() {
        _machineNameCollector = new();
    }

    /// <summary>
    /// Tests if the <see cref="MachineNameCollector.Collect" /> method returns an instance of <see cref="MachineNameInformation" />.
    /// </summary>
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public void Collect_ShouldReturnMachineNameInformationInstance() {
        if (_machineNameCollector is not null) {
            Information result = _machineNameCollector.Collect();
            Assert.IsNotNull(result, "Result should not be null.");
            Assert.IsInstanceOfType(result, typeof(MachineNameInformation),
                "Result should be of type MachineNameInformation.");
        }
    }

    /// <summary>
    /// Tests to check if <see cref="MachineNameCollector.Collect"/> method returns the expected value
    /// </summary>
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public void Test_MachineName_ReturnsExpectedValue() {
        string expectedMachineName = Environment.MachineName;

        string? actualMachineName = _machineNameCollector?.Collect().ToString();

        Assert.AreEqual(expectedMachineName, actualMachineName,
            $"{actualMachineName} does not match expected machine name.");
    }

    /// <summary>
    /// Tests to check if <see cref="MachineNameCollector.Collect"/> method returns a non-empty string value
    /// </summary>
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public void Test_MachineName_ReturnsNonEmptyString() {
        string? actualMachineName = _machineNameCollector?.Collect().ToString();
        Assert.IsFalse(string.IsNullOrWhiteSpace(actualMachineName), "Machine name should not be empty.");
        Assert.IsNotNull(actualMachineName);
    }
}