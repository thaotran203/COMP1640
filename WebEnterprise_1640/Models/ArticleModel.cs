﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebEnterprise_1640.Models
{
    public class ArticleModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime SubmitDate { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public string UserId { get; set; }
        public UserModel User { get; set; }

        [Required]
        [ForeignKey("Magazines")]
        public int MagazineId { get; set; }
        public MagazineModel Magazine { get; set; }


        [NotMapped]
        public List<CommentModel> Comments { get; set; }

        [NotMapped]
        public List<DocumentModel> Documents { get; set; }
    }

    public class ArticleViewModel : ArticleModel
    {
        public string TimeStart { get; set; }

        public string TimeEnd { get; set; }

        public string TimeSubmit { get; set; }

        public List<DocumentModel> File { get; set; }
    }
    public class search
    {
        public string searchKey { get; set; }
    }
}
