using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UITManagerAgent.DataCollectors;
using UITManagerAgent.BasicInformation;
using System.Runtime.Versioning;

namespace UITManagerAgent.Tests.DataCollectors {
    [TestClass]
    public class DiskCollectorTest {
        private DiskCollector? _diskCollector;

        [TestInitialize]
        public void Setup() {
            _diskCollector = new DiskCollector();
        }

        /// <summary>
        /// Tests if the <see cref="DiskCollector.Collect" /> method returns an instance of <see cref="DiskInformation" />.
        /// </summary>
        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void Collect_ShouldReturnDiskInformationInstance() {
            if (_diskCollector != null) {
                Information result = _diskCollector.Collect();

                Assert.IsNotNull(result, "Result should not be null.");
                Assert.IsInstanceOfType(result, typeof(DiskInformation), "Result should be of type DiskInformation.");
            }
        }

        /// <summary>
        ///     Tests if the <see cref="DiskCollector.Collect" /> method handles exceptions gracefully.
        /// </summary>
        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void Collect_ShouldHandleExceptionsGracefully() {
            try {
                if (_diskCollector != null) {
                    Information result = _diskCollector.Collect();

                    Assert.IsNotNull(result, "Result should not be null even in case of an exception.");
                }
            }
            catch (Exception ex) {
                Assert.Fail($"The Collect method threw an unexpected exception: {ex.Message}");
            }
        }

        /// <summary>
        ///     Tests if the <see cref="DiskCollector.Collect" /> method returns a non-null and non-empty list of Disk when Disk
        ///     exist.
        /// </summary>
        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void Collect_DiskListShouldNotBeNullOrEmpty_WhenDiskExist() {
            if (_diskCollector != null) {
                DiskInformation result = (DiskInformation)_diskCollector.Collect();

                Assert.IsNotNull(result.GetFirstDiskName(), "diskName list should not be null.");
                Assert.IsTrue(result.Disks.Count >= 1, "disk list should contain at least one item.");
            }
        }

        /// <summary>
        ///     Tests if the <see cref="DiskCollector.Collect" /> method returns a new instance of <see cref="DiskInformation" />
        ///     on each call.
        /// </summary>
        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void Collect_ShouldReturnNewInstanceOnEachCall() {
            if (_diskCollector != null) {
                DiskInformation firstResult = (DiskInformation)_diskCollector.Collect();
                DiskInformation secondResult = (DiskInformation)_diskCollector.Collect();

                Assert.AreNotSame(firstResult, secondResult,
                    "Each call to Collect should return a new instance of DiskInformation.");
            }
        }

    }
}
