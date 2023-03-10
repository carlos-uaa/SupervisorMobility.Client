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
            _lup = await LupService.GetLupById(LupId);

        }

        void GoToJobObservation(int jobObservationId)
        {
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{jobObservationId}");
        }
    }
}
