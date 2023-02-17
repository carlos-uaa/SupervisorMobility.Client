namespace SupervisorMobility.Client.Services.GroupService
{
    public interface IGroupService
    {
        // Get all groups
        Task<List<Group>> GetGroups();

        // Get group by Id
        Task<Group> GetGroupById(int id);

        // Create group
        Task<Group> CreateGroup(Group group);

        // Update group
        Task<bool> UpdateGroup(Group group);

        // Delete group
        Task DeleteGroup(int id);
    }
}
