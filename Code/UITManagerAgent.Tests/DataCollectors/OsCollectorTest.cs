using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;
using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.Tests.DataCollectors;

/// <summary>
/// Unit tests for the <see cref="OsCollector"/> class, ensuring that OS information
/// is collected and populated correctly.
/// </summary>
[TestClass]
[SupportedOSPlatform("windows")]
public class OsCollectorTest {
    private OsCollector? _osCollector;

    /// <summary>
    /// Initializes a new instance of <see cref="OsCollector"/> before each test.
    /// </summary>
    [TestInitialize]
    public void Setup() {
        _osCollector = new OsCollector();
    }

    /// <summary>
    /// Tests that <see cref="OsCollector.Collect"/> returns an <see cref="OsInformation"/> object
    /// with non-empty OS name and version.
    /// </summary>
    [TestMethod]
    public void Test_Collect_ReturnsOsInformationWithValues() {
        if (_osCollector != null) {
            OsInformation? osInfo = (OsInformation)_osCollector.Collect();

            Assert.IsNotNull(osInfo);
            Assert.IsFalse(string.IsNullOrEmpty(osInfo.OsName), "OS name shouldn't be null or empty");
            Assert.IsFalse(string.IsNullOrEmpty(osInfo.OsVersion), "OS version shouldn't be null or empty");
        }
    }

    /// <summary>
    /// Tests that <see cref="OsCollector.Collect"/> returns <see cref="OsInformation"/> with non-null OS name and version.
    /// </summary>
    [TestMethod]
    public void Test_Collect_PopulatesOsInformation() {
        if (_osCollector != null) {
            OsInformation osInfo = (OsInformation)_osCollector.Collect();

            Assert.IsNotNull(osInfo);
            Assert.IsFalse(string.IsNullOrWhiteSpace(osInfo.OsName),
                "OS name shouldn't be null, empty, or composed only of whitespace");
            Assert.IsFalse(string.IsNullOrWhiteSpace(osInfo.OsVersion),
                "OS version shouldn't be null, empty, or composed only of whitespace");
        }
    }

    /// <summary>
    /// Tests that the OS name and version contain expected characters (basic format check).
    /// </summary>
    [TestMethod]
    public void Test_Collect_ValidatesOsNameAndVersionFormat() {
        if (_osCollector != null) {
            OsInformation osInfo = (OsInformation)_osCollector.Collect();

            Assert.IsTrue(osInfo.OsName != null && osInfo.OsName.Length > 3, "OS name should be a valid non-trivial string");
            Assert.IsTrue(osInfo.OsVersion != null && osInfo.OsVersion.Contains('.'),
                "OS version should contain a dot, suggesting a valid version format");
        }
    }
}