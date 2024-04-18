using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using WebEnterprise_1640.Data;

namespace WebEnterprise_1640.Areas.Coordinator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public EmailController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("SendEmail")]
        public IActionResult SendEmail([FromBody] string body)
        {
            string fromMail = "beemagazine3@gmail.com";
            string fromPassword = "aulftywznetqjinz";

            var usersEmails = _context.Users.Select(u => u.Email).ToList();

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = "Article Submission Denied";

            foreach (var email in usersEmails)
            {
                message.To.Add(new MailAddress(email));
            }

            message.Body = $@"<html>
                                <body>
                                    <p>Dear student,</p>
                                    <p>Your article submission has been denied. Please review the feedback and make necessary changes.</p>
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
