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
                .AsQueryable();

            alarms = sortOrder switch {
                "Machine_desc" => alarms.OrderByDescending(a => a.Machine.Name),
                "Machine" => alarms.OrderBy(a => a.Machine.Name),
                "Status_desc" => alarms.OrderByDescending(a => a.Status),
                "Status" => alarms.OrderBy(a => a.Status),
                "Severity_desc" => alarms.OrderByDescending(a => a.NormGroup.Severity),
                "Severity" => alarms.OrderBy(a => a.NormGroup.Severity),
                "AlarmGroup_desc" => alarms.OrderByDescending(a => a.NormGroup.Name),
                "AlarmGroup" => alarms.OrderBy(a => a.NormGroup.Name),
                "Date_desc" => alarms.OrderByDescending(a => a.TriggeredAt),
                "Date" => alarms.OrderBy(a => a.TriggeredAt),
                _ => alarms.OrderBy(a => a.NormGroup.Severity)
            };

            return View(await alarms.ToListAsync());
        }


        [HttpPost]
        [Route("Alarms/UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request) {
            if (request == null || request.Id == 0 || string.IsNullOrEmpty(request.Status)) {
                return BadRequest("Invalid data.");
            }

            var alarm = await _context.Alarms.FindAsync(request.Id);
            if (alarm == null) {
                return NotFound("Alarm not found.");
            }

            alarm.Status = Enum.Parse<AlarmStatus>(request.Status);
            _context.Alarms.Update(alarm);
            await _context.SaveChangesAsync();

            return Ok("Status updated successfully.");
        }
    }

    public class UpdateStatusRequest {
        public int Id { get; set; }
        public string Status { get; set; }
    }
}