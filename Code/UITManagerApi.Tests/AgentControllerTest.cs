using Moq;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using UITManagerApi.Controllers;
using UITManagerApi.Data;
using UITManagerApi.Hubs;
using UITManagerApi.Models;

namespace UITManagerApi.Tests {
    
    [TestClass]
    public class AgentControllerTest {
        private Mock<IHubContext<ApiHub>> _mockhub;
        private Mock<IClientProxy> _clientProxyMock; 
        private ApplicationDbContext _dbContext;
        AgentController agentController;

        [TestInitialize]
        public void Setup() {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new ApplicationDbContext(options);
            _mockhub = new Mock<IHubContext<ApiHub>>();
            _clientProxyMock = new Mock<IClientProxy>();
            
            
            var clientsMock = new Mock<IHubClients>();
            clientsMock.Setup(clients => clients.All).Returns(_clientProxyMock.Object);
            _mockhub.Setup(hub => hub.Clients).Returns(clientsMock.Object);
            
            agentController = new AgentController(_dbContext, _mockhub.Object);
        }

        /// <summary>
        /// Test method for verifying the behavior of the PostMachine method.
        /// </summary>
        /// <remarks>
        /// This test ensures that:
        /// - A machine is successfully added to the database.
        /// - SignalR is triggered to notify clients of the new machine.
        /// - The machine's properties match the expected values.
        /// </remarks>
        [TestMethod]
        public async Task PostMachine_ShouldUpdateMachineAndSendMessage()
        {

            var machineAgent = new MachineAgent
            {
                Name = "Test Machine",
                Model = "Test Model",
                Informations = new List<InformationAgent>
                {
                    new InformationAgent {
                        Name = "Test Information Agent",
                        Value = "Test Information Agent",
                        Format = "Test Information Agent",
                    }
                }
            };

            
            MachineAgent result = new MachineAgent();
            result = await agentController.PostMachine(machineAgent);

            Assert.IsNotNull(result);
            Assert.AreEqual("Test Machine", result.Name);
            Assert.AreEqual("Test Model", result.Model);

            Assert.IsNotNull(result);
            Assert.AreEqual("Test Machine", result.Name);
            Assert.AreEqual("Test Model", result.Model);

            var machineInDb = await _dbContext.Machines.FirstOrDefaultAsync();
            Assert.IsNotNull(machineInDb);
            Assert.AreEqual("Test Machine", machineInDb.Name);

            _clientProxyMock.Verify(client =>
                    client.SendCoreAsync(
                        "ReceiveMessage",
                        It.Is<object[]>(args => args[0].Equals(machineInDb.Id)),
                        default),
                Times.Once);
        }
    }
}