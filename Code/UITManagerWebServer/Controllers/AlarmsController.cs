using Microsoft.AspNetCore.Identity;
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
    public class AlarmsController : Controller {
        private readonly ApplicationDbContext _context;

        public AlarmsController(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder) {
            ViewData["SortOrder"] = sortOrder;

            ViewData["MachineSortParm"] = sortOrder == "Machine" ? "Machine_desc" : "Machine";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "Status_desc" : "Status";
            ViewData["SeveritySortParm"] = sortOrder == "Severity" ? "Severity_desc" : "Severity";
            ViewData["AlarmGroupSortParm"] = sortOrder == "AlarmGroup" ? "AlarmGroup_desc" : "AlarmGroup";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "Date_desc" : "Date";

            var alarms = _context.Alarms
                .Include(a => a.Machine)
                .Include(a => a.NormGroup)
                .Include(a => a.AlarmHistories)
                .ThenInclude(aStatus => aStatus.StatusType)
                .AsQueryable();

            alarms = sortOrder switch {
                "Machine_desc" => alarms.OrderByDescending(a => a.Machine.Name),
                "Machine" => alarms.OrderBy(a => a.Machine.Name),
                "Status_desc" => alarms.OrderByDescending(a => a.GetLatestAlarmHistory().StatusType.Name),
                "Status" => alarms.OrderBy(a => a.GetLatestAlarmHistory().StatusType.Name),
                //"Severity_desc" => alarms.OrderByDescending(a => a.NormGroup.Severity),
                //"Severity" => alarms.OrderBy(a => a.NormGroup.Severity),
                "AlarmGroup_desc" => alarms.OrderByDescending(a => a.NormGroup.Name),
                "AlarmGroup" => alarms.OrderBy(a => a.NormGroup.Name),
                "Date_desc" => alarms.OrderByDescending(a => a.TriggeredAt),
                "Date" => alarms.OrderBy(a => a.TriggeredAt),
                //_ => alarms.OrderBy(a => a.NormGroup.Severity)
                _ => alarms.OrderBy(a => a.Machine.Name)
            };

            ViewData["AlarmStatusTypes"] = await _context.AlarmStatusTypes.ToListAsync();
            
            return View(await alarms.ToListAsync());
        }


        [HttpPost]
        [Route("Alarms/UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request) {
            if (request == null || request.Id == 0 || string.IsNullOrEmpty(request.Status)) {
                return BadRequest("Invalid data.");
            }

            var alarm = await _context.Alarms.Include(a => a.AlarmHistories).FirstOrDefaultAsync(a => a.Id == request.Id);
            if (alarm == null) {
                return NotFound("Alarm not found.");
            }

            var statusType = await _context.AlarmStatusTypes.FirstOrDefaultAsync(s => s.Name == request.Status);
            if (statusType == null) {
                return BadRequest("Invalid status.");
            }

            var newAlarmHistory = new AlarmStatusHistory {
                StatusTypeId = statusType.Id, ModificationDate = DateTime.UtcNow, ModifierId = 1 // A CHANGER PLUS TARD POUR L'ID DE L'UTILISATEUR EN SESSION
            };
            
            alarm.AddAlarmHistory(newAlarmHistory);
            _context.Alarms.Update(alarm);
            await _context.SaveChangesAsync();

            return Ok("Status updated successfully.");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Alarm alarm) {
            if (ModelState.IsValid) {
                var newStatusType = _context.AlarmStatusTypes
                    .FirstOrDefault(s => s.Name == "New");

                if (newStatusType == null) {
                    ModelState.AddModelError("", "Le statut 'New' est introuvable dans la base de données.");
                    return View(alarm);
                }

                var alarmHistory = new AlarmStatusHistory {
                    StatusTypeId = newStatusType.Id,
                    ModificationDate = DateTime.Now,
                    ModifierId = null              
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

    public class UpdateStatusRequest {
        public int Id { get; set; }
        public string Status { get; set; }
    }
}