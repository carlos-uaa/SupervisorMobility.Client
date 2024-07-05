
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.IS_Services.ProblemDefectService
{
    public class ProblemDefectServices : IProblemDefectServices
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public ProblemDefectServices(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<ProblemDefect?> CreateProblemDefect(ProblemDefect ProblemDefectToCreate)
        {
            var response = await _http.PostAsJsonAsync($"IS/Appearance/Problems", ProblemDefectToCreate);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var ProblemDefect = JsonSerializer.Deserialize<ProblemDefect>(content, _options);

            return ProblemDefect; 

        }
        public async Task<ProblemDefect?> GetProblemDefect(int ProblemDefect_id)
        {
            var response = await _http.GetAsync($"IS/Appearance/Problems/{ProblemDefect_id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var ProblemDefect = JsonSerializer.Deserialize<ProblemDefect>(content, _options);

            return ProblemDefect;
        }


        public async Task<List<ProblemDefect>> GetAllProblemDefects()
        {
            var response = await _http.GetAsync($"IS/Appearance/Problems");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var ProblemDefects = JsonSerializer.Deserialize<List<ProblemDefect>>(content, _options);

            return ProblemDefects;
        }

        public async Task<ProblemDefect?> UpdateProblemDefect(ProblemDefect ProblemDefectToUpdate)
        {
            var response = await _http.PutAsJsonAsync($"IS/Appearance/Problems/{ProblemDefectToUpdate.ProblemDefectId}", ProblemDefectToUpdate);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var ProblemDefect = JsonSerializer.Deserialize<ProblemDefect>(content, _options);

            return ProblemDefect;
        }
        public async Task<bool> DeleteProblemDefect(int ProblemDefect_id)
        {
            var response = await _http.DeleteAsync($"IS/Appearance/Problems/{ProblemDefect_id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return  false;
            }

                return  true;
        }


    }
}
