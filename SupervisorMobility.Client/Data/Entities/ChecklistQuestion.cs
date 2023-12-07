namespace SupervisorMobility.Client.Data.Entities
{
    public class ChecklistQuestion
    {
        public int QuestionID { get; set; }
        public string Prompt { get; set; } = string.Empty;
        public int PillarId { get; set; }
        public string NotGood { get; set; }
        public int CategorySequence { get; set; }
        public string Container { get; set; } = "QuestionContainer";
        public bool? IsActive { get; set; } = false;
        public int ChecklistCategoryId { get; set; }
        public bool show { get; set; } = false;
        public ICollection<FileUpload>? Evidences { get; set; } = new List<FileUpload>();

        public string? CommentarySV { get; set; } = string.Empty;
        public string? CommentarySSV { get; set; } = string.Empty;
    }
}
