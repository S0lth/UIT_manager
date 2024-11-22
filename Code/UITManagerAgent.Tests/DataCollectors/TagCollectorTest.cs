using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;
using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.Tests.DataCollectors {
    [TestClass]
    public class TagCollectorTest { 
        private TagCollector? _tagCollector;

        [TestInitialize]
        public void Setup() {
            _tagCollector = new TagCollector();
        }

        /// <summary>
        /// Tests if the <see cref="TagCollector.Collect" /> method returns an instance of <see cref="TagInformation" />.
        /// </summary>
        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void Collect_ShouldReturnTagInformationInstance() {
            if (_tagCollector != null) {
                Information result = _tagCollector.Collect();

                Assert.IsNotNull(result, "Result should not be null.");
                Assert.IsInstanceOfType(result, typeof(TagInformation), "Result should be of type TagInformation.");
            }
        }

        /// <summary>
        /// Tests if the <see cref="TagCollector.Collect" /> method handles exceptions gracefully.
        /// </summary>
        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void Collect_ShouldHandleExceptionsGracefully() {
            try {
                if (_tagCollector != null) {
                    Information result = _tagCollector.Collect();
                    Assert.IsNotNull(result, "Result should not be null even in case of an exception.");
                }
            }
            catch (Exception ex) {
                Assert.Fail($"The Collect method threw an unexpected exception: {ex.Message}");
            }
        }

        /// <summary>
        ///     Tests if the <see cref="TagCollector.Collect" /> method returns a non-null and non-empty tag
        /// </summary>
        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void Collect_DiskListShouldNotBeNullOrEmpty_WhenDiskExist() {
            if (_tagCollector != null) {
                TagInformation result = (TagInformation)_tagCollector.Collect();

                Assert.IsNotNull(result.TagService, "tagService should not be null.");
            }
        }

        /// <summary>
        ///     Tests if the <see cref="TagCollector.Collect" /> method returns a new instance of <see cref="TagInformation" />
        ///     on each call.
        /// </summary>
        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void Collect_ShouldReturnNewInstanceOnEachCall() {
            if (_tagCollector != null) {
                TagInformation firstResult = (TagInformation)_tagCollector.Collect();
                TagInformation secondResult = (TagInformation)_tagCollector.Collect();

                Assert.AreNotSame(firstResult, secondResult,
                        "Each call to Collect should return a new instance of TagInformation.");
            }
        }
    }
}