using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.View;
using Newtonsoft.Json;
using UITManagerWebServer.Data;
using UITManagerWebServer.Models;
using UITManagerWebServer.Models.ModelsView;

namespace UITManagerWebServer.Controllers {
    public class HomeController : Controller {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context) {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context) {
            base.OnActionExecuting(context);

            TempData["PreviousUrl"] = Request.Headers["Referer"].ToString();
        }

        [Authorize]
        public async Task<IActionResult> Index(string sortOrder, string solutionFilter, string authorFilter,
            string tab, string sortOrderNote) {
            TempData["PreviousUrl"] = Request.Headers["Referer"].ToString();
            Console.WriteLine(Request.Headers["Referer"].ToString());
            ViewData["SolutionFilter"] = solutionFilter;
            ViewData["AuthorFilter"] = authorFilter;
            ViewData["SortOrder"] = sortOrder;
            ViewData["SortOrderNote"] = sortOrderNote;

            if (string.IsNullOrEmpty(tab) || (tab != "unprocessed" && tab != "newest" && tab != "overdue")) {
                return RedirectToAction("Index", new {
                    sortOrder,
                    solutionFilter,
                    authorFilter,
                    sortOrderNote,
                    tab = "unprocessed"
                });
            }

            List<AlarmViewModel> selectedAlarms;
            if (tab == "unprocessed") {
                selectedAlarms = await GetAlarmsWithDetails("New", sortOrder);
            }
            else if (tab == "newest") {
                selectedAlarms = await GetAlarmsWithDetails("Resolved", sortOrder, orderByDate: true);
            }
            else if (tab == "overdue") {
                selectedAlarms = await GetAlarmsWithDetails("Resolved", sortOrder, overdue: true, orderByDate: true);
            }
            else {
                selectedAlarms = await GetAlarmsWithDetails("New", sortOrder);
            }
            

            var viewModel = new HomePageViewModel {
                Notes = await FetchFilteredNotes(solutionFilter, authorFilter, sortOrderNote),
                TotalMachines = await GetTotalMachines(),
                MachinesWithActiveAlarms = await GetMachinesWithActiveAlarms(),
                AlarmsNotResolvedCount = await GetAlarmsNotResolvedCount(),
                AlarmsTriggeredTodayCount = await GetAlarmsTriggeredTodayCount(),
                NormGroupAlarmsCount = await GetNormGroupAlarmsCount(),
                AssignedOrNotAlarmCount = await GetAssignedOrNotAlarmCount(),
                AlarmCountsBySiteAndSeverity = await FetchAlarmCountsBySiteAndSeverity(),
                Alarms = selectedAlarms,
                Authors = ViewBag.Authors = await _context.Users.ToListAsync(),
                ActiveTab = tab
            };

            if (string.IsNullOrEmpty(sortOrder)) {
                sortOrder = "date_desc";
            }

            ViewData["SortOrder"] = sortOrder;
            ViewData["SortOrderNote"] = sortOrderNote;
            ViewData["MachineSortParm"] = sortOrder.Contains("machine_desc") ? "machine" : "machine_desc";
            ViewData["ModelSortParm"] = sortOrder.Contains("model_desc") ? "model" : "model_desc";
            ViewData["StatusSortParm"] = sortOrder.Contains("status_desc") ? "status" : "status_desc";
            ViewData["SeveritySortParm"] = sortOrder.Contains("severity_desc") ? "severity" : "severity_desc";
            ViewData["AlarmGroupSortParm"] = sortOrder.Contains("alarmgroup_desc") ? "alarmgroup" : "alarmgroup_desc";
            ViewData["DateSortParm"] = sortOrder.Contains("date_desc") ? "date" : "date_desc";


            ViewData["AlarmTypeOccurrences"] = JsonConvert.SerializeObject(viewModel.NormGroupAlarmsCount);

            ViewData["AssignedOrNotAlarmCount"] = JsonConvert.SerializeObject(viewModel.AssignedOrNotAlarmCount);

            ViewData["AlarmCountsBySiteAndSeverity"] =
                JsonConvert.SerializeObject(viewModel.AlarmCountsBySiteAndSeverity);


            return View(viewModel);
        }

        private async Task<List<NoteViewModel>> FetchFilteredNotes(string solutionFilter, string authorFilter,
            string sortOrderNote) {
            var notesQuery = _context.Notes.Include(n => n.Author).AsQueryable();

            if (!string.IsNullOrEmpty(solutionFilter) && solutionFilter != "all") {
                bool isSolution = solutionFilter.ToLower() == "true";
                notesQuery = notesQuery.Where(n => n.IsSolution == isSolution);
            }

            if (sortOrderNote == "ndate_desc") {
                notesQuery = notesQuery.OrderByDescending(n => n.CreatedAt);
            }
            else {
                notesQuery = notesQuery.OrderBy(n => n.CreatedAt);
            }

            var notes = await notesQuery.ToListAsync();

            if (!string.IsNullOrEmpty(authorFilter)) {
                notes = notes.Where(n => n.AuthorId == authorFilter).ToList();
            }

            return notes.Select(n => new NoteViewModel {
                Author = n.Author != null ? n.Author.FirstName + " " + n.Author.LastName : "Unknown",
                Id = n.Id,
                Date = n.CreatedAt,
                IsSolution = n.IsSolution,
                Title = n.Title
            }).ToList();
        }

        private async Task<List<AlarmViewModel>> GetAlarmsWithDetails(
            string statusFilter = null,
            string sortOrder = null,
            bool overdue = false,
            bool orderByDate = false,
            int? takeTop = null) {
            var alarmsQuery = _context.Alarms
                .Include(a => a.Machine)
                .Include(a => a.NormGroup)
                .Include(a => a.AlarmHistories)
                .ThenInclude(aStatus => aStatus.StatusType)
                .Include(a => a.NormGroup.SeverityHistories)
                .ThenInclude(sh => sh.Severity)
                .AsQueryable();

            if (statusFilter == "New") {
                alarmsQuery = alarmsQuery.Where(a => a.AlarmHistories
                    .OrderByDescending(h => h.ModificationDate)
                    .FirstOrDefault().StatusType.Name == "New");
            }

            if (overdue) {
                alarmsQuery = alarmsQuery
                    .Where(a =>
                        a.TriggeredAt < DateTime.UtcNow - a.NormGroup.MaxExpectedProcessingTime)
                    .Include(a =>
                        a.NormGroup.SeverityHistories.OrderByDescending(aa => aa.Severity.Name != "Resolved"));
            }

            if (orderByDate) {
                alarmsQuery = alarmsQuery.OrderBy(a => a.TriggeredAt);
            }

            alarmsQuery = ApplySorting(alarmsQuery, sortOrder);

            if (takeTop.HasValue) {
                alarmsQuery = alarmsQuery.Take(takeTop.Value);
            }

            var alarms = await alarmsQuery.ToListAsync();

            return alarms.Select(a => new AlarmViewModel {
                MachineName = a.Machine.Name,
                MachineId = a.Machine.Id,
                AlarmId = a.Id,
                ModelName = a.Machine.Model,
                Status = a.AlarmHistories.OrderByDescending(h => h.ModificationDate).FirstOrDefault()?.StatusType.Name,
                Severity = a.NormGroup.SeverityHistories.OrderByDescending(sh => sh.UpdateDate).FirstOrDefault()
                    ?.Severity.Name,
                AlarmGroupName = a.NormGroup.Name,
                TriggeredAt = a.TriggeredAt
            }).ToList();
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
                case "alarmgroup_desc":
                    return query.OrderByDescending(a => a.NormGroup.SeverityHistories
                        .OrderByDescending(sh => sh.NormGroup.Name)
                        .FirstOrDefault().NormGroup.Name);
                case "alarmgroup":
                    return query.OrderBy(a => a.NormGroup.SeverityHistories
                        .OrderByDescending(sh => sh.NormGroup.Name)
                        .FirstOrDefault().NormGroup.Name);
                case "date_desc":
                    return query.OrderByDescending(a => a.TriggeredAt);
                case "date":
                    return query.OrderBy(a => a.TriggeredAt);
                default:
                    return query.OrderByDescending(a => a.TriggeredAt);
            }
        }

        private async Task<int> GetTotalMachines() {
            return await _context.Machines.CountAsync();
        }

        private async Task<int> GetMachinesWithActiveAlarms() {
            var activeAlarms = await _context.Alarms
                .Where(a => a.AlarmHistories.Any(ah => ah.StatusType.Name != "Closed"))
                .Select(a => a.Machine)
                .Distinct()
                .CountAsync();

            return activeAlarms;
        }

        private async Task<Dictionary<string, int>> GetNormGroupAlarmsCount() {
            var normGroupCounts = await _context.Alarms
                .Where(a => a.AlarmHistories
                    .OrderByDescending(ah => ah.ModificationDate)
                    .FirstOrDefault().StatusType.Name != "Resolved")
                .GroupBy(a => a.NormGroup.Name)
                .Select(g => new { NormGroupName = g.Key, AlarmCount = g.Count() })
                .ToListAsync();

            return normGroupCounts.ToDictionary(g => g.NormGroupName, g => g.AlarmCount);
        }


        private async Task<Dictionary<string, int>> GetAssignedOrNotAlarmCount() {
            var alarmCounts = await _context.Alarms
                .Where(a => a.AlarmHistories
                    .OrderByDescending(ah => ah.ModificationDate)
                    .FirstOrDefault().StatusType.Name != "Resolved")
                .ToListAsync();

            var assignedCount = alarmCounts.Count(a => a.UserId != null);
            var unassignedCount = alarmCounts.Count(a => a.UserId == null);

            var result =
                new Dictionary<string, int> { { "Assigned", assignedCount }, { "Unassigned", unassignedCount } };

            return result;
        }

        private async Task<Dictionary<string, Dictionary<string, double>>> FetchAlarmCountsBySiteAndSeverity() {
            var alarms = await _context.Alarms
                .Include(a => a.Machine)
                .Include(a => a.NormGroup)
                .ThenInclude(ng => ng.SeverityHistories)
                .ThenInclude(sh => sh.Severity)
                .ToListAsync();

            var result = new Dictionary<string, Dictionary<string, double>>();

            var groupedBySite = alarms
                .GroupBy(a => {
                    var machineName = a.Machine.Name;
                    var siteName = machineName.Split('-')[1];
                    return siteName;
                })
                .ToList();

            foreach (var siteGroup in groupedBySite) {
                var totalAlarms = siteGroup.Count();

                var severityCounts = siteGroup
                    .Select(a => {
                        var mostRecentSeverity = a.NormGroup.SeverityHistories
                            .OrderByDescending(sh => sh.UpdateDate)
                            .FirstOrDefault()?.Severity.Name;

                        return mostRecentSeverity;
                    })
                    .Where(severity => severity != null)
                    .GroupBy(severity => severity)
                    .Select(g => new { Severity = g.Key, Count = g.Count() })
                    .ToList();

                var severityPercentages = severityCounts.ToDictionary(
                    g => g.Severity,
                    g => (double)g.Count / totalAlarms * 100);

                result[siteGroup.Key] = severityPercentages;
            }

            return result;
        }

        [HttpGet]
        public async Task<ActionResult> GetFilteredNotes(string solutionFilter, string authorFilter,
            string sortOrderNote) {
            var notes = await FetchFilteredNotes(solutionFilter, authorFilter, sortOrderNote);
            return PartialView("_NotesList", notes);
        }

        [HttpGet]
        public async Task<IActionResult> GetFilteredAlarmsList(string tab, string sortOrder) {
            List<AlarmViewModel> selectedAlarms = new List<AlarmViewModel>();

            switch (tab) {
                case "unprocessed":
                    selectedAlarms = await GetAlarmsWithDetails("New", sortOrder);
                    break;
                case "newest":
                    selectedAlarms = await GetAlarmsWithDetails("Resolved", sortOrder, orderByDate: true);
                    break;
                case "overdue":
                    selectedAlarms =
                        await GetAlarmsWithDetails("Resolved", sortOrder, overdue: true, orderByDate: true);
                    break;
                default:
                    selectedAlarms = await GetAlarmsWithDetails("New", sortOrder);
                    break;
            }

            return PartialView("_AlarmsList", selectedAlarms);
        }
        
        private async Task<int> GetAlarmsNotResolvedCount() {
            var alarmsNotResolvedCount = await _context.Alarms
                .Where(a => a.AlarmHistories
                    .OrderByDescending(ah => ah.ModificationDate)
                    .FirstOrDefault().StatusType.Name != "Resolved")
                .CountAsync();

            return alarmsNotResolvedCount;
        }
        
        private async Task<int> GetAlarmsTriggeredTodayCount() {
            var today = DateTime.UtcNow.Date;
    
            var alarmsTriggeredTodayCount = await _context.Alarms
                .Where(a => a.TriggeredAt.Date == today)
                .CountAsync();

            return alarmsTriggeredTodayCount;
        }

        public async Task<IActionResult> GetAlarmCountsBySiteAndSeverity() {
            var alarmCountsBySiteAndSeverity = await FetchAlarmCountsBySiteAndSeverity();
            return PartialView("_AlarmDistributionBySiteAndSeverity", alarmCountsBySiteAndSeverity);
        }
    }
}