
using Microsoft.JSInterop;
using MudBlazor;

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
                setSnackbarConfig();
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
                setSnackbarConfig();
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
                setSnackbarConfig();
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

        private void setSnackbarConfig()
        {
            snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
        }
    }
}
