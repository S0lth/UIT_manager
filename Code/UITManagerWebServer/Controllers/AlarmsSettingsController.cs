using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UITManagerWebServer.Data;

namespace UITManagerWebServer.Controllers {
    public class AlarmsSettingsController : Controller {
        private readonly ApplicationDbContext _context;

        public AlarmsSettingsController(ApplicationDbContext context) {
            _context = context;
        }
        
        public async Task<IActionResult> Details(int? id) {
            if (id == null) return NotFound();
            
            var normGroup = await _context.NormGroups
                .Include(ng => ng.Norms)
                .Include(ng => ng.SeverityHistories)
                .FirstOrDefaultAsync(ng => ng.Id == id);
            if(normGroup == null) return NotFound();
            var severities = await _context.Severities
                .ToListAsync();
            ViewData["Severities"] = severities;

            return View(normGroup);
        }
    }
}