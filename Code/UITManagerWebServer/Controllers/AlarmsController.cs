using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UITManagerWebServer.Data;
using UITManagerWebServer.Models;

namespace UITManagerWebServer.Controllers
{
    public class AlarmsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public AlarmsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["SortOrder"] = sortOrder;
            ViewData["WhoIsAssigned"] = sortOrder;
            ViewData["MachineSortParm"] = sortOrder == "Machine" ? "Machine_desc" : "Machine";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "Status_desc" : "Status";
            ViewData["SeveritySortParm"] = sortOrder == "Severity" ? "Severity_desc" : "Severity";
            ViewData["AlarmGroupSortParm"] = sortOrder == "AlarmGroup" ? "AlarmGroup_desc" : "AlarmGroup";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "Date_desc" : "Date";
            ViewData["ModelSortParam"] = sortOrder == "Model" ? "Model_desc" : "Model";
            ViewData["AttributionSortParam"] = sortOrder == "Attribution" ? "Attribution_desc" : "Attribution";

            var users = _context.Users.ToList();
            var user = await _userManager.GetUserAsync(User);
            var alarms = _context.Alarms
                .Include(a => a.Machine)
                .Include(a => a.NormGroup)
                .Include(a => a.AlarmHistories)
                .ThenInclude(aStatus => aStatus.StatusType)
                .Include(a => a.NormGroup.SeverityHistories) // Include SeverityHistories
                .ThenInclude(sh => sh.Severity) // Include related Severity
                .Include(a => a.User) // Include the User related to the Alarm
                .AsQueryable()
                .Where(a => a.AlarmHistories
                    .OrderByDescending(h => h.ModificationDate)
                    .Select(h => h.StatusType.Name)
                    .FirstOrDefault() != "Resolved");


            alarms = sortOrder switch
            {
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

                "default" => alarms.OrderBy(a => true),
                "b" => alarms.Where(a => a.AlarmHistories.Any() &&
                                         a.AlarmHistories
                                             .OrderByDescending(h => h.ModificationDate) 
                                             .First().User.Id == user.Id),
                "c" => alarms.Where(a => a.UserId == null),
                _ => alarms.OrderBy(a => a.Machine.Name)
            };

            ViewData["AlarmStatusTypes"] = await _context.AlarmStatusTypes.ToListAsync();
            ViewData["user"] = users;
            ViewData["WhoIsAssigned"] = sortOrder ?? "a";
            
            if (User.IsInRole("Admin"))
            {
                ViewData["Role"] = "Admin";
            }
            else if (User.IsInRole("User"))
            {
                ViewData["Role"] = "User";
            }
            else
            {
                ViewData["Role"] = "Unknown";
            }
            return View(await alarms.ToListAsync());
        }


        [HttpPost]
        [Authorize(Roles = "Technician , ITDirector, MaintenanceManager")]
        [Route("Alarms/UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            if (request == null || request.Id == 0 || string.IsNullOrEmpty(request.Status))
            {
                return BadRequest(new { success = false, message = "Invalid data." });
            }

            var alarm = await _context.Alarms
                .Include(a => a.AlarmHistories)
                .FirstOrDefaultAsync(a => a.Id == request.Id);

            if (alarm == null)
            {
                return NotFound(new { success = false, message = "Alarm not found." });
            }

            var statusType = await _context.AlarmStatusTypes.FirstOrDefaultAsync(s => s.Name == request.Status);
            if (statusType == null)
            {
                return BadRequest(new { success = false, message = "Invalid status." });
            }

            var newAlarmHistory = new AlarmStatusHistory
            {
                StatusTypeId = statusType.Id,
                ModificationDate = DateTime.UtcNow,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            };

            alarm.AlarmHistories.Add(newAlarmHistory);

            try
            {
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Status updated successfully." });
            }

            catch (Exception ex)
            {
                // Log en cas d'exception
                // Exemple : _logger.LogError(ex, "Error updating alarm status.");
                return StatusCode(500, new { success = false, message = "An error occurred while updating status." });
            }
        }


        [HttpPost]
        [Authorize(Roles = "ITDirector, MaintenanceManager")]
        [Route("Alarms/Attribution")]
        public async Task<IActionResult> UpdateAttribution([FromBody] UpdateAssignedUserRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Id) || string.IsNullOrEmpty(request.UserId))
            {
                return BadRequest(new { success = false, message = "Invalid request data." });
            }

            try
            {
                // Récupérer l'alarme correspondant à l'ID
                var alarm = await _context.Alarms
                    .Include(a => a.Machine)
                    .Include(a => a.NormGroup)
                    .FirstOrDefaultAsync(a => a.Id.ToString() == request.Id);

                if (alarm == null)
                {
                    return NotFound(new { success = false, message = "Alarm not found." });
                }

                // Vérifier si l'utilisateur existe
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "User not found." });
                }

                // Mettre à jour l'utilisateur associé à l'alarme
                alarm.UserId = request.UserId;

                // Ajouter un historique de statut pour refléter le changement d'attribution
                var alarmStatusHistory = new AlarmStatusHistory
                {
                    AlarmId = alarm.Id,
                    StatusType = await _context.AlarmStatusTypes.FirstOrDefaultAsync(s => s.Name == "In Progress"), // Exemple de statut
                    ModificationDate = DateTime.UtcNow,
                    UserId = request.UserId
                };

                _context.AlarmHistories.Add(alarmStatusHistory);

                // Enregistrer les modifications
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Alarm attribution updated successfully." });
            }
            catch (Exception ex)
            {
                // Gestion des erreurs
                return StatusCode(500, new { success = false, message = "An error occurred while updating attribution.", error = ex.Message });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Alarm alarm)
        {
            if (ModelState.IsValid)
            {
                var newStatusType = _context.AlarmStatusTypes
                    .FirstOrDefault(s => s.Name == "New");

                if (newStatusType == null)
                {
                    ModelState.AddModelError("", "Le statut 'New' est introuvable dans la base de données.");
                    return View(alarm);
                }

                var alarmHistory = new AlarmStatusHistory
                {
                    StatusTypeId = newStatusType.Id,
                    ModificationDate = DateTime.Now,
                    // UserId = null              
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
    }

    public class UpdateStatusRequest
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }

    public class UpdateAssignedUserRequest
    {
        public string Id { get; set; } // ID de l'alarme
        public string UserId { get; set; } // ID de l'utilisateurD du nouvel utilisateur attribué
    }
}