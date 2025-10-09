namespace SupervisorMobility.Client.Data.Entities.SOS_Process.STRO.Collection.Insurance.Dtos
{
    public class InsuranceFeaturesDynamicDTO
    {
        public int SectionId { get; set; }
        public string InputInsuranceFeatures{ get; set; } = string.Empty;
        public List<InsuranceFeatures> InsuranceFeatures { get; set; } = new List<InsuranceFeatures>();
    }
}
