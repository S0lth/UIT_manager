using Asp.Versioning;
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
    public class AgentController : ControllerBase {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ApiHub> _hubContext;

        public AgentController(ApplicationDbContext context, IHubContext<ApiHub> hubContext) {
            _context = context;
            _hubContext = hubContext;
        }

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

        // POST: api/Agent
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MachineAgent>> PostMachine(MachineAgent machineAgent) {
            
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

            return CreatedAtAction("GetMachine", new { id = machineDb.Id }, machineAgent);
        }

        private void ProcessInformationAgent(InformationAgent info, Machine machine,
            ICollection<Information> parentCollection) {
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