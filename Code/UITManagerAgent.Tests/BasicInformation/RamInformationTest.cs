using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;
using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.Tests.BasicInformation;

/// <summary>
/// Contains unit tests for the <see cref="RamInformation"/> class.
/// </summary>
[TestClass]
[SupportedOSPlatform("windows")]
public class RamInformationTest {
    private RamInformation? _ramInformation;

    /// <summary>
    /// Initializes a new instance of the <see cref="RamInformation"/> class before each test.
    /// </summary>
    [TestInitialize]
    public void Setup() {
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

        if (_ramInformation != null) {
            _ramInformation.TotalMemory = 8 * 1024 * 1024;
            _ramInformation.UsedMemory = 5 * 1024 * 1024;
            _ramInformation.FreeMemory = 3 * 1024 * 1024;

            string result = _ramInformation.ToString();

            string expected = $"Total memory : {totalMemory / (float)(1024 * 1024):F2} GB" + Environment.NewLine +
                              $"Used memory : {usedMemory / (float)(1024 * 1024):F2} GB" + Environment.NewLine +
                              $"Free memory : {freeMemory / (float)(1024 * 1024):F2} GB";

            Assert.AreEqual(expected, result);
        }
    }

    /// <summary>
    /// Tests setting memory values to ensure they are stored correctly.
    /// </summary>
    [TestMethod]
    public void Test_SetMemoryValues_SetsValuesCorrectly() {
        ulong totalMemory = 16 * 1024 * 1024;
        ulong freeMemory = 6 * 1024 * 1024;

        if (_ramInformation != null) {
            _ramInformation.TotalMemory = totalMemory;
            _ramInformation.FreeMemory = freeMemory;
            _ramInformation.UsedMemory = totalMemory - freeMemory;

            Assert.AreEqual(totalMemory, _ramInformation.TotalMemory);
            Assert.AreEqual(freeMemory, _ramInformation.FreeMemory);
            Assert.AreEqual<ulong>(10 * 1024 * 1024, _ramInformation.UsedMemory);
        }
    }

    /// <summary>
    /// Tests the <see cref="RamInformation.ToString"/> method to ensure it returns "0.00 GB" when all memory values are zero.
    /// </summary>
    [TestMethod]
    public void Test_ToString_ReturnsZeroGB_WhenMemoryIsZero() {
        ulong totalMemory = 0;
        ulong usedMemory = 0;
        ulong freeMemory = 0;

        if (_ramInformation != null) {
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


    /// <summary>
    /// Tests the <see cref="DiskInformation.ToJson"/> method to verify that it generates valid JSON 
    /// containing the value of the <see cref="DiskInformation.NumberDisk"/> property when it is set.
    /// </summary>
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public void ToJson_ShouldReturnValidJson_WhenNumberDiskIsSet() {
        if (_ramInformation != null) {
            DiskCollector diskCollector = new();
            string json = _ramInformation.ToJson();
            string expected = $"{{\"TotalMemory\":{_ramInformation.TotalMemory / (float)(1024 * 1024):F2},\"UsedMemory\":{_ramInformation.UsedMemory / (float)(1024 * 1024):F2},\"FreeMemory\":{_ramInformation.FreeMemory / (float)(1024 * 1024):F2}}}";
            StringAssert.Contains(json, expected);
        }
    }
}