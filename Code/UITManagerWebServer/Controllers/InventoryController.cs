using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using UITManagerWebServer.Data;
using UITManagerWebServer.Models;

namespace UITManagerWebServer.Controllers {
    public class InventoryController : Controller {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context) {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context) {
            base.OnActionExecuting(context);

            TempData["PreviousUrl"] = Request.Headers["Referer"].ToString();
        }

        [Authorize]
        public async Task<IActionResult> Index(string sortOrder, bool? statusFilter) {
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

            ViewData["StatusFilter"] = statusFilter.HasValue ? statusFilter : null;

            IQueryable<Machine> machinesQuery =
                _context.Machines.Include(m => m.Informations).Include(m => m.Notes).AsQueryable();

            if (statusFilter.HasValue) {
                machinesQuery = machinesQuery.Where(m => m.IsWorking == statusFilter);
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


        // GET: Inventory/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            Machine? machine = await _context.Machines
                .FirstOrDefaultAsync(m => m.Id == id);
            if (machine == null) {
                return NotFound();
            }

            return View(machine);
        }

        // GET: Inventory/Create
        public IActionResult Create() {
            return View();
        }

        // POST: Inventory/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,IsWorking")] Machine machine) {
            if (ModelState.IsValid) {
                _context.Add(machine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(machine);
        }

        // GET: Inventory/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            Machine? machine = await _context.Machines.FindAsync(id);
            if (machine == null) {
                return NotFound();
            }

            return View(machine);
        }

        // POST: Inventory/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,IsWorking")] Machine machine) {
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

        // GET: Inventory/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            Machine? machine = await _context.Machines
                .FirstOrDefaultAsync(m => m.Id == id);
            if (machine == null) {
                return NotFound();
            }

            return View(machine);
        }

        // POST: Inventory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            Machine? machine = await _context.Machines.FindAsync(id);
            if (machine != null) {
                _context.Machines.Remove(machine);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MachineExists(int id) {
            return _context.Machines.Any(e => e.Id == id);
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