using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using UITManagerWebServer.Data;
using UITManagerWebServer.Models;

namespace UITManagerWebServer.Controllers;

public class HomeController : Controller {
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context) {
        _context = context;
    }

    public async Task<IActionResult> Index(string sortOrder) {
        var alarms = await _context.Alarms
            .Include(a => a.Machine)
            .Include(a => a.NormGroup)
            .Include(a => a.AlarmHistories)
            .ThenInclude(s => s.StatusType)
            .ToListAsync();

        var oldestUnprocessedAlarms = alarms
            .Where(a => a.GetLatestAlarmHistory()?.StatusType.Name == "New")
            .ToList();

        var recentNewAlarms = alarms
            .Where(a => a.GetLatestAlarmHistory()?.StatusType.Name == "New")
            .OrderByDescending(a => a.TriggeredAt)
            .Take(5)
            .ToList();

        var totalMachines = await _context.Machines.CountAsync();

        var machinesWithAlarms = await _context.Alarms
            .Select(a => a.MachineId)
            .Distinct()
            .CountAsync();

        var lastNotes = await _context.Notes
            .Include(n => n.Machine)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        var alarmTypeOccurrences = await _context.Alarms
            .Include(a => a.NormGroup)
            .GroupBy(a => a.NormGroup.Name)
            .Select(group => new { Type = group.Key, Count = group.Count() })
            .ToDictionaryAsync(g => g.Type, g => g.Count);

        var model = new HomePageViewModel {
            OldestUnprocessedAlarms = oldestUnprocessedAlarms,
            RecentNewAlarms = recentNewAlarms,
            LastNotes = lastNotes,
            AlarmTypeOccurrences = alarmTypeOccurrences,
            TotalMachines = totalMachines,
            MachinesWithAlarms = machinesWithAlarms
        };

        ViewData["AlarmTypeOccurrences"] = System.Text.Json.JsonSerializer.Serialize(model.AlarmTypeOccurrences);

        return View(model);
    }


    public class HomePageViewModel {
        public List<Alarm> OldestUnprocessedAlarms { get; set; }
        public List<Alarm> RecentNewAlarms { get; set; }
        public List<Note> LastNotes { get; set; }
        public Dictionary<string, int> AlarmTypeOccurrences { get; set; }
        public int TotalMachines { get; set; }
        public int MachinesWithAlarms { get; set; }
    }
}