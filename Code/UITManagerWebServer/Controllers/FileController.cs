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

    private void SetBreadcrumb(ActionExecutingContext context) {
        List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>();

        breadcrumbs.Add(new BreadcrumbItem { Title = "Home", Url = Url.Action("Index", "Home"), IsActive = false });

        breadcrumbs.Add(new BreadcrumbItem { Title = "File", Url = Url.Action("Index", "File"), IsActive = false });

        string currentAction = context.ActionDescriptor.RouteValues["action"];

        switch (currentAction) {
            case "Index":
                breadcrumbs.Last().IsActive = true;
                break;

            case "Details":
                int fileId = Convert.ToInt32(context.ActionArguments["fileId"]);
                var file = _context.Files.FirstOrDefault(f => f.Id == fileId);
                if (file != null) {
                    breadcrumbs.Add(new BreadcrumbItem {
                        Title = $"File Details - {file.FileName}", Url = string.Empty, IsActive = true
                    });
                }
                
                break;

            case "Create":
                breadcrumbs.Add(new BreadcrumbItem { Title = "Create a File", Url = string.Empty, IsActive = true });
                break;

            case "Edit":
                breadcrumbs.Add(new BreadcrumbItem { Title = "Edit File", Url = string.Empty, IsActive = true });
                break;

            case "Delete":
                breadcrumbs.Add(new BreadcrumbItem { Title = "Delete File", Url = string.Empty, IsActive = true });
                break;

            default:
                break;
        }

        ViewData["Breadcrumbs"] = breadcrumbs;
    }

    public override void OnActionExecuting(ActionExecutingContext context) {
        base.OnActionExecuting(context);

        SetBreadcrumb(context);

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