using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.Tests.BasicInformation;

/// <summary>
/// This class contains the Unit Tests for method <see cref="UsersInformation.ToJson"/>
/// </summary>
[TestClass]
public class UsersInformationTest {

    private UsersInformation? _usersInformation;
    /// <summary>
    /// Initialize a new instance of the <see cref="UsersInformation"/> class before each test.
    /// </summary>
    [TestInitialize]
    public void Setup() {
        _usersInformation = new UsersInformation();
    }

    /// <summary>
    /// Test method to check if <see cref="UsersInformation.ToJson"/>
    /// return a correct JSON format with manually set IPS
    /// </summary>
    [TestMethod]
    public void ToJson_ShouldReturnValidJson_WhenIPSAddressesAreManuallySet() {
        if (_usersInformation != null) {
            _usersInformation.UsersList = new() {
            new UsersInformation.User("admin", "local"),
            new UsersInformation.User("guest", "local"),
            new UsersInformation.User("bob", "local"),
            };

            List<UsersInformation.User> usersList = _usersInformation.UsersList;
            string expectedJson = $"{{\"UsersList\":[{usersList[0].ToJson()},{usersList[1].ToJson()},{usersList[2].ToJson()}]}}";

            Assert.AreEqual(expectedJson, _usersInformation.ToJson());
        }
        else {
            Assert.Fail();
        }
    }

    /// <summary>
    /// Test method to check if <see cref="UsersInformation.ToJson"/>
    /// return a json with no values if UsersList is empty
    /// </summary>
    [TestMethod]
    public void ToJson_ShouldReturnValidJson_WhenIPSAddressesAreEmpty() {
        string expectedJson = "{\"UsersList\":[]}";
        if (_usersInformation != null) {
            _usersInformation.UsersList = new();
            Assert.AreEqual(expectedJson, _usersInformation.ToJson());
        }
        else {
            Assert.Fail();
        }
    }
}
