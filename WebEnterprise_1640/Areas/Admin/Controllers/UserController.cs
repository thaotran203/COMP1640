using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Web.Helpers;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;

namespace WebEnterprise_1640.Areas.Admin.Controllers
{
	public class UserController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<UserModel> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly SignInManager<UserModel> _signInManager;
		public UserController(ApplicationDbContext context, UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager, SignInManager<UserModel> signInManager)
		{
			_context = context;
			_userManager = userManager;
			_roleManager = roleManager;
			_signInManager = signInManager;
		}

		// GET: Admin/User
		public async Task<IActionResult> Index(string search = "")
		{
            //Search
            List<UserModel> users = await _context.Users.Include(u => u.Faculty).Where(u => u.Role != "Admin").Where(f => f.FullName.Contains(search)).ToListAsync();
            ViewBag.Search = search;
            return View(users);
        }

		// GET: Admin/User/Register
		public async Task<IActionResult> Register()
		{
			_roleManager.CreateAsync(new IdentityRole("Student")).GetAwaiter().GetResult();
			_roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
			_roleManager.CreateAsync(new IdentityRole("Manager")).GetAwaiter().GetResult();
			_roleManager.CreateAsync(new IdentityRole("Coordinator")).GetAwaiter().GetResult();
			RegisterVM registerVM = new RegisterVM()
			{
				RoleList = _roleManager.Roles.Where(x => x.Name != "Admin" & x.Name != "Manager").Select(x => x.Name).Select(i => new SelectListItem
				{
					Text = i,
					Value = i
				}),
				FacultyList = _context.Faculties.Select(c => new SelectListItem
				{
					Text = c.Name,
					Value = c.Id.ToString(),
				})
			};

			return View(registerVM);
		}
		[HttpPost]
		public async Task<IActionResult> Register(RegisterVM registerVM)
		{
			if (ModelState.IsValid)
			{
				var passHash = Crypto.HashPassword(registerVM.Password);
				var user = new UserModel()
				{
					Id = Guid.NewGuid().ToString(),
					UserName = registerVM.Email,
					FullName = registerVM.FullName,
					PhoneNum = registerVM.PhoneNum,
					Email = registerVM.Email,
					Role = registerVM.Role,
					PasswordHash = passHash,
					FacultyId = registerVM.FacultyId
				};
				var checkEmail = _context.Users.Any(x => x.Email == user.Email);
				if (checkEmail)
				{
					ModelState.AddModelError("Email", "This email already exists!");
				}
				IdentityResult identityResult = null;
				if (user.Role == "Coordinator")
				{
					var coordinator = _context.Users.Where(f => f.FacultyId == registerVM.FacultyId)
											   .Where(c => c.Role == "Coordinator")
											   .FirstOrDefault();
					if (coordinator == null)
					{
						identityResult = await _userManager.CreateAsync(user);
						if (identityResult.Succeeded)
						{

							await _userManager.AddToRoleAsync(user, registerVM.Role);
							await _signInManager.SignInAsync(user, isPersistent: false);
							TempData["success"] = "User created successfully";
							return RedirectToAction("Index");
						}
						foreach (var error in identityResult.Errors)
						{
							ModelState.AddModelError("", error.Description);
						}

						ModelState.AddModelError(string.Empty, "Invalid");
					}
					else
					{
						TempData["error"] = "Coordinator with this faculty already exists!";
                        registerVM.RoleList = _roleManager.Roles.Where(x => x.Name != "Admin" & x.Name != "Manager").Select(x => x.Name).Select(i => new SelectListItem
                        {
                            Text = i,
                            Value = i
                        });
                        registerVM.FacultyList = _context.Faculties.Select(c => new SelectListItem
                        {
                            Text = c.Name,
                            Value = c.Id.ToString(),
                        });
                        return View(registerVM);
                    }

				}
				else //register user role is not coordinator
				{
					identityResult = await _userManager.CreateAsync(user);
					if (identityResult.Succeeded)
					{

						await _userManager.AddToRoleAsync(user, registerVM.Role);
						await _signInManager.SignInAsync(user, isPersistent: false);
						TempData["success"] = "User created successfully";
						return RedirectToAction("Index");
					}
				}
			}
			return RedirectToAction("Index");

		}
	}
}
