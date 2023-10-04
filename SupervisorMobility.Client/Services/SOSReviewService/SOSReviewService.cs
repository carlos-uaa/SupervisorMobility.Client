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
        public async Task<SOSReviewProgram> CreateSOSReview(SOSReviewProgram SOSReview)
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
        
      

        public async Task DeleteSOSReview(int id)
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

        public async Task<SOSReviewProgram> GetSOSById(int sosid, bool includeCollections = false)
        {
            var response = await _http.GetAsync($"SOSReview/{sosid}?includeCollections={includeCollections}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<SOSReviewProgram>();
                return content;
            }

            return null;
        }

        public async Task<bool> UpdateSOSReview(SOSReviewProgram sosentity)
        {
            var response = await _http.PutAsJsonAsync($"SOSReview/{sosentity.SOSid}", sosentity);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

        public async Task<List<SOSRegisterJobObservation>> GetSOSRegisters(int sosid)
        {
            try
            {
                var response = await _http.GetAsync($"SOSReview/Registers/{sosid}");

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var SOS_registerList = JsonSerializer.Deserialize<List<SOSRegisterJobObservation>>(content, _options);

                    response.Dispose();

                    return SOS_registerList;
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error al obtener la lista de registros SOS REGISTERS: {ex.Message}");
            }

            return null;
        }

        public async Task<SOSRegisterJobObservation> CreateSOSRegister(int SOSid, int month, int year, JobObservation JobEntity)
        {

            //int SOSid, int month, int year,
            var response = await _http.PostAsJsonAsync($"SOSReview/Registers/{SOSid}?month={month}&year={year}", JobEntity);

            if (response.IsSuccessStatusCode)
            {
                var Final_SOS_Review = await response.Content.ReadFromJsonAsync<SOSRegisterJobObservation>();
                return Final_SOS_Review;
            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error Create SOS Review: {response.Content.ReadAsStringAsync().Result}");
            }

            return null;
        }

            public async Task<List<SOSRegUserOperation>> GetSOSRegUserOperation(int sosid)
        {
            try
            {
                var response = await _http.GetAsync($"SOSReview/Registers/UserOp/{sosid}");

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var SOS_registerList = JsonSerializer.Deserialize<List<SOSRegUserOperation>>(content, _options);

                    response.Dispose();

                    return SOS_registerList;
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error al obtener la lista de registros Supervisro Revisor: {ex.Message}");
            }

            return null;
        }

        public async Task<SOSRegUserOperation> CreateSOSRegUserOperation(int SOSid, int SupervisorId, int OperationId)
        {

            //int SOSid, int month, int year,
            var response = await _http.PostAsync($"SOSReview/Registers/UserOp/{SOSid}?SupervisorId={SupervisorId}&OperationId={OperationId}", null);

            if (response.IsSuccessStatusCode)
            {
                var Final_SOS_Review = await response.Content.ReadFromJsonAsync<SOSRegUserOperation>();
                return Final_SOS_Review;
            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error Create Supervisro Revisor: {response.Content.ReadAsStringAsync().Result}");
            }

            return null;
        }

       
    }
}
