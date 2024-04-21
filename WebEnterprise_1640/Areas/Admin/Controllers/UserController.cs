using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Mail;
using System.Net;
using System.Web.Helpers;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;
using WebEnterprise_1640.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using WebEnterprise_1640.Models.NewFolder;

namespace WebEnterprise_1640.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(ApplicationDbContext context, UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index(string search = "")
        {
            //Get all users first
            var users = await _userManager.Users.ToListAsync();
            //Filter out users with the 'Admin' role
            users = users.Where(u => !_userManager.IsInRoleAsync(u, "Admin").Result).ToList();
            //Search
            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u => u.FullName.Contains(search) || u.Email.Contains(search)).ToList();
            }
            ViewBag.Search = search;
            return View(users);
        }

        // GET: Admin/User/Register
        public async Task<IActionResult> Register()
        {
            if (!_context.Roles.Any())
            {
                await _roleManager.CreateAsync(new IdentityRole { Id = "1", Name = "Guest" });
                await _roleManager.CreateAsync(new IdentityRole { Id = "2", Name = "Student" });
                await _roleManager.CreateAsync(new IdentityRole { Id = "3", Name = "Coordinator" });
                await _roleManager.CreateAsync(new IdentityRole { Id = "4", Name = "Manager" });
                await _roleManager.CreateAsync(new IdentityRole { Id = "5", Name = "Admin" });
            }
            RegisterVM registerVM = new RegisterVM()
            {
                RoleList = _roleManager.Roles.Where(x => x.Name != "Manager" && x.Name != "Admin").Select(x => x.Name).Select(i => new SelectListItem
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
                    PhoneNumber = registerVM.PhoneNumber,
                    Email = registerVM.Email,
                    PasswordHash = passHash,
                    FacultyId = registerVM.FacultyId
                };
                var checkEmail = _context.Users.Any(x => x.Email == user.Email);
                if (checkEmail)
                {
                    ModelState.AddModelError("Email", "User with this email already exists!");
                    registerVM.RoleList = _roleManager.Roles.Where(x => x.Name != "Manager" && x.Name != "Admin").Select(x => x.Name).Select(i => new SelectListItem
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

                IdentityResult identityResult = null;
                if (registerVM.Role == "Coordinator")
                {
                    var coordinators = await _userManager.GetUsersInRoleAsync("Coordinator");
                    var coordinator = coordinators.FirstOrDefault(c => c.FacultyId == registerVM.FacultyId);
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
                        registerVM.RoleList = _roleManager.Roles.Where(x => x.Name != "Manager" && x.Name != "Admin").Select(x => x.Name).Select(i => new SelectListItem
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
            if (id == null)
            {
                return NotFound();
            }
            UserModel? userModel = await _userManager.FindByIdAsync(id);
            var userRoles = await _userManager.GetRolesAsync(userModel);
            RegisterVM registerVM = new RegisterVM()
            {
                RoleList = _roleManager.Roles.Where(x => x.Name != "Manager" && x.Name != "Admin").Select(x => x.Name).Select(i => new SelectListItem
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
                PhoneNumber = userModel.PhoneNumber,
                Email = userModel.Email,
                Password = userModel.PasswordHash,
                FacultyId = userModel.FacultyId,
                Role = userRoles.FirstOrDefault(),
            };
            return View(registerVM);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(RegisterVM registerVM, string? id)
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
                if (userModel.PhoneNumber != registerVM.PhoneNumber)
                {
                    userModel.PhoneNumber = registerVM.PhoneNumber;
                }
                if (userModel.Email != registerVM.Email)
                {
                    var checkEmail = _context.Users.Any(x => x.Email == registerVM.Email);
                    if (checkEmail)
                    {
                        ModelState.AddModelError("Email", "User with this email already exists! ");
                        registerVM.RoleList = _roleManager.Roles.Where(x => x.Name != "Manager" && x.Name != "Admin").Select(x => x.Name).Select(i => new SelectListItem
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
                var userRoles = await _userManager.GetRolesAsync(userModel);
                if (userRoles.Any())
                {
                    //Remove existing roles
                    await _userManager.RemoveFromRolesAsync(userModel, userRoles.ToArray()); 
                }
                if (!string.IsNullOrEmpty(registerVM.Role))
                {
                    //Add new role
                    await _userManager.AddToRoleAsync(userModel, registerVM.Role);
                }
                if (userModel.FacultyId != registerVM.FacultyId)
                {
                    userModel.FacultyId = registerVM.FacultyId;
                }
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
