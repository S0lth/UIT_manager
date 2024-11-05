using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.Tests.DataCollectors;

/// <summary>
///     Contains unit tests for the <see cref="UserCollector" /> class.
/// </summary>
[TestClass]
public class UserCollectorTests {
    private UserCollector? _userCollector;

    /// <summary>
    ///     Initializes a new instance of the <see cref="UserCollector" /> class before each test.
    /// </summary>
    [TestInitialize]
    public void Setup() {
        _userCollector = new UserCollector();
    }

    /// <summary>
    ///     Tests if the <see cref="UserCollector.Collect" /> method returns an instance of <see cref="UsersInformation" />.
    /// </summary>
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public void Collect_ShouldReturnUsersInformationInstance() {
        if (_userCollector != null) {
            Information result = _userCollector.Collect();

            Assert.IsNotNull(result, "Result should not be null.");
            Assert.IsInstanceOfType(result, typeof(UsersInformation), "Result should be of type UsersInformation.");
        }
    }

    /// <summary>
    ///     Tests if the <see cref="UserCollector.Collect" /> method handles exceptions gracefully.
    /// </summary>
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public void Collect_ShouldHandleExceptionsGracefully() {
        try {
            if (_userCollector != null) {
                Information result = _userCollector.Collect();

                Assert.IsNotNull(result, "Result should not be null even in case of an exception.");
            }
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
    [SupportedOSPlatform("windows")]
    public void Collect_UsersListShouldNotBeNullOrEmpty_WhenUsersExist() {
        if (_userCollector != null) {
            UsersInformation result = (UsersInformation)_userCollector.Collect();

            Assert.IsNotNull(result.usersList, "User list should not be null.");
            Assert.IsTrue(result.usersList.Count >= 1, "User list should contain at least one item.");
        }
    }

    /// <summary>
    ///     Tests if the <see cref="UserCollector.Collect" /> method returns a new instance of <see cref="UsersInformation" />
    ///     on each call.
    /// </summary>
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public void Collect_ShouldReturnNewInstanceOnEachCall() {
        if (_userCollector != null) {
            UsersInformation firstResult = (UsersInformation)_userCollector.Collect();
            UsersInformation secondResult = (UsersInformation)_userCollector.Collect();

            Assert.AreNotSame(firstResult, secondResult,
                "Each call to Collect should return a new instance of UsersInformation.");
        }
    }
}