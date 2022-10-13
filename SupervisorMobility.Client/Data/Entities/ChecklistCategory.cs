namespace SupervisorMobility.Client.Data.Entities
{
    public class ChecklistCategory
    {
        public int ChecklistCategoryId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Sequence { get; set; }
        public bool? IsActive { get; set; }

        public ICollection<ChecklistQuestion> ChecklistQuestions { get; set; } = new List<ChecklistQuestion>();
    }
}
