using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Pages.Configuration.AssyChartPage;
using SupervisorMobility.Client.Pages.Configuration.PlantPage;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.AssyChartService
{
    public class AssyChartService : IAssyChartService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;


        public AssyChartService(HttpClient http)
        {
            _http = http;
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
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var assychart = JsonSerializer.Deserialize<AssyChart>(content, _options);
            return assychart;
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

      

        public Task<List<AssyChart>> GetAssyChartsByArea(int plantId, int areaId)
        {
            throw new NotImplementedException();
        }

        public Task<List<AssyChart>> GetAssyChartsByDistribution(int plantId, int areaId, int distributionId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<AssyChart>> GetAssyChartsByPlant(int plantId)
        {
            var response = await _http.GetAsync($"assycharts/byplantid/{plantId}");

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var assycharts = JsonSerializer.Deserialize<List<AssyChart>>(content, _options);

            return assycharts;
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


    }
}
