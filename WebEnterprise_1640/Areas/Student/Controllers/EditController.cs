using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;
using WebEnterprise_1640.Utility;

namespace WebEnterprise_1640.Areas.Student.Controllers
{
    [Area("Student")]
    public class EditController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EditController(ApplicationDbContext context)

        {
            _context = context;
        }
        [HttpGet]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Index(int id)
        {
            var userJson = HttpContext.Session.GetString("USER");
            UserModel user = null;
            if (userJson != null && userJson.Length > 0)
            {
                user = JsonSerializer.Deserialize<UserModel>(userJson);
            }
            if (user == null)
            {
                return Redirect("/Account/Login");
            }
            var userRole = _context.UserRoles.FirstOrDefault(ur => ur.UserId == user.Id);
            if (userRole == null)
            {
                return Redirect("/Account/Login");
            }
            var role = _context.Roles.FirstOrDefault(r => r.Id == userRole.RoleId);
            if (role == null)
            {
                return Redirect("/Account/Login");
            }
            if (role.Name.ToLower() != "student")
            {
                return Redirect("/Account/Login");
            }
            var data = _context.Articles.FirstOrDefault(x => x.Id == id);
            var model = new ArticleModel();
            model = data;
            var docs = _context.Documents.Where(x => x.ArticleId == id).ToList();
            var tempDocsJson = HttpContext.Session.GetString("TEMP_DOCS");
            if (tempDocsJson != null && tempDocsJson.Count() > 0)
            {
                var tempDocs = JsonSerializer.Deserialize<List<DocumentModel>>(tempDocsJson);
                if (tempDocs != null && tempDocs.Count > 0)
                {
                    if (docs != null && docs.Count > 0)
                    {
                        foreach (var tempDoc in tempDocs)
                        {
                            var doc = docs.FirstOrDefault(d => d.Id == tempDoc.Id);
                            if (doc != null)
                            {
                                if (tempDoc.File != null && tempDoc.File.Length == 0 && tempDoc.Image != null && tempDoc.Image.Length == 0)
                                {
                                    docs.Remove(doc);
                                }
                                else
                                {
                                    if (tempDoc.File == null)
                                    {
                                        tempDoc.File = "";
                                    }
                                    if (tempDoc.Image == null)
                                    {
                                        tempDoc.Image = "";
                                    }
                                    int index = docs.IndexOf(doc);
                                    docs.Remove(doc);
                                    docs.Insert(index, tempDoc);
                                }
                            }
                        }
                    }
                }
            }
            ViewBag.User = user;
            ViewBag.MagazineId = data.MagazineId;
            ViewBag.File = docs;
            ViewBag.check = true;
            return View(model);
        }

        [Route("[controller]/[action]")]
        public async Task<IActionResult> EditData(ArticleModel input)
        {
            var userJson = HttpContext.Session.GetString("USER");
            UserModel user = null;
            if (userJson != null && userJson.Length > 0)
            {
                user = JsonSerializer.Deserialize<UserModel>(userJson);
            }
            if (user == null)
            {
                return Redirect("/Account/Login");
            }
            var userRole = _context.UserRoles.FirstOrDefault(ur => ur.UserId == user.Id);
            if (userRole == null)
            {
                return Redirect("/Account/Login");
            }
            var role = _context.Roles.FirstOrDefault(r => r.Id == userRole.RoleId);
            if (role == null)
            {
                return Redirect("/Account/Login");
            }
            if (role.Name.ToLower() != "student")
            {
                return Redirect("/Account/Login");
            }
            if (string.IsNullOrEmpty(input.Name))
            {
                TempData["ErrorMessage"] = "Title must not empty!";
                return RedirectToAction("Index", "Edit", new { id = input.Id });
            }
            if (string.IsNullOrEmpty(input.Description))
            {
                TempData["ErrorMessage"] = "Description must not empty!";
                return RedirectToAction("Index", "Edit", new { id = input.Id });
            }
            var data = _context.Articles.FirstOrDefault(x => x.Id == input.Id);
            data.Name = input.Name;
            data.Description = input.Description;
            var formfile = await Request.ReadFormAsync();
            if (formfile != null && formfile.Files.Count > 0)
            {
                bool isHaveDoc = false;
                bool isHaveImage = false;
                for (var i = 0; i < formfile.Files.Count; i++)
                {
                    var list = formfile.Files.ToArray();
                    var file = list[i];
                    if (file.FileName.ToLower().Contains("png") || file.FileName.ToLower().Contains("jpg"))
                    {
                        isHaveImage = true;
                    }
                    else if (file.FileName.ToLower().Contains("docx") || file.FileName.ToLower().Contains("doc"))
                    {
                        isHaveDoc = true;
                    }
                }
                if (!isHaveDoc && !isHaveImage)
                {
                    TempData["ErrorMessage"] = "Choose at least one file doc or one image!";
                    return RedirectToAction("Index", "Edit", new { id = input.Id });
                }
            }
            var document = new DocumentModel();
            document = await GUIUtils.UploadImageAsync(formfile);
            document.ArticleId = input.Id;

            if (document.Image != null || document.File != null)
            {
                if (document.Image == null)
                {
                    document.Image = "";
                }
                if (document.File == null)
                {
                    document.File = "";
                }
                _context.Documents.Add(document);
            }
            data.Status = "Submitted";
            _context.Articles.Update(data);
            var tempDocsJson = HttpContext.Session.GetString("TEMP_DOCS");
            if (tempDocsJson != null && tempDocsJson.Length > 0)
            {
                var tempDocs = JsonSerializer.Deserialize<List<DocumentModel>>(tempDocsJson);
                if (tempDocs != null && tempDocs.Count > 0)
                {
                    foreach (var doc in tempDocs)
                    {
                        if (doc.File != null && doc.File.Length == 0 && doc.Image != null && doc.Image.Length == 0)
                        {
                            _context.Documents.Remove(doc);
                        }
                        else
                        {
                            _context.Documents.Update(doc);
                        }
                    }
                }
                HttpContext.Session.Remove("TEMP_DOCS");
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "GetbyId", new { id = input.MagazineId });
        }

        
    }
}
