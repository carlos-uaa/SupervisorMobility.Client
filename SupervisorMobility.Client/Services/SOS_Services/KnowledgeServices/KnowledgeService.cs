// +============================================================+\\
// +=================== KNOWLEDGE SERVICE ======================+\\
// +============================================================+\\

// - Core .NET imports
using System.Net.Http.Json;

// - External imports
using Microsoft.JSInterop;


/// <summary>
/// Service to manage Knowledge entities through HTTP requests.
/// Provides CRUD operations for Knowledge.
/// </summary>
namespace SupervisorMobility.Client.Services.SOS_Services.KnowledgeServices
{
    public class KnowledgeService : IKnowledgeService
    {
        // +============== MEMBERS ===============+\\
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        // +============ CONSTRUCTOR =============+\\
        /// <summary>
        /// Initializes a new instance of the <see cref="KnowledgeService"/> class.
        /// </summary>
        /// <param name="customHttpClientService">HTTP client for API communication.</param>
        /// <param name="jSRuntime">JS runtime for potential future use.</param>
        public KnowledgeService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // +============== METHODS ===============+\\

        /// <summary>
        /// Creates a new Knowledge entity in the backend.
        /// </summary>
        /// <param name="Knowledge">The Knowledge data transfer object to create.</param>
        /// <returns>The created <see cref="Knowledge"/> object.</returns>
        /// <exception cref="ApplicationException">Thrown if the HTTP request fails.</exception>
        public async Task<Knowledge> CreateKnowledge(CreateKnowledgeDto Knowledge)
        {
            var response = await _http.PostAsJsonAsync("SOS/STRO/Collections/Knowledge", Knowledge);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var newKnowledge = JsonSerializer.Deserialize<Knowledge>(content, _options);
            return newKnowledge;
        }

        /// <summary>
        /// Retrieves a Knowledge entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the Knowledge to retrieve.</param>
        /// <returns>The <see cref="Knowledge"/> object with the specified ID.</returns>
        /// <exception cref="ApplicationException">Thrown if the HTTP request fails.</exception>
        public async Task<Knowledge> GetKnowledgeById(int id)
        {
            var response = await _http.GetAsync($"SOS/STRO/Collections/Knowledge/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var Knowledge = JsonSerializer.Deserialize<Knowledge>(content, _options);

            return Knowledge;
        }

        /// <summary>
        /// Retrieves all Knowledge entities from the backend.
        /// </summary>
        /// <returns>A list of <see cref="Knowledge"/> objects.</returns>
        /// <exception cref="ApplicationException">Thrown if the HTTP request fails.</exception>
        public async Task<List<Knowledge>> GetKnowledges()
        {
            var response = await _http.GetAsync("SOS/STRO/Collections/Knowledge");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var Knowledges = JsonSerializer.Deserialize<List<Knowledge>>(content, _options);

            return Knowledges;
        }


    }
}
