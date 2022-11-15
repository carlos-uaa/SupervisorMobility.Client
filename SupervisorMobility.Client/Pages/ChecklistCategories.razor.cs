using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class ChecklistCategories
    {
        public List<ChecklistCategory> _checklistCategories { get; set; } = new();

        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Checklist categories", href: "", disabled: true),
        };

        protected async override Task OnInitializedAsync()
        {
            _checklistCategories = await ChecklistService.GetChecklistCategories();
        }

        void CategoryDetails(int categoryId)
        {
            NavigationManager.NavigateTo($"/checklistcategories/category/{categoryId}");
        }

        void EditCategory(int categoryId)
        {
            NavigationManager.NavigateTo($"checklistcategories/category/updatecategory/{categoryId}");
        }
    }
}
