using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.Tests.BasicInformation {
    [TestClass]
    public class CpuInformationTest {
        private CpuInformation? _cpuInformation;
        private CpuInformation? _cpuInformation2;

        /// <summary>
        /// Initialize a new instance of the <see cref="CpuInformation"/> class before each test.
        /// </summary>
        [TestInitialize]
        public void Setup() {
            _cpuInformation = new();
            _cpuInformation2 = new CpuInformation();
            _cpuInformation.InformationAgents[1].Value = "1";
        }


        /// <summary>
        /// Tests the <see cref="CpuInformation.ToJson"/> method to verify that it generates valid JSON 
        /// containing the value of the <see cref="CpuInformation.CoreCount"/> property when it is set.
        /// </summary>
        [TestMethod]
        public void ToJson_ShouldReturnValidJson_WhenCoreCountIsSet() {
            if (_cpuInformation != null) {
                string json = _cpuInformation.ToJson();
                _cpuInformation2.InformationAgents[1].Value = "1";
                string expected =_cpuInformation2.ToJson();
                StringAssert.Contains(json, expected);
            }
            else {
                Assert.Fail();
            }
        }
    }
}