using System;
using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;
using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.Tests.BasicInformation {
    [TestClass]
    public class UpTimeInformationTest {
        private UpTimeInformation? _upTimeInformation;

        /// <summary>
        /// Initialize a new instance of the <see cref="UpTimeInformation"/> class before each test.
        /// </summary>
        [TestInitialize]
        public void Setup() {
            _upTimeInformation = new();
        }


        /// <summary>
        /// Tests the <see cref="UpTimeInformation.ToJson"/> method to verify that it generates valid JSON 
        /// containing the value of the <see cref="UpTimeInformation.Milliseconds"/> property when it is set.
        /// </summary>
        [TestMethod]
        public void ToJson_ShouldReturnValidJson_WhenMillisecondsIsSet() {
            if (_upTimeInformation != null) {
                _upTimeInformation.Milliseconds = 1000;
                string json = _upTimeInformation.ToJson();
                string expected = $"{{\"Days\":0,\"Hours\":00,\"Minutes\":00,\"Seconds\":01}}";
                StringAssert.Contains(json, expected);
            }
        }
    }
}