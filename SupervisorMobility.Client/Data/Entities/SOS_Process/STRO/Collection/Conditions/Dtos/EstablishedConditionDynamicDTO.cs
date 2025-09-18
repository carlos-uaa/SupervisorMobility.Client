namespace SupervisorMobility.Client.Data.Entities.SOS_Process.STRO.Collection.Conditions.Dtos
{
    public class EstablishedConditionDynamicDTO
    {
        public int SectionId { get; set; }
        public string InputEstablishedCondition{ get; set; } = string.Empty;
        public List<EstablishedConditions> EstablishedConditions { get; set; } = new List<EstablishedConditions>();
    }
}
