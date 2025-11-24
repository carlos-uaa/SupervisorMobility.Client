
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Services.ExportationService
{
    public class ExportationService : IExportationService
    {
        private readonly HttpClient _http;
        private readonly ISnackbar snackbar;
        private readonly IJSRuntime _js;
        public ExportationService(HttpClient http, ISnackbar snackbar, IJSRuntime js)
        {
            _http = http;
            this.snackbar = snackbar;
            _js = js;
        }

        public async Task ExportAnalysisToExcel(int idAnalysis)
        {
            var response = await _http.GetAsync($"Exportation/Excel/Analyses/{idAnalysis}");

            if (!response.IsSuccessStatusCode)
            {
                snackbar.Add("Error while exporting, could not download file", Severity.Error);
            }
            else
            {
                var filename = response.Content.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", filename, streamRef);
            }
        }

        public async Task ExportSequenceToExcel(int idSequence)
        {
            var response = await _http.GetAsync($"Exportation/Excel/Sequence/{idSequence}");

            if (!response.IsSuccessStatusCode)
            {
                snackbar.Add("Error while exporting, could not download file", Severity.Error);
            }
            else
            {
                var filename = response.Content.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", filename, streamRef);
            }
        }

        public async Task ExportDistributionToExcel(int idDistribution)
        {
            var response = await _http.GetAsync($"Exportation/Excel/Distribution/{idDistribution}");

            if (!response.IsSuccessStatusCode)
            {
                snackbar.Add("Error while exporting, could not download file", Severity.Error);
            }
            else
            {
                var filename = response.Content.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", filename, streamRef);
            }
        }

        public async Task ExportCombinationToExcel(int idCombination)
        {
            try
            {
                var response = await _http.GetAsync($"Exportation/Excel/Combination/{idCombination}");

                if (!response.IsSuccessStatusCode)
                {
                    snackbar.Add("Error while exporting, could not download file", Severity.Error);
                }
                else
                {
                    var filename = response.Content.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                    var fileStream = response.Content.ReadAsStreamAsync();
                    using var streamRef = new DotNetStreamReference(stream: await fileStream);
                    await _js.InvokeVoidAsync("downloadFileFromStream", filename, streamRef);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error in download of combination: {ex.Message} \n Inner Exception: {ex.InnerException}");
            }
        }

        public async Task ExportYearlyPATToExcel(int idPAT)
        {
            var response = await _http.GetAsync($"Exportation/Excel/PATYearly/{idPAT}");

            if (!response.IsSuccessStatusCode)
            {
                snackbar.Add("Error while exporting, could not download file", Severity.Error);
            }
            else
            {
                var filename = response.Content.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", filename, streamRef);
            }
        }

        public async Task ExportMonthlyPATToExcel(int idPAT, int month)
        {
            var response = await _http.GetAsync($"Exportation/Excel/PATMonthly/{idPAT}?AplicationMonth={month}");

            if (!response.IsSuccessStatusCode)
            {
                snackbar.Add("Error while exporting, could not download file", Severity.Error);
            }
            else
            {
                var filename = response.Content.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", filename, streamRef);
            }
        }

        public async Task ExportHCIToExcel(int idHCI)
        {
            var response = await _http.GetAsync($"Exportation/Excel/HCI/{idHCI}");

            if (!response.IsSuccessStatusCode)
            {
                snackbar.Add("Error while exporting, could not download file", Severity.Error);
            }
            else
            {
                var filename = response.Content.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", filename, streamRef);
            }
        }

        public async Task ExportFlowToExcel(int idFlow, MultipartFormDataContent content)
        {
            var response = await _http.PostAsync($"Exportation/Excel/Flow/{idFlow}", content);

            if (!response.IsSuccessStatusCode)
            {
                snackbar.Add("Error while exporting, could not download file", Severity.Error);
            }
            else
            {
                var filename = response.Content.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", filename, streamRef);
            }
        }
        public async Task ExportCombinatationToExcel(int CombinationId)
        {
            var response = await _http.GetAsync($"Exportation/Excel/Combination/{CombinationId}");
            if (!response.IsSuccessStatusCode)
            {
                snackbar.Add("Error while exporting, could not download file", Severity.Error);
            }
            else
            {
                var filename = response.Content.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", filename, streamRef);
            }
        }
    }
}
