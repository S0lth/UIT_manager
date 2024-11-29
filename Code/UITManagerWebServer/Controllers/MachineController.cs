using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UITManagerWebServer.Data;
using UITManagerWebServer.Models;

namespace UITManagerWebServer.Controllers
{
    public class MachineController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MachineController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Machine
        public async Task<IActionResult> Index()
        {
            return View(await _context.Machines.ToListAsync());
        }
        

        private bool MachineExists(int id)
        {
            return _context.Machines.Any(e => e.Id == id);
        }
    }
}
