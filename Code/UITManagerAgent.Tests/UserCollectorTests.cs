using UITManagerAgent.DataCollectors;
using System.Runtime.InteropServices;

namespace UITManagerAgent.Tests
{
    [TestClass]
    public class UserCollectorTests
    {
        private UserCollector userCollector;

        [TestInitialize]
        public void Setup()
        {
            userCollector = new UserCollector();
        }

        [TestMethod]
        public void Collect_ShouldReturnUsersInformationInstance()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.Inconclusive("Ce test s'exécute uniquement sur Windows.");
                return;
            }

            var result = userCollector.Collect();

            Assert.IsNotNull(result, "Le résultat ne doit pas être nul.");
            Assert.IsInstanceOfType(result, typeof(UsersInformation), "Le résultat doit être de type UsersInformation.");
        }

        [TestMethod]
        public void Collect_ShouldHandleExceptionsGracefully()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.Inconclusive("Ce test s'exécute uniquement sur Windows.");
                return;
            }

            try
            {
                var result = userCollector.Collect();

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
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.Inconclusive("Ce test s'exécute uniquement sur Windows.");
                return;
            }

            var result = (UsersInformation)userCollector.Collect();

            Assert.IsNotNull(result.GetUsersList(), "La liste des utilisateurs ne doit pas être nulle.");
            Assert.IsTrue(result.GetUsersList().Count >= 1, "La liste des utilisateurs doit contenir au moins 1 élément.");
        }

        [TestMethod]
        public void Collect_ShouldReturnNewInstanceOnEachCall()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.Inconclusive("Ce test s'exécute uniquement sur Windows.");
                return;
            }

            var firstResult = (UsersInformation)userCollector.Collect();
            var secondResult = (UsersInformation)userCollector.Collect();

            Assert.AreNotSame(firstResult, secondResult, "Chaque appel à Collect devrait renvoyer une nouvelle instance de UsersInformation.");
        }
    }
}
