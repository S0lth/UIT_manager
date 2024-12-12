using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UITManagerWebServer.Data;
using UITManagerWebServer.Models;
using File = UITManagerWebServer.Models.File;

namespace UITManagerWebServer.Controllers
{
    public class NoteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NoteController(ApplicationDbContext context)
        {
            _context = context;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        
            TempData["PreviousUrl"] = Request.Headers["Referer"].ToString();
        }

        // GET: Notes
        [HttpGet]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string search, bool? isSolution, string authorId, string sortOrder)
        {
            ViewData["Search"] = search;
            ViewData["IsSolution"] = isSolution;
            ViewData["AuthorId"] = authorId;

            ViewBag.Authors = await _context.Users.ToListAsync();
            
            if (string.IsNullOrEmpty(sortOrder))
            {
                sortOrder = "createdat";
            }

            ViewData["SortOrder"] = sortOrder;
            ViewData["TitleSortParm"] = sortOrder == "title" ? "title_desc" : "title";
            ViewData["MachineSortParm"] = sortOrder == "machine" ? "machine_desc" : "machine";
            ViewData["CreatedAtSortParm"] = sortOrder == "createdat" ? "createdat_desc" : "createdat";
            ViewData["AuthorSortParm"] = sortOrder == "author" ? "author_desc" : "author";
            ViewData["IsSolutionSortParm"] = sortOrder == "issolution" ? "issolution_desc" : "issolution";

            IQueryable<Note> notesQuery = _context.Notes.Include(n => n.Author).Include(n => n.Machine).AsQueryable();
            
            if (!string.IsNullOrEmpty(search))
            {
                notesQuery = notesQuery.Where(n =>
                    n.Title.Contains(search) ||
                    n.Content.Contains(search) ||
                    n.Author.LastName.Contains(search) ||
                    n.Machine.Name.Contains(search)
                );
            }

            if (isSolution.HasValue)
            {
                notesQuery = notesQuery.Where(n => n.IsSolution == isSolution);
            }

            if (!string.IsNullOrEmpty(authorId))
            {
                notesQuery = notesQuery.Where(n => n.AuthorId == authorId);
            }

            switch (sortOrder)
            {
                case "title":
                    notesQuery = notesQuery.OrderBy(n => n.Title);
                    break;
                case "title_desc":
                    notesQuery = notesQuery.OrderByDescending(n => n.Title);
                    break;
                case "machine":
                    notesQuery = notesQuery.OrderBy(n => n.Machine.Name);
                    break;
                case "machine_desc":
                    notesQuery = notesQuery.OrderByDescending(n => n.Machine.Name);
                    break;
                case "createdat":
                    notesQuery = notesQuery.OrderBy(n => n.CreatedAt);
                    break;
                case "createdat_desc":
                    notesQuery = notesQuery.OrderByDescending(n => n.CreatedAt);
                    break;
                case "author":
                    notesQuery = notesQuery.OrderBy(n => n.Author.LastName);
                    break;
                case "author_desc":
                    notesQuery = notesQuery.OrderByDescending(n => n.Author.LastName);
                    break;
                case "issolution":
                    notesQuery = notesQuery.OrderBy(n => n.IsSolution);
                    break;
                case "issolution_desc":
                    notesQuery = notesQuery.OrderByDescending(n => n.IsSolution);
                    break;
                default:
                    notesQuery = notesQuery.OrderBy(n => n.CreatedAt);
                    break;
            }

            List<Note> notes = await notesQuery.ToListAsync();

            return View(notes);
        }

        // GET: Notes/Details/5
        [HttpGet]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            Note? note = await _context.Notes
                .Include(n => n.Author)
                .Include(n => n.Machine)
                .Include(n => n.Files)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (note == null)
            {
                return NotFound();
            }
            
            foreach (File file in note.Files)
            {
                string fileUrl = $"{Request.Scheme}://{Request.Host}/files/{file.Id}";
                note.Content = note.Content.Replace($"({file.FileName})", $"({fileUrl})");
            }

            return View(note);
        }

        // GET: Notes/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["MachineId"] = new SelectList(_context.Machines, "Id", "Id");
            return View();
        }

        // POST: Notes/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Content,CreatedAt,AuthorId,MachineId,IsSolution")] Note note)
        {
            if (ModelState.IsValid)
            {
                _context.Add(note);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", note.AuthorId);
            ViewData["MachineId"] = new SelectList(_context.Machines, "Id", "Id", note.MachineId);
            return View(note);
        }

        // GET: Notes/Edit/5
        [HttpGet]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Note? note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", note.AuthorId);
            ViewData["MachineId"] = new SelectList(_context.Machines, "Id", "Id", note.MachineId);
            return View(note);
        }

        // POST: Notes/Edit/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content,CreatedAt,AuthorId,MachineId,IsSolution")] Note note)
        {
            if (id != note.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(note);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoteExists(note.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", note.AuthorId);
            ViewData["MachineId"] = new SelectList(_context.Machines, "Id", "Id", note.MachineId);
            return View(note);
        }

        // GET: Notes/Delete/5
        [HttpGet]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Note? note = await _context.Notes
                .Include(n => n.Author)
                .Include(n => n.Machine)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Note? note = await _context.Notes.FindAsync(id);
            if (note != null)
            {
                _context.Notes.Remove(note);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NoteExists(int id)
        {
            return _context.Notes.Any(e => e.Id == id);
        }
    }
}
