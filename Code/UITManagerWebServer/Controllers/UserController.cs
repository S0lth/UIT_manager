using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UITManagerWebServer.Data;
using UITManagerWebServer.Models;

namespace UITManagerWebServer.Controllers
{
    public class UserController : Controller {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context) {
            _context = context;
        }

        [Authorize(Roles = "ITDirector")]
        public async Task<IActionResult> Index(string sortOrder) {

            var userRolesDb = _context.Users
                .Select(user => new {
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    StartDate = user.StartDate,  
                    EndDate = user.EndDate,
                    IsActivate = user.IsActivate,
                    Role = _context.UserRoles
                        .Where(ur => ur.UserId == user.Id)
                        .Join(_context.Roles,
                            ur => ur.RoleId,
                            r => r.Id,
                            (ur, r) => r.Name)
                        .FirstOrDefault()
                });

            var users = new List<UserViewModel> ();
            
            foreach (var user in userRolesDb) {
                users.Add(
                    new UserViewModel {
                        Id = user.UserId,
                        FullName = user.LastName + " " + user.FirstName,
                        Email = user.Email,
                        StartDate = user.StartDate,
                        EndDate = user.EndDate,
                        IsActivate = user.IsActivate,
                        Role = user.Role
                    }
                );
            }
            
            ViewData["SortOrder"] = sortOrder;

            ViewData["FullNameSortParm"] = sortOrder == "FullName" ? "FullName_desc" : "FullName";
            ViewData["EmailSortParm"] = sortOrder == "Email" ? "Email_desc" : "Email";
            ViewData["RoleSortParm"] = sortOrder == "Role" ? "Role_desc" : "Role";
            ViewData["StartDateSortParm"] = sortOrder == "StartDate" ? "StartDate_desc" : "StartDate";
            ViewData["EndDateSortParm"] = sortOrder == "EndDate" ? "EndDate_desc" : "EndDate";
            ViewData["ActiveSortParm"] = sortOrder == "Active" ? "Active_desc" : "Active";

            users = new List<UserViewModel>(sortOrder switch {
                "FullName_desc" => users.OrderByDescending(u => u.FullName),
                "FullName" => users.OrderBy(u => u.FullName),
                "Email_desc" => users.OrderByDescending(u => u.Email),
                "Email" => users.OrderBy(u => u.Email),
                "Role_desc" => users.OrderByDescending(u => u.Role),
                "Role" => users.OrderBy(u => u.Role),
                "StartDate_desc" => users.OrderByDescending(u => u.StartDate),
                "StartDate" => users.OrderBy(u => u.StartDate),
                "EndDate_desc" => users.OrderByDescending(u => u.EndDate),
                "EndDate" => users.OrderBy(u => u.EndDate),
                "Active_desc" => users.OrderByDescending(u => u.IsActivate),
                "Active" => users.OrderBy(u => u.IsActivate),
                _ => users.OrderBy(u => u.FullName)
            });
            return View(users);
        }
        
        [HttpPost]
        [Authorize(Roles = "ITDirector")]
        public async Task<IActionResult> ToggleIsActive(string id, bool isActive) {
            var user = await _context.Users.FindAsync(id);
            if (user == null) {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == id) {
                return RedirectToAction(nameof(Index));
            }

            if (isActive) {
                user.StartDate = DateTime.UtcNow;
                user.EndDate = null;
            }
            else {
                user.EndDate = DateTime.UtcNow;
            }

            user.IsActivate = isActive;
            _context.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public class UserViewModel {
            public string Id {get; set;}
            public string FullName { get; set; }
            public string Email { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public bool IsActivate { get; set; }
            public string Role { get; set; }
        }

    }
}
