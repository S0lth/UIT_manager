using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using UITManagerApi.Data;
using UITManagerApi.Hubs;
using UITManagerApi.Models;

namespace UITManagerApi.Controllers {
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class AgentController : ControllerBase {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ApiHub> _hubContext;

        public AgentController(ApplicationDbContext context, IHubContext<ApiHub> hubContext) {
            _context = context;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Retrieves a specific machine along with its related information based on the provided ID.
        /// </summary>
        /// <param name="id">The unique identifier of the machine to retrieve.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing the machine and its related information if found, or a 404 status if not found.
        /// </returns>
        // GET: api/Agent/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Machine>> GetMachine(int id) {
            // Récupère la machine avec ses informations liées
            var machine = await _context.Machines
                .Include(m => m.Informations)
                .Where(m => m.Id == id).FirstOrDefaultAsync();

            Console.WriteLine(machine);

            if (machine == null) {
                return NotFound();
            }

            return Ok(machine);
        }

        /// <summary>
        /// Creates or updates a machine and sends a notification via SignalR.
        /// </summary>
        /// <param name="machineAgent">The <see cref="MachineAgent"/> object containing the machine details and related information.</param>
        /// <returns>
        /// The <see cref="MachineAgent"/> object that was processed.
        /// </returns>
        /// <remarks>
        /// - If the machine already exists in the database, its information is updated.
        /// - If the machine does not exist, a new entry is created.
        /// - After saving changes, a SignalR notification is sent to all clients.
        /// </remarks>
        // POST: api/Agent
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<MachineAgent> PostMachine(MachineAgent machineAgent) {
            
            var machineDb = await _context.Machines
                .Include(m => m.Informations)
                .FirstOrDefaultAsync(m => m.Name == machineAgent.Name);
            
            if (machineDb != null) {
                machineDb.Informations = new List<Information>();
                foreach (var info in machineAgent.Informations) {
                    ProcessInformationAgent(info, machineDb, machineDb.Informations);
                }

                machineDb.LastSeen = DateTime.UtcNow;
                _context.Machines.Update(machineDb);
            }
            else {
                machineDb = new Machine {
                    Name = machineAgent.Name,
                    Model = machineAgent.Model,
                    IsWorking = true,
                    LastSeen = DateTime.UtcNow,
                    Informations = new List<Information>()
                };

                foreach (var info in machineAgent.Informations) {
                    ProcessInformationAgent(info, machineDb, machineDb.Informations);
                }

                _context.Machines.Add(machineDb);
            }

            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", machineDb.Id);
            return machineAgent;
        }
        
        /// <summary>
        /// Processes an <see cref="InformationAgent"/> and adds it to the provided parent collection.
        /// </summary>
        /// <param name="info">The <see cref="InformationAgent"/> to process.</param>
        /// <param name="machine">The <see cref="Machine"/> to which the information is related.</param>
        /// <param name="parentCollection">
        /// A collection of <see cref="Information"/> to which the processed information or component will be added.
        /// </param>
        /// <remarks>
        /// - If the <paramref name="info"/> has child information, it is processed recursively as a component.
        /// - Otherwise, it is added as a value to the parent collection.
        /// </remarks>
        private void ProcessInformationAgent(InformationAgent info, Machine machine, ICollection<Information> parentCollection) {
            
            if (info.InformationAgents == null) {
                parentCollection.Add(new Value {
                    Machine = machine, Name = info.Name, Value = info.Value, Format = info.Format
                });
            }
            else {
                var component = new Component {
                    Machine = machine, Name = info.Name, Value = info.Value, Format = info.Format
                };

                foreach (var childInfo in info.InformationAgents) {
                    ProcessInformationAgent(childInfo, machine, component.Children!);
                }

                parentCollection.Add(component);
            }
        }
    }
}