using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UITManagerWebServer.Data;
using UITManagerWebServer.Models;

namespace UITManagerWebServer {
    public class AlarmDetail : Controller {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AlarmDetail(UserManager<ApplicationUser> userManager, ApplicationDbContext context) {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Configures the breadcrumb trail for the current action in the controller.
        /// </summary>
        /// <param name="context">
        /// The <see cref="ActionExecutingContext"/> object that provides context for the action being executed.
        /// </param>
        private void SetBreadcrumb(ActionExecutingContext context) {
            List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>();

            breadcrumbs.Add(new BreadcrumbItem {
                Title = "Home", Url = Url.Action("Index", "Home"), IsActive = false
            });

            string currentAction = context.ActionDescriptor.RouteValues["action"];

            switch (currentAction) {
                case "Index":
                    int alarmId = Convert.ToInt32(context.ActionArguments["id"]);
                    var alarm = _context.Alarms.FirstOrDefault(a => a.Id == alarmId);
                    if (alarm != null) {
                        breadcrumbs.Add(new BreadcrumbItem {
                            Title = "Machine's Alarm Details",
                            Url = string.Empty,
                            IsActive = true
                        });
                    }
                    break;

                case "Details":
                    breadcrumbs.Add(new BreadcrumbItem {
                        Title = "Alarm's details", Url = string.Empty, IsActive = true
                    });
                    
                    break;
                
                case "Edit":
                    alarmId = Convert.ToInt32(context.ActionArguments["id"]);
                    alarm = _context.Alarms.FirstOrDefault(a => a.Id == alarmId);
                    
                    if (alarm != null) {
                        breadcrumbs.Add(new BreadcrumbItem {
                            Title = "Edit machine's Alarm",
                            Url = string.Empty,
                            IsActive = true
                        });
                    }
                    break;

                case "Delete":
                    breadcrumbs.Add(new BreadcrumbItem {
                        Title = "Delete alarm", Url = string.Empty, IsActive = true
                    });
                    break;
            }

            ViewData["Breadcrumbs"] = breadcrumbs;
        }


        public override void OnActionExecuting(ActionExecutingContext context) {
            base.OnActionExecuting(context);

            SetBreadcrumb(context);

            TempData["PreviousUrl"] = Request.Headers["Referer"].ToString();
        }

        [Authorize]
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

            Alarm? alarm = await getAlarm(id);

            if (alarm == null) {
                return NotFound();
            }

            int alarmCount = await _context.Alarms
                .Where(a => a.MachineId == alarm.MachineId && a.NormGroupId == alarm.MachineId)
                .CountAsync();

            int alarmCountAll = await _context.Alarms
                .Where(a => a.NormGroupId == alarm.NormGroupId)
                .CountAsync();

            ViewData["AlarmStatusTypes"] = await _context.AlarmStatusTypes.ToListAsync();
            ViewData["Alarm"] = alarm;
            ViewData["user"] = _context.Users.ToList();
            ViewData["AlarmCount"] = alarmCount;
            ViewData["AlarmCountAll"] = alarmCountAll;


            List<Note> notes = await FetchFilteredNotes(solutionFilter, authorFilter, sortOrderNote);

            notes = ApplySorting(notes.AsQueryable(), sortOrder).ToList();
            ViewData["Notes"] = notes;


            ViewData["Authors"] = ViewBag.Authors = await _context.Users.ToListAsync();

            List<dynamic> triggeredInfoList = new List<dynamic>();
            foreach (Norm norm in alarm.NormGroup.Norms) {
                if (norm.InformationName != null) {
                    string infoName = norm.InformationName.Name;
                    string machineValue = alarm.Machine.GetInformationValueByName(infoName);

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

        [Authorize]
        public List<Information> FindMatchingInformations(int machineId, string informationName) {
            Machine? machine = _context.Machines
                .Include(m => m.Informations)
                .FirstOrDefault(m => m.Id == machineId);

            if (machine == null) {
                return new List<Information>();
            }

            List<Information> matchingInformations = machine.Informations
                .SelectMany(info => info.Children)
                .Where(child =>
                    child.Name.Equals(informationName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return matchingInformations;
        }

        private async Task<List<Note>> FetchFilteredNotes(string solutionFilter, string authorFilter,
            string sortOrderNote) {
            IQueryable<Note> notesQuery = _context.Notes.Include(n => n.Author).AsQueryable();

            if (!string.IsNullOrEmpty(solutionFilter) && solutionFilter != "all") {
                bool isSolution = solutionFilter.ToLower() == "true";
                notesQuery = notesQuery.Where(n => n.IsSolution == isSolution);
            }

            if (sortOrderNote == "ndate_desc") {
                notesQuery = notesQuery.OrderByDescending(n => n.CreatedAt);
            }
            else if (sortOrderNote == "ndate") {
                notesQuery = notesQuery.OrderBy(n => n.CreatedAt);
            }

            List<Note> notes = await notesQuery.ToListAsync();

            if (!string.IsNullOrEmpty(authorFilter)) {
                notes = notes.Where(n => n.AuthorId == authorFilter).ToList();
            }

            return notes;
        }

        private async Task<Alarm> getAlarm(int id) {
            Alarm? alarm = await _context.Alarms
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
        [ValidateAntiForgeryToken]
        [Authorize]
        [Route("AlarmDetail/UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request) {
            if (request == null || request.Id == 0 || string.IsNullOrEmpty(request.Status)) {
                return BadRequest(new { success = false, message = "Invalid data." });
            }

            Alarm? alarm = await _context.Alarms
                .Include(a => a.AlarmHistories)
                .FirstOrDefaultAsync(a => a.Id == request.Id);

            if (alarm == null) {
                return NotFound(new { success = false, message = "Alarm not found." });
            }

            AlarmStatusType? statusType =
                await _context.AlarmStatusTypes.FirstOrDefaultAsync(s => s.Name == request.Status);
            if (statusType == null) {
                return BadRequest(new { success = false, message = "Invalid status." });
            }

            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            AlarmStatusHistory newAlarmHistory = new AlarmStatusHistory {
                StatusTypeId = statusType.Id, ModificationDate = DateTime.UtcNow, UserId = userId
            };

            alarm.AlarmHistories.Add(newAlarmHistory);

            try {
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Status updated successfully." });
            }
            catch (Exception ex) {
                return StatusCode(500, new { success = false, message = "An error occurred while updating status." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ITDirector, MaintenanceManager")]
        [Route("AlarmDetail/Attribution")]
        public async Task<IActionResult> UpdateAttribution([FromBody] UpdateAssignedUserRequest request) {
            if (request == null || string.IsNullOrEmpty(request.Id) || string.IsNullOrEmpty(request.UserId)) {
                return BadRequest(new { success = false, message = "Invalid request data." });
            }

            try {
                Alarm? alarm = await _context.Alarms
                    .Include(a => a.Machine)
                    .Include(a => a.NormGroup)
                    .FirstOrDefaultAsync(a => a.Id.ToString() == request.Id);

                if (alarm == null) {
                    return NotFound(new { success = false, message = "Alarm not found." });
                }

                ApplicationUser? user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null) {
                    return NotFound(new { success = false, message = "User not found." });
                }

                alarm.UserId = request.UserId;

                AlarmStatusHistory alarmStatusHistory = new AlarmStatusHistory {
                    AlarmId = alarm.Id,
                    StatusType =
                        await _context.AlarmStatusTypes.FirstOrDefaultAsync(s =>
                            s.Name == "In Progress"),
                    ModificationDate = DateTime.UtcNow,
                    UserId = request.UserId
                };

                _context.AlarmHistories.Add(alarmStatusHistory);

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Alarm attribution updated successfully." });
            }
            catch (Exception ex) {
                return StatusCode(500,
                    new {
                        success = false, message = "An error occurred while updating attribution.", error = ex.Message
                    });
            }
        }

        [Authorize]
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            Alarm? alarm = await _context.Alarms
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
        [HttpGet]
        [Authorize(Roles = "ITDirector, MaintenanceManager")]
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
        [Authorize(Roles = "ITDirector, MaintenanceManager")]
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
        [HttpGet]
        [Authorize(Roles = "ITDirector, MaintenanceManager")]
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            Alarm? alarm = await _context.Alarms.FindAsync(id);
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
        [Authorize]
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
        [HttpGet, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ITDirector, MaintenanceManager")]
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            Alarm? alarm = await _context.Alarms
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
        [Authorize(Roles = "ITDirector, MaintenanceManager")]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            Alarm? alarm = await _context.Alarms.FindAsync(id);
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
        public string Id { get; set; }
        public string UserId { get; set; }
    }
}