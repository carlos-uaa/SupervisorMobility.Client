using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SupervisorMobility.Client.Pages.Inicio.HOEPage
{
    public partial class CreateHOE
    {
        private List<BreadcrumbItem> _links;

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        public int userType = 0;
        public string otherInformation = "In case of doubt contact supervisor or leader and stop, call and wait." +
            "Use bare hands or lint free gloves when attaching the rubber gasket. Do not re-use the water pump gasket." +
            " Do not use dropped gaskets.";

        private string analysis = string.Empty;
        private List<Segment> segments = new List<Segment>();

        public class Segment
        {
            public string MainPoint { get; set; }
            public List<string> CriticalPoints { get; set; } = new List<string>();
        }

        private bool visibleSteps = false;

        void CloseSteps()
        {
            visibleSteps = false;
        }
        private DialogOptions dialogStepsOptions = new() { CloseOnEscapeKey = false, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true, CloseButton = true };


        protected async override Task OnInitializedAsync()
        {

            _links = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem(text: Localizer["home"], href: "/"),
                    new BreadcrumbItem(text: Localizer["hoe"], href: "", disabled: true)
                };
            BreadcrumbServices.UpdateBreadcrumbs(_links);
            await GetUserAsync();
            logged = await HasPropertyAsync();
            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error You have to log in", Severity.Error);
                NavigationManager.NavigateTo($"/");
            }

            StateHasChanged();
        }

      


        //Local storage user
        private async Task GetUserAsync()
        {
            if (!await TryGetAsync())
                user = new();
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



        public void AnalyzeText()
        {
            segments.Clear();

            // Split the analysis text by '-' to get each segment
            var segmentTexts = analysis.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var segmentText in segmentTexts)
            {
                var segment = new Segment();

                var regex = new Regex(@"\*(.*?)\*");
                var matches = regex.Matches(segmentText);

                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        segment.CriticalPoints.Add(match.Groups[1].Value);
                    }
                }

                segment.MainPoint = regex.Replace(segmentText, string.Empty).Trim();
                segments.Add(segment);
            }
        }


        public void ShowStepsDialog()
        {
            visibleSteps = true;
        }

    }
}