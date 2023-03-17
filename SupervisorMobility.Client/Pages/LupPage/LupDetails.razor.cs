using MudBlazor;

namespace SupervisorMobility.Client.Pages.LupPage
{
    public partial class LupDetails
    {

        [Parameter]
        public int LupId { get; set; }

        public Lup _lup { get; set; } = new();

        protected async override Task OnInitializedAsync()
        {
            _lup = await LupServices.GetLupByIdWhitFile(LupId);

        }

        void GoToJobObservation(int jobObservationId)
        {
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{jobObservationId}");
        }
    }
}
