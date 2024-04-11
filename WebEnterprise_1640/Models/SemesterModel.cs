﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebEnterprise_1640.Models
{
    public class SemesterModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime FinalClosureDate { get; set; }

        [NotMapped]
        public List<MagazineModel> Magazines { get; set; }
    }

    public class SemesterModelView
    {

        public int Id { get; set; }
        public string FinalClosureDate { get; set; }


    }
}
