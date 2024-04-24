using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;

namespace WebEnterprise_1640.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class ArticleController : Controller
    {
        private readonly ILogger<ArticleController> _logger;
        private readonly ApplicationDbContext _context;

        public ArticleController(ILogger<ArticleController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> SelectedMagazine(int? facultyId)
        {
            IQueryable<MagazineModel> magazinesQuery = _context.Magazines.Include(m => m.Faculty).Include(m => m.Semester);

            if (facultyId.HasValue)
            {
                magazinesQuery = magazinesQuery.Where(m => m.FacultyId == facultyId.Value);
            }

            var magazines = await magazinesQuery.ToListAsync();
            ViewBag.Faculties = await _context.Faculties.ToListAsync();
            return View(magazines);
        }


        public async Task<IActionResult> ArticleContent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = _context.Articles
                .Include(a => a.User)
                .Include(a => a.Magazine)
                .FirstOrDefault(a => a.Id == id);

            if (article == null)
            {
                return NotFound();
            }

            article.Documents = _context.Documents.Where(d => d.ArticleId == article.Id).ToList();
            article.Comments = _context.Comments.Where(c => c.ArticleId == article.Id).Include(c => c.User).ToList();

            return View(new List<ArticleModel> { article });
        }


        // GET: Manager/Article
        public async Task<IActionResult> Index(int magazineId)
        {
            int pageSize = 4;

            var articles = await _context.Articles
                .Include(a => a.User)
                .Where(a => a.MagazineId == magazineId && a.Status == "approved")
                .OrderByDescending(a => a.SubmitDate)
                .ToListAsync();

            //ViewData["page"] = page; // Pass the page parameter to the view

            return View(articles);
        }


        public async Task<IActionResult> DownloadDocuments(int articleId)
        {
            var documents = _context.Documents.Where(d => d.ArticleId == articleId).ToList();

            if (documents.Count == 0)
            {
                return NotFound();
            }

            // Create a memory stream to hold the zip file
            var memoryStream = new MemoryStream();

            // Create a zip archive
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var document in documents)
                {
                    // Construct the full path of the document
                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "media", document.File);

                    if (!System.IO.File.Exists(fullPath))
                    {
                        return NotFound();
                    }

                    // Add each document to the zip archive
                    var entry = archive.CreateEntry(Path.GetFileName(document.File));

                    // Write the content of the document to the zip archive
                    using (var entryStream = entry.Open())
                    using (var fileStream = new FileStream(fullPath, FileMode.Open))
                    {
                        await fileStream.CopyToAsync(entryStream);
                    }

                    // If the document has an associated image, include it in the zip archive
                    if (!string.IsNullOrEmpty(document.Image))
                    {
                        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "media", document.Image);

                        if (System.IO.File.Exists(imagePath))
                        {
                            var imageEntry = archive.CreateEntry(Path.GetFileName(document.Image));

                            using (var imageEntryStream = imageEntry.Open())
                            using (var imageStream = new FileStream(imagePath, FileMode.Open))
                            {
                                await imageStream.CopyToAsync(imageEntryStream);
                            }
                        }
                    }
                }
            }

            // Set the position of the memory stream to the beginning
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Return the zip file as a file download
            return File(memoryStream, "application/zip", "Documents.zip");
        }



        private bool ArticleModelExists(int id)
        {
            return (_context.Articles?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
