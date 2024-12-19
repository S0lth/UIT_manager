
using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.Tests.BasicInformation;


/// <summary>
/// Contains unit tests for the <see cref="DomainNameInformation"/> class.
/// </summary>
[TestClass]
public class DomainNameTest {

    private DomainNameInformation? _domainName;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainNameInformation"/> class before each test.
    /// </summary>
    [TestInitialize]
    public void Setup() {
        _domainName = new DomainNameInformation();
    }

    /// <summary>
    /// Tests the <see cref="DomainNameInformation.ToJson"/> method to verify that it generates valid JSON 
    /// containing the value of the <see cref="DomainNameInformation.DomainName"/> property when it is set.
    /// </summary>
    [TestMethod]
    public void ToJson_ShouldReturnValidJson_WhenDomainNameIsSet() {

        if (_domainName != null) {
            _domainName.DomaineName.Value = "test.com";
            var json = _domainName.ToJson();
            var expectedJson = $"{{\"Name\":\"{_domainName.DomaineName.Name}\",\"Value\":\"test.com\",\"Format\":\"{_domainName.DomaineName.Format}\"}}";
            StringAssert.Contains(json, expectedJson);
        }
    }

}