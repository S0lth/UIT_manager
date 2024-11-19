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

        public async Task<IActionResult> Index(string sortOrder)
        {
      
            
            // Oldest unprocessed alarms
            List<Alarm> oldestUnprocessedAlarms = await _context.Alarms
                .Include(a => a.Machine)
                .Include(a => a.NormGroup)
                .Where(a => a.Status == AlarmStatus.New)
                .ToListAsync(); // Charge la liste en mémoire



            
            
            // 5 most recently triggered new alarms
            var recentNewAlarms = await _context.Alarms
                .Include(a => a.Machine)
                .Include(a => a.NormGroup)
                .OrderByDescending(a => a.TriggeredAt)
                .Take(5)
                .ToListAsync();

  
            // Last notes
            var lastNotes = await _context.Notes
                .Include(n => n.Machine)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            
            
            var model = new HomePageViewModel
            {
                OldestUnprocessedAlarms = oldestUnprocessedAlarms,
                RecentNewAlarms = recentNewAlarms,
                LastNotes = lastNotes
            };

            return View(model);
        }
    

    public class HomePageViewModel
    {
        public List<Alarm> OldestUnprocessedAlarms { get; set; }
        public List<Alarm> RecentNewAlarms { get; set; }
        public List<Note> LastNotes { get; set; }
    }
}