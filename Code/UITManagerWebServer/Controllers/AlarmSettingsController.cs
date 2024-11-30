using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UITManagerWebServer.Data;
using UITManagerWebServer.Models;

namespace UITManagerWebServer.Controllers {
    public class AlarmSettingsController : Controller {
        private readonly ApplicationDbContext _context;

        public AlarmSettingsController(ApplicationDbContext context) {
            _context = context;
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

        
        [HttpPost]
        public async Task<IActionResult> ToggleIsEnable(int id, bool isEnable)
        {
            var normGroup = await _context.NormGroups.FindAsync(id);
            
            if (normGroup == null)
            {
                return NotFound();
            }
            
            normGroup.IsEnable = isEnable;  // Mise à jour du champ IsEnable
            _context.Update(normGroup);     // Mettre à jour l'entité
            await _context.SaveChangesAsync();  // Sauvegarder les modifications en base de données
        
            return RedirectToAction(nameof(Index)); // Rediriger vers l'index après modification
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
