namespace SupervisorMobility.Client.Data.Entites.Dtos.HRIRevisionItemsDtos
{
    public class UpdateHRIRevisionItemDto
    {
        public int ItemId { get; set; }
        public bool Deleted { get; set; } = false;
        public int HriId { get; set; }
        public int ItemNumber { get; set; }
        public string RevisionPoint { get; set; }
        public int? RevisionMethodId { get; set; } 
        public int? VeredictId { get; set; }
        public int? FrequencyId { get; set; }
    }
}
