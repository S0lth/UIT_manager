using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UITManagerWebServer.Data;
using UITManagerWebServer.Models;
using UITManagerWebServer.Models.ModelsView;

namespace UITManagerWebServer {
    public class AlarmDetail : Controller {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AlarmDetail(UserManager<ApplicationUser> userManager, ApplicationDbContext context) {
            _context = context;
            _userManager = userManager;
        }

        public override void OnActionExecuting(ActionExecutingContext context) {
            base.OnActionExecuting(context);

            TempData["PreviousUrl"] = Request.Headers["Referer"].ToString();
        }

        public async Task<IActionResult> Index(string sortOrder, int id, string solutionFilter, string authorFilter,
            string sortOrderNote) {
            if (string.IsNullOrEmpty(sortOrder)) {
                sortOrder = "date";
            }

            ViewData["SortOrder"] = sortOrder;
            ViewData["SolutionFilter"] = solutionFilter;
            ViewData["AuthorFilter"] = authorFilter;
            ViewData["SortOrderNote"] = sortOrderNote;

            ViewData["NoteSortParm"] = sortOrder.Contains("note_desc") ? "note" : "note_desc";
            ViewData["DateSortParm"] = sortOrder.Contains("date_desc") ? "date" : "date_desc";
            ViewData["TypeSortParm"] = sortOrder.Contains("type_desc") ? "type" : "type_desc";
            ViewData["AuthorSortParm"] = sortOrder.Contains("author_desc") ? "author" : "author_desc";

            var alarm = await getAlarm(id);
            if (alarm == null) {
                return NotFound();
            }

            var alarmCount = await _context.Alarms
                .Where(a => a.MachineId == alarm.MachineId && a.NormGroupId == alarm.MachineId)
                .CountAsync();

            var alarmCountAll = await _context.Alarms
                .Where(a => a.NormGroupId == alarm.NormGroupId)
                .CountAsync();

            ViewData["AlarmStatusTypes"] = await _context.AlarmStatusTypes.ToListAsync();
            ViewData["Alarm"] = alarm;
            ViewData["user"] = _context.Users.ToList();
            ViewData["AlarmCount"] = alarmCount;
            ViewData["AlarmCountAll"] = alarmCountAll;


            var notes = await FetchFilteredNotes(solutionFilter, authorFilter, sortOrderNote);

            notes = ApplySorting(notes.AsQueryable(), sortOrder).ToList();
            ViewData["Notes"] = notes;


            ViewData["Authors"] = ViewBag.Authors = await _context.Users.ToListAsync();

            var triggeredInfoList = new List<dynamic>();
            foreach (var norm in alarm.NormGroup.Norms) {
                if (norm.InformationName != null) {
                    var infoName = norm.InformationName.Name;
                    var machineValue = alarm.Machine.GetInformationValueByName(infoName);

                    if (!string.IsNullOrEmpty(machineValue)) {
                        var triggeredInfo = new {
                            InfoName = infoName,
                            MachineValue = machineValue,
                            NormValue = norm.Value,
                            Condition = norm.Condition,
                            Format = norm.Format
                        };
                        triggeredInfoList.Add(triggeredInfo);
                    }
                }
            }

            Console.WriteLine(triggeredInfoList.ToArray());

            ViewData["TriggeredInfoValue"] = triggeredInfoList;

            return View(ViewData.Model);
        }

        private IQueryable<Note> ApplySorting(IQueryable<Note> query, string sortOrder) {
            switch (sortOrder) {
                case "note_desc":
                    return query.OrderByDescending(n => n.Title);
                case "note":
                    return query.OrderBy(n => n.Title);
                case "date_desc":
                    return query.OrderByDescending(n => n.CreatedAt);
                case "date":
                    return query.OrderBy(n => n.CreatedAt);
                case "type_desc":
                    return query.OrderByDescending(n => n.IsSolution);
                case "type":
                    return query.OrderBy(n => n.IsSolution);
                case "author_desc":
                    return query.OrderByDescending(n => n.Author.LastName);
                case "author":
                    return query.OrderBy(n => n.Author.LastName);
                default:
                    return query.OrderByDescending(n => n.CreatedAt);
            }
        }


        public List<Information> FindMatchingInformations(int machineId, string informationName) {
            var machine = _context.Machines
                .Include(m => m.Informations) // Charger les informations de la machine
                .FirstOrDefault(m => m.Id == machineId);

            if (machine == null) {
                return new List<Information>();
            }

            var matchingInformations = machine.Informations
                .SelectMany(info => info.Children)
                .Where(child =>
                    child.Name.Equals(informationName, StringComparison.OrdinalIgnoreCase)) // Filtrer par nom
                .ToList();

            return matchingInformations;
        }


        private async Task<List<Note>> FetchFilteredNotes(string solutionFilter, string authorFilter,
            string sortOrderNote) {
            var notesQuery = _context.Notes.Include(n => n.Author).AsQueryable();

            // Filtrer par type de solution si nécessaire
            if (!string.IsNullOrEmpty(solutionFilter) && solutionFilter != "all") {
                bool isSolution = solutionFilter.ToLower() == "true";
                notesQuery = notesQuery.Where(n => n.IsSolution == isSolution);
            }

            // Appliquer le tri des notes selon l'ordre spécifié
            if (sortOrderNote == "ndate_desc") {
                notesQuery = notesQuery.OrderByDescending(n => n.CreatedAt);
            }
            else if (sortOrderNote == "ndate") {
                notesQuery = notesQuery.OrderBy(n => n.CreatedAt);
            }

            var notes = await notesQuery.ToListAsync();

            // Filtrer par auteur si nécessaire
            if (!string.IsNullOrEmpty(authorFilter)) {
                notes = notes.Where(n => n.AuthorId == authorFilter).ToList();
            }

            return notes;
        }

        private async Task<Alarm> getAlarm(int id) {
            var alarm = await _context.Alarms
                .Include(a => a.Machine)
                .ThenInclude(aa => aa.Informations)
                .ThenInclude(i => i.Children)
                .Include(a => a.NormGroup)
                .ThenInclude(ng => ng.Norms)
                .ThenInclude(norm => norm.InformationName)
                .ThenInclude(infoName => infoName.SubInformationNames)
                .Include(a => a.User)
                .Include(a => a.AlarmHistories.OrderByDescending(b => b.ModificationDate))
                .ThenInclude(aStatus => aStatus.StatusType)
                .Include(a => a.NormGroup.SeverityHistories)
                .ThenInclude(sh => sh.Severity)
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();

            return alarm;
        }


        [HttpPost]
        [Authorize(Roles = "Technician , ITDirector, MaintenanceManager")]
        [Route("AlarmDetail/UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request) {
            if (request == null || request.Id == 0 || string.IsNullOrEmpty(request.Status)) {
                return BadRequest(new { success = false, message = "Invalid data." });
            }

            var alarm = await _context.Alarms
                .Include(a => a.AlarmHistories)
                .FirstOrDefaultAsync(a => a.Id == request.Id);

            if (alarm == null) {
                return NotFound(new { success = false, message = "Alarm not found." });
            }

            var statusType = await _context.AlarmStatusTypes.FirstOrDefaultAsync(s => s.Name == request.Status);
            if (statusType == null) {
                return BadRequest(new { success = false, message = "Invalid status." });
            }

            // Obtenir l'ID utilisateur
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Créer un nouvel historique d'alarme
            var newAlarmHistory = new AlarmStatusHistory {
                StatusTypeId = statusType.Id, ModificationDate = DateTime.UtcNow, UserId = userId // Peut être null
            };

            alarm.AlarmHistories.Add(newAlarmHistory);

            try {
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Status updated successfully." });
            }
            catch (Exception ex) {
                // Log en cas d'exception
                // Exemple : _logger.LogError(ex, "Error updating alarm status.");
                return StatusCode(500, new { success = false, message = "An error occurred while updating status." });
            }
        }


        [HttpPost]
        [Authorize(Roles = "ITDirector, MaintenanceManager")]
        [Route("AlarmDetail/Attribution")]
        public async Task<IActionResult> UpdateAttribution([FromBody] UpdateAssignedUserRequest request) {
            if (request == null || string.IsNullOrEmpty(request.Id) || string.IsNullOrEmpty(request.UserId)) {
                return BadRequest(new { success = false, message = "Invalid request data." });
            }

            try {
                // Récupérer l'alarme correspondant à l'ID
                var alarm = await _context.Alarms
                    .Include(a => a.Machine)
                    .Include(a => a.NormGroup)
                    .FirstOrDefaultAsync(a => a.Id.ToString() == request.Id);

                if (alarm == null) {
                    return NotFound(new { success = false, message = "Alarm not found." });
                }

                // Vérifier si l'utilisateur existe
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null) {
                    return NotFound(new { success = false, message = "User not found." });
                }

                // Mettre à jour l'utilisateur associé à l'alarme
                alarm.UserId = request.UserId;

                // Ajouter un historique de statut pour refléter le changement d'attribution
                var alarmStatusHistory = new AlarmStatusHistory {
                    AlarmId = alarm.Id,
                    StatusType =
                        await _context.AlarmStatusTypes.FirstOrDefaultAsync(s =>
                            s.Name == "In Progress"), // Exemple de statut
                    ModificationDate = DateTime.UtcNow,
                    UserId = request.UserId
                };

                _context.AlarmHistories.Add(alarmStatusHistory);

                // Enregistrer les modifications
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Alarm attribution updated successfully." });
            }
            catch (Exception ex) {
                // Gestion des erreurs
                return StatusCode(500,
                    new {
                        success = false, message = "An error occurred while updating attribution.", error = ex.Message
                    });
            }
        }


        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            var alarm = await _context.Alarms
                .Include(a => a.Machine)
                .Include(a => a.NormGroup)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (alarm == null) {
                return NotFound();
            }

            return View(alarm);
        }

        // GET: AlarmDetail/Create
        public IActionResult Create() {
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
        public async Task<IActionResult> Create([Bind("Id,TriggeredAt,MachineId,NormGroupId,UserId")] Alarm alarm) {
            if (ModelState.IsValid) {
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
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var alarm = await _context.Alarms.FindAsync(id);
            if (alarm == null) {
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
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,TriggeredAt,MachineId,NormGroupId,UserId")]
            Alarm alarm) {
            if (id != alarm.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(alarm);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) {
                    if (!AlarmExists(alarm.Id)) {
                        return NotFound();
                    }
                    else {
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
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var alarm = await _context.Alarms
                .Include(a => a.Machine)
                .Include(a => a.NormGroup)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (alarm == null) {
                return NotFound();
            }

            return View(alarm);
        }

        // POST: AlarmDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var alarm = await _context.Alarms.FindAsync(id);
            if (alarm != null) {
                _context.Alarms.Remove(alarm);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlarmExists(int id) {
            return _context.Alarms.Any(e => e.Id == id);
        }


        private IQueryable<Alarm> ApplySorting(IQueryable<Alarm> query, string sortOrder) {
            if (string.IsNullOrEmpty(sortOrder)) {
                return query.OrderByDescending(a => a.TriggeredAt);
            }

            switch (sortOrder.ToLower()) {
                case "machine_desc":
                    return query.OrderByDescending(a => a.Machine.Name);
                case "machine":
                    return query.OrderBy(a => a.Machine.Name);
                case "model_desc":
                    return query.OrderByDescending(a => a.Machine.Model);
                case "model":
                    return query.OrderBy(a => a.Machine.Model);
                case "status_desc":
                    return query.OrderByDescending(a => a.AlarmHistories
                        .OrderByDescending(h => h.ModificationDate)
                        .FirstOrDefault().StatusType.Name);
                case "status":
                    return query.OrderBy(a => a.AlarmHistories
                        .OrderByDescending(h => h.ModificationDate)
                        .FirstOrDefault().StatusType.Name);
                case "severity_desc":
                    return query.OrderByDescending(a => a.NormGroup.SeverityHistories
                        .OrderByDescending(sh => sh.UpdateDate)
                        .FirstOrDefault().Severity.Name);
                case "severity":
                    return query.OrderBy(a => a.NormGroup.SeverityHistories
                        .OrderByDescending(sh => sh.UpdateDate)
                        .FirstOrDefault().Severity.Name);
                case "date_desc":
                    return query.OrderByDescending(a => a.TriggeredAt);
                case "date":
                    return query.OrderBy(a => a.TriggeredAt);
                case "attribution_desc":
                    return query.OrderByDescending(a => a.User == null ? "" : a.User.FirstName);
                case "attribution":
                    return query.OrderBy(a => a.User == null ? "" : a.User.FirstName);
                default:
                    return query.OrderByDescending(a => a.TriggeredAt);
            }
        }
    }

    public class UpdateStatusRequest {
        public int Id { get; set; }
        public string Status { get; set; }
    }

    public class UpdateAssignedUserRequest {
        public string Id { get; set; } // ID de l'alarme
        public string UserId { get; set; } // ID de l'utilisateurD du nouvel utilisateur attribué
    }
}