using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.Tests
{
    [TestClass]
    public class UserCollectorTests
    {
        private UserCollector userCollector;

        [TestInitialize]
        public void Setup()
        {
            // Initialisation avant chaque test
            userCollector = new UserCollector();
        }

        [TestMethod]
        public void Collect_ShouldReturnUsersInformationInstance()
        {
            // Act
            var result = userCollector.Collect();

            // Assert
            Assert.IsNotNull(result, "Le résultat ne doit pas être nul.");
            Assert.IsInstanceOfType(result, typeof(UsersInformation), "Le résultat doit être de type UsersInformation.");
        }

        [TestMethod]
        public void Collect_ShouldHandleExceptionsGracefully()
        {
            try
            {
                // Act
                var result = userCollector.Collect();

                // Assert
                Assert.IsNotNull(result, "Le résultat ne doit pas être nul même en cas d'exception.");
            }
            catch (Exception ex)
            {
                Assert.Fail($"La méthode Collect a levé une exception inattendue : {ex.Message}");
            }
        }

        [TestMethod]
        public void Collect_UsersListShouldNotBeNullOrEmpty_WhenUsersExist()
        {
            // Act
            var result = (UsersInformation)userCollector.Collect();

            // Assert
            Assert.IsNotNull(result.usersList, "La liste des utilisateurs ne doit pas être nulle.");
            Assert.IsTrue(result.usersList.Count >= 1, "La liste des utilisateurs doit contenir au moins 1 élément.");
        }

        [TestMethod]
        public void Collect_ShouldReturnNewInstanceOnEachCall()
        {
            // Act
            var firstResult = (UsersInformation)userCollector.Collect();
            var secondResult = (UsersInformation)userCollector.Collect();

            // Assert
            Assert.AreNotSame(firstResult, secondResult, "Chaque appel à Collect devrait renvoyer une nouvelle instance de UsersInformation.");
        }
    }
}
