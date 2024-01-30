using Microsoft.JSInterop;
using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.JobStructureCategoryPage
{
    public partial class JobStructureCategoryIndex
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "/"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Job Structure categories", href: "", disabled: true),
        };

        // Objects
        public List<JobCategoryStructure> _checklistCategories { get; set; } = new();

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _checklistCategories = await ChecklistService.GetChecklistCategories();

            await GetUserAsync();
            logged = await HasPropertyAsync();
            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error You have to log in", Severity.Error);
                NavigationManager.NavigateTo($"/");
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

        // Category details
        void CategoryDetails(int categoryId)
        {
            NavigationManager.NavigateTo($"/checklistcategories/category/{categoryId}");
        }

        // Delete category
        async Task DeleteCategory(int categoryId)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this category?");

            if (confirm)
            {
                _checklistCategories.RemoveAll(category => category.JobCategoryStructureId == categoryId);
                await ChecklistService.DeleteCategory(categoryId);
            }
        }

        // Update category
        void UpdateCategory(int categoryId)
        {
            NavigationManager.NavigateTo($"checklistcategories/category/updatecategory/{categoryId}");
        }

        private string searchString = "";

        private bool FilterFunc(JobCategoryStructure element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.JobCategoryStructureId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Code.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Sequence.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.JobCategoryStructureId} {element.Code} {element.Description} {element.Sequence}".Contains(searchString))
                return true;
            return false;
        }

        private int selectedRowNumber = -1;
        private MudTable<JobCategoryStructure> SelectTableEvent;

        private void RowClickEvent(TableRowClickEventArgs<JobCategoryStructure> tableRowClickEventArgs)
        {
        }

        private string SelectedRowClassFunc(JobCategoryStructure element, int rowNumber)
        {
            if (selectedRowNumber == rowNumber)
            {
                selectedRowNumber = -1;
                if (SelectTableEvent.SelectedItem != null && SelectTableEvent.SelectedItem.Equals(element) && element.Type == StructureType.Checklist )
                {
                    NavigationManager.NavigateTo($"checklistcategories/category/{element.JobCategoryStructureId}");
                }
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

        private string GetStructureTypeText(StructureType type)
        {
            switch (type)
            {
                case StructureType.Titular:
                    return "Title & Info";
                case StructureType.Checklist:
                    return "Checklist Section";
                case StructureType.Timer:
                    return "Cicle Timer's"; 
                case StructureType.LUP:
                    return "LUP";
                case StructureType.Signature:
                    return "Signature & Commentary";
                default:
                    return "Desconocido";
            }
        }

    }
}
