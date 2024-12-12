using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.Tests.BasicInformation;

/// <summary>
/// Contains unit tests for the <see cref="RamInformation"/> class.
/// </summary>
[TestClass]
[SupportedOSPlatform("windows")]
public class RamInformationTest {
    private RamInformation? _ramInformation;
    private RamInformation? _ramInformation2;

    /// <summary>
    /// Initializes a new instance of the <see cref="RamInformation"/> class before each test.
    /// </summary>
    [TestInitialize]
    public void Setup() {
        _ramInformation = new RamInformation();
        _ramInformation2 = new RamInformation();
        _ramInformation2.TotalRam = 1000000;
        
    }

    /// <summary>
    /// Tests the <see cref="RamInformation.ToString"/> method to ensure it returns the correct format.
    /// </summary>
    [TestMethod]
    public void Test_ToString_ReturnsCorrectFormat() {
        ulong TotalRam = 8 * 1024 * 1024;
        ulong UsedRam = 5 * 1024 * 1024;
        ulong FreeRam = TotalRam - UsedRam;

        if (_ramInformation != null) {
            _ramInformation.TotalRam = 8 * 1024 * 1024;
            _ramInformation.UsedRam = 5 * 1024 * 1024;
            _ramInformation.FreeRam = 3 * 1024 * 1024;

            string result = _ramInformation.ToString();

            string expected = $"Total memory : {TotalRam / (float)(1024 * 1024):F2} GB" + Environment.NewLine +
                              $"Used memory : {UsedRam / (float)(1024 * 1024):F2} GB" + Environment.NewLine +
                              $"Free memory : {FreeRam / (float)(1024 * 1024):F2} GB";

            Assert.AreEqual(expected, result);
        }
    }

    /// <summary>
    /// Tests setting memory values to ensure they are stored correctly.
    /// </summary>
    [TestMethod]
    public void Test_SetMemoryValues_SetsValuesCorrectly() {
        ulong TotalRam = 16 * 1024 * 1024;
        ulong FreeRam = 6 * 1024 * 1024;

        if (_ramInformation != null) {
            _ramInformation.TotalRam = TotalRam;
            _ramInformation.FreeRam = FreeRam;
            _ramInformation.UsedRam = TotalRam - FreeRam;

            Assert.AreEqual(TotalRam, _ramInformation.TotalRam);
            Assert.AreEqual(FreeRam, _ramInformation.FreeRam);
            Assert.AreEqual<ulong>(10 * 1024 * 1024, _ramInformation.UsedRam);
        }
    }

    /// <summary>
    /// Tests the <see cref="RamInformation.ToString"/> method to ensure it returns "0.00 GB" when all memory values are zero.
    /// </summary>
    [TestMethod]
    public void Test_ToString_ReturnsZeroGB_WhenMemoryIsZero() {
        ulong TotalRam = 0;
        ulong UsedRam = 0;
        ulong FreeRam = 0;

        if (_ramInformation != null) {
            _ramInformation.TotalRam = 0;
            _ramInformation.FreeRam = 0;
            _ramInformation.UsedRam = 0;

            string result = _ramInformation.ToString();

            string expected = $"Total memory : {TotalRam / (float)(1024 * 1024):F2} GB" + Environment.NewLine +
                              $"Used memory : {UsedRam / (float)(1024 * 1024):F2} GB" + Environment.NewLine +
                              $"Free memory : {FreeRam / (float)(1024 * 1024):F2} GB";

            Assert.AreEqual(expected, result);
        }
    }


    /// <summary>
    /// Tests the <see cref="RamInformation.ToJson"/> method to verify that it generates valid JSON 
    /// containing the value of the <see cref="RamInformation.TotalRam"/> property when it is set.
    /// </summary>
    [TestMethod]
    public void ToJson_ShouldReturnValidJson_WhenTotalRamIsSet() {
        if (_ramInformation != null) {
            _ramInformation.TotalRam = 1000000;
            string json = _ramInformation.ToJson();
            string expected = _ramInformation2.ToJson();
            StringAssert.Contains(json, expected);
        }
    }
}