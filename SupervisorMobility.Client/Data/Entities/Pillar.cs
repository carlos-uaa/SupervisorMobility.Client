namespace SupervisorMobility.Client.Data.Entities
{
    public class Pillar
    {
        public int PillarId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; } = false;
        public ICollection<ChecklistQuestion> ChecklistQuestions { get; set; } = new List<ChecklistQuestion>();
    }
}
