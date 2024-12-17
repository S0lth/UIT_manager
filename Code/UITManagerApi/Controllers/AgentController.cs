using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using UITManagerApi.Data;
using UITManagerApi.Hubs;
using UITManagerApi.Models;

namespace UITManagerApi.Controllers
{
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AgentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ApiHub> _hubContext;

        public AgentController(ApplicationDbContext context,IHubContext<ApiHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: api/Agent
        [HttpGet]
        public async Task<List<Machine>> GetMachines() {
            //return await _context.Components.ToListAsync();
            return await _context.Machines.Include(m => m.Informations).ThenInclude(c => c.Children).ToListAsync();
        }

        // GET: api/Agent/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Machine>> GetMachine(int id)
        {
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
        
        private List<InformationAgent> MapInformations(ICollection<Information> informations)
        {
            var uniqueIds = new HashSet<int>(); // Pour traquer les IDs uniques
        
            return informations
                .Where(info => uniqueIds.Add(info.Id)) // Ajoute au HashSet et filtre les doublons par ID
                .Select(info => new InformationAgent
                {
                    Name = info.Name,
                    Value = info is Value value ? value.Value : null,
                    Format = info.Format,
                    InformationAgents = info is Component component
                        ? MapInformations(component.Children)
                        : null
                }).ToList();
        }

        // PUT: api/Agent/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMachine(int id, Machine machine)
        {
            if (id != machine.Id)
            {
                return BadRequest();
            }

            _context.Entry(machine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MachineExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Agent
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MachineAgent>> PostMachine(MachineAgent machineAgent) {
            
            
            Machine machine = new Machine {
                Name = machineAgent.Name, Model = machineAgent.Model, IsWorking = true, LastSeen = DateTime.UtcNow
            };
            
            foreach (var info in machineAgent.Informations) {
                ProcessInformationAgent(info, machine, machine.Informations);
            }
            
            _context.Machines.Update(machine);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", machine.Id);
            return CreatedAtAction("GetMachine", new { id = machine.Id }, machine);
        }
        
        private void ProcessInformationAgent(InformationAgent info, Machine machine, ICollection<Information> parentCollection) {
            
            if (info.InformationAgents == null) {
                parentCollection.Add(new Value {
                    Machine = machine,
                    Name = info.Name,
                    Value = info.Value,
                    Format = info.Format
                });
            } else {
                var component = new Component {
                    Machine = machine,
                    Name = info.Name,
                    Value = info.Value,
                    Format = info.Format
                };
        
                foreach (var childInfo in info.InformationAgents) {
                    ProcessInformationAgent(childInfo, machine, component.Children);
                }
        
                parentCollection.Add(component);
            }
        }
        

        // DELETE: api/Agent/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMachine(int id)
        {
            var machine = await _context.Machines.FindAsync(id);
            if (machine == null)
            {
                return NotFound();
            }

            _context.Machines.Remove(machine);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MachineExists(int id)
        {
            return _context.Machines.Any(e => e.Id == id);
        }
    }
}
