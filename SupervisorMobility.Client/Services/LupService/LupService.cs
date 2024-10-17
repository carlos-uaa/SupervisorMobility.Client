using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.LupService
{
    public class LupService : ILupService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public LupService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<Lup> CreateLup(Lup lup)
        {
            var response = await _http.PostAsJsonAsync($"lup", lup);
            var newLup = await response.Content.ReadFromJsonAsync<Lup>();

            return newLup;
        }

        public async Task<Lup> CreateEvidencesLup(MultipartFormDataContent lup)
        {
            var response = await _http.PostAsync($"lup/evidencesLup", lup);

            //var content = await response.Content.ReadAsStringAsync();

            //if (!response.IsSuccessStatusCode)
            //{
            //    return null;
            //}

            var newLup = await response.Content.ReadFromJsonAsync<Lup>();

            return newLup;
        }

        public async Task DeleteLup(int lupId)
        {
            var response = await _http.DeleteAsync($"lup/{lupId}");
        }


        public async Task<List<Lup>> GetAllLup()
        {
            var response = await _http.GetAsync($"lup");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var lup = JsonSerializer.Deserialize<List<Lup>>(content, _options);

            return lup;
        }
        
        public async Task<List<Lup>> GetAllLupInsidences(int QuestionId, int supervisor_id)
        {
            var response = await _http.GetAsync($"lup/Insidences/{QuestionId}?supervisor_id={supervisor_id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var lup = JsonSerializer.Deserialize<List<Lup>>(content, _options);

            return lup;
        }

        public async Task<List<Lup>> GetAllLupInsidences(int checklistQuestionId, int supervisor_id, int distributionId)
        {
            var response = await _http.GetAsync($"lup/Insidences/{checklistQuestionId}?supervisor_id={supervisor_id}&distributionId={distributionId}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var lup = JsonSerializer.Deserialize<List<Lup>>(content, _options);

            return lup;
        }

        public async Task<List<Lup>> GetLupsByFilters(DateTime? startDate, DateTime? endDate, int plantId, int areaId, int distributionId, int operationId, int supervisorId, int status)
        {
            var response = await _http.GetAsync($"lup/ByFilters?startDate={startDate}&endDate={endDate}&plantId={plantId}&areaId={areaId}&distributionId={distributionId}&operationId={operationId}&supervisorId={supervisorId}&status={status}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var lup = JsonSerializer.Deserialize<List<Lup>>(content, _options);

            return lup;
        }

        public async Task<Lup> GetLupById(int lupId)
        {
            var response = await _http.GetAsync($"lup/{lupId}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var lup = JsonSerializer.Deserialize<Lup>(content, _options);

            return lup;
        }

        public async Task<Lup> GetLupByIdWhitFile(int lupId)
        {
            var response = await _http.GetAsync($"lup/{lupId}?includeFile=true");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var lup = JsonSerializer.Deserialize<Lup>(content, _options);
                return lup;
            }
            await _js.InvokeVoidAsync("alert", $"Error : {response.Content.ReadAsStringAsync().Result}");
            return null;

        }

        public async Task<bool> UpdateLup(Lup lup)
        {
            var response = await _http.PutAsJsonAsync($"lup/{lup.LupId}", lup);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> RemoveEvidence(int lupId, int fileUploadId)
        {
            var response = await _http.PostAsJsonAsync($"lup/{lupId}/evidence/remove", fileUploadId);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

    }
}
