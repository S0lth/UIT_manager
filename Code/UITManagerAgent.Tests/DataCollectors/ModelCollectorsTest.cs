using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;
using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.Tests.DataCollectors {
    [TestClass]
    public class ModelCollectorsTest {
        private ModelCollectors? _modelCollectors;

        [TestInitialize]
        public void Setup() {
            _modelCollectors = new ModelCollectors();
        }

        /// <summary>
        /// Tests if the <see cref="ModelCollectors.Collect" /> method returns an instance of <see cref="ModelInformation" />.
        /// </summary>
        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void Collect_ShouldReturnModelInformationInstance() {
            if (_modelCollectors != null) {
                Information result = _modelCollectors.Collect();

                Assert.IsNotNull(result, "Result should not be null.");
                Assert.IsInstanceOfType(result, typeof(ModelInformation), "Result should be of type ModelInformation.");
            }
        }

        /// <summary>
        /// Tests if the <see cref="ModelCollectors.Collect" /> method handles exceptions gracefully.
        /// </summary>
        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void Collect_ShouldHandleExceptionsGracefully() {
            try {
                if (_modelCollectors != null) {
                    Information result = _modelCollectors.Collect();
                    Assert.IsNotNull(result, "Result should not be null even in case of an exception.");
                }
            }
            catch (Exception ex) {
                Assert.Fail($"The Collect method threw an unexpected exception: {ex.Message}");
            }
        }

        /// <summary>
        ///     Tests if the <see cref="ModelCollectors.Collect" /> method returns a non-null and non-empty model
        /// </summary>
        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void Collect_DiskListShouldNotBeNullOrEmpty_WhenDiskExist() {
            if (_modelCollectors != null) {
                ModelInformation result = (ModelInformation)_modelCollectors.Collect();

                Assert.IsNotNull(result.Model, "model should not be null.");
            }
        }

        /// <summary>
        ///     Tests if the <see cref="ModelCollectors.Collect" /> method returns a new instance of <see cref="ModelInformation" />
        ///     on each call.
        /// </summary>
        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void Collect_ShouldReturnNewInstanceOnEachCall() {
            if (_modelCollectors != null) {
                ModelInformation firstResult = (ModelInformation)_modelCollectors.Collect();
                ModelInformation secondResult = (ModelInformation)_modelCollectors.Collect();

                Assert.AreNotSame(firstResult, secondResult,
                        "Each call to Collect should return a new instance of ModelInformation.");
            }
        }
    }
}