namespace SupervisorMobility.Client.Data.Entities.Dtos
{
    public class UpdateAreasForSuperiorDto
    {
        public int UserId { get; set; }
        public int SuperiorId { get; set; }
        public List<int> AreaIds { get; set; } = new List<int>();
    }
}
