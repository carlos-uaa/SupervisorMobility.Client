using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities.IS;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.IS_Services.DataPanelService
{
    public class DataPanelService : IDataPanelService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public DataPanelService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        #region DataPanel
        public  async Task<DataPanel> CreateDataPanel(DataPanel dataPaneltoCreate)
        {
            var response = await _http.PostAsJsonAsync($"IS/Aparence/DataPanels", dataPaneltoCreate);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var datapanel = JsonSerializer.Deserialize<DataPanel>(content, _options);

            return datapanel;
        } 

       

        public async Task<List<DataPanel>> GetAllDataPanels(bool includeSpecifications = false)
        {
            var response = await _http.GetAsync($"IS/Aparence/DataPanels?includeSpecifications={includeSpecifications}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var datapanels = JsonSerializer.Deserialize<List<DataPanel>>(content, _options);

            return datapanels;
        }

        public async Task<DataPanel> GetDataPanel(int id_datapanel, bool includeSpecifications = false)
        {
            var response = await _http.GetAsync($"IS/Aparence/DataPanels/{id_datapanel}?includeSpecifications={includeSpecifications}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var datapanel = JsonSerializer.Deserialize<DataPanel>(content, _options);

            return datapanel;
        }

        public async Task<DataPanel?> UpdateDataPanel(DataPanel dataPaneltoUpdate)
        {
            var response = await _http.PutAsJsonAsync($"IS/Aparence/DataPanels/{dataPaneltoUpdate.DataPanelId}", dataPaneltoUpdate);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var datapanel = JsonSerializer.Deserialize<DataPanel>(content, _options);

            return datapanel;
        }
        public async Task<bool> UpdatePanelSequence(int datapanel_Id, DataPanel dataPanel)
        {
            var response = await _http.PutAsJsonAsync($"IS/Aparence/DataPanels/sequence/{datapanel_Id}", dataPanel);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed");
                return false;
            }
            else
            {
                Console.WriteLine("Ok");
                return true;
            }
        }

        public async Task<DataPanel> DeleteDataPanel(int id)
        {
            var response = await _http.DeleteAsync($"IS/Aparence/DataPanels/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var datapanel = JsonSerializer.Deserialize<DataPanel>(content, _options);

            return datapanel;
        }

        #endregion

        #region DataPanelSpecification
        public async Task<DataPanelSpecification> CreateSpecification(DataPanelSpecification specification)
        {
            var response = await _http.PostAsJsonAsync($"IS/Aparence/DataPanels/Specification", specification);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var datapanel = JsonSerializer.Deserialize<DataPanelSpecification>(content, _options);

            return datapanel;
        }


        public async Task<List<DataPanelSpecification>> GetAllSpecificationsFromDataPanel(bool includeDataPanel = false)
        {
            var response = await _http.GetAsync($"IS/Aparence/DataPanels/Specification?includeDataPanel={includeDataPanel}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var datapanelspecifications = JsonSerializer.Deserialize<List<DataPanelSpecification>>(content, _options);

            return datapanelspecifications;
        }

       
        public async Task<DataPanelSpecification> GetDataPanelSpecification(int id_DataPanelSpecification, bool includeDataPanel = false)
        {
            var response = await _http.GetAsync($"IS/Aparence/DataPanels/Specification/{id_DataPanelSpecification}?includeDataPanel={includeDataPanel}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var datapanelspecification = JsonSerializer.Deserialize<DataPanelSpecification>(content, _options);

            return datapanelspecification;
        }

        public async Task<bool> UpdateSpecificationSequence(int datapanel_Id, DataPanelSpecification dataSpecification)
        {
            var response = await _http.PutAsJsonAsync($"IS/Aparence/DataPanels/Specification/sequence/{datapanel_Id}", dataSpecification);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed");
                return false;
            }
            else
            {
                Console.WriteLine("Ok");
                return true;
            }
        }
        #endregion
    }
}
