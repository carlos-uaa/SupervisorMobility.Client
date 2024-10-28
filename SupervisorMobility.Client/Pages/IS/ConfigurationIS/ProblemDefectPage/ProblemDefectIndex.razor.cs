using Microsoft.JSInterop;
using MudBlazor;

namespace SupervisorMobility.Client.Pages.IS.ConfigurationIS.ProblemDefectPage
{
    public partial class ProblemDefectIndex
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>();

        // Objects
        public List<ProblemDefect> _ProblemDefects { get; set; } = new();

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        //Loading
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };
        public bool ShowLoading = true;

        // Initialization
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


            _links = new List<BreadcrumbItem>
             {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["configurationIS"], href: "/configurationIS"),
                new BreadcrumbItem(text: Localizer["ProblemDefects"], href: "", disabled: true)
            };

            BreadcrumbService.UpdateBreadcrumbs(_links);

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

                _ProblemDefects = await ProblemDefectsServices.GetAllProblemDefects();

            }
            catch (Exception ex)
            {

            }
            finally
            {
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


        // Create CreateProblemDefects
        void CreateProblemDefects()
        {
            NavigationManager.NavigateTo($"configurationIS/ProblemDefect/Create");
        }
        // Details CreateProblemDefects
        void ProblemDefectsDetails(int ProblemDefectsId)
        {
            NavigationManager.NavigateTo($"configurationIS/ProblemDefect/Details/{ProblemDefectsId}");
        }

        // Update category
        void ProblemDefectUpdate(int ProblemDefectsId)
        {
            NavigationManager.NavigateTo($"configurationIS/ProblemDefect/Update/{ProblemDefectsId}");
        }

        async Task DeleteProblemDefects(int ProblemDefectsId)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this ProblemDefects?");

            if (confirm)
            {
                _ProblemDefects.RemoveAll(p => p.ProblemDefectId == ProblemDefectsId);
                await ProblemDefectsServices.DeleteProblemDefect(ProblemDefectsId);
            }
        }

        private string searchString = "";

        private bool FilterFunc(ProblemDefect element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;


            if (element.DefectDescription.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

      
            var searchWords = searchString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Verificar si todas las palabras en searchWords están contenidas en DataTitle
            if (searchWords.All(word => element.DefectDescription.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0))
                return true;



            return false;
        }

        private int selectedRowNumber = -1;
        private MudTable<ProblemDefect> SelectTableEvent;

        private void RowClickEvent(TableRowClickEventArgs<ProblemDefect> args)
        {
            var visibleItems = SelectTableEvent.FilteredItems.ToList();
            var rowIndex = visibleItems.IndexOf(args.Item);

            if (selectedRowNumber == rowIndex)
            {
                    ProblemDefectsDetails(args.Item.ProblemDefectId);
            }
            else
            {
                SelectTableEvent.SelectedItem = args.Item;
                selectedRowNumber = rowIndex;
                StateHasChanged();
            }
        }

        private string SelectedRowClassFunc(ProblemDefect element, int rowNumber)
        {
            var visibleItems = SelectTableEvent.FilteredItems.ToList();

            if (selectedRowNumber == rowNumber)
            {
                return "selected"; // Marca la fila seleccionada
            }
            else if (SelectTableEvent.SelectedItem != null && SelectTableEvent.SelectedItem.Equals(element))
            {
                selectedRowNumber = visibleItems.IndexOf(element);  // Usa el índice filtrado
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }



    }
}