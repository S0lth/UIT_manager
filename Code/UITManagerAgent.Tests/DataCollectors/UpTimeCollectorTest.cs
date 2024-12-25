using System.Runtime.Versioning;
using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.Tests.DataCollectors;

[TestClass]
[SupportedOSPlatform("windows")]
public class UpTimeCollectorTest {
    private UpTimeCollector? _upTimeCollector;

    [TestInitialize]
    public void Setup() {
        _upTimeCollector = new UpTimeCollector();
    }

    /// <summary>
    /// Test to ensure that Collect method returns a non-null UpTimeInformation instance.
    /// </summary>
    [TestMethod]
    public void Test_Collect_ReturnsNonNull() {
        if (_upTimeCollector != null) {
            var result = _upTimeCollector.Collect();
            Assert.IsNotNull(result);
        }
    }
/*
    /// <summary>
    /// Test to verify that the Milliseconds property in UpTimeInformation is non-negative.
    /// </summary>
    [TestMethod]
    public void Test_Collect_ReturnsNonNegativeMilliseconds() {
        if (_upTimeCollector != null) {
            UpTimeInformation upTimeInformation = (UpTimeInformation)_upTimeCollector.Collect();
            Assert.IsTrue(upTimeInformation.Milliseconds >= 0, "Milliseconds should be non-negative.");
        }
    }

    /// <summary>
    /// Test to validate that the system's uptime in days is a reasonable value (typically less than 500 days).
    /// </summary>
    [TestMethod]
    public void Test_Collect_ReasonableUptimeInDays() {
        if (_upTimeCollector != null) {
            UpTimeInformation upTimeInformation = (UpTimeInformation)_upTimeCollector.Collect();
            TimeSpan uptime = TimeSpan.FromMilliseconds(upTimeInformation.Milliseconds);
            Assert.IsTrue(uptime.TotalDays < 500, "Uptime in days should be less than 500.");
        }
    }

    /// <summary>
    /// Test to validate ToString with an explicit uptime value of zero.
    /// </summary>
    [TestMethod]
    public void Test_ToString_ReturnsZeroUptime() {
        UpTimeInformation upTimeInformation = new UpTimeInformation { Milliseconds = 0 };
        string expected = upTimeInformation.ToString();

        var upTimeInfo = new UpTimeInformation { Milliseconds = 0 };
        string output = upTimeInfo.ToString();

        Assert.AreEqual(expected, output);
    }

    /// <summary>
    /// Test to ensure negative values for Milliseconds are handled or avoided.
    /// </summary>
    [TestMethod]
    public void Test_UpTimeInformation_NegativeMillisecondsNotAllowed() {
        UpTimeInformation upTimeInformation = new();
        upTimeInformation.Milliseconds = -1;
        Assert.IsFalse(upTimeInformation.Milliseconds >= 0, "Milliseconds should be positive.");

        upTimeInformation.Milliseconds = 0;
        Assert.IsTrue(upTimeInformation.Milliseconds >= 0, "Milliseconds is positive.");

        upTimeInformation.Milliseconds = 1000 * 60 * 60 * 24;
        Assert.IsTrue(upTimeInformation.Milliseconds > 0, "Milliseconds is positive.");
    }

    /// <summary>
    /// Test Collect method multiple times to confirm Milliseconds increases between calls.
    /// </summary>
    [TestMethod]
    public void Test_Collect_MillisecondsIncreaseOverTime() {
        if (_upTimeCollector != null) {
            UpTimeInformation firstResult = (UpTimeInformation)_upTimeCollector.Collect();
            Thread.Sleep(100);
            UpTimeInformation secondResult = (UpTimeInformation)_upTimeCollector.Collect();

            Assert.IsTrue(secondResult.Milliseconds > firstResult.Milliseconds,
                "Milliseconds should increase over time.");
        }
    }
    */
}