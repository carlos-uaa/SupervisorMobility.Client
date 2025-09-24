// +============================================================+\\
// +===================== SKILL SERVICE ========================+\\
// +============================================================+\\

// - Core .NET imports
using System.Net.Http.Json;

// - External imports
using Microsoft.JSInterop;


/// <summary>
/// Service to manage Skill entities through HTTP requests.
/// Provides CRUD operations for Skills.
/// </summary>
namespace SupervisorMobility.Client.Services.SOS_Services.SkillServices
{
    public class SkillService : ISkillService
    {
        // +============== MEMBERS ===============+\\
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        // +============ CONSTRUCTOR =============+\\

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillService"/> class.
        /// </summary>
        /// <param name="customHttpClientService">HTTP client for API communication.</param>
        /// <param name="jSRuntime">JS runtime for potential future use.</param>
        public SkillService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // +============== METHODS ===============+\\

        /// <summary>
        /// Creates a new Skill in the backend.
        /// </summary>
        /// <param name="Skill">The Skill data transfer object to create.</param>
        /// <returns>The created <see cref="Skill"/> object.</returns>
        /// <exception cref="ApplicationException">Thrown if the HTTP request fails.</exception>
        public async Task<Skill> CreateSkill(CreateSkillDto Skill)
        {
            var response = await _http.PostAsJsonAsync("SOS/STRO/Collections/Skill", Skill);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var newSkill = JsonSerializer.Deserialize<Skill>(content, _options);
            return newSkill;
        }

        /// <summary>
        /// Retrieves a Skill by its ID.
        /// </summary>
        /// <param name="id">The ID of the Skill to retrieve.</param>
        /// <returns>The <see cref="Skill"/> object with the specified ID.</returns>
        /// <exception cref="ApplicationException">Thrown if the HTTP request fails.</exception>
        public async Task<Skill> GetSkillById(int id)
        {
            var response = await _http.GetAsync($"SOS/STRO/Collections/Skill/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var Skill = JsonSerializer.Deserialize<Skill>(content, _options);

            return Skill;
        }

        /// <summary>
        /// Retrieves all Skills from the backend.
        /// </summary>
        /// <returns>A list of <see cref="Skill"/> objects.</returns>
        /// <exception cref="ApplicationException">Thrown if the HTTP request fails.</exception>
        public async Task<List<Skill>> GetSkills()
        {
            var response = await _http.GetAsync("SOS/STRO/Collections/Skill");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var Skills = JsonSerializer.Deserialize<List<Skill>>(content, _options);

            return Skills;
        }

    }
}
