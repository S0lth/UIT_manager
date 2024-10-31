using System.Runtime.InteropServices;
using UITManagerAgent.DataCollectors;

/// <summary>
///     Contains unit tests for the <see cref="UserCollector" /> class.
/// </summary>
[TestClass]
public class UserCollectorTests {
    private UserCollector userCollector;

    /// <summary>
    ///     Initializes a new instance of the <see cref="UserCollector" /> class before each test.
    /// </summary>
    [TestInitialize]
    public void Setup() {
        userCollector = new UserCollector();
    }

    /// <summary>
    ///     Tests if the <see cref="UserCollector.Collect" /> method returns an instance of <see cref="UsersInformation" />.
    /// </summary>
    [TestMethod]
    public void Collect_ShouldReturnUsersInformationInstance() {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            Assert.Inconclusive("This test only runs on Windows.");
            return;
        }

        Information result = userCollector.Collect();

        Assert.IsNotNull(result, "Result should not be null.");
        Assert.IsInstanceOfType(result, typeof(UsersInformation), "Result should be of type UsersInformation.");
    }

    /// <summary>
    ///     Tests if the <see cref="UserCollector.Collect" /> method handles exceptions gracefully.
    /// </summary>
    [TestMethod]
    public void Collect_ShouldHandleExceptionsGracefully() {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            Assert.Inconclusive("This test only runs on Windows.");
            return;
        }

        try {
            Information result = userCollector.Collect();

            Assert.IsNotNull(result, "Result should not be null even in case of an exception.");
        }
        catch (Exception ex) {
            Assert.Fail($"The Collect method threw an unexpected exception: {ex.Message}");
        }
    }

    /// <summary>
    ///     Tests if the <see cref="UserCollector.Collect" /> method returns a non-null and non-empty list of users when users
    ///     exist.
    /// </summary>
    [TestMethod]
    public void Collect_UsersListShouldNotBeNullOrEmpty_WhenUsersExist() {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            Assert.Inconclusive("This test only runs on Windows.");
            return;
        }

        UsersInformation result = (UsersInformation)userCollector.Collect();

        Assert.IsNotNull(result.GetUsersList(), "User list should not be null.");
        Assert.IsTrue(result.GetUsersList().Count >= 1, "User list should contain at least one item.");
    }

    /// <summary>
    ///     Tests if the <see cref="UserCollector.Collect" /> method returns a new instance of <see cref="UsersInformation" />
    ///     on each call.
    /// </summary>
    [TestMethod]
    public void Collect_ShouldReturnNewInstanceOnEachCall() {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            Assert.Inconclusive("This test only runs on Windows.");
            return;
        }

        UsersInformation firstResult = (UsersInformation)userCollector.Collect();
        UsersInformation secondResult = (UsersInformation)userCollector.Collect();

        Assert.AreNotSame(firstResult, secondResult,
            "Each call to Collect should return a new instance of UsersInformation.");
    }
}