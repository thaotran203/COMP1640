namespace WebEnterprise_1640.Models
{
    public class HomeViewModel
    {
        public List<MagazineModel> Magazines { get; set; }

        public int? FacultyIdSort { get; set; }

        public string? Search { get; set; }


        public FacultyModel? CurFacility { get; set; }
        public List<FacultyModel> Faculties { get; set; }

        public UserModel? User { get; set; }

        public string? UserRole { get; set; }
    }
}
