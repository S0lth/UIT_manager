using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;
using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.Tests.BasicInformation {
    [TestClass]
    public class CpuInformationTest {
        private CpuInformation? _cpuInformation;

        /// <summary>
        /// Initialize a new instance of the <see cref="CpuInformation"/> class before each test.
        /// </summary>
        [TestInitialize]
        public void Setup() {
            _cpuInformation = new();
        }


        /// <summary>
        /// Tests the <see cref="CpuInformation.ToJson"/> method to verify that it generates valid JSON 
        /// containing the value of the <see cref="CpuInformation.CoreCount"/> property when it is set.
        /// </summary>
        [TestMethod]
        public void ToJson_ShouldReturnValidJson_WhenCoreCountIsSet() {
            if (_cpuInformation != null) {
                _cpuInformation.CoreCount = 1;
                string json = _cpuInformation.ToJson();
                string expected = $"{{\"LogicalCpu\":0,\"CoreCount\":{_cpuInformation.CoreCount},\"ClockSpeed\":0,\"Model\":\"\"}}";
                StringAssert.Contains(json, expected);
            }
            else {
                Assert.Fail();
            }
        }
    }
}