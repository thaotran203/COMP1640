namespace WebEnterprise_1640.Models.NewFolder
{
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
    public class SemesterModelView
    {
        public int Id { get; set; }
        public string FinalClosureDate { get; set; }
    }

}
