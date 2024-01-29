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

        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _checklistCategories = await ChecklistService.GetChecklistCategories();
        }

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
    }
}
