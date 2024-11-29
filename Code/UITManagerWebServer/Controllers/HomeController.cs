using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UITManagerWebServer.Data;
using UITManagerWebServer.Models;

namespace UITManagerWebServer.Controllers {
    public class HomeController : Controller {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index(string sortOrder, string solutionFilter, string authorFilter,
            string tab) {
            var user = await _userManager.GetUserAsync(User);

            if (string.IsNullOrEmpty(tab) || (tab != "unprocessed" && tab != "newest" && tab != "overdue")) {
                return RedirectToAction("Index", new { sortOrder, solutionFilter, authorFilter, tab = "unprocessed" });
            }

            List<AlarmViewModel> selectedAlarms;
            if (tab == "unprocessed") {
                selectedAlarms = await GetAlarmsWithDetails("New", sortOrder);
            }
            else if (tab == "newest") {
                selectedAlarms = await GetAlarmsWithDetails("Resolved", sortOrder, takeTop: 100, orderByDate: true);
            }
            else if (tab == "overdue") {
                selectedAlarms = await GetAlarmsWithDetails("Resolved", sortOrder, overdue: true, orderByDate: true);
            }
            else {
                selectedAlarms = await GetAlarmsWithDetails("New", sortOrder);
            }

            var notes = await GetFilteredNotes(solutionFilter, authorFilter, sortOrder);
            var authors = notes.Select(n => n.Author).Distinct().ToList();
            var alarmCountsBySiteAndSeverity = await GetAlarmCountsBySiteAndSeverity();

            var viewModel = new HomePageViewModel {
                Notes = notes,
                TotalMachines = await GetTotalMachines(),
                MachinesWithActiveAlarms = await GetMachinesWithActiveAlarms(),
                NormGroupAlarmsCount = await GetNormGroupAlarmsCount(),
                AssignedOrNotAlarmCount = await GetAssignedOrNotAlarmCount(),
                AlarmCountsBySiteAndSeverity = await GetAlarmCountsBySiteAndSeverity(),
                UnprocessedAlarms = tab == "unprocessed" ? selectedAlarms : new List<AlarmViewModel>(),
                NewestAlarms = tab == "newest" ? selectedAlarms : new List<AlarmViewModel>(),
                OverdueAlarms = tab == "overdue" ? selectedAlarms : new List<AlarmViewModel>(),
                Authors = authors,
                ActiveTab = tab
            };

            if (string.IsNullOrEmpty(sortOrder)) {
                sortOrder = "date_desc";
            }

            ViewData["SortOrder"] = sortOrder;
            ViewData["MachineSortParm"] = sortOrder.Contains("machine_desc") ? "machine" : "machine_desc";
            ViewData["ModelSortParm"] = sortOrder.Contains("model_desc") ? "model" : "model_desc";
            ViewData["StatusSortParm"] = sortOrder.Contains("status_desc") ? "status" : "status_desc";
            ViewData["SeveritySortParm"] = sortOrder.Contains("severity_desc") ? "severity" : "severity_desc";
            ViewData["AlarmGroupSortParm"] = sortOrder.Contains("alarmgroup_desc") ? "alarmgroup" : "alarmgroup_desc";
            ViewData["DateSortParm"] = sortOrder.Contains("date_desc") ? "date" : "date_desc";


            ViewData["AlarmTypeOccurrences"] = JsonConvert.SerializeObject(viewModel.NormGroupAlarmsCount);

            ViewData["AssignedOrNotAlarmCount"] = JsonConvert.SerializeObject(viewModel.AssignedOrNotAlarmCount);
            
            ViewData["AlarmCountsBySiteAndSeverity"] = JsonConvert.SerializeObject(viewModel.AlarmCountsBySiteAndSeverity);
            
            var alarmCountsBySit1eAndSeverity = await GetAlarmCountsBySiteAndSeverity();
            Console.WriteLine(JsonConvert.SerializeObject(alarmCountsBySit1eAndSeverity)); // ou utiliser Debug.WriteLine()

            return View(viewModel);
        }

        private async Task<List<NoteViewModel>> GetFilteredNotes(string solutionFilter, string authorFilter,
            string sortOrder) {
            var notesQuery = _context.Notes.Include(n => n.Author).AsQueryable();

            if (!string.IsNullOrEmpty(solutionFilter) && solutionFilter != "all") {
                bool isSolution = solutionFilter.ToLower() == "true";
                notesQuery = notesQuery.Where(n => n.IsSolution == isSolution);
            }

            if (sortOrder == "date_desc") {
                notesQuery = notesQuery.OrderByDescending(n => n.CreatedAt);
            }
            else {
                notesQuery = notesQuery.OrderBy(n => n.CreatedAt);
            }

            var notes = await notesQuery.ToListAsync();

            if (!string.IsNullOrEmpty(authorFilter) && authorFilter != "all") {
                notes = notes.Where(n =>
                    (n.Author.FirstName + " " + n.Author.LastName).Contains(authorFilter,
                        StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return notes.Select(n => new NoteViewModel {
                Author = n.Author != null ? n.Author.FirstName + " " + n.Author.LastName : "Unknown",
                Id = n.Id,
                Date = n.CreatedAt,
                IsSolution = n.IsSolution,
                Content = n.Content
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

        private async Task<Dictionary<string, Dictionary<string, double>>> GetAlarmCountsBySiteAndSeverity() {
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
                    var siteName = machineName.Split('-')[1];  // Récupère la partie entre "Site-" et le premier tiret
                    return siteName;  // Renvoie la partie du site
                })
                .ToList();

            foreach (var siteGroup in groupedBySite) {
                var totalAlarms = siteGroup.Count(); // Total des alarmes pour le site

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




        public class AlarmViewModel {
            public string MachineName { get; set; }
            public int MachineId { get; set; }
            public string ModelName { get; set; }
            public int AlarmId { get; set; }
            public string Status { get; set; }
            public string Severity { get; set; }
            public string AlarmGroupName { get; set; }
            public DateTime TriggeredAt { get; set; }
        }

        // ViewModel pour les notes
        public class NoteViewModel {
            public string Author { get; set; }
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public bool IsSolution { get; set; }
            public string Content { get; set; }
        }

        // ViewModel de la page d'accueil
        public class HomePageViewModel {
            public List<NoteViewModel> Notes { get; set; }
            public int TotalMachines { get; set; }
            public int MachinesWithActiveAlarms { get; set; }
            public Dictionary<string, int> NormGroupAlarmsCount { get; set; }
            public Dictionary<string, int> AssignedOrNotAlarmCount { get; set; }
            public Dictionary<string, Dictionary<string, double>> AlarmCountsBySiteAndSeverity { get; set; }
            public List<AlarmViewModel> UnprocessedAlarms { get; set; }
            public List<AlarmViewModel> NewestAlarms { get; set; }
            public List<AlarmViewModel> OverdueAlarms { get; set; }
            public List<string> Authors { get; set; }
            public string ActiveTab { get; set; }
        }
    }
}