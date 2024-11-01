using UITManagerAgent.BasicInformation;
using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.Tests.DataCollectors;

/// <summary>
/// Contains unit tests for the <see cref="RamCollector"/> class.
/// </summary>
[TestClass]
public class CpuCollectorTest
{
    private CpuCollectors _CpuCollector ;


    /// <summary>
    /// Initializes a new instance of the <see cref="RamCollector"/> class and the <see cref="RamInformation"/> class before each test.
    /// </summary>
    [TestInitialize]
    public void Setup()
    {
        _CpuCollector = new CpuCollectors();
    }

    /// <summary>
    /// Tests the <see cref="CpuCollecors.GetProcessorCount"/> method to ensure it sets ProcessorCount correctly.
    /// </summary>
    [TestMethod]
    public void Test_GetProcessorCount()
    {
        int ProcessorCount = _CpuCollector.GetProcessorCount();

        Assert.IsNotNull(ProcessorCount);
        Assert.IsTrue(ProcessorCount > 0, "Total Cpu count should be greater than 0");

    }
    /// <summary>
    /// Tests the <see cref="CpuCollecors.GetCurrentClockSpeedt"/> method to ensure it sets ClockSpeed correctly.
    /// </summary>
    
    [TestMethod]
    public void Test_GetCurrentClockSpeed() {
        int ClockSpeed = _CpuCollector.GetCurrentClockSpeed();

        Assert.IsNotNull(ClockSpeed);
        Assert.IsTrue(ClockSpeed > 0, "Clock speed should be greater than 0");

    }
    /// <summary>
    /// Tests the <see cref="CpuCollecor.GetNumberOfCores"/> method to ensure it sets Core correctly.
    /// </summary>
    [TestMethod]
    public void Test_GetNumberOfCores() {
        int Core = _CpuCollector.GetNumberOfCores();

        Assert.IsNotNull(Core);
        Assert.IsTrue(Core > 0, "Number of Core should be greater than 0");

    }
    /// <summary>
    /// Tests the <see cref="CpuCollecor.GetModelCPU"/> method to ensure it sets Model correctly.
    /// </summary>
    [TestMethod]
    public void Test_GetModelCPU() {
        string Model = _CpuCollector.GetModelCPU();

        Assert.IsNotNull(Model);

    }

}