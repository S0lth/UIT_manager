using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UITManagerWebServer.Data;
using UITManagerWebServer.Models;

namespace UITManagerWebServer.Controllers {
    public class AlarmSettingsController : Controller {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AlarmSettingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "MaintenanceManager,ITDirector")]
        public async Task<IActionResult> Index(string sortOrder) {

            var normGroups = _context.NormGroups
                .Include(ng => ng.SeverityHistories)
                .AsQueryable();

            var viewModels = new List<NormGroupPageViewModel>();

            foreach (var ng in normGroups) {
                
                viewModels.Add(new NormGroupPageViewModel {
                    Id = ng.Id,
                    NormGroupName = ng.Name,
                    Priority = ng.Priority,
                    IsEnable = ng.IsEnable
                });
            }
            
            foreach (var viewModel in viewModels) {
                viewModel.Severity = await _context.SeverityHistories
                    .Where(sh => sh.IdNormGroup == viewModel.Id)
                    .OrderByDescending(sh => sh.UpdateDate)
                    .Select(sh => sh.Severity.Name)
                    .FirstOrDefaultAsync();
                
                viewModel.TotalAlarms = await GetTotalAlarms(viewModel.NormGroupName);
            }
            
            ViewData["SortOrder"] = sortOrder;

            ViewData["AlarmGroupSortParm"] = sortOrder == "AlarmGroup" ? "AlarmGroup_desc" : "AlarmGroup";
            ViewData["SeveritySortParm"] = sortOrder == "Severity" ? "Severity_desc" : "Severity";
            ViewData["PrioritySortParm"] = sortOrder == "Priority" ? "Priority_desc" : "Priority";
            ViewData["NbAlarmSortParm"] = sortOrder == "NbAlarm" ? "NbAlarm_desc" : "NbAlarm";
            ViewData["EnableSortParm"] = sortOrder == "Enable" ? "Enable_desc" : "Enable";
            
            
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
        public async Task<IActionResult> Delete(int id)
        {
            var normGroup = await _context.NormGroups.FindAsync(id);

            if (normGroup == null)
            {
                return NotFound();
            }

            _context.NormGroups.Remove(normGroup);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        
        [Authorize(Roles = "MaintenanceManager,ITDirector")]
        public async Task<IActionResult> Details(int? id) {
            
            if (id == null) return NotFound();
            var normGroup = await _context.NormGroups
                .Include(ng => ng.Norms)
                .Include(ng => ng.SeverityHistories)
                .FirstOrDefaultAsync(ng => ng.Id == id);
            
            if(normGroup == null) return NotFound();
            
            ViewData["Severities"] = await _context
                .Severities
                .ToListAsync();;
            
            ViewData["Info"] = await _context
                .InformationNames
                .ToListAsync();
            
            ViewData["History"]  =  await _context
                .SeverityHistories
                .Where(s => s.IdNormGroup == id)
                .ToListAsync();
            
            return View(normGroup);
        }
        
        [HttpPost]
        public IActionResult DeleteNorm(int normId) {
            int id = 0;
            var norm = _context.Norms.FirstOrDefault(n => n.Id == normId);
            if (norm != null) {
                id = norm.NormGroupId;
                if (id != 0) {
                    _context.Norms.Remove(norm);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Details", new { id = id });
        }


        [HttpGet]
        [Authorize(Roles = "MaintenanceManager,ITDirector")]
        public async Task<IActionResult> Edit(int id)
        {
            var normGroup = await _context.NormGroups
                .Include(ng => ng.Norms)
                .Include(ng => ng.SeverityHistories)
                .FirstOrDefaultAsync(ng => ng.Id == id);
            if (normGroup == null) {
                return NotFound();
            }

            var latestSeverityHistory = normGroup.GetLatestSeverityHistory();
            var idSeverity = latestSeverityHistory.IdSeverity;

            var norms = normGroup.Norms;
            var severityHistories = normGroup.SeverityHistories;

            var informationNames = await _context.InformationNames.ToListAsync();
            var severities = await _context.Severities.ToListAsync();

            var normgroupmodel = new NormGroupModel {
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
        public async Task<IActionResult> Edit(NormGroupModel model)
        {
            var normGroup = await _context.NormGroups
                .Include(ng => ng.Norms)
                .Include(ng => ng.SeverityHistories)
                .FirstOrDefaultAsync(c => c.Id == model.Id);
            if (normGroup == null)
            {
                return NotFound();
            }
            ApplicationUser user = await _userManager.GetUserAsync(User);
            
            normGroup.Name = model.NormGroupName;
            normGroup.MaxExpectedProcessingTime = model.MaxExpectedProcessingTime;
            normGroup.Priority = model.Priority;
            normGroup.IsEnable = model.IsEnable;
            normGroup.SeverityHistories.Add(
                new SeverityHistory {
                    IdNormGroup = normGroup.Id,
                    IdSeverity = model.IdSeverity,
                    UpdateDate = DateTime.UtcNow,
                    UserId = user.Id,
                });

            for (int i = 0; i < model.Norms.Count; i++) {
                normGroup.Norms[i].Name = model.Norms[i].Name;
            }
            foreach (var n in model.Norms) {
                Console.WriteLine(n.Name);
            }
            var latestSeverityHistory = normGroup.GetLatestSeverityHistory();
            latestSeverityHistory.IdSeverity = model.IdSeverity;
            
            _context.Update(normGroup);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = model.Id });
        }

       

        
        [HttpPost]
        public async Task<IActionResult> ToggleIsEnable(int id, bool isEnable)
        {
            var normGroup = await _context.NormGroups.FindAsync(id);
            
            if (normGroup == null)
            {
                return NotFound();
            }
            
            normGroup.IsEnable = isEnable;  
            _context.Update(normGroup);    
            await _context.SaveChangesAsync(); 
        
            return RedirectToAction(nameof(Index)); 
        }


        private bool NormGroupExists(int id)
        {
            return _context.NormGroups.Any(e => e.Id == id);
        }
        
        private async Task<int> GetTotalAlarms(string normGroupName) {
            return await _context.Alarms
                .Where(a => a.NormGroup.Name == normGroupName)
                .CountAsync();
        }

        private async Task<string> GetLastSeverity(int idNormGroup) {
            return await _context.SeverityHistories
                .Where(sh => sh.IdNormGroup == idNormGroup)
                .OrderByDescending(sh => sh.UpdateDate)
                .Select(sh => sh.Severity.Name)
                .FirstOrDefaultAsync();         
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
            public List<SeverityHistory> SeverityHistories { get; set; } = new();
            public List<InformationName> Informations { get; set; } = new();
        }

        public class NormGroupPageViewModel {
            
            public int Id {get;set;}
            public string? NormGroupName { get; set; }
            public string? Severity { get; set; }
            public int Priority { get; set; }
            public int TotalAlarms { get; set; }
            public bool IsEnable { get; set; }
        }
    }
}
