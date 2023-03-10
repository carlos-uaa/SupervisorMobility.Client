using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using System.Globalization;

namespace SupervisorMobility.Client.Pages.LupPage
{
    public partial class LupUpdate
    {

        [Parameter]
        public int LupId { get; set; }

        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("LUP", href: "/lup"),
            new BreadcrumbItem("Update Lup", href: "/", disabled: true),
        };

        public Lup _lup { get; set; } = new();

        protected async override Task OnInitializedAsync()
        {
            _lup = await LupService.GetLupById(LupId);
        }
        private async Task EditLup()
        {

            _lup.Status = 2;

            var result = await LupService.UpdateLup(_lup);

            if (result)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Lup {LupId} Updated", Severity.Info);
                NavigationManager.NavigateTo("/lup");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
        }

        public async void CancelLup()
        {
            _lup.EndDate = DateTime.Now;
            _lup.Status = 4;

            var result = await LupService.UpdateLup(_lup);

            if (result)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Lup {LupId} Canceled", Severity.Info);
                NavigationManager.NavigateTo("/lup");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
        }

        void GoBack()
        {
            NavigationManager.NavigateTo("/lup");
        }

        public async void FinishedLup()
        {

            _lup.EndDate = DateTime.Now;
            _lup.Status = 3;

            var result = await LupService.UpdateLup(_lup);

            if (result)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Lup {LupId} Finished", Severity.Info);
                NavigationManager.NavigateTo("/lup");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert

        }

        void GoToJobObservation(int jobObservationId)
        {
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{jobObservationId}");
        }

    }
}
