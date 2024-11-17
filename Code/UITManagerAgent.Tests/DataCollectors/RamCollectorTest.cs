using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;
using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.Tests.DataCollectors;

/// <summary>
/// Contains unit tests for the <see cref="RamCollector"/> class.
/// </summary>
[TestClass]
[SupportedOSPlatform("windows")]
public class RamCollectorTest {
    private RamCollector? _ramCollector;
    private RamInformation? _ramInformation;

    /// <summary>
    /// Initializes a new instance of the <see cref="RamCollector"/> class and the <see cref="RamInformation"/> class before each test.
    /// </summary>
    [TestInitialize]
    public void Setup() {
        _ramCollector = new RamCollector();
        _ramInformation = new RamInformation();
    }

    /// <summary>
    /// Tests the <see cref="RamCollector.Collect"/> method to ensure it sets memory values correctly.
    /// </summary>
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public void Test_Collect_SetsMemoryValuesCorrectly() {
        if (_ramCollector != null) {
            _ramInformation = (RamInformation)_ramCollector.Collect()!;
            Assert.IsNotNull(_ramInformation);
            Assert.IsTrue(_ramInformation.TotalMemory > 0, "Total memory should be greater than 0");
            Assert.IsTrue(_ramInformation.FreeMemory >= 0, "Free memory should be non-negative");
            Assert.IsTrue(_ramInformation.UsedMemory >= 0, "Used memory should be non-negative");
        }
    }

    /// <summary>
    /// Tests the <see cref="RamCollector.Collect"/> method to ensure it calculates used memory correctly.
    /// </summary>
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public void Test_Collect_CalculatesUsedMemoryCorrectly() {
        if (_ramCollector != null) {
            _ramInformation = (RamInformation)_ramCollector.Collect();

            Assert.IsTrue(_ramInformation.UsedMemory <= _ramInformation.TotalMemory,
                "Used memory cannot exceed total memory");
            Assert.IsTrue(_ramInformation.FreeMemory + _ramInformation.UsedMemory <= _ramInformation.TotalMemory,
                "Free memory + Used memory should not exceed Total memory");
        }
    }
}