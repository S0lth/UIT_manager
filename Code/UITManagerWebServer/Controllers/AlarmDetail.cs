using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using UITManagerWebServer.Data;
using UITManagerWebServer.Models;

namespace UITManagerWebServer
{
    public class AlarmDetail : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AlarmDetail( UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;

        }

        public async Task<IActionResult> Index(string SortOrder, int id,  string solutionFilter)
        {
            ViewData["SortOrder"] = SortOrder;
            ViewData["date"] = SortOrder == "date" ? "date_desc" : "date";
            ViewData["Note"] = SortOrder == "note" ? "note_desc" : "note";
            ViewData["SolutionFilter"] = solutionFilter;

            var users = _context.Users.ToList();
            
            
            
            
            var alarm = await _context.Alarms
                .Include(a => a.Machine)
                .Include(a => a.NormGroup)
                .Include(a => a.AlarmHistories)
                .ThenInclude(ah => ah.User) 
                // Inclure les utilisateurs associés aux AlarmHistories
                .Include(a => a.User)
                .Include(a => a.NormGroup.SeverityHistories) // Include SeverityHistories
                .ThenInclude(sh => sh.Severity)// Si l'alarme a un utilisateur directement lié
                .FirstOrDefaultAsync(a => a.Id == id);

            
            var notes = _context.Notes
                .Include(note => note.Machine) // Inclut les machines associées
                .Include(note => note.Author)  // Inclut les informations de l'auteur, si applicable
                .ToList();

            
            notes = SortOrder switch
            {
                "date" => notes.OrderBy(n => n.CreatedAt).ToList(),
                "date_desc" => notes.OrderByDescending(n => n.CreatedAt).ToList(),
                "note" => notes.OrderBy(n => n.Title).ToList(),
                "note_desc" => notes.OrderByDescending(n => n.Title).ToList(),
                _ => notes
            };
            
            notes = solutionFilter switch
            {
                "true" => notes.Where(n => n.IsSolution).ToList(),
                "false" => notes.Where(n => !n.IsSolution).ToList(),
                _ => notes
            };
            
            var machine = await _context.Machines
                .FirstOrDefaultAsync(m => m.Id == id);
            
            var information = await getMachineInformation(id);
            var authors = ViewBag.Authors = await _context.Users.ToListAsync();
            var detailView = new DetailsViewModel {
                Id = machine.Id,
                Name = machine.Name, 
                LastSeen = machine.LastSeen, 
                Model = machine.Model,
                IsWorking = machine.IsWorking,
                //Notes = notes,
                //Alarms = alarm,
                Authors = authors ?? new ApplicationUser(),
                Informations = information,
            };
            
            if (alarm == null)
            {
                return NotFound();
            }
            ViewData["AlarmStatusTypes"] = await _context.AlarmStatusTypes.ToListAsync();
            ViewData["user"] = users;
            ViewData["Machine"] = machine;
            ViewData["Notes"] = notes;

            return View(alarm);
        }

   
        [HttpPost]
        [Authorize(Roles = "Technician , ITDirector, MaintenanceManager")]
        [Route("AlarmDetail/UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            if (request == null || request.Id == 0 || string.IsNullOrEmpty(request.Status))
            {
                return BadRequest(new { success = false, message = "Invalid data." });
            }

            var alarm = await _context.Alarms
                .Include(a => a.AlarmHistories)
                .FirstOrDefaultAsync(a => a.Id == request.Id);

            if (alarm == null)
            {
                return NotFound(new { success = false, message = "Alarm not found." });
            }

            var statusType = await _context.AlarmStatusTypes.FirstOrDefaultAsync(s => s.Name == request.Status);
            if (statusType == null)
            {
                return BadRequest(new { success = false, message = "Invalid status." });
            }

            // Obtenir l'ID utilisateur
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Créer un nouvel historique d'alarme
            var newAlarmHistory = new AlarmStatusHistory
            {
                StatusTypeId = statusType.Id,
                ModificationDate = DateTime.UtcNow,
                UserId = userId // Peut être null
            };

            alarm.AlarmHistories.Add(newAlarmHistory);

            try
            {
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Status updated successfully." });
            }
            catch (Exception ex)
            {
                // Log en cas d'exception
                // Exemple : _logger.LogError(ex, "Error updating alarm status.");
                return StatusCode(500, new { success = false, message = "An error occurred while updating status." });
            }
        }


        [HttpPost]
        [Authorize(Roles = "ITDirector, MaintenanceManager")]
        [Route("AlarmDetail/Attribution")]
        public async Task<IActionResult> UpdateAttribution([FromBody] UpdateAssignedUserRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Id) || string.IsNullOrEmpty(request.UserId))
            {
                return BadRequest(new { success = false, message = "Invalid request data." });
            }

            try
            {
                // Récupérer l'alarme correspondant à l'ID
                var alarm = await _context.Alarms
                    .Include(a => a.Machine)
                    .Include(a => a.NormGroup)
                    .FirstOrDefaultAsync(a => a.Id.ToString() == request.Id);

                if (alarm == null)
                {
                    return NotFound(new { success = false, message = "Alarm not found." });
                }

                // Vérifier si l'utilisateur existe
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "User not found." });
                }

                // Mettre à jour l'utilisateur associé à l'alarme
                alarm.UserId = request.UserId;

                // Ajouter un historique de statut pour refléter le changement d'attribution
                var alarmStatusHistory = new AlarmStatusHistory
                {
                    AlarmId = alarm.Id,
                    StatusType = await _context.AlarmStatusTypes.FirstOrDefaultAsync(s => s.Name == "In Progress"), // Exemple de statut
                    ModificationDate = DateTime.UtcNow,
                    UserId = request.UserId
                };

                _context.AlarmHistories.Add(alarmStatusHistory);

                // Enregistrer les modifications
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Alarm attribution updated successfully." });
            }
            catch (Exception ex)
            {
                // Gestion des erreurs
                return StatusCode(500, new { success = false, message = "An error occurred while updating attribution.", error = ex.Message });
            }
        }
        
        
        
        
        
        
        
        
        
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alarm = await _context.Alarms
                .Include(a => a.Machine)
                .Include(a => a.NormGroup)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (alarm == null)
            {
                return NotFound();
            }

            return View(alarm);
        }

        // GET: AlarmDetail/Create
        public IActionResult Create()
        {
            ViewData["MachineId"] = new SelectList(_context.Machines, "Id", "Id");
            ViewData["NormGroupId"] = new SelectList(_context.NormGroups, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: AlarmDetail/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TriggeredAt,MachineId,NormGroupId,UserId")] Alarm alarm)
        {
            if (ModelState.IsValid)
            {
                _context.Add(alarm);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MachineId"] = new SelectList(_context.Machines, "Id", "Id", alarm.MachineId);
            ViewData["NormGroupId"] = new SelectList(_context.NormGroups, "Id", "Id", alarm.NormGroupId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", alarm.UserId);
            return View(alarm);
        }
        
        
        
        

        // GET: AlarmDetail/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alarm = await _context.Alarms.FindAsync(id);
            if (alarm == null)
            {
                return NotFound();
            }

            ViewData["MachineId"] = new SelectList(_context.Machines, "Id", "Id", alarm.MachineId);
            ViewData["NormGroupId"] = new SelectList(_context.NormGroups, "Id", "Id", alarm.NormGroupId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", alarm.UserId);
            return View(alarm);
        }

        // POST: AlarmDetail/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TriggeredAt,MachineId,NormGroupId,UserId")] Alarm alarm)
        {
            if (id != alarm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(alarm);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlarmExists(alarm.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["MachineId"] = new SelectList(_context.Machines, "Id", "Id", alarm.MachineId);
            ViewData["NormGroupId"] = new SelectList(_context.NormGroups, "Id", "Id", alarm.NormGroupId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", alarm.UserId);
            return View(alarm);
        }

        // GET: AlarmDetail/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alarm = await _context.Alarms
                .Include(a => a.Machine)
                .Include(a => a.NormGroup)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (alarm == null)
            {
                return NotFound();
            }

            return View(alarm);
        }

        // POST: AlarmDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alarm = await _context.Alarms.FindAsync(id);
            if (alarm != null)
            {
                _context.Alarms.Remove(alarm);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlarmExists(int id)
        {
            return _context.Alarms.Any(e => e.Id == id);
        }
        
        
        
        
        
        
                private async Task<List<ComponentsViewModel>> getMachineInformation(int? id) {
            var list = _context.Components.Where(a => a.MachinesId == id).AsQueryable();
            var result = new List<ComponentsViewModel>();

            // Parcourir la liste pour trouver les éléments sans parent (racines)
            var rootElements = await list.Where(e => e.ParentId == null).ToListAsync();
            var model = rootElements.Select(
                a => new ComponentsViewModel {
                MachineId = a.Machine.Id,
                ParentId = a.ParentId,
                Name = a.Name,
                id = a.Id,
                Value = a.Values,
                Children = new List<ComponentsViewModel>()
            }).ToList();

            foreach (var root in model)
            {
                var hierarchy = await BuildHierarchy(root, list.ToList());
                result.Add(hierarchy);
            }

            return result;
        }
        
        private async Task<ComponentsViewModel> BuildHierarchy(ComponentsViewModel parent, List<Informations> list)
        {
            var children = list.Where(e => e.ParentId == parent.id).ToList();

            var parentViewModel = new ComponentsViewModel
            {
                MachineId = parent.MachineId,
                ParentId = parent.ParentId,
                id = parent.id,
                Name = parent.Name,
                Value = parent.Value,
                Children = new List<ComponentsViewModel>()
            };

            // Ajouter les enfants comme sous-éléments
            foreach (var child in children) {
                var info = new ComponentsViewModel {
                    MachineId = child.Machine.Id,
                    ParentId = child.ParentId,
                    Name = child.Name,
                    id = child.Id,
                    Value = child.Values,
                    Children = new List<ComponentsViewModel>()
                };
                var childHierarchy = await BuildHierarchy(info, list);
                parentViewModel.Children.Add(childHierarchy);
            }

            return parentViewModel;
        }

        
        
    }
    
    
    
    
    
    
    public class AlarmViewModel {
        public int MachineId { get; set; }
        public int AlarmId { get; set; }
        public string Status { get; set; }
        public string Severity { get; set; }
        public string AlarmGroupName { get; set; }
        public DateTime TriggeredAt { get; set; }
    }

    // ViewModel pour les notes
    public class NoteViewModel {
        public string Author { get; set; }
        public int MachineId { get; set; }
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool IsSolution { get; set; }
        public string Title { get; set; }
    }

    // viewModel pour les information d'une machine
    public class ComponentsViewModel {
        public int MachineId { get; set; }
        public int? ParentId { get; set; }
        public int id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public List<ComponentsViewModel> Children { get; set; }
    }

    public class DetailsViewModel {
        public int? Id { get; set; }
        public List<AlarmViewModel> Alarms { get; set; }
        public List<NoteViewModel> Notes { get; set; }
        public List<ComponentsViewModel> Informations { get; set; }
        public List<ApplicationUser> Authors { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public bool IsWorking { get; set; }
        public DateTime? LastSeen { get; set; }
            
    }
    
    public class UpdateStatusRequest
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }

    public class UpdateAssignedUserRequest
    {
        public string Id { get; set; } // ID de l'alarme
        public string UserId { get; set; } // ID de l'utilisateurD du nouvel utilisateur attribué
    }
    
}
