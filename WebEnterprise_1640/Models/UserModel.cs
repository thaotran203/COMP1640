using Humanizer.Localisation.TimeToClockNotation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebEnterprise_1640.Models
{
    public class UserModel : IdentityUser<string>
    {
        public string FullName { get; set; }
        public string? Address { get; set; }
        public string PhoneNum { get; set; }
        public string? PersonalImage { get; set; }
        [Required]
        [ForeignKey("Faculties")]
        public int FacultyId { get; set; }
        public FacultyModel Faculty { get; set; }
        [ValidateNever]
        [NotMapped]
        //public List<ArticleModel> Articles { get; set; } = new List<ArticleModel>();

        public List<ArticleModel> Articles { get; set; }
        [ValidateNever]
        [NotMapped]
        //public List<CommentModel> Comments { get; set; } = new List<CommentModel>();

        public List<CommentModel> Comments { get; set; }
        [ValidateNever]
        public string? Role { get; set; }

    }
}
