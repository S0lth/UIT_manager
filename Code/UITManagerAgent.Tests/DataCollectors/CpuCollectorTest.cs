using System.Runtime.Versioning;
using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.Tests.DataCollectors;

/// <summary>
/// Contains unit tests for the <see cref="RamCollector"/> class.
/// </summary>
[TestClass]
public class CpuCollectorTest {
    private CpuCollectors? _cpuCollector;


    /// <summary>
    /// Initializes a new instance of the <see cref="CpuCollectors"/> class.
    /// </summary>
    [TestInitialize]
    public void Setup() {
        _cpuCollector = new CpuCollectors();
    }

    /// <summary>
    /// Tests the <see cref="CpuCollectors.GetProcessorCount"/> method to ensure it sets ProcessorCount correctly.
    /// </summary>
    /// 
    [SupportedOSPlatform("windows")]
    [TestMethod]
    public void Test_ShouldHaveOneProcessorAtLeast() {
        if (_cpuCollector != null) {
            int ProcessorCount = _cpuCollector.GetProcessorCount();

            Assert.IsNotNull(ProcessorCount);
            Assert.IsTrue(ProcessorCount > 0, "Total Cpu count should be greater than 0");
        }
    }

    /// <summary>
    /// Tests the <see cref="CpuCollectors.GetCurrentClockSpeedt"/> method to ensure it sets ClockSpeed correctly.
    /// </summary>
    [SupportedOSPlatform("windows")]
    [TestMethod]
    public void Test_ShouldHaveClockSpeedGreaterThanZero() {
        if (_cpuCollector != null) {
            int ClockSpeed = _cpuCollector.GetCurrentClockSpeed();

            Assert.IsNotNull(ClockSpeed);
            Assert.IsTrue(ClockSpeed > 0, "Clock speed should be greater than 0");
        }
    }

    /// <summary>
    /// Tests the <see cref="CpuCollectors.GetNumberOfCores"/> method to ensure it sets Core correctly.
    /// </summary>
    /// 
    [SupportedOSPlatform("windows")]
    [TestMethod]
    public void Test_ShouldHaveOneCoreAtLeast() {
        if (_cpuCollector != null) {
            int Core = _cpuCollector.GetNumberOfCores();

            Assert.IsNotNull(Core);
            Assert.IsTrue(Core > 0, "Number of Core should be greater than 0");
        }
    }

    /// <summary>
    /// Tests the <see cref="CpuCollectors.GetModelCPU"/> method to ensure it sets Model correctly.
    /// </summary>
    /// 
    [SupportedOSPlatform("windows")]
    [TestMethod]
    public void Test_CpuShouldHaveName() {
        if (_cpuCollector != null) {
            string Model = _cpuCollector.GetModelCPU();

            Assert.IsNotNull(Model);
        }
    }
}