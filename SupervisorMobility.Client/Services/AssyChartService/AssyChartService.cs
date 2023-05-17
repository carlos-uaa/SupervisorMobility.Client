using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Pages.Configuration.AssyChartPage;
using SupervisorMobility.Client.Pages.Configuration.PlantPage;
using System.IO;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.AssyChartService
{
    public class AssyChartService : IAssyChartService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;
        private readonly HttpClient _httpBridge;
        private readonly JsonSerializerOptions _options;


        public AssyChartService(CustomHttpClientService customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService.GetApiHttpClient();
            _httpBridge = customHttpClientService.GetBridgeHttpClient();
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
        //Create Assychart
        public async Task<AssyChart> CreateAssyChart(AssyChart _newAssyChart)
        {
            var response = await _http.PostAsJsonAsync($"assycharts", _newAssyChart);
            var newAssyChart = await response.Content.ReadFromJsonAsync<AssyChart>();
            return newAssyChart;
        }
        //delete assychart
        public async Task DeleteAssyChart(int assychartId)
        {
            var response = await _http.DeleteAsync($"assycharts/{assychartId}");
        }
        //get assychart by id
        public async Task<AssyChart> GetAssyChart(int assyChartId)
        {
            var response = await _http.GetAsync($"assycharts/{assyChartId}");
            var content = await response.Content.ReadFromJsonAsync<AssyChart>(); 
            return content;
        } 
        
        public async Task<AssyChart> GetAssyChartAdvance(int plantId, int areaId, int distributionId, int operationId)
        {
            var response = await _http.GetAsync($"assycharts/plant/{plantId}/area/{areaId}/distribution/{distributionId}/operation/{operationId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<AssyChart>(content, _options);
                return result;
            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error Get Folders: {response.Content.ReadAsStringAsync().Result}");
            }

            return null;

        }

        public async Task<List<AssyChart>> GetAssyCharts()
        {
            var response = await _http.GetAsync("assycharts");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var assycharts = JsonSerializer.Deserialize<List<AssyChart>>(content, _options);

            return assycharts;
        }

      

        public async Task<List<AssyChart>> GetAssyChartsByArea(int plantId, int areaId)
        {
            var response = await _http.GetAsync($"assycharts/plant/{plantId}/area/{areaId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<AssyChart>>(content, _options);
                return result;
            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error Get AssyCharts By Area: {response.Content.ReadAsStringAsync().Result}");
            }

            return null;
        }

        public async Task<List<AssyChart>> GetAssyChartsByDistribution(int plantId, int areaId, int distributionId)
        {
            var response = await _http.GetAsync($"assycharts/plant/{plantId}/area/{areaId}/distribution/{distributionId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<AssyChart>>(content, _options);
                return result;
            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error Get AssyCharts By Plant: {response.Content.ReadAsStringAsync().Result}");
            }

            return null;
        }

        public async Task<List<AssyChart>> GetAssyChartsByPlant(int plantId)
        {
            var response = await _http.GetAsync($"assycharts/plant/{plantId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<AssyChart>>(content, _options);
                return result;
            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error Get AssyCharts By Plant: {response.Content.ReadAsStringAsync().Result}");
            }

            return null;
        }

        public async Task<bool> UpdateAssyChart(int assychartId, AssyChart AssyChartToUpdate)
        {
            var response = await _http.PutAsJsonAsync($"assycharts/{assychartId}", AssyChartToUpdate);
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }


        public async Task DownloadAssyChartFormat()
        {
            var response = await _http.GetAsync($"assycharts/DownloadAssyChartFormat");

            if (!response.IsSuccessStatusCode)
            {
                await _js.InvokeVoidAsync("alert", "Error File Download");
            }
            else
            {
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", "AssyChartFormat.xlsx", streamRef);
            }
        }

    }
}
