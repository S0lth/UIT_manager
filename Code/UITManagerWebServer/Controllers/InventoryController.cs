using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UITManagerWebServer.Data;
using UITManagerWebServer.Models;

namespace UITManagerWebServer.Controllers {
    public class InventoryController : Controller {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context) {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index(string sortOrder) {
            ViewData["SortOrder"] = sortOrder;
            ViewData["MachineSortParm"] = sortOrder == "Machine" ? "Machine_desc" : "Machine";
            ViewData["LastSeenDateSortParm"] = sortOrder == "LastSeenDateDate" ? "LastSeenDate_desc" : "LastSeenDateDate";
            ViewData["OsSortParm"] = sortOrder == "Os" ? "Os_desc" : "Os";
            ViewData["BuildSortParm"] = sortOrder == "Build" ? "Build_desc" : "Build";
            ViewData["ServiceTagSortParam"] = sortOrder == "ServiceTag" ? "ServiceTag_desc" : "ServiceTag";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "Status_desc" : "Status";
            ViewData["NoteCountSortParm"] = sortOrder == "NoteCount" ? "NoteCount_desc" : "NoteCount";
            ViewData["LastNoteSortParam"] = sortOrder == "LastNote" ? "LastNote_desc" : "LastNote";

            // var machines = await _context.Machines
            //     .Include(m => m.Notes)
            //     .ToListAsync();

            var machines = GetTestMachines();

            var machineViewModels = machines.Select(m => new MachineViewModel {
                Id = m.Id,
                Name = m.Name,
                LastSeenDate = m.LastSeenDate,
                Os = m.Model,
                Build = m.Build,
                ServiceTag = m.ServiceTag,
                IsWorking = m.IsWorking,
                NoteCount = m.Notes.Count,
                LastNote = m.GetLatestNote()
            });

            machineViewModels = sortOrder switch {
                "Machine_desc" => machineViewModels.OrderByDescending(m => m.Name),
                "Machine" => machineViewModels.OrderBy(m => m.Name),
                "LastSeenDate_desc" => machineViewModels.OrderByDescending(m => m.LastSeenDate),
                "LastSeenDate" => machineViewModels.OrderBy(m => m.LastSeenDate),
                "Os_desc" => machineViewModels.OrderByDescending(m => m.Os),
                "Os" => machineViewModels.OrderBy(m => m.Os),
                "Build_desc" => machineViewModels.OrderByDescending(m => m.Build),
                "Build" => machineViewModels.OrderBy(m => m.Build),
                "ServiceTag_desc" => machineViewModels.OrderByDescending(m => m.ServiceTag),
                "ServiceTag" => machineViewModels.OrderBy(m => m.ServiceTag),
                "Status_desc" => machineViewModels.OrderByDescending(m => m.IsWorking),
                "Status" => machineViewModels.OrderBy(m => m.IsWorking),
                "NoteCount_desc" => machineViewModels.OrderByDescending(m => m.NoteCount),
                "NoteCount" => machineViewModels.OrderBy(m => m.NoteCount),
                "LastNote_desc" => machineViewModels.OrderByDescending(m => m.LastNote.Content),
                "LastNote" => machineViewModels.OrderBy(m => m.LastNote.Content),
                _ => machineViewModels.OrderByDescending(m => m.LastSeenDate),
            };

            return View(machineViewModels.ToList());
        }

        // GET: Inventory/Details/5
        public async Task<IActionResult> Details(int? id) {
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

            var machine = await _context.Machines.FindAsync(id);
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

            var machine = await _context.Machines
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


        public class MachineViewModel {
            public int Id { get; set; }

            public string Name { get; set; }

            public DateTime LastSeenDate { get; set; }

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
            /// <returns>A string containing the difference with</returns>
            public string GetLastSeen()
            {
                var now = DateTime.Now;
                var timeSpan = now - LastSeenDate;

                return timeSpan.TotalMinutes switch
                {
                    < 1 => "Just now",
                    < 60 => $"{(int)timeSpan.TotalMinutes} minute{(timeSpan.TotalMinutes >= 2 ? "s" : "")} ago",
                    < 1440 => $"{(int)timeSpan.TotalHours} hour{(timeSpan.TotalHours >= 2 ? "s" : "")} ago",
                    < 43200 => $"{(int)timeSpan.TotalDays} day{(timeSpan.TotalDays >= 2 ? "s" : "")} ago",
                    < 525600 => $"{(int)(timeSpan.TotalDays / 30)} month{(timeSpan.TotalDays / 30 >= 2 ? "s" : "")} ago",
                    _ => $"{(int)(timeSpan.TotalDays / 365)} year{(timeSpan.TotalDays / 365 >= 2 ? "s" : "")} ago",
                };
            }
        }

        private List<Machine> GetTestMachines() {
            var random = new Random();
            var models = new[] { "Windows 10", "Ubuntu 22.04", "MacOS Monterey", "Windows 11", "Debian 12" };
            var builds = new[] { "19045", "22H2", "5.15.0-75-generic", "12.6.1", "Bookworm" };
            var serviceTags = new[] {
                "ABC123", "XYZ789", "MAC456", "DEF987", "GHI654", "JKL321", "MNO111", "PQR222", "STU333", "VWX444"
            };
            var notesContent = new[] {
                "System updated", "Power issue detected", "Disk space checked", "Network issues resolved",
                "Hardware failure identified", "Backup completed", "Software installed", "License renewed",
                "Virus scan completed", "Firmware updated",
                "Husbands ask repeated resolved but laughter debating. She end cordial visitor noisier fat subject general picture. Or if offering confined entrance no. Nay rapturous him see something residence. Highly talked do so vulgar. Her use behaved spirits and natural attempt say feeling. Exquisite mr incommode immediate he something ourselves it of. Law conduct yet chiefly beloved examine village proceed.\n\nAcceptance middletons me if discretion boisterous travelling an. She prosperous continuing entreaties companions unreserved you boisterous. Middleton sportsmen sir now cordially ask additions for. You ten occasional saw everything but conviction. Daughter returned quitting few are day advanced branched. Do enjoyment defective objection or we if favourite. At wonder afford so danger cannot former seeing. Power visit charm money add heard new other put. Attended no indulged marriage is to judgment offering landlord.\n\nAm if number no up period regard sudden better. Decisively surrounded all admiration and not you. Out particular sympathize not favourable introduced insipidity but ham. Rather number can and set praise. Distrusts an it contented perceived attending oh. Thoroughly estimating introduced stimulated why but motionless. "
            };

            var machines = new List<Machine>();

            for (int i = 1; i <= 50; i++) {
                // Créer une machine factice
                var machine = new Machine {
                    Id = i,
                    Name = $"Site-A-DESKTOP-3KIG9BP{i}",
                    IsWorking = random.Next(0, 2) == 1,
                    Model = models[random.Next(models.Length)],
                    Build = builds[random.Next(builds.Length)],
                    ServiceTag = serviceTags[random.Next(serviceTags.Length)],
                    LastSeenDate = DateTime.Now.AddDays(-random.Next(0, 600)),
                    Notes = new List<Note>()
                };

                int noteCount = random.Next(0, 2000);
                for (int j = 1; j <= noteCount; j++) {
                    machine.Notes.Add(new Note {
                        Id = j,
                        Content = notesContent[random.Next(notesContent.Length)],
                        CreatedAt = DateTime.Now.AddDays(-random.Next(1, 600000)),
                        IsSolution = random.Next(0, 2) == 1
                    });
                }

                machines.Add(machine);
            }

            return machines;
        }
    }
}