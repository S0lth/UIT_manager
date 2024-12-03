using Microsoft.AspNetCore.Mvc;
using UITManagerWebServer.Data;

public class FilesController : Controller
{
    private readonly ApplicationDbContext _context;

    public FilesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("/files/{fileId}")]
    public IActionResult GetFile(int fileId)
    {
        var file = _context.Files.FirstOrDefault(f => f.Id == fileId);

        if (file == null)
        {
            return NotFound();
        }

        return File(file.FileContent, file.MimeType);
    }

}
