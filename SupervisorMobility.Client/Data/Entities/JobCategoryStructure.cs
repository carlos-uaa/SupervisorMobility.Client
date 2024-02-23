namespace SupervisorMobility.Client.Data.Entities
{
    public enum StructureType
    {
        Titular,
        Checklist,
        Timer,
        LUP,
        LUP_SSV,
        Signature
    }
    public class JobCategoryStructure
    {
        public int JobCategoryStructureId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Sequence { get; set; }
        public StructureType Type { get; set; }
        public bool? IsActive { get; set; } = false;

        public string Container { get; set; } = "CategoryContainer";
        public ICollection<ChecklistQuestion> ChecklistQuestions { get; set; } = new List<ChecklistQuestion>();
    }
}
