
using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;
using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.Tests.BasicInformation;
/// <summary>
/// This class contains the Unit Tests for method <see cref="OsInformation.ToJson"/>
/// </summary>
[TestClass]
public class OsInformationTest {
    private OsInformation? _osInformation;
    
    /// <summary>
    /// Initialize a new instance of the <see cref="OsInformation"/> class before each test.
    /// </summary>
    [TestInitialize]
    [SupportedOSPlatform("windows")]
    public void Setup() {
        _osInformation = new();
    }
    
    /// <summary>
    /// Test method to check if the method <see cref="OsInformation.ToJson"/>
    /// returns the correct format
    /// </summary>
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public void ToJson_ShouldReturnValidJson_WhenOsAttributesAreSet() {
        if (_osInformation != null) {
            _osInformation.OsName = "Test";
            _osInformation.OsVersion = "1.0.0.0";
            
            string json = _osInformation.ToJson();
            int expectedLeftBrace = json.Count(c => c == '{'),
                expectedRightBrace = json.Count(c => c == '}');
            
            Assert.AreEqual(expectedRightBrace, expectedLeftBrace);
            
        } else {
            Assert.Fail("Os Information cannot be null");
        }
    }
    
    /// <summary>
    /// Test method which check if main informations (OsName, OsVersion, etc.) are in
    /// <see cref="OsInformation.ToJson"/>
    /// </summary>
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public void ToJson_ShouldReturnImportantInformation_WhenOsAttributeAreReceived() {
        OsCollector osCollector = new();
        _osInformation = (OsInformation)osCollector.Collect(); 
        string json = _osInformation.ToJson();
        
        List<string> informations = new() {
            "OsName",
            "OsVersion",
        };
        
        foreach (string informationName in informations) {
            StringAssert.Contains(json, informationName);
        }
    }
}

