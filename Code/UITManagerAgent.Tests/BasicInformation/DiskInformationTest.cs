using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;
using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.Tests.BasicInformation {
    [TestClass]
    public class DiskInformationTest {
        private DiskInformation? _diskInformation;

        /// <summary>
        /// Initialize a new instance of the <see cref="DiskInformation"/> class before each test.
        /// </summary>
        [TestInitialize]
        public void Setup() {
            _diskInformation = new();
        }


        /// <summary>
        /// Tests the <see cref="DiskInformation.ToJson"/> method to verify that it generates valid JSON 
        /// containing the value of the <see cref="DiskInformation.NumberDisk"/> property when it is set.
        /// </summary>
        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void ToJson_ShouldReturnValidJson_WhenNumberDiskIsSet() {
            if (_diskInformation != null) {
                DiskCollector diskCollector = new();
                string json = _diskInformation.ToJson();
                string expected = $"{{\"Disks\":[],\"NumberDisk\":{_diskInformation.NumberDisk},\"FormatNumberDisk\":\"NUMBER\"}}";
                StringAssert.Contains(json, expected);
            }
        }
    }
}