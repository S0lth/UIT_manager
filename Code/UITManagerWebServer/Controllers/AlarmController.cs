using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UITManagerWebServer.Data;
using UITManagerWebServer.Models;

namespace UITManagerWebServer.Controllers {
    public class AlarmController : Controller {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AlarmController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) {
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

            breadcrumbs.Add(new BreadcrumbItem { Title = "Home", Url = Url.Action("Index", "Home"), IsActive = false });

            breadcrumbs.Add(new BreadcrumbItem {
                Title = "Raised Alarms", Url = Url.Action("Index", "Alarm"), IsActive = false
            });

            string? currentAction = context.ActionDescriptor.RouteValues["action"];
            string? id = context.ActionArguments.ContainsKey("id") ? context.ActionArguments["id"]?.ToString() : null;

            switch (currentAction) {
                case "Index":
                    breadcrumbs.Last().IsActive = true; 
                    break;

                case "Details":
                    var alarm = _context.Alarms
                        .Include(a => a.Machine)
                        .Include(a => a.NormGroup)
                        .FirstOrDefault(a => a.Id.ToString() == id);

                    if (alarm != null) {
                        breadcrumbs.Add(new BreadcrumbItem { 
                            Title = alarm.Machine?.Name, 
                            Url = Url.Action("Details", "Machine", new { id = alarm.Machine.Id }), 
                            IsActive = false 
                        });

                        breadcrumbs.Add(new BreadcrumbItem { 
                            Title = alarm.NormGroup.Name, 
                            Url = string.Empty, 
                            IsActive = true 
                        });
                    }
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
        public async Task<IActionResult> Index(string sortOrder, string search) {
            ViewData["SortOrder"] = sortOrder;
            ViewData["MachineSortParam"] = sortOrder == "Machine" ? "Machine_desc" : "Machine";
            ViewData["StatusSortParam"] = sortOrder == "Status" ? "Status_desc" : "Status";
            ViewData["SeveritySortParam"] = sortOrder == "Severity" ? "Severity_desc" : "Severity";
            ViewData["AlarmGroupSortParam"] = sortOrder == "AlarmGroup" ? "AlarmGroup_desc" : "AlarmGroup";
            ViewData["DateSortParam"] = sortOrder == "Date" ? "Date_desc" : "Date";
            ViewData["ModelSortParam"] = sortOrder == "Model" ? "Model_desc" : "Model";
            ViewData["AttributionSortParam"] = sortOrder == "Attribution" ? "Attribution_desc" : "Attribution";

            List<ApplicationUser> users = _context.Users.ToList();
            ApplicationUser? user = await _userManager.GetUserAsync(User);

            IQueryable<Alarm> alarms = _context.Alarms
                .Include(a => a.Machine)
                .Include(a => a.NormGroup)
                .Include(a => a.AlarmHistories)
                .Include(a => a.AlarmHistories.OrderByDescending(b => b.ModificationDate))
                .ThenInclude(aStatus => aStatus.StatusType)
                .Include(a => a.NormGroup.SeverityHistories)
                .ThenInclude(sh => sh.Severity)
                .Include(a => a.User)
                .AsQueryable()
                .Where(a => a.AlarmHistories
                    .OrderByDescending(h => h.ModificationDate)
                    .FirstOrDefault()!
                    .StatusType.Name != "Resolved");

            if (!string.IsNullOrEmpty(search)) {
                string[] searchTerms = search.ToLower().Split(',');

                alarms = alarms.Where(a =>
                    searchTerms.All(term =>
                        a.Machine.Name.ToLower().Contains(term) ||
                        a.Machine.Model.ToLower().Contains(term) ||
                        a.NormGroup.Name.ToLower().Contains(term) ||
                        a.User.FirstName.ToLower().Contains(term) ||
                        a.User.LastName.ToLower().Contains(term) ||
                        a.AlarmHistories
                            .OrderByDescending(h => h.ModificationDate)
                            .FirstOrDefault()
                            .StatusType.Name.ToLower().Contains(term) ||
                        a.TriggeredAt.ToString().Contains(term) ||
                        a.NormGroup.SeverityHistories
                            .OrderByDescending(sh => sh.UpdateDate)
                            .Select(sh => sh.Severity.Name.ToLower())
                            .FirstOrDefault().Contains(term)
                    )
                ).AsQueryable();
            }


            alarms = sortOrder switch {
                "Attribution_desc" => alarms.OrderByDescending(a => a.User == null ? "" : a.User.FirstName),
                "Attribution" => alarms.OrderBy(a => a.User == null ? "" : a.User.FirstName),
                "Machine_desc" => alarms.OrderByDescending(a => a.Machine.Name),
                "Machine" => alarms.OrderBy(a => a.Machine.Name),
                "Status_desc" => alarms.OrderByDescending(a => a.AlarmHistories
                    .OrderByDescending(h => h.ModificationDate)
                    .Select(h => h.StatusType.Name)
                    .FirstOrDefault()),
                "Status" => alarms.OrderBy(a =>
                    a.AlarmHistories.OrderByDescending(h => h.ModificationDate).Select(h => h.StatusType.Name)
                        .FirstOrDefault()),
                "Severity_desc" => alarms.OrderByDescending(a => a.NormGroup.SeverityHistories
                    .OrderByDescending(sh => sh.UpdateDate)
                    .Select(sh => sh.Severity.Name == "Critical" ? 0 :
                        sh.Severity.Name == "High" ? 1 :
                        sh.Severity.Name == "Medium" ? 2 :
                        sh.Severity.Name == "Low" ? 3 :
                        sh.Severity.Name == "Warning" ? 4 : int.MaxValue)
                    .FirstOrDefault()),
                "Severity" => alarms.OrderBy(a => a.NormGroup.SeverityHistories
                    .OrderByDescending(sh => sh.UpdateDate)
                    .Select(sh => sh.Severity.Name == "Critical" ? 0 :
                        sh.Severity.Name == "High" ? 1 :
                        sh.Severity.Name == "Medium" ? 2 :
                        sh.Severity.Name == "Low" ? 3 :
                        sh.Severity.Name == "Warning" ? 4 : int.MaxValue)
                    .FirstOrDefault()),
                "AlarmGroup_desc" => alarms.OrderByDescending(a => a.NormGroup.Name),
                "AlarmGroup" => alarms.OrderBy(a => a.NormGroup.Name),
                "Date_desc" => alarms.OrderByDescending(a => a.TriggeredAt),
                "Date" => alarms.OrderBy(a => a.TriggeredAt),
                "Model" => alarms.OrderBy(a => a.Machine.Model),
                "Model_desc" => alarms.OrderByDescending(a => a.Machine.Model),
                "assigned_to_me" => alarms.Where(a => a.AlarmHistories
                                                          .OrderByDescending(h => h.ModificationDate)
                                                          .FirstOrDefault() != null &&
                                                      a.AlarmHistories
                                                          .OrderByDescending(h => h.ModificationDate)
                                                          .FirstOrDefault().User.Id == user.Id),
                "unassigned" => alarms.Where(a => a.UserId == null),
                "assigned" => alarms.Where(a => a.UserId != null),
                "triggered_today" => alarms.Where(a => a.TriggeredAt.Date == DateTime.UtcNow.Date),
                _ => alarms.OrderBy(a => a.Machine.Name)
            };

            ViewData["AlarmStatusTypes"] = await _context.AlarmStatusTypes.ToListAsync();
            ViewData["user"] = users;

            if (User.IsInRole("Admin")) {
                ViewData["Role"] = "Admin";
            }
            else if (User.IsInRole("User")) {
                ViewData["Role"] = "User";
            }
            else {
                ViewData["Role"] = "Unknown";
            }

            return View(await alarms.ToListAsync());
        }

        [HttpPost]
        [Authorize]
        [Route("Alarm/UpdateStatus")]
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

            string userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            AlarmStatusHistory newAlarmHistory = new AlarmStatusHistory {
                StatusTypeId = statusType.Id, ModificationDate = DateTime.UtcNow, UserId = userId
            };

            alarm.AlarmHistories.Add(newAlarmHistory);

            try {
                await _context.SaveChangesAsync();

                return PartialView("_AlertMessage", new { success = true, message = "Status updated successfully." });
            }
            catch (Exception ex) {
                return StatusCode(500,
                    new { success = false, message = "An error occurred while updating status.", error = ex.Message });
            }
        }


        [HttpPost]
        [Authorize(Roles = "IT Director, Maintenance Manager")]
        [Route("Alarm/Attribution")]
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

                return PartialView("_AlertMessage",
                    new { success = true, message = "Attribution updated successfully." });
            }
            catch (Exception ex) {
                return StatusCode(500,
                    new {
                        success = false, message = "An error occurred while updating attribution.", error = ex.Message
                    });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Create(Alarm alarm) {
            if (ModelState.IsValid) {
                AlarmStatusType? newStatusType = _context.AlarmStatusTypes
                    .FirstOrDefault(s => s.Name == "New");

                if (newStatusType == null) {
                    ModelState.AddModelError("", "Le statut 'New' est introuvable dans la base de données.");
                    return View(alarm);
                }

                AlarmStatusHistory alarmHistory = new AlarmStatusHistory {
                    StatusTypeId = newStatusType.Id, ModificationDate = DateTime.Now,
                };
                alarm.AddAlarmHistory(alarmHistory);

                _context.Alarms.Add(alarm);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.MachineId = new SelectList(_context.Machines, "Id", "Name", alarm.MachineId);
            ViewBag.NormGroupId = new SelectList(_context.NormGroups, "Id", "Name", alarm.NormGroupId);
            return View(alarm);
        }
        
        [Authorize(Roles = "IT Director, Maintenance Managern Technician")]
        public async Task<IActionResult> Details(string sortOrder, int  id, string solutionFilter, string authorFilter,
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