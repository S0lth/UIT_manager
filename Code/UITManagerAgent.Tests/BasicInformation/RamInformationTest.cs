using JetBrains.Annotations;
using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.Tests.Basicinformation;

/// <summary>
/// Contains unit tests for the <see cref="RamInformation"/> class.
/// </summary>
[TestClass]
[TestSubject(typeof(RamInformation))]
public class RamInformationTest
{
    private RamInformation _ramInformation;

    /// <summary>
    /// Initializes a new instance of the <see cref="RamInformation"/> class before each test.
    /// </summary>
    [TestInitialize]
    public void Setup()
    {
        _ramInformation = new RamInformation();
    }

    /// <summary>
    /// Tests the <see cref="RamInformation.ToString"/> method to ensure it returns the correct format.
    /// </summary>
    [TestMethod]
    public void Test_ToString_ReturnsCorrectFormat()
    {
        _ramInformation.SetTotalMemory(8 * 1024 * 1024);
        _ramInformation.SetFreeMemory(3 * 1024 * 1024);
        _ramInformation.SetUsedMemory(5 * 1024 * 1024);

        string result = _ramInformation.ToString();

        Assert.AreEqual("Total: 8.00 GB; Used: 5.00 GB", result);
    }

    /// <summary>
    /// Tests setting memory values to ensure they are stored correctly.
    /// </summary>
    [TestMethod]
    public void Test_SetMemoryValues_SetsValuesCorrectly()
    {
        ulong totalMemory = 16 * 1024 * 1024;
        ulong freeMemory = 6 * 1024 * 1024;

        _ramInformation.SetTotalMemory(totalMemory);
        _ramInformation.SetFreeMemory(freeMemory);
        _ramInformation.SetUsedMemory(totalMemory - freeMemory);

        Assert.AreEqual(totalMemory, _ramInformation.GetTotalMemory());
        Assert.AreEqual(freeMemory, _ramInformation.GetFreeMemory());
        Assert.AreEqual<ulong>(10 * 1024 * 1024, _ramInformation.GetUsedMemory());
    }

    /// <summary>
    /// Tests the <see cref="RamInformation.ToString"/> method to ensure it returns "0.00 GB" when all memory values are zero.
    /// </summary>
    [TestMethod]
    public void Test_ToString_ReturnsZeroGB_WhenMemoryIsZero()
    {
        _ramInformation.SetTotalMemory(0);
        _ramInformation.SetFreeMemory(0);
        _ramInformation.SetUsedMemory(0);

        string result = _ramInformation.ToString();

        Assert.AreEqual("Total: 0.00 GB; Used: 0.00 GB", result);
    }
}