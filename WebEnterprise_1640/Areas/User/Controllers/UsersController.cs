using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Thêm dòng này
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;
using System.Linq;
using System.Collections.Generic; 

namespace WebEnterprise_1640.Controllers
{
    [Area("User")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public UserController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index(int? id) 
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = id.ToString();

            var user = _dbContext.Users
                               .Include(u => u.Faculty)
                               .FirstOrDefault(a => a.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            return View("Index", user);


        }
    }
}
