using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UITManagerWebServer.Data;
using UITManagerWebServer.Models;

namespace UITManagerWebServer.Controllers {
    public class MachineController : Controller {
        private readonly ApplicationDbContext _context;

        public MachineController(ApplicationDbContext context) {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context) {
            base.OnActionExecuting(context);

            TempData["PreviousUrl"] = Request.Headers["Referer"].ToString();
        }

        [Authorize]
        public async Task<IActionResult> Index(string sortOrder, string filter, string search) {
            if (string.IsNullOrEmpty(sortOrder)) {
                sortOrder = "LastSeen_desc";
            }

            ViewData["SortOrder"] = sortOrder;
            ViewData["MachineSortParam"] = sortOrder == "Machine" ? "Machine_desc" : "Machine";
            ViewData["LastSeenSortParam"] = sortOrder == "LastSeen" ? "LastSeen_desc" : "LastSeen";
            ViewData["OsSortParam"] = sortOrder == "Os" ? "Os_desc" : "Os";
            ViewData["BuildSortParam"] = sortOrder == "Build" ? "Build_desc" : "Build";
            ViewData["ServiceTagSortParam"] = sortOrder == "ServiceTag" ? "ServiceTag_desc" : "ServiceTag";
            ViewData["StatusSortParam"] = sortOrder == "Status" ? "Status_desc" : "Status";
            ViewData["NoteCountSortParam"] = sortOrder == "NoteCount" ? "NoteCount_desc" : "NoteCount";
            ViewData["LastNoteSortParam"] = sortOrder == "LastNote" ? "LastNote_desc" : "LastNote";

            ViewData["Filter"] = filter;

            IQueryable<Machine> machinesQuery =
                _context.Machines.Include(m => m.Informations).Include(m => m.Notes).AsQueryable();

            if (filter == "working") {
                machinesQuery = machinesQuery.Where(m => m.IsWorking == true);
            }
            if (filter == "not_working") {
                machinesQuery = machinesQuery.Where(m => m.IsWorking == false);
            }
            if (filter == "alarm_triggered") {
                machinesQuery = machinesQuery.Where(m => m.Alarms
                    .Any(a => a.AlarmHistories
                        .OrderByDescending(ah => ah.ModificationDate)
                        .FirstOrDefault() != null && 
                        a.AlarmHistories
                            .OrderByDescending(ah => ah.ModificationDate)
                            .FirstOrDefault().StatusType.Name != "resolved"
                    )
                );
            }
            
            if (!string.IsNullOrEmpty(search)) {
                string searchLower = search.ToLower();
                var machinesList = await machinesQuery.ToListAsync();
                machinesList = machinesList.Where(m =>
                    m.Name.ToLower().Contains(searchLower) ||
                    m.Model.ToLower().Contains(searchLower) ||
                    m.GetServiceTag().ToLower().Contains(searchLower) ||
                    m.GetOsBuild().ToLower().Contains(searchLower) ||
                    m.GetOsName().ToLower().Contains(searchLower) ||
                    m.GetOsVersion().ToLower().Contains(searchLower)
                ).ToList();
            }

            
            List<Machine> machines = await machinesQuery.ToListAsync();

            IEnumerable<MachineViewModel> machineViewModels = machines.Select(m => new MachineViewModel {
                Id = m.Id,
                Name = m.Name,
                LastSeen = m.LastSeen,
                Model = m.Model,
                Os = m.GetOsName() + " " + m.GetOsVersion(),
                Build = m.GetOsBuild(),
                ServiceTag = m.GetServiceTag(),
                IsWorking = m.IsWorking,
                NoteCount = m.Notes.Count,
                LastNote = m.GetLatestNote()
            });

            machineViewModels = sortOrder switch {
                "Machine" => machineViewModels.OrderBy(m => m.Name),
                "Machine_desc" => machineViewModels.OrderByDescending(m => m.Name),
                "LastSeen" => machineViewModels.OrderBy(m => m.LastSeen),
                "LastSeen_desc" => machineViewModels.OrderByDescending(m => m.LastSeen),
                "Os" => machineViewModels.OrderBy(m => m.Os),
                "Os_desc" => machineViewModels.OrderByDescending(m => m.Os),
                "Build" => machineViewModels.OrderBy(m => m.Build),
                "Build_desc" => machineViewModels.OrderByDescending(m => m.Build),
                "ServiceTag" => machineViewModels.OrderBy(m => m.ServiceTag),
                "ServiceTag_desc" => machineViewModels.OrderByDescending(m => m.ServiceTag),
                "Status" => machineViewModels.OrderBy(m => m.IsWorking),
                "Status_desc" => machineViewModels.OrderByDescending(m => m.IsWorking),
                "NoteCount" => machineViewModels.OrderBy(m => m.NoteCount),
                "NoteCount_desc" => machineViewModels.OrderByDescending(m => m.NoteCount),
                "LastNote" => machineViewModels.OrderBy(m => m.LastNote?.Content),
                "LastNote_desc" => machineViewModels.OrderByDescending(m => m.LastNote?.Content),
                _ => machineViewModels.OrderByDescending(m => m.LastSeen),
            };

            return View(machineViewModels.ToList());
        }

        [Authorize]
        // GET: Machine/Details/5
        public async Task<IActionResult> Details(int? id, string sortOrder, string solutionFilter, string authorFilter,
            string typeFilter) {
            var machine = await _context.Machines
                .FirstOrDefaultAsync(m => m.Id == id);

            var notes = await getFilteredNotes(sortOrder, solutionFilter, authorFilter, id);
            var alarms = await getFilteredAlrms(sortOrder, id, typeFilter);
            var information = await getMachineInformation(id);
            var authors = ViewBag.Authors = await _context.Users.ToListAsync();
            var detailView = new DetailsViewModel {
                Id = machine.Id,
                Name = machine.Name,
                LastSeen = machine.LastSeen,
                Model = machine.Model,
                IsWorking = machine.IsWorking,
                Notes = notes,
                Alarms = alarms,
                Authors = authors,
                Informations = information,
                AnyAlarms = alarms.Any(),
                AnyNote = notes.Any(),
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
            ViewData["AttributionSortParam"] = sortOrder == "Attribution" ? "Attribution_desc" : "Attribution";
            ViewData["SolutionFilter"] = solutionFilter;
            ViewData["AuthorFilter"] = authorFilter;
            ViewData["SortOrderNote"] = sortOrder.Contains("ndate") ? "ndate_desc" : "ndate";
            ViewData["TypeFilter"] = typeFilter;


            return View(detailView);
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

            foreach (var root in model) {
                var hierarchy = await BuildHierarchy(root, list.ToList());
                result.Add(hierarchy);
            }

            return result;
        }

        private async Task<ComponentsViewModel> BuildHierarchy(ComponentsViewModel parent, List<Information> list) {
            var children = list.Where(e => e.ParentId == parent.id).ToList();

            var parentViewModel = new ComponentsViewModel {
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

        private async Task<List<AlarmViewModel>> getFilteredAlrms(string sortOrder, int? id, string typeFilter) {
            var alarmsQuery = _context.Alarms
                .Include(a => a.Machine)
                .Include(a => a.NormGroup)
                .Include(a => a.User)
                .Include(a => a.AlarmHistories)
                .ThenInclude(aStatus => aStatus.StatusType)
                .Include(a => a.NormGroup.SeverityHistories)
                .ThenInclude(sh => sh.Severity)
                .Where(a => a.MachineId == id)
                .AsQueryable();

            if (typeFilter == "Resolved") {
                alarmsQuery = alarmsQuery.Where(a =>
                    a.AlarmHistories.OrderByDescending(h => h.ModificationDate).FirstOrDefault().StatusType.Name ==
                    typeFilter);
            }

            if (typeFilter != "Resolved" && typeFilter != "All") {
                alarmsQuery = alarmsQuery.Where(a =>
                    a.AlarmHistories.OrderByDescending(h => h.ModificationDate).FirstOrDefault().StatusType.Name !=
                    "Resolved");
            }

            alarmsQuery = ApplySorting(alarmsQuery, sortOrder);

            var alarms = await alarmsQuery.ToListAsync();

            return alarms.Select(a => new AlarmViewModel {
                MachineId = a.Machine.Id,
                AlarmId = a.Id,
                Status = a.AlarmHistories.OrderByDescending(h => h.ModificationDate).FirstOrDefault()?.StatusType.Name,
                Severity = a.NormGroup.SeverityHistories.OrderByDescending(sh => sh.UpdateDate).FirstOrDefault()
                    ?.Severity.Name,
                AlarmGroupName = a.NormGroup.Name,
                TriggeredAt = a.TriggeredAt,
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
                case "attribution_desc":
                    return query.OrderByDescending(a => a.User == null ? "" : a.User.FirstName);
                case "attribution":
                    return query.OrderBy(a => a.User == null ? "" : a.User.FirstName);
                default:
                    return query.OrderByDescending(a => a.TriggeredAt);
            }
        }

        private async Task<List<NoteViewModel>> getFilteredNotes(string sortOrder, string solutionFilter,
            string authorFilter, int? id) {
            var notesQuery = _context.Notes.Include(n => n.Author).Where(n => n.MachineId == id).AsQueryable();

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
                Title = n.Title,
                MachineId = n.MachineId,
            }).ToList();
        }

        // GET: Machine/Create
        public IActionResult Create() {
            return View();
        }

        // POST: Machine/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,IsWorking,Model,LastSeen")] Machine machine) {
            if (ModelState.IsValid) {
                _context.Add(machine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(machine);
        }

        // GET: Machine/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var machine = await _context.Machines.FindAsync(id);
            if (machine == null) {
                return NotFound();
            }

            return View(machine);
        }

        // POST: Machine/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,IsWorking,Model,LastSeen")] Machine machine) {
            if (id != machine.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(machine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) {
                    if (!MachineExists(machine.Id)) {
                        return NotFound();
                    }
                    else {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(machine);
        }

        // GET: Machine/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var machine = await _context.Machines
                .FirstOrDefaultAsync(m => m.Id == id);
            if (machine == null) {
                return NotFound();
            }

            return View(machine);
        }

        // POST: Machine/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var machine = await _context.Machines.FindAsync(id);
            if (machine != null) {
                _context.Machines.Remove(machine);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MachineExists(int id) {
            return _context.Machines.Any(e => e.Id == id);
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
            public bool AnyNote { get; set; }
            public bool AnyAlarms { get; set; }
        }

        /// <summary>
        /// Represents the data for a machine, including its details, status, and associated notes.
        /// </summary>
        public class MachineViewModel {
            public int Id { get; set; }

            public string Name { get; set; }

            public string Model { get; set; }

            public DateTime LastSeen { get; set; }

            public string Os { get; set; }

            public string Build { get; set; }

            public string ServiceTag { get; set; }

            public bool IsWorking { get; set; }

            public int NoteCount { get; set; }

            public Note LastNote { get; set; }

            /// <summary>
            /// Gets the difference between the system date and the last seen date
            /// The latest note is determined by the most recent creation date.
            /// </summary>
            /// <returns>A string containing the difference</returns>
            public string GetLastSeen() {
                TimeSpan timeSpan = DateTime.Now - LastSeen;

                return timeSpan.TotalMinutes switch {
                    < 1 => "Just now",
                    < 60 => $"{(int)timeSpan.TotalMinutes} minute{(timeSpan.TotalMinutes >= 2 ? "s" : "")} ago",
                    < 1440 => $"{(int)timeSpan.TotalHours} hour{(timeSpan.TotalHours >= 2 ? "s" : "")} ago",
                    < 43200 => $"{(int)timeSpan.TotalDays} day{(timeSpan.TotalDays >= 2 ? "s" : "")} ago",
                    < 525600 =>
                        $"{(int)(timeSpan.TotalDays / 30)} month{(timeSpan.TotalDays / 30 >= 2 ? "s" : "")} ago",
                    _ => $"{(int)(timeSpan.TotalDays / 365)} year{(timeSpan.TotalDays / 365 >= 2 ? "s" : "")} ago",
                };
            }
        }
    }
}