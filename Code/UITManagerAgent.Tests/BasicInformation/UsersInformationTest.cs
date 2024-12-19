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
            _usersInformation.InformationAgents = new() {
            };

            List<InnerValue> usersList = _usersInformation.InformationAgents;
            string expectedJson = "{\"Name\": \"Users List\",\"Value\": \"null\",\"Format\": \"null\",\"InformationAgents\": []}";

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
        string expectedJson = "{\"Name\": \"Users List\",\"Value\": \"null\",\"Format\": \"null\",\"InformationAgents\": []}";
        if (_usersInformation != null) {
            _usersInformation.InformationAgents = new();
            Assert.AreEqual(expectedJson, _usersInformation.ToJson());
        }
        else {
            Assert.Fail();
        }
    }
}
