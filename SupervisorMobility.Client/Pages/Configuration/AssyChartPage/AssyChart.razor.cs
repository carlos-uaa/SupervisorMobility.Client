using Microsoft.JSInterop;
using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.AssyChartPage
{
    public partial class AssyChart
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Assy Chart", href: "", disabled: true),
        };

        // Objects
        public List<ChecklistCategory> _assychart { get; set; } = new();
        public List<ChecklistCategory> _checklistCategories { get; set; } = new();

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
                _checklistCategories.RemoveAll(category => category.ChecklistCategoryId == categoryId);
                await ChecklistService.DeleteCategory(categoryId);
            }
        }

        // Update category
        void UpdateCategory(int categoryId)
        {
            NavigationManager.NavigateTo($"checklistcategories/category/updatecategory/{categoryId}");
        }
    }
}
