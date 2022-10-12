namespace SupervisorMobility.Client.Data.Entities
{
    public class ChecklistCategory
    {
        public int ChecklistCategoryId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int Sequence { get; set; }
        public bool? IsActive { get; set; }
    }
}
