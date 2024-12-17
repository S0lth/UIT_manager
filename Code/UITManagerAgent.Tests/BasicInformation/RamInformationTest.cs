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
        _ramInformation2.InformationAgents[0].Value = 1000000+"";
        
    }

    /// <summary>
    /// Tests the <see cref="RamInformation.ToString"/> method to ensure it returns the correct format.
    /// </summary>
    /*
    [TestMethod]
    public void Test_ToString_ReturnsCorrectFormat() {
        ulong TotalRam = 8 * 1024 * 1024;
        ulong UsedRam = 5 * 1024 * 1024;
        ulong FreeRam = TotalRam - UsedRam;

        if (_ramInformation != null) {
            _ramInformation.InformationAgents[0].Value = (8 * 1024 * 1024)+"";
            _ramInformation.InformationAgents[1].Value  = (5 * 1024 * 1024)+"";
            _ramInformation.InformationAgents[2].Value  = (3 * 1024 * 1024)+"";

            string result = _ramInformation.ToString();

            string expected = $"Total memory : {TotalRam / (float)(1024 * 1024):F2} GB" + Environment.NewLine +
                              $"Used memory : {UsedRam / (float)(1024 * 1024):F2} GB" + Environment.NewLine +
                              $"Free memory : {FreeRam / (float)(1024 * 1024):F2} GB";

            Assert.AreEqual(expected, result);
        }
    }*/

    /// <summary>
    /// Tests setting memory values to ensure they are stored correctly.
    /// </summary>
    [TestMethod]
    public void Test_SetMemoryValues_SetsValuesCorrectly() {
        ulong TotalRam = 16 * 1024 * 1024 ;
        ulong FreeRam = 6 * 1024 * 1024 ;

        if (_ramInformation != null) {
            _ramInformation.InformationAgents[0].Value = TotalRam +"";
            _ramInformation.InformationAgents[1].Value = FreeRam +"";
            _ramInformation.InformationAgents[2].Value = TotalRam - FreeRam+"";

            Assert.AreEqual(TotalRam+"", _ramInformation.InformationAgents[0].Value);
            Assert.AreEqual(FreeRam+"",_ramInformation.InformationAgents[1].Value);
            Assert.AreEqual((10 * 1024 * 1024)+"", _ramInformation.InformationAgents[2].Value);
        }
    }
    


    /// <summary>
    /// Tests the <see cref="RamInformation.ToJson"/> method to verify that it generates valid JSON 
    /// containing the value of the <see cref="RamInformation.TotalRam"/> property when it is set.
    /// </summary>
    [TestMethod]
    public void ToJson_ShouldReturnValidJson_WhenTotalRamIsSet() {
        if (_ramInformation != null) {
            _ramInformation.InformationAgents[0].Value = 1000000+"";
            string json = _ramInformation.ToJson();
            string expected = _ramInformation2.ToJson();
            StringAssert.Contains(json, expected);
        }
    }
}