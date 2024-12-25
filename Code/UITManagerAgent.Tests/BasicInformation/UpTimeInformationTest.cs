using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.Tests.BasicInformation {
    [TestClass]
    public class UpTimeInformationTest {
        private UpTimeInformation? _upTimeInformation;

        /// <summary>
        /// Initialize a new instance of the <see cref="UpTimeInformation"/> class before each test.
        /// </summary>
        [TestInitialize]
        [SupportedOSPlatform("windows")]
        public void Setup() {
            _upTimeInformation = new();
        }


        /// <summary>
        /// Tests the <see cref="UpTimeInformation.ToJson"/> method to verify that it generates valid JSON 
        /// containing the value of the <see cref="UpTimeInformation.Milliseconds"/> property when it is set.
        /// </summary>
        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void ToJson_ShouldReturnValidJson_WhenMillisecondsIsSet() {
            if (_upTimeInformation != null) {
                _upTimeInformation.UpTime.Value = "1000";
                string json = _upTimeInformation.ToJson();
                string expected = $"{{\"Name\":\"{ _upTimeInformation.UpTime.Name}\",\"Value\":\"1000\",\"Format\":\"{ _upTimeInformation.UpTime.Format}\"}}";
                StringAssert.Contains(json, expected);
            }
        }
    }
}