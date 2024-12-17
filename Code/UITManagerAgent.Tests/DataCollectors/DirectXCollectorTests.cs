using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;
using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.Tests.DataCollectors;

/// <summary>
/// Contains unit tests for the <see cref="DirectXCollector"/> class.
/// </summary>
[TestClass]
public class DirectXCollectorTests {
    private DirectXCollector? _directXCollector;

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectXCollector"/> class before each test.
    /// </summary>
    [TestInitialize]
    public void Setup() {
        _directXCollector = new DirectXCollector();
    }

    /// <summary>
    /// Tests if the <see cref="DirectXCollector.Collect"/> method returns an instance of <see cref="DirectXInformation"/>.
    /// </summary>
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public void Collect_ShouldReturnDirectXInformationInstance() {
        if (_directXCollector != null) {
            Information result = _directXCollector.Collect();

            Assert.IsInstanceOfType(result, typeof(DirectXInformation), "Result should be of type DirectXInformation.");
        }
    }

    /// <summary>
    /// Tests if the <see cref="DirectXCollector.Collect"/> method handles exceptions gracefully.
    /// </summary>
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public void Collect_ShouldHandleExceptionsGracefully() {
        try {
            if (_directXCollector != null) {
                Information result = _directXCollector.Collect();

                Assert.IsNotNull(result, "Result should not be null even in case of an exception.");
            }
        }
        catch (Exception ex) {
            Assert.Fail($"The Collect method threw an unexpected exception: {ex.Message}");
        }
    }

    /// <summary>
    /// Tests if the <see cref="DirectXCollector.Collect"/> method correctly identifies DirectX version when available.
    /// </summary>
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public void Collect_ShouldReturnDirectXVersion_WhenDirectXExists() {
        if (_directXCollector != null) {
            DirectXInformation result = (DirectXInformation)_directXCollector.Collect();
            if (result.DirectX != null) {
                Assert.IsTrue(result.DirectX.Name.Contains("DirectX"),
                    "The DirectX version should contain the word 'DirectX'.");
            }
        }
    }

    /// <summary>
    /// Tests if the <see cref="DirectXCollector.Collect"/> method returns a new instance of <see cref="DirectXInformation"/> on each call.
    /// </summary>
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public void Collect_ShouldReturnNewInstanceOnEachCall() {
        if (_directXCollector != null) {
            DirectXInformation firstResult = (DirectXInformation)_directXCollector.Collect();
            DirectXInformation secondResult = (DirectXInformation)_directXCollector.Collect();

            Assert.AreNotSame(firstResult, secondResult,
                "Each call to Collect should return a new instance of DirectXInformation.");
        }
    }
}