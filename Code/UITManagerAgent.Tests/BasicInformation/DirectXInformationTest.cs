using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;
using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.Tests.BasicInformation;

/// <summary>
/// This class contains the Unit Tests for method <see cref="DirectXInformation.ToJson"/>
/// </summary>
[TestClass]
public class DirectXInformationTest {

    private DirectXInformation? _directXInformation;

    /// <summary>
    /// Initialize a new instance of the <see cref="DirectXInformation"/> class before each test.
    /// </summary>
    [TestInitialize]
    public void Setup() {
        _directXInformation = new DirectXInformation();
    }

    /// <summary>
    /// Test method to check if the method <see cref="DirectXInformation.ToJson"/>
    /// returns the correct format with define value
    /// </summary>
    [TestMethod]
    public void ToJson_ShouldReturnValidJson_WhenDirectXIsManuallySet() {
        if (_directXInformation != null) {
            _directXInformation.DirectXVersion = "DirectX 12";
            string json = _directXInformation.ToJson();
            string expected = "{\"DirectX\":\"DirectX 12\"}";
            StringAssert.Contains(expected, json);
        }
        else {
            Assert.Fail("DirectX cannot be null");
        }
    }
}