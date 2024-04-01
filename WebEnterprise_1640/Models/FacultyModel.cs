using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebEnterprise_1640.Models
{
    public class FacultyModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public int CoordinatorId { get; set; }
        [ValidateNever]
        [NotMapped]
        public List<UserModel> Users { get; set; }
        [ValidateNever]
        [NotMapped]
        public List<MagazineModel> Magazines { get; set; }
    }
}
