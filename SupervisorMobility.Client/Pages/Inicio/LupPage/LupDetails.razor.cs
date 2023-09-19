using MudBlazor;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.Inicio.LupPage
{
    public partial class LupDetails
    {

        [Parameter]
        public int LupId { get; set; }

        public Lup _lup { get; set; } = new();
        public JobObservation jobObservation { get; set; } = new();

        protected async override Task OnInitializedAsync()
        {
            _lup = await LupServices.GetLupByIdWhitFile(LupId);
            jobObservation = await JobObservationService.GetJobObservationById(_lup.JobObservationId);

        }

        void GoToJobObservation(int jobObservationId)
        {
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{jobObservationId}");
        }

        private async Task DownloadFile(int fileId, string filename)
        {
            await FilesServices.DownloadFileEvidence(fileId, filename);
        }
    }
}
