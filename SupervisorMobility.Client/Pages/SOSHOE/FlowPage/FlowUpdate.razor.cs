using MudBlazor;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;
using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities.SOS_Process;

namespace SupervisorMobility.Client.Pages.SOSHOE.FlowPage
{
    public partial class FlowUpdate
    {
        [Parameter]
        public int? FlowId { get; set; }

        SOSFlow _sosFlow { get; set; } = new();
     
        
        public bool UpdateButton = false;


        //Commentaries and Logbok
        private DialogOptions dialogCommentariesOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };
        private DialogOptions dialogLogbookOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };

        private bool visibleCommentaries = false;
        private bool visibleLogbook = false;

        //Loading
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<MudBlazor.Color> _Colors = new List<MudBlazor.Color>() { MudBlazor.Color.Default, MudBlazor.Color.Primary, MudBlazor.Color.Secondary, MudBlazor.Color.Success, MudBlazor.Color.Info, MudBlazor.Color.Default, MudBlazor.Color.Primary, MudBlazor.Color.Secondary, MudBlazor.Color.Success, MudBlazor.Color.Info };
        public bool ShowLoading = true;
        private double totalTime;

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;
        protected async override Task OnInitializedAsync()
        {

            _sourceMsgLoading.Add($"{Localizer1["Loading1"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading2"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading3"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading4"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading5"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading6"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading7"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading8"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading9"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading10"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading11"]}");

            logged = await HasPropertyAsync();

            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error You have to log in", Severity.Error);
                NavigationManager.NavigateTo($"/");
            }
            else
            {
                _sosFlow = await SOSFlowServices.GetSOSFlow((int)FlowId, true, true, true, true, true);
            }

            ShowLoading = false;
            StateHasChanged();
        }

        #region User
        //Local storage user
        private async Task GetUserAsync()
        {
            if (!await TryGetAsync())
            {
                user = new();
            }
        }

        private async Task<bool> TryGetAsync()
        {
            bool hasProperty = await HasPropertyAsync();
            if (hasProperty)
            {
                json = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user");
                user = JsonSerializer.Deserialize<User>(json) ?? new();

            }
            return hasProperty;
        }

        private async Task<bool> HasPropertyAsync()
            => await JSRuntime.InvokeAsync<bool>("localStorage.hasOwnProperty", "user");


        #endregion

    

        public bool TryGetFlowLogbooksElementAtIndex(int index, out SOSFlowLogbook? item)
        {
            item = null;  
            if (_sosFlow.FlowLogbooks == null || _sosFlow.FlowLogbooks.Count == 0)
            {
                return false;
            }

            int invertedIndex = _sosFlow.FlowLogbooks.Count - 1 - index;

            if (invertedIndex >= 0 && invertedIndex < _sosFlow.FlowLogbooks.Count)
            {
                item = _sosFlow.FlowLogbooks?.ElementAt(invertedIndex);
                return true;
            }

            return false;
        }

        public static string ReasonFormat(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            if (!input.StartsWith("("))
            {
                input = "(" + input;
            }

            if (!input.EndsWith(")"))
            {
                input = input + ")";
            }

            return input;
        }

        private void OpenCommentariesDialog()
        {
            visibleCommentaries = true;
        }

        private void CloseCommentariesDialog()
        {
            visibleCommentaries = false;
        }

        private void OpenLogbookDialog()
        {
            visibleLogbook = true;

        }

        private void CloseLogbookDialog()
        {
            visibleLogbook = false;
        }

        private async void DownloadExcel()
        {
            //await Exportation.ExportFlowToExcel(FlowId.Value);
        }


        void HoeDetails(int HoeId)
        {
            NavigationManager.NavigateTo($"/soshoe/Hub/Details/{HoeId}");
        }
        void UpdateFlow(int FlowId)
        {
            NavigationManager.NavigateTo($"/soshoe/Flow/Update/{FlowId}");
        }


        private bool visibleApproveFlow = false;
        private void OpenApproveFlow()
        {
            visibleApproveFlow = true;
        }
        void CloseSign() => visibleApproveFlow = false;
        private DialogOptions dialogSignOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, DisableBackdropClick = true, CloseButton = true };


        public async Task ApproveFlow()
        {

            _sosFlow.FlowLogbooks.Last()!.Status = 2;

            await UpdateFlow();

            visibleApproveFlow = false;
        }

        private async Task UpdateFlow()
        {
            Snackbar.Clear();
            UpdateButton = true;

           
            //var resultSOS = await SOSHubServices.UpdateSOSHub(_sosAnalysis.SOSHub);

            //if (resultSOS != null)
            //{
            //    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            //    Snackbar.Add($"SOS Updated!", Severity.Info);
            //}
            //else
            //    await JSRuntime.InvokeVoidAsync("alert", "Error al actualizar!");

            var result = await SOSFlowServices.UpdateSOSFlow(_sosFlow);

            if (result != null)
            {
                Snackbar.Add($"Flow Updated!", Severity.Info);

                _sosFlow = result;

                NavigationManager.NavigateTo("/soshoe/Flow");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Error al actualizar!");
            UpdateButton = false;


        }
    }
}