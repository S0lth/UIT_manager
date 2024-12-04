using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UITManagerWebServer.Data;

public class FilesController : Controller {
    private readonly ApplicationDbContext _context;

    public FilesController(ApplicationDbContext context) {
        _context = context;
    }

    public override void OnActionExecuting(ActionExecutingContext context) {
        base.OnActionExecuting(context);

        TempData["PreviousUrl"] = Request.Headers["Referer"].ToString();
    }

    [HttpGet("/files/{fileId}")]
    public IActionResult GetFile(int fileId) {
        var file = _context.Files.FirstOrDefault(f => f.Id == fileId);

        if (file == null) {
            return NotFound();
        }

        return File(file.FileContent, file.MimeType);
    }
}