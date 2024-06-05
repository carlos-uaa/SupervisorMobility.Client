using Microsoft.JSInterop;
using MudBlazor;
using static SupervisorMobility.Client.Pages.ConfigurationIS.DataPanelPage.DataPanelForm;
using SupervisorMobility.Client.Data.Entities.IS;

namespace SupervisorMobility.Client.Pages.ConfigurationIS.DataPanelPage.DataPanelSpecificationPage
{
    public partial class DataPanelSpecificationForm
    {
        // Parameters
        [Parameter]

        public int DataPanelId { get; set; }
        // Parameters
        [Parameter]

        public int? DataSpecificationId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>();

        // Objects
        DataPanel _DataPanel = new();
        DataPanelSpecification _DataPanelSpecification = new();

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        JobCategoryStructure _checklistCategory = new();
        public List<ChecklistQuestion> _checklistQuestions { get; set; } = new();
        string currentLanguage = "es-ES";

        //Loading Elements
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };

        public bool ShowLoading = true;

        public enum PageType
        {
            Create,
            Update
        }

        public PageType pageType { get; set; }

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            var currentUrl = NavigationManager.Uri;
            pageType = currentUrl.Contains("Create", StringComparison.OrdinalIgnoreCase) ? PageType.Create : PageType.Update;

            _links = new List<BreadcrumbItem>
                     {
                         new BreadcrumbItem(text: Localizer["home"], href: "/"),
                         new BreadcrumbItem(text: Localizer["configurationIS"], href: "/configurationIS"),
                         new BreadcrumbItem(text: Localizer["DataPanels"], href: $"/configurationIS/DataPanels"),
                     };

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

            try
            {
                await GetUserAsync();
                logged = await HasPropertyAsync();
                if (!logged)
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error You have to log in", Severity.Error);
                    NavigationManager.NavigateTo($"/");
                }

                _DataPanel = await DataPanelServices.GetDataPanel(DataPanelId, true);
               
                _links.Add(new BreadcrumbItem(text: _DataPanel.DataTitle, href: $"/configurationIS/DataPanels/Details/{DataPanelId}"));

                switch (pageType)
                {
                    case PageType.Create:
                        _DataPanelSpecification.IsActive = true;
                        _DataPanelSpecification.DataPanelId = DataPanelId;
                        _links.Add(new BreadcrumbItem(text: Localizer["DataSpecificationCreate"], href: $"/configurationIS/DataPanels/Details/{DataPanelId}", disabled: true));
                        break;

                    case PageType.Update:
                        if (DataSpecificationId != null)
                        {
                            _DataPanelSpecification = _DataPanel.Specifications.ToList().Find(s => s.DataPanelSpecificationId == DataSpecificationId);
                            _links.Add(new BreadcrumbItem(text: Localizer["DataSpecificationUpdate"], href: $"/configurationIS/DataPanels/Details/{DataPanelId}", disabled: true));
                            _links.Add(new BreadcrumbItem(text: _DataPanelSpecification.DataSpecification, href: $"/configurationIS/DataPanels/Details/{DataPanelId}", disabled: true));
                        }
                        break;
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
               
                BreadcrumbService.UpdateBreadcrumbs(_links);
                ShowLoading = false;
                base.StateHasChanged();
            }
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


        private async void SubmitOperations()
        {
            switch (pageType)
            {
                case PageType.Create:
                        

                    var resultCreate = await DataPanelServices.CreateSpecification(_DataPanelSpecification);

                    if (resultCreate != null)
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Create Succes", Severity.Success);
                        NavigationManager.NavigateTo($"/configurationIS/DataPanels/Details/{DataPanelId}");
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Create Fail", Severity.Error);
                    }

                    break;

                case PageType.Update:
                    DataPanel? resultUpdate = await DataPanelServices.UpdateDataPanel(_DataPanel);

                    if (resultUpdate != null)
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Update Succes", Severity.Success);
                        NavigationManager.NavigateTo($"/configurationIS/DataPanels/Details/{DataPanelId}");

                    }
                    else
                    {
                        //suces create
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Update Fail", Severity.Error);
                    }
                    break;
            }

        }

        void CancelPage()
        {
            NavigationManager.NavigateTo($"/configurationIS/DataPanels/Details/{DataPanelId}");
        }

    }//end class
}