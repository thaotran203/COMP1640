using System.ComponentModel.DataAnnotations;

namespace WebEnterprise_1640.Models
{
    public class Login
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
