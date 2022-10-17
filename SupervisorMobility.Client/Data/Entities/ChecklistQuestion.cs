namespace SupervisorMobility.Client.Data.Entities
{
    public class ChecklistQuestion
    {
        public int QuestionID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Prompt { get; set; } = string.Empty;
        public int CategorySequence { get; set; }
        public int? AnswerSetID { get; set; }
        public bool? IsActive { get; set; } = false;
        public int ChecklistCategoryId { get; set; }
        public int QuestionTypeId { get; set; }
    }
}
