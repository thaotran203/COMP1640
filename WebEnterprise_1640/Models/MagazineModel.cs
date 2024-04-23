using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebEnterprise_1640.Models
{
    public class MagazineModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ClosureDate { get; set; }

        [Required]
        [ForeignKey("Faculties")]
        public int FacultyId { get; set; }
        [ValidateNever]
        public FacultyModel Faculty { get; set; }

        [Required]
        [ForeignKey("Semesters")]
        public int SemesterId { get; set; }
        [ValidateNever]
        public SemesterModel Semester { get; set; }

        [NotMapped]
        [ValidateNever]
        public List<ArticleModel> Articles { get; set; }
    }
}