using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.GroupService
{
    public class GroupService : IGroupService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public GroupService(HttpClient http)
        {
            _http = http;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // Create group
        public async Task<Group> CreateGroup(Group group)
        {
            var response = await _http.PostAsJsonAsync("groups", group);
            var newGroup = await response.Content.ReadFromJsonAsync<Group>();

            return newGroup;
        }

        // Delete group
        public async Task DeleteGroup(int id)
        {
            var response = await _http.DeleteAsync($"groups/{id}");
        }

        // Get group by Id
        public async Task<Group> GetGroupById(int id)
        {
            var response = await _http.GetAsync($"groups/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var group = JsonSerializer.Deserialize<Group>(content, _options);

            return group;
        }

        // Get all groups
        public async Task<List<Group>> GetGroups()
        {
            var response = await _http.GetAsync("groups");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var groups = JsonSerializer.Deserialize<List<Group>>(content, _options);

            return groups;
        }

        // Update group
        public async Task<bool> UpdateGroup(Group group)
        {
            var response = await _http.PutAsJsonAsync($"groups/{group.GroupId}", group);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }
    }
}
