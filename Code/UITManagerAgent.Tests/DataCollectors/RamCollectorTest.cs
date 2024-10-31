using UITManagerAgent.BasicInformation;
using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.Tests.DataCollectors;

/// <summary>
/// Contains unit tests for the <see cref="RamCollector"/> class.
/// </summary>
[TestClass]
public class RamCollectorTest
{
    private RamCollector _ramCollector;
    private RamInformation _ramInformation;

    /// <summary>
    /// Initializes a new instance of the <see cref="RamCollector"/> class and the <see cref="RamInformation"/> class before each test.
    /// </summary>
    [TestInitialize]
    public void Setup()
    {
        _ramCollector = new RamCollector();
        _ramInformation = new RamInformation();
    }

    /// <summary>
    /// Tests the <see cref="RamCollector.Collect"/> method to ensure it sets memory values correctly.
    /// </summary>
    [TestMethod]
    public void Test_Collect_SetsMemoryValuesCorrectly()
    {
        _ramInformation = (RamInformation)_ramCollector.Collect();

        Assert.IsNotNull(_ramInformation);
        Assert.IsTrue(_ramInformation.GetTotalMemory() > 0, "Total memory should be greater than 0");
        Assert.IsTrue(_ramInformation.GetFreeMemory() >= 0, "Free memory should be non-negative");
        Assert.IsTrue(_ramInformation.GetUsedMemory() >= 0, "Used memory should be non-negative");
    }

    /// <summary>
    /// Tests the <see cref="RamCollector.Collect"/> method to ensure it calculates used memory correctly.
    /// </summary>
    [TestMethod]
    public void Test_Collect_CalculatesUsedMemoryCorrectly()
    {
        _ramInformation = (RamInformation)_ramCollector.Collect();

        Assert.IsTrue(_ramInformation.GetUsedMemory() <= _ramInformation.GetTotalMemory(),
            "Used memory cannot exceed total memory");
        Assert.IsTrue(
            _ramInformation.GetFreeMemory() + _ramInformation.GetUsedMemory() <= _ramInformation.GetTotalMemory(),
            "Free memory + Used memory should not exceed Total memory");
    }
}