using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;
using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.Tests.BasicInformation {
    
    [TestClass]
    public class TagInformationTest {
        private TagInformation? _tagInformation;

        /// <summary>
        /// Initialize a new instance of the <see cref="TagInformation"/> class before each test.
        /// </summary>
        [TestInitialize]
        public void Setup() {
            _tagInformation = new();
        }


        /// <summary>
        /// Tests the <see cref="TagInformation.ToJson"/> method to verify that it generates valid JSON 
        /// containing the value of the <see cref="TagInformation.TagService"/> property when it is set.
        /// </summary>
        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void ToJson_ShouldReturnValidJson_WhenTagServiceIsSet() {
            if (_tagInformation != null) {
                _tagInformation.TagService.Value = "Test";
                string json = _tagInformation.ToJson();
                string expected =$"{{\"Name\":\"{_tagInformation.TagService.Name}\",\"Value\":\"Test\",\"Format\":\"{_tagInformation.TagService.Format}\"}}";
                StringAssert.Contains(json, expected);
            }
        }
    }
}