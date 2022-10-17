namespace SupervisorMobility.Client.Data.Entities
{
    public class QuestionType
    {
        public int QuestionTypeId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }

        public ICollection<ChecklistQuestion> ChecklistQuestions { get; set; } = new List<ChecklistQuestion>();
    }
}
