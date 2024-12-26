using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
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

        /// <summary>
        /// Configures the breadcrumb trail for the current action in the controller.
        /// </summary>
        /// <param name="context">
        /// The <see cref="ActionExecutingContext"/> object that provides context for the action being executed.
        /// </param>
        private void SetBreadcrumb(ActionExecutingContext context) {
            List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>();

            breadcrumbs.Add(new BreadcrumbItem { Title = "Home", Url = Url.Action("Index", "Home"), IsActive = false });

            if (context.ActionArguments.ContainsKey("tab")) {
                string tab = context.ActionArguments["tab"].ToString();
                switch (tab) {
                    case "unprocessed":
                        breadcrumbs.Add(new BreadcrumbItem {
                            Title = "Unprocessed Alarms", Url = string.Empty, IsActive = true
                        });
                        break;
                    case "newest":
                        breadcrumbs.Add(new BreadcrumbItem {
                            Title = "Newest Alarms", Url = string.Empty, IsActive = true
                        });
                        break;
                    case "overdue":
                        breadcrumbs.Add(
                            new BreadcrumbItem { Title = "Overdue Alarms", Url = string.Empty, IsActive = true });
                        break;
                }
            }

            ViewData["Breadcrumbs"] = breadcrumbs;
        }


        /// <summary>
        /// This method is called before the action execution in an ASP.NET Core controller.
        /// It allows performing preparation tasks before the action itself is executed.
        /// </summary>
        /// <param name="context">The context of the action execution, containing information about the request and action parameters.</param>
        public override void OnActionExecuting(ActionExecutingContext context) {
            base.OnActionExecuting(context);

            SetBreadcrumb(context);

            TempData["PreviousUrl"] = Request.Headers["Referer"].ToString();
        }

        /// <summary>
        /// Handles the request to display the homepage with alarms and related data.
        /// The method processes various filters for sorting and tab selection, fetches the data accordingly,
        /// and then returns the appropriate view with the data. 
        /// </summary>
        /// <param name="sortOrder">The order in which the alarms should be sorted (e.g., by date, severity, etc.).</param>
        /// <param name="solutionFilter">A filter to apply on the solution field when fetching notes.</param>
        /// <param name="authorFilter">A filter to apply on the author field when fetching notes.</param>
        /// <param name="tab">The selected tab, which determines the type of alarms to fetch (e.g., unprocessed, newest, overdue).</param>
        /// <param name="sortOrderNote">The order in which the notes should be sorted.</param>
        /// <returns>A ViewResult containing the homepage view with alarms and related data.</returns>
        [Authorize(Roles = "IT Director, Maintenance Manager, Technician")]
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
                selectedAlarms = await GetAlarmsWithDetails(statusFilter: "New", sortOrder: sortOrder, overdue: false,
                    orderByDate: false, newest: false, takeTop: null);
            }
            else if (tab == "newest") {
                selectedAlarms = await GetAlarmsWithDetails(statusFilter: null, sortOrder: sortOrder, overdue: false,
                    orderByDate: true, newest: true, takeTop: null);
            }
            else if (tab == "overdue") {
                selectedAlarms = await GetAlarmsWithDetails(statusFilter: null, sortOrder: sortOrder, overdue: true,
                    orderByDate: true, newest: false, takeTop: null);
            }
            else {
                selectedAlarms = await GetAlarmsWithDetails(statusFilter: "New", sortOrder: sortOrder, overdue: false,
                    orderByDate: false, newest: false, takeTop: null);
            }


            var viewModel = new HomePageViewModel {
                Notes = await FetchFilteredNotes(solutionFilter, authorFilter, sortOrderNote),
                TotalMachines = await GetTotalMachines(),
                MachinesWithActiveAlarms = await GetMachinesWithActiveAlarms(),
                AlarmsNotResolvedCount = await GetAlarmsNotResolvedCount(),
                AlarmsTriggeredTodayCount = await GetAlarmsTriggeredTodayCount(),
                SeverityAlarmsCount = await GetAlarmsBySeverity(),
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
            ViewData["MachineSortParam"] = sortOrder.Contains("machine_desc") ? "machine" : "machine_desc";
            ViewData["ModelSortParam"] = sortOrder.Contains("model_desc") ? "model" : "model_desc";
            ViewData["StatusSortParam"] = sortOrder.Contains("status_desc") ? "status" : "status_desc";
            ViewData["SeveritySortParam"] = sortOrder.Contains("severity_desc") ? "severity" : "severity_desc";
            ViewData["AlarmGroupSortParam"] = sortOrder.Contains("alarmgroup_desc") ? "alarmgroup" : "alarmgroup_desc";
            ViewData["DateSortParam"] = sortOrder.Contains("date_desc") ? "date" : "date_desc";


            ViewData["SeverityAlarmsCount"] = JsonConvert.SerializeObject(viewModel.SeverityAlarmsCount);

            ViewData["AssignedOrNotAlarmCount"] = JsonConvert.SerializeObject(viewModel.AssignedOrNotAlarmCount);

            ViewData["AlarmCountsBySiteAndSeverity"] =
                JsonConvert.SerializeObject(viewModel.AlarmCountsBySiteAndSeverity);


            return View(viewModel);
        }

        /// <summary>
        /// Fetches a list of notes from the database, applying various filters and sorting options.
        /// The method filters notes based on the solution filter, author filter, and sort order for the notes.
        /// </summary>
        /// <param name="solutionFilter">A filter to apply to the notes based on whether they are marked as a solution. If "all" is passed, no filtering is applied.</param>
        /// <param name="authorFilter">A filter to apply to the notes based on the author's ID. If null or empty, no filtering is applied.</param>
        /// <param name="sortOrderNote">Defines the order in which the notes should be sorted. Accepts "ndate_desc" for descending order or default ascending order.</param>
        /// <returns>A list of filtered and sorted notes as a list of <see cref="NoteViewModel"/>.</returns>
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

        /// <summary>
        /// Fetches a list of alarms from the database with additional details, including the machine, norm group,
        /// alarm histories, and severity histories. It applies various filters such as status, overdue, and sorting options.
        /// </summary>
        /// <param name="statusFilter">Optional filter for the alarm status. If "New" is provided, only alarms with a "New" status are returned.</param>
        /// <param name="sortOrder">Optional sorting order for the alarms. Determines how the alarms are ordered (e.g., by severity or date).</param>
        /// <param name="overdue">Flag indicating whether to filter for overdue alarms. An alarm is overdue if its triggered time is beyond the expected processing time and is not resolved or no longer triggered.</param>
        /// <param name="orderByDate">Flag indicating whether to order the alarms by their triggered date (ascending).</param>
        /// <param name="newest">Flag indicating whether to filter for alarms that were triggered in the last 10 days.</param>
        /// <param name="takeTop">Optional parameter specifying how many top alarms to retrieve. If null, no limit is applied.</param>
        /// <returns>A list of <see cref="AlarmViewModel"/> objects representing the alarms that match the specified filters.</returns>
        private async Task<List<AlarmViewModel>> GetAlarmsWithDetails(
            string statusFilter = null,
            string sortOrder = null,
            bool overdue = false,
            bool orderByDate = false,
            bool newest = false,
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

            if (newest) {
                DateTime tenDaysAgo = DateTime.UtcNow.AddDays(-10);

                alarmsQuery = alarmsQuery.Where(a => a.TriggeredAt >= tenDaysAgo);
            }

            if (overdue) {
                alarmsQuery = alarmsQuery
                    .Where(a =>
                        a.TriggeredAt < DateTime.UtcNow - a.NormGroup.MaxExpectedProcessingTime)
                    .Where(a =>
                        a.AlarmHistories
                            .OrderByDescending(ah => ah.ModificationDate)
                            .FirstOrDefault().StatusType.Name != "Resolved")
                    .Where(a =>
                        a.AlarmHistories
                            .OrderByDescending(ah => ah.ModificationDate)
                            .FirstOrDefault().StatusType.Name != "Not Triggered Anymore");
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

        /// <summary>
        /// Applies sorting to a query of alarms based on the specified sorting order.
        /// </summary>
        /// <param name="query">The queryable collection of alarms to be sorted.</param>
        /// <param name="sortOrder">A string representing the sorting order. The string can include sorting options like "machine", "model", "status", "severity", etc.</param>
        /// <returns>A sorted <see cref="IQueryable{Alarm}"/> collection based on the specified sortOrder.</returns>
        /// <remarks>
        /// The method checks the provided <paramref name="sortOrder"/> and sorts the alarms according to the specified criteria. 
        /// If no valid <paramref name="sortOrder"/> is provided or if the sortOrder is unrecognized, the default sorting is applied (by TriggeredAt in descending order).
        /// </remarks>
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

        /// <summary>
        /// Asynchronously retrieves the total number of machines from the database.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the asynchronous operation. The task result contains an integer representing the total number of machines.
        /// </returns>
        /// <remarks>
        /// This method performs a count query on the `Machines` table in the database and returns the number of machines available.
        /// </remarks>
        private async Task<int> GetTotalMachines() {
            return await _context.Machines.CountAsync();
        }

        /// <summary>
        /// Asynchronously retrieves the count of machines that have active alarms, where the alarms are not resolved and not triggered anymore.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the asynchronous operation. The task result contains an integer representing the number of machines with active alarms.
        /// </returns>
        /// <remarks>
        /// This method queries the `Alarms` table and checks the latest status for each alarm from its `AlarmHistories`. 
        /// It filters out alarms that are marked as "Resolved" or "Not Triggered Anymore". 
        /// The query returns the count of distinct machines that have active alarms.
        /// </remarks>
        private async Task<int> GetMachinesWithActiveAlarms() {
            int activeAlarms = await _context.Alarms
                .Where(a => a.AlarmHistories
                    .OrderByDescending(ah => ah.ModificationDate)
                    .FirstOrDefault().StatusType.Name != "Resolved")
                .Where(a =>
                    a.AlarmHistories
                        .OrderByDescending(ah => ah.ModificationDate)
                        .FirstOrDefault().StatusType.Name != "Not Triggered Anymore")
                .Select(a => a.Machine)
                .Distinct()
                .CountAsync();

            return activeAlarms;
        }

        /// <summary>
        /// Asynchronously retrieves the count of alarms grouped by severity for those that are not marked as "Resolved" or "Not Triggered Anymore".
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the asynchronous operation. The task result contains a dictionary where the key is the severity level (as a string),
        /// and the value is the number of alarms with that severity.
        /// </returns>
        /// <remarks>
        /// This method queries the `Alarms` table and filters alarms by their most recent status in `AlarmHistories`, excluding those with the status "Resolved" or "Not Triggered Anymore". 
        /// It then groups the alarms by their severity (from `SeverityHistories`), counts the number of alarms per severity, and returns the results as a dictionary.
        /// </remarks>
        private async Task<Dictionary<string, int>> GetAlarmsBySeverity() {
            var severityCounts = await _context.Alarms
                .Where(a => a.AlarmHistories
                    .OrderByDescending(ah => ah.ModificationDate)
                    .FirstOrDefault().StatusType.Name != "Resolved")
                .Where(a =>
                    a.AlarmHistories
                        .OrderByDescending(ah => ah.ModificationDate)
                        .FirstOrDefault().StatusType.Name != "Not Triggered Anymore")
                .Select(a => new {
                    Severity = a.NormGroup.SeverityHistories
                        .OrderByDescending(sh => sh.UpdateDate)
                        .FirstOrDefault().Severity.Name
                })
                .GroupBy(a => a.Severity)
                .Select(g => new { Severity = g.Key, AlarmCount = g.Count() })
                .ToListAsync();

            return severityCounts.ToDictionary(g => g.Severity, g => g.AlarmCount);
        }


        /// <summary>
        /// Asynchronously retrieves the count of alarms that are either assigned or unassigned to a user.
        /// Excludes alarms that are marked as "Resolved" or "Not Triggered Anymore".
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the asynchronous operation. The task result contains a dictionary with two keys:
        /// "Assigned" and "Unassigned", where each key corresponds to the number of alarms that are assigned to a user or not, respectively.
        /// </returns>
        /// <remarks>
        /// This method filters out alarms that are marked as "Resolved" or "Not Triggered Anymore" by checking the most recent status in `AlarmHistories`.
        /// It then counts how many alarms have a non-null `UserId` (assigned alarms) and how many have a null `UserId` (unassigned alarms),
        /// and returns this data as a dictionary with the counts for each category.
        /// </remarks>
        private async Task<Dictionary<string, int>> GetAssignedOrNotAlarmCount() {
            var alarmCounts = await _context.Alarms
                .Where(a => a.AlarmHistories
                    .OrderByDescending(ah => ah.ModificationDate)
                    .FirstOrDefault().StatusType.Name != "Resolved")
                .Where(a =>
                    a.AlarmHistories
                        .OrderByDescending(ah => ah.ModificationDate)
                        .FirstOrDefault().StatusType.Name != "Not Triggered Anymore")
                .ToListAsync();

            var assignedCount = alarmCounts.Count(a => a.UserId != null);
            var unassignedCount = alarmCounts.Count(a => a.UserId == null);

            var result =
                new Dictionary<string, int> { { "Assigned", assignedCount }, { "Unassigned", unassignedCount } };

            return result;
        }

        /// <summary>
        /// Asynchronously fetches alarm counts grouped by site and severity. 
        /// The method calculates the percentage of alarms for each severity within each site.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the asynchronous operation. The task result contains a dictionary 
        /// where the keys are site names (derived from the machine names) and the values are dictionaries of severity names 
        /// with the corresponding percentages of alarms for that severity in each site.
        /// </returns>
        /// <remarks>
        /// This method:
        /// - Groups alarms by site (extracted from the machine name).
        /// - For each site, counts how many alarms belong to each severity type.
        /// - Calculates the percentage of alarms for each severity type within the site.
        /// 
        /// The result is a dictionary where:
        /// - The key is the site name (extracted from the machine name, which is assumed to be in the format "siteName-machineName").
        /// - The value is another dictionary, where the key is the severity name, and the value is the percentage of alarms
        /// with that severity for that site.
        /// </remarks>
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
                    var siteName = machineName.Split('-')[0];
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

        /// <summary>
        /// Asynchronously retrieves a filtered list of notes based on the specified filters and sorting order, 
        /// and returns a partial view with the notes list.
        /// </summary>
        /// <param name="solutionFilter">A filter to indicate whether to show notes that are solutions. Accepts a string value ("true" or "false") or "all" for no filter.</param>
        /// <param name="authorFilter">The author ID to filter notes by a specific author. If null or empty, no filter is applied.</param>
        /// <param name="sortOrderNote">The sorting order for the notes, such as "ndate_desc" for descending order by date. If null, the default order is used.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the asynchronous operation. The task result contains an 
        /// <see cref="ActionResult"/> representing the partial view to be rendered, which contains the filtered notes.
        /// </returns>
        /// <remarks>
        /// This action method calls <see cref="FetchFilteredNotes"/> to get a list of notes based on the provided filters and sorting order,
        /// and then returns a partial view ("_NotesList") with the filtered notes.
        /// 
        /// - The <paramref name="solutionFilter"/> determines if the notes shown are solutions ("true") or not ("false"), or all notes ("all").
        /// - The <paramref name="authorFilter"/> allows filtering notes by a specific author's ID.
        /// - The <paramref name="sortOrderNote"/> defines the sorting order of the notes, such as sorting by creation date in descending order.
        /// </remarks>
        [HttpGet]
        public async Task<ActionResult> GetFilteredNotes(string solutionFilter, string authorFilter,
            string sortOrderNote) {
            var notes = await FetchFilteredNotes(solutionFilter, authorFilter, sortOrderNote);
            return PartialView("_NotesList", notes);
        }

        /// <summary>
        /// Gets the count of alarms that are not resolved (i.e., alarms that are neither in the "Resolved" nor "Not Triggered Anymore" status).
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the count of unresolved alarms.
        /// </returns>
        /// <remarks>
        /// This method checks the most recent status of each alarm to determine if it is neither "Resolved" nor "Not Triggered Anymore". 
        /// It then returns the total count of such unresolved alarms in the system.
        /// </remarks>
        private async Task<int> GetAlarmsNotResolvedCount() {
            var alarmsNotResolvedCount = await _context.Alarms
                .Where(a => a.AlarmHistories
                    .OrderByDescending(ah => ah.ModificationDate)
                    .FirstOrDefault().StatusType.Name != "Resolved")
                .Where(a =>
                    a.AlarmHistories
                        .OrderByDescending(ah => ah.ModificationDate)
                        .FirstOrDefault().StatusType.Name != "Not Triggered Anymore")
                .CountAsync();

            return alarmsNotResolvedCount;
        }

        /// <summary>
        /// Gets the count of alarms triggered today that are not in the "Resolved" or "Not Triggered Anymore" status.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the count of alarms triggered today that are unresolved.
        /// </returns>
        /// <remarks>
        /// This method filters alarms that were triggered on the current day (using the UTC date). It then checks that the most recent status of each alarm is neither "Resolved" nor "Not Triggered Anymore". 
        /// Only those alarms that meet both criteria (triggered today and unresolved) are counted.
        /// </remarks>
        private async Task<int> GetAlarmsTriggeredTodayCount() {
            var today = DateTime.UtcNow.Date;

            var alarmsTriggeredTodayCount = await _context.Alarms
                .Where(a => a.TriggeredAt.Date == today)
                .Where(a => a.AlarmHistories
                    .OrderByDescending(ah => ah.ModificationDate)
                    .FirstOrDefault().StatusType.Name != "Resolved")
                .Where(a =>
                    a.AlarmHistories
                        .OrderByDescending(ah => ah.ModificationDate)
                        .FirstOrDefault().StatusType.Name != "Not Triggered Anymore")
                .CountAsync();

            return alarmsTriggeredTodayCount;
        }

        /// <summary>
        /// Retrieves and returns a partial view showing the distribution of alarm counts by site and severity.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> 
        /// that renders a partial view with the alarm distribution data by site and severity.
        /// </returns>
        /// <remarks>
        /// This method calls the <see cref="FetchAlarmCountsBySiteAndSeverity"/> helper method to get alarm distribution 
        /// grouped by site and severity. The results are then passed to the partial view "_AlarmDistributionBySiteAndSeverity" 
        /// for rendering. This is typically used to display the alarm distribution in a visual format (e.g., a table or chart) 
        /// on the user interface.
        /// </remarks>
        public async Task<IActionResult> GetAlarmCountsBySiteAndSeverity() {
            var alarmCountsBySiteAndSeverity = await FetchAlarmCountsBySiteAndSeverity();
            return PartialView("_AlarmDistributionBySiteAndSeverity", alarmCountsBySiteAndSeverity);
        }
    }
}