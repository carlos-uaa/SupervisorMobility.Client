namespace SupervisorMobility.Client.Data.Entities.Dtos
{
    public class UserManagmentFormDto
    {
        public User User { get; set; }
        public List<UpdateAreasForSuperiorDto>? Subordinates { get; set; }
    }
}
