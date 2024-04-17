using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Web.Helpers;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;
using WebEnterprise_1640.Models.ViewModels;

namespace WebEnterprise_1640.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly IPasswordHasher<UserModel> _passwordHash;
        public UserController(ApplicationDbContext context, UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager, SignInManager<UserModel> signInManager, IPasswordHasher<UserModel> passwordHash)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _passwordHash = passwordHash;
        }

        // GET: Admin/User
        public async Task<IActionResult> Index(string search = "")
        {
            //ViewBag
            ViewBag.Search = search;
            var usersQuery = _context.Users.Include(u => u.Faculty).Where(u => u.Role != "Admin");
            //Search
            if (!string.IsNullOrEmpty(search))
            {
                usersQuery = usersQuery.Where(f => f.FullName.Contains(search) || f.Email.Contains(search));
            }
            var users = await usersQuery.ToListAsync();
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
                    ModelState.AddModelError("Email", "User with this email already exists!");
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
                    return View (registerVM);
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
                            TempData["success"] = "User created successfully!";
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
                        TempData["error"] = "A coordinator already exists within the faculty!";
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
                        TempData["success"] = "User created successfully!";
                        return RedirectToAction("Index");
                    }
                }
            }
            return RedirectToAction("Index");

        }

        // GET: Admin/User/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null || id == null)
            {
                return NotFound();
            }
            UserModel? userModel = await _userManager.FindByIdAsync(id);
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
                }),
                FullName = userModel.FullName,
                PhoneNum = userModel.PhoneNum,
                Email = userModel.Email,
                Role = userModel.Role,
                Password = userModel.PasswordHash,
                FacultyId = userModel.FacultyId
            };
            return View(registerVM);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(RegisterVM registerVM, string? id, string password)
        {
            if (ModelState.IsValid)
            {
                UserModel userModel = await _userManager.FindByIdAsync(id);
                if (userModel == null)
                {
                    return NotFound();
                }
                if (userModel.UserName != registerVM.Email)
                {
                    userModel.UserName = registerVM.Email;
                }
                if (userModel.FullName != registerVM.FullName)
                {
                    userModel.FullName = registerVM.FullName;
                }
                if (userModel.PhoneNum != registerVM.PhoneNum)
                {
                    userModel.PhoneNum = registerVM.PhoneNum;
                }
                if (userModel.Email != registerVM.Email)
                {
                    var checkEmail = _context.Users.Any(x => x.Email == registerVM.Email);
                    if (checkEmail)
                    {
                        ModelState.AddModelError("Email", "User with this email already exists! ");
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
                    userModel.Email = registerVM.Email;
                }
                if (userModel.Role != registerVM.Role)
                {
                    userModel.Role = registerVM.Role;
                }
                if (userModel.FacultyId != registerVM.FacultyId)
                {
                    userModel.FacultyId = registerVM.FacultyId;
                }
                if (!string.IsNullOrEmpty(password))
                {
                    userModel.PasswordHash = _passwordHash.HashPassword(userModel, password);
                }
                if (!string.IsNullOrEmpty(password))
                {
                    IdentityResult identityResult = await _userManager.UpdateAsync(userModel);
                    if (identityResult.Succeeded)
                    {
                        TempData["success"] = "User updated successfully!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in identityResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        ModelState.AddModelError(string.Empty, "Invalid");
                    }
                }

            }

            return RedirectToAction("Index");
        }

        // GET: Admin/User/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null || id == null)
            {
                return NotFound();
            }
            UserModel? userModel = await _userManager.FindByIdAsync(id);
            return View(userModel);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(UserModel userModel, string? id)
        {
            userModel = await _userManager.FindByIdAsync(id);
            if (userModel == null)
            {
                return NotFound();
            }
            else
            {
                IdentityResult identityResult = await _userManager.DeleteAsync(userModel);
                if (identityResult.Succeeded)
                {
                    TempData["success"] = "User deleted successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    ModelState.AddModelError(string.Empty, "Invalid");
                }
            }
            return RedirectToAction("Index");

        }
    }
}
