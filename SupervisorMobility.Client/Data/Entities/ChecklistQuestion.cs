namespace SupervisorMobility.Client.Data.Entities
{
    public class ChecklistQuestion
    {
        public int QuestionID { get; set; }
        public string Prompt { get; set; } = string.Empty;
        public int PillarId { get; set; }
        public int Sequence { get; set; }
        public string NotGood { get; set; }
        public int CategorySequence { get; set; }
        public bool? IsActive { get; set; } = false;
        public int ChecklistCategoryId { get; set; }
    }
}
