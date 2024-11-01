using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.Tests.Basicinformation;

/// <summary>
/// Contains unit tests for the <see cref="RamInformation"/> class.
/// </summary>
[TestClass]
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
    public void Test_ToString_ReturnsCorrectFormat() {
        ulong totalMemory = 8 * 1024 * 1024;
        ulong usedMemory = 5 * 1024 * 1024;
        ulong freeMemory = totalMemory - usedMemory;
            
        _ramInformation.TotalMemory = 8 * 1024 * 1024;
        _ramInformation.UsedMemory = 5 * 1024 * 1024;
        _ramInformation.FreeMemory = 3 * 1024 * 1024;

        string result = _ramInformation.ToString();
        
        string expected = $"Total memory : {totalMemory / (float)(1024 * 1024):F2} GB" + Environment.NewLine + 
                          $"Used memory : {usedMemory / (float)(1024 * 1024):F2} GB" + Environment.NewLine + 
                          $"Free memory : {freeMemory / (float)(1024 * 1024):F2} GB";
        
        Assert.AreEqual(expected, result);
    }

    /// <summary>
    /// Tests setting memory values to ensure they are stored correctly.
    /// </summary>
    [TestMethod]
    public void Test_SetMemoryValues_SetsValuesCorrectly()
    {
        ulong totalMemory = 16 * 1024 * 1024;
        ulong freeMemory = 6 * 1024 * 1024;

        _ramInformation.TotalMemory = totalMemory;
        _ramInformation.FreeMemory = freeMemory;
        _ramInformation.UsedMemory = totalMemory - freeMemory;

        Assert.AreEqual(totalMemory, _ramInformation.TotalMemory);
        Assert.AreEqual(freeMemory, _ramInformation.FreeMemory);
        Assert.AreEqual<ulong>(10 * 1024 * 1024, _ramInformation.UsedMemory);
    }

    /// <summary>
    /// Tests the <see cref="RamInformation.ToString"/> method to ensure it returns "0.00 GB" when all memory values are zero.
    /// </summary>
    [TestMethod]
    public void Test_ToString_ReturnsZeroGB_WhenMemoryIsZero() {
        ulong totalMemory = 0;
        ulong usedMemory = 0;
        ulong freeMemory = 0;
        
        _ramInformation.TotalMemory = 0;
        _ramInformation.FreeMemory = 0;
        _ramInformation.UsedMemory = 0;

        string result = _ramInformation.ToString();
        
        string expected = $"Total memory : {totalMemory / (float)(1024 * 1024):F2} GB" + Environment.NewLine + 
                          $"Used memory : {usedMemory / (float)(1024 * 1024):F2} GB" + Environment.NewLine + 
                          $"Free memory : {freeMemory / (float)(1024 * 1024):F2} GB";

        Assert.AreEqual(expected, result);
    }
}