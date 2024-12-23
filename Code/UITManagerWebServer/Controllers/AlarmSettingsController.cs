using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using UITManagerWebServer.Data;
using UITManagerWebServer.Hubs;
using UITManagerWebServer.Models;

namespace UITManagerWebServer.Controllers {
    public class AlarmSettingsController : Controller {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<WebAppHub> _hubContext;

        public AlarmSettingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,IHubContext<WebAppHub> hubContext) {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
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
                Title = "Alarms", Url = Url.Action("Index", "AlarmSettings"), IsActive = false
            });

            string currentAction = context.ActionDescriptor.RouteValues["action"];
            string controllerName = context.ActionDescriptor.RouteValues["controller"];


            switch (currentAction) {
                case "Index":
                    breadcrumbs.Last().IsActive = true;
                    break;
                
                case "Details":
                case "Edit":
                    if (context.ActionArguments.ContainsKey("id") && 
                        int.TryParse(context.ActionArguments["id"]?.ToString(), out int normGroupId)) {

                        var normGroup = _context.NormGroups.FirstOrDefault(a => a.Id == normGroupId);

                        if (normGroup != null) {
                            breadcrumbs.Add(new BreadcrumbItem {
                                Title = normGroup.Name,
                                Url = string.Empty,
                                IsActive = true
                            });
                        }
                    }
                    break;

                case "Create":
                    breadcrumbs.Add(new BreadcrumbItem {
                        Title = "Create an alarm", Url = string.Empty, IsActive = true
                    });

                    break;
            }

            if (controllerName == "AlarmSettings" && context.ActionDescriptor.RouteValues.ContainsKey("id")) {
                if (int.TryParse(context.ActionDescriptor.RouteValues["id"], out int normGroupId2)) {
                    breadcrumbs.Add(new BreadcrumbItem {
                        Title = $"Criteria Group {normGroupId2}", Url = string.Empty, IsActive = true
                    });
                }
            }

            ViewData["Breadcrumbs"] = breadcrumbs;
        }


        public override void OnActionExecuting(ActionExecutingContext context) {
            base.OnActionExecuting(context);

            SetBreadcrumb(context);

            TempData["PreviousUrl"] = Request.Headers["Referer"].ToString();
        }

        [Authorize(Roles = "Maintenance Manager, IT Director")]
        public async Task<IActionResult> Index(string sortOrder) {
            IQueryable<NormGroup> normGroups = _context.NormGroups
                .Include(ng => ng.SeverityHistories)
                .AsQueryable();

            List<NormGroupPageViewModel> viewModels = new List<NormGroupPageViewModel>();

            foreach (NormGroup ng in normGroups) {
                viewModels.Add(new NormGroupPageViewModel {
                    Id = ng.Id, NormGroupName = ng.Name, Priority = ng.Priority, IsEnable = ng.IsEnable
                });
            }

            foreach (NormGroupPageViewModel viewModel in viewModels) {
                viewModel.Severity = await _context.SeverityHistories
                    .Where(sh => sh.IdNormGroup == viewModel.Id)
                    .OrderByDescending(sh => sh.UpdateDate)
                    .Select(sh => sh.Severity.Name)
                    .FirstOrDefaultAsync();

                viewModel.TotalAlarms = await GetTotalAlarms(viewModel.NormGroupName);
            }

            ViewData["SortOrder"] = sortOrder;

            ViewData["AlarmGroupSortParam"] = sortOrder == "AlarmGroup" ? "AlarmGroup_desc" : "AlarmGroup";
            ViewData["SeveritySortParam"] = sortOrder == "Severity" ? "Severity_desc" : "Severity";
            ViewData["PrioritySortParam"] = sortOrder == "Priority" ? "Priority_desc" : "Priority";
            ViewData["NbAlarmSortParam"] = sortOrder == "NbAlarm" ? "NbAlarm_desc" : "NbAlarm";
            ViewData["EnableSortParam"] = sortOrder == "Enable" ? "Enable_desc" : "Enable";

            viewModels = sortOrder switch {
                "AlarmGroup_desc" => viewModels.OrderByDescending(vm => vm.NormGroupName).ToList(),
                "AlarmGroup" => viewModels.OrderBy(vm => vm.NormGroupName).ToList(),
                "Severity_desc" => viewModels.OrderByDescending(vm =>
                    vm.Severity == "Critical" ? 0 :
                    vm.Severity == "High" ? 1 :
                    vm.Severity == "Medium" ? 2 :
                    vm.Severity == "Low" ? 3 :
                    vm.Severity == "Warning" ? 4 : int.MaxValue
                ).ToList(),
                "Severity" => viewModels.OrderBy(vm =>
                    vm.Severity == "Critical" ? 0 :
                    vm.Severity == "High" ? 1 :
                    vm.Severity == "Medium" ? 2 :
                    vm.Severity == "Low" ? 3 :
                    vm.Severity == "Warning" ? 4 : int.MaxValue
                ).ToList(),
                "Priority_desc" => viewModels.OrderByDescending(vm => vm.Priority).ToList(),
                "Priority" => viewModels.OrderBy(vm => vm.Priority).ToList(),
                "NbAlarm_desc" => viewModels.OrderByDescending(vm => vm.TotalAlarms).ToList(),
                "NbAlarm" => viewModels.OrderBy(vm => vm.TotalAlarms).ToList(),
                "Enable_desc" => viewModels.OrderByDescending(vm => vm.IsEnable).ToList(),
                "Enable" => viewModels.OrderBy(vm => vm.IsEnable).ToList(),
                _ => viewModels.OrderBy(vm =>
                    vm.Severity == "Critical" ? 0 :
                    vm.Severity == "High" ? 1 :
                    vm.Severity == "Medium" ? 2 :
                    vm.Severity == "Low" ? 3 :
                    vm.Severity == "Warning" ? 4 : int.MaxValue
                ).ToList(),
            };

            return View(viewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Maintenance Manager, IT Director")]
        public async Task<IActionResult> Delete(int id) {
            NormGroup? normGroup = await _context.NormGroups.FindAsync(id);

            if (normGroup == null) {
                return NotFound();
            }

            _context.NormGroups.Remove(normGroup);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Maintenance Manager, IT Director")]
        public async Task<IActionResult> Details(int? id) {
            if (id == null) return NotFound();
            NormGroup? normGroup = await _context.NormGroups
                .Include(ng => ng.Norms)
                .Include(ng => ng.SeverityHistories)
                .FirstOrDefaultAsync(ng => ng.Id == id);
            ViewData["Severities"] = await _context
                .Severities
                .ToListAsync();
            ;
            ViewData["Info"] = await _context
                .InformationNames
                .ToListAsync();

            ViewData["History"] = await _context
                .SeverityHistories
                .Where(s => s.IdNormGroup == id)
                .ToListAsync();

            List<object> histories = new List<object>();

            foreach (SeverityHistory history in normGroup.SeverityHistories) {
                ApplicationUser? user = await _userManager.FindByIdAsync(history.UserId);
                if (user != null) {
                    histories.Add(new {
                        UserFirstName = user.FirstName,
                        UserLastName = user.LastName,
                        SeverityName = history.Severity.Name,
                        DateUpdate = history.UpdateDate
                    });
                }
            }

            if (normGroup == null) return NotFound();
            ViewData["History"] = histories;
            return View(normGroup);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Maintenance Manager, IT Director")]
        public IActionResult DeleteNorm(int normId) {
            int id = 0;
            Norm? norm = _context.Norms.FirstOrDefault(n => n.Id == normId);
            if (norm != null) {
                id = norm.NormGroupId;
                if (id != 0) {
                    _context.Norms.Remove(norm);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Edit", new { id = id });
        }

        [HttpGet]
        [Authorize(Roles = "Maintenance Manager, IT Director")]
        public async Task<IActionResult> Edit(int id) {
            NormGroup? normGroup = await _context.NormGroups
                .Include(ng => ng.Norms)
                .Include(ng => ng.SeverityHistories)
                .FirstOrDefaultAsync(ng => ng.Id == id);
            if (normGroup == null) {
                return NotFound();
            }

            SeverityHistory latestSeverityHistory = normGroup.GetLatestSeverityHistory();
            int idSeverity = latestSeverityHistory.IdSeverity;
            List<Norm> norms = normGroup.Norms;
            List<SeverityHistory> severityHistories = normGroup.SeverityHistories;
            List<InformationName> informationNames = await _context.InformationNames
                .Include(ng => ng.SubInformationNames)
                .ToListAsync();
            List<Severity> severities = await _context.Severities.ToListAsync();

            NormGroupModel normgroupmodel = new NormGroupModel {
                Id = id,
                IdSeverity = idSeverity,
                IsEnable = normGroup.IsEnable,
                MaxExpectedProcessingTime = normGroup.MaxExpectedProcessingTime,
                Norms = norms,
                Informations = informationNames,
                NormGroupName = normGroup.Name,
                Severities = severities,
                SeverityHistories = severityHistories,
                Priority = normGroup.Priority
            };

            return View(normgroupmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Maintenance Manager, IT Director")]
        public async Task<IActionResult> Edit(NormGroupModel model, string expected) {
            NormGroup? normGroup = await _context.NormGroups
                .Include(ng => ng.Norms)
                .Include(ng => ng.SeverityHistories)
                .FirstOrDefaultAsync(c => c.Id == model.Id);

            Regex regex = new Regex(@"^(\d{1,3})\s(\d{2}):(\d{2}):(\d{2})$");
            Match match = regex.Match(expected);

            if (!match.Success) {
                return View("Index");
            }

            int days = int.Parse(match.Groups[1].Value);
            int hours = int.Parse(match.Groups[2].Value);
            int minutes = int.Parse(match.Groups[3].Value);
            int seconds = int.Parse(match.Groups[4].Value);

            TimeSpan timeSpan = new TimeSpan(days, hours, minutes, seconds);

            if (normGroup == null) {
                return NotFound();
            }

            ApplicationUser user = await _userManager.GetUserAsync(User);
            bool hasErrors = false;
            normGroup.Name = model.NormGroupName;
            normGroup.MaxExpectedProcessingTime = timeSpan;
            normGroup.Priority = model.Priority;
            normGroup.IsEnable = model.IsEnable;

            if (model.IdSeverity != normGroup.GetLatestSeverityHistory().IdSeverity) {
                normGroup.SeverityHistories.Add(
                    new SeverityHistory {
                        IdNormGroup = normGroup.Id,
                        IdSeverity = model.IdSeverity,
                        UpdateDate = DateTime.UtcNow,
                        UserId = user.Id,
                    });
            }

            if (model.Norms.Count == 0) {
                TempData["Error"] = "You cannot have a criteria group without any norms";
                return RedirectToAction("Edit", new { model = model });
            }

            for (int i = 0; i < model.Norms.Count; i++) {
                if (!string.IsNullOrEmpty(model.Norms[i].Name) && !string.IsNullOrEmpty(model.Norms[i].Value)) {
                    try {
                        normGroup.Norms[i].Name = model.Norms[i].Name;
                        normGroup.Norms[i].Value = model.Norms[i].Value;
                        normGroup.Norms[i].InformationNameId = model.Norms[i].InformationNameId;
                        normGroup.Norms[i].Condition = model.Norms[i].Condition;
                        normGroup.Norms[i].Format = model.Norms[i].Format;
                    }
                    catch (Exception e) {
                        normGroup.Norms.Add(new Norm {
                            Name = model.Norms[i].Name,
                            Value = model.Norms[i].Value,
                            InformationNameId = model.Norms[i].InformationNameId,
                            Condition = model.Norms[i].Condition,
                            Format = model.Norms[i].Format,
                        });
                    }
                }
                else {
                    TempData["Error"] = "Norm should have a name and a value";
                    hasErrors = true;
                }
            }

            SeverityHistory latestSeverityHistory = normGroup.GetLatestSeverityHistory();
            latestSeverityHistory.IdSeverity = model.IdSeverity;

            _context.Update(normGroup);
            await _context.SaveChangesAsync();
            if (hasErrors) {
                return RedirectToAction("Edit", new { model = model });
            }
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", normGroup.Id);

            return RedirectToAction("Details", new { id = model.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Maintenance Manager, IT Director")]
        public async Task<IActionResult> ToggleIsEnable(int id, bool isEnable) {
            NormGroup? normGroup = await _context.NormGroups.FindAsync(id);

            if (normGroup == null) {
                return NotFound();
            }

            normGroup.IsEnable = isEnable;
            _context.Update(normGroup);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Maintenance Manager, IT Director")]
        public async Task<IActionResult> Create() {
            ViewData["InformationsName"] = await _context
                .InformationNames
                .Include(n => n.SubInformationNames)
                .ToListAsync();
            ViewData["SeveritiesName"] = await _context.Severities.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Maintenance Manager, IT Director")]
        public async Task<IActionResult> Create(NormGroupModel normGroupModel, string expected) {
            Regex regex = new Regex(@"^(\d{1,3})\s(\d{2}):(\d{2}):(\d{2})$");
            Match match = regex.Match(expected);

            if (!match.Success) {
                return View("Index");
            }

            int days = int.Parse(match.Groups[1].Value);
            int hours = int.Parse(match.Groups[2].Value);
            int minutes = int.Parse(match.Groups[3].Value);
            int seconds = int.Parse(match.Groups[4].Value);

            TimeSpan timeSpan = new TimeSpan(days, hours, minutes, seconds);
            ApplicationUser user = await _userManager.GetUserAsync(User);

            List<SeverityHistory> sh = new List<SeverityHistory> {
                new SeverityHistory {
                    IdSeverity = normGroupModel.IdSeverity, UserId = user.Id, UpdateDate = DateTime.UtcNow
                }
            };

            List<Norm> norms =
                normGroupModel.Norms.FindAll(n => !string.IsNullOrEmpty(n.Name) && !string.IsNullOrEmpty(n.Value));
            if (norms.Count == 0) {
                TempData["Error"] = "You cannot create a group without any valid norms";
                return RedirectToAction("Create");
            }

            NormGroup toAddNormGroup = new NormGroup {
                IsEnable = normGroupModel.IsEnable,
                Name = normGroupModel.NormGroupName,
                SeverityHistories = sh,
                MaxExpectedProcessingTime = timeSpan,
                Priority = normGroupModel.Priority,
                Norms = norms,
            };
            toAddNormGroup.SeverityHistories[0].IdNormGroup = toAddNormGroup.Id;
            if (!string.IsNullOrEmpty(toAddNormGroup.Name)) {
                _context.NormGroups.Add(toAddNormGroup);
                await _context.SaveChangesAsync();
                
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", normGroupModel.Id);
            }
            else {
                TempData["Error"] = "You cannot have a criteria group without a name";
                return View(normGroupModel);
            }

            return RedirectToAction("Index");
        }

        private bool NormGroupExists(int id) {
            return _context.NormGroups.Any(e => e.Id == id);
        }

        private async Task<int> GetTotalAlarms(string normGroupName) {
            return await _context.Alarms
                .Where(a => a.NormGroup.Name == normGroupName &&
                            a.AlarmHistories
                                .OrderByDescending(h => h.ModificationDate)
                                .FirstOrDefault()!
                                .StatusType.Name != "Resolved")
                .CountAsync();
        }


        private async Task<string> GetLastSeverity(int idNormGroup) {
            return await _context.SeverityHistories
                .Where(sh => sh.IdNormGroup == idNormGroup)
                .OrderByDescending(sh => sh.UpdateDate)
                .Select(sh => sh.Severity.Name)
                .FirstOrDefaultAsync();
        }

        public class SeverityHistoryModel {
            public int Id { get; set; }
            public Severity? Severity { get; set; }
            public int SeverityId { get; set; }
            public string UserId { get; set; }
            public string UserName { get; set; }
            public string UserFirstName { get; set; }
        }

        public class NormGroupModel {
            public int Id { get; set; }
            public string? NormGroupName { get; set; }
            public int IdSeverity { get; set; }
            public int Priority { get; set; }
            public TimeSpan MaxExpectedProcessingTime { get; set; }
            public bool IsEnable { get; set; }

            public List<Severity> Severities { get; set; } = new();
            public List<Norm> Norms { get; set; } = new();
            public List<SeverityHistory>? SeverityHistories { get; set; } = new();
            public List<InformationName> Informations { get; set; } = new();
            public List<int> IdNormToDelete { get; set; } = new();
        }

        public class NormGroupPageViewModel {
            public int Id { get; set; }
            public string? NormGroupName { get; set; }
            public string? Severity { get; set; }
            public int Priority { get; set; }
            public int TotalAlarms { get; set; }
            public bool IsEnable { get; set; }
        }
    }
}