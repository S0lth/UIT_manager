using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UITManagerWebServer.Data;
using UITManagerWebServer.Models;
using File = UITManagerWebServer.Models.File;

public class FileController : Controller {
    private readonly ApplicationDbContext _context;

    public FileController(ApplicationDbContext context) {
        _context = context;
    }

    public override void OnActionExecuting(ActionExecutingContext context) {
        base.OnActionExecuting(context);

        TempData["PreviousUrl"] = Request.Headers["Referer"].ToString();
    }

    [HttpGet("/files/{fileId}")]
    public IActionResult GetFile(int fileId) {
        File? file = _context.Files.FirstOrDefault(f => f.Id == fileId);

        if (file == null) {
            return NotFound();
        }

        return File(file.FileContent, file.MimeType);
    }
}