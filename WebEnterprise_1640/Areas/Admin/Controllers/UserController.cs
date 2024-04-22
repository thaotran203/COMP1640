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
        private const string LowercaseCharacters = "abcdefghijklmnopqrstuvwxyz";
        private const string UppercaseCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string DigitCharacters = "0123456789";
        private const string SpecialCharacters = "!@#$%^&*()-_=+";

        public static string GenerateRandomPassword(int length = 12)
        {
            var allCharacters = LowercaseCharacters + UppercaseCharacters + DigitCharacters + SpecialCharacters;
            var passwordChars = new char[length];
            var random = new Random();
            //Ensure at least one character from each character set
            passwordChars[0] = LowercaseCharacters[random.Next(LowercaseCharacters.Length)];
            passwordChars[1] = UppercaseCharacters[random.Next(UppercaseCharacters.Length)];
            passwordChars[2] = DigitCharacters[random.Next(DigitCharacters.Length)];
            passwordChars[3] = SpecialCharacters[random.Next(SpecialCharacters.Length)];
            //Fill the rest of the password with random characters
            for (int i = 4; i < length; i++)
            {
                passwordChars[i] = allCharacters[random.Next(allCharacters.Length)];
            }
            // Shuffle the characters to make the password more random
            return new string(passwordChars);
        }

        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(ApplicationDbContext context, UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index(string search = "", List<string> roles = null)
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
            // Filter role
            if (roles != null && roles.Count > 0)
            {
                var usersInRoles = new List<UserModel>();
                foreach (var role in roles)
                {
                    var usersInCurrentRole = await _userManager.GetUsersInRoleAsync(role);
                    usersInRoles.AddRange(usersInCurrentRole);
                }
                users = users.Where(u => usersInRoles.Select(x => x.Id).Contains(u.Id)).ToList();
            }
            ViewBag.AllRoles = (await _roleManager.Roles.Where(r => r.Name != "Admin").ToListAsync()).Select(r => r.Name).ToList();
            ViewBag.Roles = roles;

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
                            try
                            {
                                //Send the email and password to the user via email using EmailSender
                                await _userManager.AddToRoleAsync(user, registerVM.Role);
                                TempData["success"] = "User's account has been successfully created!";
                                SendEmailCreate(user.FullName, user.UserName, registerVM.Password);
                                return RedirectToAction("Index");
                            }
                            catch (Exception ex)
                            {
                                //Log or handle the exception accordingly
                                ModelState.AddModelError(string.Empty, "Failed to send the account information via email.");
                            }
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
                        try
                        {
                            //Send the email and password to the user via email using EmailSender
                            await _userManager.AddToRoleAsync(user, registerVM.Role);
                            TempData["success"] = "User's account has been successfully created!";
                            SendEmailCreate(user.FullName, user.UserName, registerVM.Password);
                            return RedirectToAction("Index");
                        }
                        catch (Exception ex)
                        {
                            //Log or handle the exception accordingly
                            ModelState.AddModelError(string.Empty, "Failed to send the account infomation via email.");
                        }
                    }
                }
            }
            return RedirectToAction("Index");

        }
        [HttpPost]
        public IActionResult SendEmailCreate([FromBody] string fullname, string userEmail, string password)
        {
            //The email will send password to user
            string fromMail = "beemagazine3@gmail.com";
            string fromPassword = "aulftywznetqjinz";
            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            //Send email to the specific user
            message.To.Add(new MailAddress(userEmail));
            message.Subject = "Account Information";
            message.Body = $@"<html>
                                <body>
                                    <p>Dear {fullname},</p>
                                    <p>Help about accessing the portal is available on the login page.</p>
                                    <p>Here is your account for the Bee Magazine</p>
                                    <p>Your email is <b>{userEmail}</b>.</p>
                                    <p>Your password is <b>{password}</b>.</p>
                                    <p>For more information about registration, please contact your college.</p>
                                    <p>Best regards,<br/>The Bee Magazine Team</p>
                                </body>
                            </html>";
            message.IsBodyHtml = true;
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };
            smtpClient.Send(message);
            return Ok();
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
                //Get name faculty in list
                var faculty = _context.Faculties.FirstOrDefault(f => f.Id == registerVM.FacultyId);
                IdentityResult identityResult = await _userManager.UpdateAsync(userModel);
                if (identityResult.Succeeded)
                {
                    try
                    {
                        //Send the email and password to the user via email using EmailSender
                        await _userManager.AddToRoleAsync(userModel, registerVM.Role);
                        TempData["success"] = "User's account has been successfully updated!";
                        SendEmailEdit(userModel.UserName, userModel.FullName, userModel.PhoneNumber, faculty.Name, registerVM.Role);
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        //Log or handle the exception accordingly
                        ModelState.AddModelError(string.Empty, "Failed to send the updated account information via email.");
                    }
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
        [HttpPost]
        public IActionResult SendEmailEdit([FromBody] string userEmail, string fullname, string phone, string faculty, string role)
        {
            //The email will send update information to user
            string fromMail = "beemagazine3@gmail.com";
            string fromPassword = "aulftywznetqjinz";
            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            //Send email to the specific user
            message.To.Add(new MailAddress(userEmail));
            message.Subject = "Update Account Information";
            message.Body = $@"<html>
                                <body>
                                    <p>Dear {role},</p>
                                    <p>Help with describing {role}'s personal information.</p>
                                    <p>Your fullname is <b>{fullname}</b>.</p>
                                    <p>Your email is <b>{userEmail}</b>.</p>
                                    <p>Your phone number is <b>{phone}</b>.</p>
                                    <p>Your faculty is <b>{faculty}</b>.</p>
                                    <p>For more information about registration, please contact your college.</p>
                                    <p>Best regards,<br/>The Bee Magazine Team</p>
                                </body>
                            </html>";
            message.IsBodyHtml = true;
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };
            smtpClient.Send(message);
            return Ok();
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
                    TempData["success"] = "User's account has been successfully deleted!";
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

        // Reset password
        public async Task<IActionResult> ResetPassword(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is missing.");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            //Generate a random password
            var newPassword = GenerateRandomPassword();
            //Reset the user's password
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

            if (result.Succeeded)
            {
                try
                {
                    //Send the new password to the user via email using EmailSender
                    TempData["success"] = "User's password has been successfully reseted!";
                    SendEmail(user.FullName, user.UserName, newPassword);
                    SavePasswordToFile(user.UserName, newPassword);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    //Log or handle the exception accordingly
                    ModelState.AddModelError(string.Empty, "Failed to send the new password via email.");
                }
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return RedirectToAction("Index");
        }
        private void SavePasswordToFile(string username, string password)
        {
            var directoryPath = Path.Combine("password", username);
            var filePath = Path.Combine(directoryPath, $"{username}_password.txt");
            //Create the directory if it doesn't exist
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            //Write the password to the file
            System.IO.File.WriteAllText(filePath, password);
        }
        [HttpPost]
        public IActionResult SendEmail([FromBody] string fullname, string userEmail, string password)
        {
            //The email will send password to user
            string fromMail = "beemagazine3@gmail.com";
            string fromPassword = "aulftywznetqjinz";
            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            //Send email to the specific user
            message.To.Add(new MailAddress(userEmail));
            message.Subject = "Article Submission Denied";
            message.Body = $@"<html>
                                <body>
                                    <p>Dear {fullname},</p>
                                    <p>Help about accessing the portal is available on the login page.</p>
                                    <p>Your new password is <b>{password}</b>.</p>
                                    <p>For more information about registration, please contact your college.</p>
                                    <p>Best regards,<br/>The Bee Magazine Team</p>
                                </body>
                            </html>";
            message.IsBodyHtml = true;
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };
            smtpClient.Send(message);
            return Ok();
        }
    }
}
