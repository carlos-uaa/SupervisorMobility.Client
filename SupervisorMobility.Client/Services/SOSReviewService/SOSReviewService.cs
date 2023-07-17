using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.SOSReviewService
{
    public class SOSReviewService : ISOSReviewService
    {

        private readonly HttpClient _http;
        private readonly HttpClient _httpBridge;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;


        // Constructor
        public SOSReviewService(CustomHttpClientService customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService.GetApiHttpClient();
            _httpBridge = customHttpClientService.GetBridgeHttpClient();
            _js = jSRuntime;

            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
        public async Task<SOSReviewProgram> CreateSOSRevier(SOSReviewProgram SOSReview)
        {
            var response = await _http.PostAsJsonAsync($"SOSReview", SOSReview);

            if (response.IsSuccessStatusCode)
            {
                var Final_SOS_Review  = await response.Content.ReadFromJsonAsync<SOSReviewProgram>();
                return Final_SOS_Review;
            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error Create SOS Review: {response.Content.ReadAsStringAsync().Result}");
            }

            return null;
        }

        public async Task DeleteProduct(int id)
        {
            var response = await _http.DeleteAsync($"SOSReview/{id}");

        }

        public async Task<List<SOSReviewProgram>> GetAllSOSReviews(bool includeCollections)
        {
            try
            {
                var response = await _http.GetAsync($"SOSReview?includeCollections={includeCollections}");

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var SOS_reviewsList = JsonSerializer.Deserialize<List<SOSReviewProgram>>(content, _options);

                    response.Dispose();

                    return SOS_reviewsList;
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error al obtener la lista de usuarios: {ex.Message}");
            }

            return null;
        }

        public async Task<SOSReviewProgram> GetSOSById(int sosid)
        {
            var response = await _http.GetAsync($"SOSReview/{sosid}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<SOSReviewProgram>();
                return content;
            }

            return null;
        }

        public async Task<bool> UpdateProduct(SOSReviewProgram sosentity)
        {
            var response = await _http.PutAsJsonAsync($"SOSReview/{sosentity.SOSid}", sosentity);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }
    }
}
