using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.IS.ConfigurationIS.DataPanelPage
{
    public partial class DataPanelCategoryDetails
    {
        // Parameters
        [Parameter]
     
        public int DataPanelId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>();

        // Objects
        DataPanel _DataPanel= new();

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

        // Initialization
        protected override async Task OnParametersSetAsync()
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


            try
            {
                currentLanguage = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "i18nextLng");
                Console.WriteLine($" Current:'{currentLanguage}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Load Language: {ex.Message}");
            }

        

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

            }
            catch (Exception ex)
            {

            }
            finally
            {
                _links = new List<BreadcrumbItem>
                     {
                         new BreadcrumbItem(text: Localizer["home"], href: "/"),
                         new BreadcrumbItem(text: Localizer["configurationIS"], href: "/configurationIS"),
                         new BreadcrumbItem(text: Localizer["DataPanels"], href: $"/configurationIS/DataPanels"),
                         new BreadcrumbItem(text: _DataPanel.DataTitle, href: "", disabled: true )
                     };
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

        void UpdateCategory(int categoryId)
        {
            NavigationManager.NavigateTo($"configurationIS/DataPanels/Update/{categoryId}");
        }


        // Create specification
        void CreateSpecification(int categoryId)
        {
            NavigationManager.NavigateTo($"/configurationIS/DataPanels/Details/{DataPanelId}/CreateSpecification");
        }
        //re order specification
        void SpecificationSequence(int categoryId)
        {
            NavigationManager.NavigateTo($"/configurationIS/DataPanels/Details/{DataPanelId}/sequence");
        }
        // Delete question
        async Task DeleteSpecification(int categoryId, int questionId)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this question?");

            if (confirm)
            {
                _checklistQuestions.RemoveAll(question => question.QuestionID == questionId);
                await JobStructureCategoriesService.DeleteQuestion(categoryId, questionId);
            }
        }

        // Update question
        void UpdateSpecification(int DataPanel_Id, int specificationId)
        {
            NavigationManager.NavigateTo($"/configurationIS/DataPanels/Details/{DataPanel_Id}/UpdateSpecification/{specificationId}");
        }

        private string searchString = "";

        private bool FilterFunc(DataPanelSpecification element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.DataSpecification.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.ItemOrder.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;   
            if (element.DataPanelSpecificationId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.DataPanelSpecificationId}".Contains(searchString))
                return true;


            if (element.DataSpecification.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            var searchWords = searchString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            // Verificar si todas las palabras en searchWords están contenidas en Data Specification
            if (searchWords.All(word => element.DataSpecification.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0))
                return true;

            return false;
        }

        private int selectedRowNumber = -1;
        private MudTable<DataPanelSpecification> SelectTableEvent;

        private void RowClickEvent(TableRowClickEventArgs<DataPanelSpecification> tableRowClickEventArgs)
        {
        }

        private string SelectedRowClassFunc(DataPanelSpecification element, int rowNumber)
        {
            if (selectedRowNumber == rowNumber)
            {
                selectedRowNumber = -1;
                UpdateSpecification(DataPanelId,element.DataPanelSpecificationId);
                return string.Empty;
            }
            else if (SelectTableEvent.SelectedItem != null && SelectTableEvent.SelectedItem.Equals(element))
            {
                selectedRowNumber = rowNumber;
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
