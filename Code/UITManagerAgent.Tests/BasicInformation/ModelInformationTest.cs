using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.Tests.BasicInformation {
    
    [TestClass]
    public class ModelInformationTest {
        private ModelInformation? _modelInformation;

        /// <summary>
        /// Initialize a new instance of the <see cref="ModelInformation"/> class before each test.
        /// </summary>
        [TestInitialize]
        public void Setup() {
            _modelInformation = new();
        }


        /// <summary>
        /// Tests the <see cref="ModelInformation.ToJson"/> method to verify that it generates valid JSON 
        /// containing the value of the <see cref="ModelInformation.Model"/> property when it is set.
        /// </summary>
        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void ToJson_ShouldReturnValidJson_WhenTagServiceIsSet() {
            if (_modelInformation != null) {
                _modelInformation.Model.Value = "82WM";
                string json = _modelInformation.ToJson();
                string expected = $"{{\"Name\":\"{_modelInformation.Model.Name}\",\"Value\":\"82WM\",\"Format\":\"{_modelInformation.Model.Format}\"}}";
                StringAssert.Contains(json, expected);
            }
        }
    }
}