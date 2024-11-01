using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UITManagerAgent.DataCollectors;
using System.Management;
using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.Tests {
    /// <summary>
    /// Contains unit tests for the <see cref="DomainNameCollector"/> class.
    /// </summary>
    [TestClass]
    public class DomainNameCollectorTests {
        private DomainNameCollector? _domainNameCollector;

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainNameCollector"/> class before each test.
        /// </summary>
        [TestInitialize]
        public void Setup() {
            _domainNameCollector = new DomainNameCollector();
        }

        /// <summary>
        /// Tests if the <see cref="DomainNameCollector.Collect"/> method returns an instance of <see cref="DomainNameInformation"/>.
        /// </summary>
        [TestMethod]
        public void Collect_ShouldReturnDomainNameInformationInstance() {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                Assert.Inconclusive("This test only runs on Windows.");
                return;
            }

            if (_domainNameCollector != null) {
                Information result = _domainNameCollector.Collect();

                Assert.IsNotNull(result, "Result should not be null.");
                Assert.IsInstanceOfType(result, typeof(DomainNameInformation), "Result should be of type DomainNameInformation.");
            }
        }

        /// <summary>
        /// Tests if the <see cref="DomainNameCollector.Collect"/> method handles exceptions gracefully.
        /// </summary>
        [TestMethod]
        public void Collect_ShouldHandleExceptionsGracefully() {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                Assert.Inconclusive("This test only runs on Windows.");
                return;
            }

            try {
                if (_domainNameCollector != null) {
                    Information result = _domainNameCollector.Collect();

                    Assert.IsNotNull(result, "Result should not be null even in case of an exception.");
                }
            }
            catch (Exception ex) {
                Assert.Fail($"The Collect method threw an unexpected exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Tests if the <see cref="DomainNameCollector.Collect"/> method returns "No domain" when no domain exists.
        /// </summary>
        [TestMethod]
        public void Collect_ShouldReturnNoDomain_WhenNoDomainExists() {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                Assert.Inconclusive("This test only runs on Windows.");
                return;
            }

            if (_domainNameCollector != null) {
                DomainNameInformation result = (DomainNameInformation)_domainNameCollector.Collect();

                Assert.AreEqual("No domain", result.DomainName, "Should return 'No domain' when the system is not part of a domain.");
            }
        }

        /// <summary>
        /// Tests if the <see cref="DomainNameCollector.Collect"/> method returns a valid domain name when the machine is part of a domain.
        /// </summary>
        [TestMethod]
        public void Collect_ShouldReturnDomainName_WhenDomainExists() {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                Assert.Inconclusive("This test only runs on Windows.");
                return;
            }

           if (_domainNameCollector != null) {
                DomainNameInformation result = (DomainNameInformation)_domainNameCollector.Collect();

                Assert.IsFalse(string.IsNullOrEmpty(result.DomainName), "Domain name should not be empty when the machine is part of a domain.");
            }
        }

        /// <summary>
        /// Tests if the <see cref="DomainNameCollector.Collect"/> method returns a new instance of <see cref="DomainNameInformation"/> on each call.
        /// </summary>
        [TestMethod]
        public void Collect_ShouldReturnNewInstanceOnEachCall() {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                Assert.Inconclusive("This test only runs on Windows.");
                return;
            }

            if (_domainNameCollector != null) {
                DomainNameInformation firstResult = (DomainNameInformation)_domainNameCollector.Collect();
                DomainNameInformation secondResult = (DomainNameInformation)_domainNameCollector.Collect();

                Assert.AreNotSame(firstResult, secondResult, "Each call to Collect should return a new instance of DomainNameInformation.");
            }
        }

        /// <summary>
        /// Tests if the <see cref="DomainNameCollector.Collect"/> method correctly handles ManagementExceptions.
        /// </summary>
        [TestMethod]
        public void Collect_ShouldHandleManagementException() {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                Assert.Inconclusive("This test only runs on Windows.");
                return;
            }

            DomainNameInformation result = (DomainNameInformation)_domainNameCollector.Collect();

            Assert.AreEqual("No domain", result.DomainName, "Should return 'No domain' when a ManagementException occurs.");
        }

        /// <summary>
        /// Tests if the <see cref="DomainNameCollector.Collect"/> method correctly handles general exceptions.
        /// </summary>
        [TestMethod]
        public void Collect_ShouldHandleGeneralException() {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                Assert.Inconclusive("This test only runs on Windows.");
                return;
            }

            DomainNameInformation result = (DomainNameInformation)_domainNameCollector.Collect();

            Assert.AreEqual("No domain", result.DomainName, "Should return 'No domain' when a general exception occurs.");
        }
    }
}
