using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using UITManagerApi.Controllers;
using UITManagerApi.Data;
using UITManagerApi.Hubs;
using UITManagerApi.Models;

namespace UITManagerApi.Tests {
    
    [TestClass]
    public class AgentControllerTest {
        private Mock<IHubContext<ApiHub>> _mockhub;
            
        private ApplicationDbContext _dbContext;
        AgentController agentController;

        [TestInitialize]
        public void Setup() {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new ApplicationDbContext(options);
            _mockhub = new Mock<IHubContext<ApiHub>>();
            agentController = new AgentController(_dbContext, _mockhub.Object);
        }

        [TestMethod]
        public async Task PostMachine_ShouldUpdateMachineAndSendMessage()
        {
            // Arrange
           

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
            result= await agentController.PostMachine(machineAgent);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test Machine", result.Name);
            Assert.AreEqual("Test Model", result.Model);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test Machine", result.Name);
            Assert.AreEqual("Test Model", result.Model);

            // Vérifiez que la machine a été ajoutée à la base de données
            var machineInDb = await _dbContext.Machines.FirstOrDefaultAsync();
            Assert.IsNotNull(machineInDb);
            Assert.AreEqual("Test Machine", machineInDb.Name);

            // Vérifiez que le SignalR a été appelé
            _mockhub.Verify(hub =>
               hub.Clients.All.SendAsync("ReceiveMessage", machineInDb.Id, default), Times.Once);
        }
    }
}