using Microsoft.JSInterop;
using MudBlazor;
using static MudBlazor.CategoryTypes;

namespace SupervisorMobility.Client.Pages.JobObservationSchedule
{
    public partial class DialogExample
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        void Submit() => MudDialog.Close(DialogResult.Ok(true));
        void Cancel() => MudDialog.Cancel();
    }

}
