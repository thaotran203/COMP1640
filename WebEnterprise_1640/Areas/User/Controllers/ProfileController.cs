using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;

namespace WebEnterprise_1640.Controllers
{
    [Area("User")]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public ProfileController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Test(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _dbContext.Users
                               .Include(u => u.Faculty)
                               .FirstOrDefault(a => a.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult Test(UserModel model)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest();
            }

            var existingUser = _dbContext.Users
                .Include(u => u.Faculty)
                .FirstOrDefault(u => u.Id == model.Id);

            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.FullName = model.FullName;
            existingUser.Email = model.Email;
            existingUser.PhoneNumber = model.PhoneNumber;
            existingUser.Address = model.Address;

            _dbContext.SaveChanges();
            return View(existingUser);
        }
        [HttpPost]
        public IActionResult UpdateAvatar(string Id, IFormFile Avatar)
        {
            if (Avatar != null && Avatar.Length > 0)
            {
                var imgFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\Avatar", Id); // Đường dẫn trên máy chủ

                if (!Directory.Exists(imgFolderPath))
                {
                    Directory.CreateDirectory(imgFolderPath);
                }

                var imagePath = Path.Combine(imgFolderPath, Avatar.FileName); // Tạo tên file mới để tránh trùng lặp

                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    Avatar.CopyTo(fileStream); // Lưu ảnh vào thư mục
                }

                // Cập nhật đường dẫn ảnh vào UserModel
                var user = _dbContext.Users.FirstOrDefault(u => u.Id == Id);
                if (user != null)
                {
                    user.PersonalImage = imagePath;
                    _dbContext.SaveChanges();
                }

                // Trả về đường dẫn ảnh đã lưu
                return RedirectToAction("Test", new { id = Id });
            }

            return BadRequest("No image uploaded");
        }
        [HttpPost]
        public IActionResult ChangePassword(string userId, string curPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(curPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                TempData["ErrorMessage"] = "Please fill all fields";
                return RedirectToAction("Test", "Profile", new { id = userId });
            }
            if (!newPassword.Equals(confirmPassword))
            {
                TempData["ErrorMessage"] = "New password must be the same with confirm password";
                return RedirectToAction("Test", "Profile", new { id = userId });
            }
            var user = _dbContext.Users
                               .Include(u => u.Faculty)
                               .FirstOrDefault(a => a.Id == userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Not Found User";
                return RedirectToAction("Test", "Profile", new { id = userId });
            }
            if (user.PasswordHash != curPassword)
            {
                TempData["ErrorMessage"] = "Current password is incorrect";
                return RedirectToAction("Test", "Profile", new { id = userId });
            }
            user.PasswordHash = newPassword;
            var nUser = _dbContext.Update(user);
            _dbContext.SaveChanges();
            if (nUser.Entity == null)
            {
                TempData["ErrorMessage"] = "Database connection got error!";
                return RedirectToAction("Test", "Profile", new { id = userId });
            }
            TempData["SuccessMessage"] = "Change password successful";
            return RedirectToAction("Test", "Profile", new { id = userId });
        }
    }
}
