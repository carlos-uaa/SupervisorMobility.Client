using MudBlazor;
using MudBlazor.Utilities;

namespace SupervisorMobility.Client.Pages.Configuration.JobStructureCategoryPage
{
    public partial class JobStructureCategorySequence
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "/"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Checklist categories", href: "/checklistcategories"),
            new BreadcrumbItem("Sequence", href: "", disabled: true),
        };

        // Objects
        public List<JobCategoryStructure> _checklistCategories { get; set; } = new();
        public JobCategoryStructure _checklistCategory { get; set; } = new();

        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _checklistCategories = await ChecklistService.GetChecklistCategories();
        }

        // Drag and drop category
        async void ItemUpdated(MudItemDropInfo<JobCategoryStructure> dropItem)
        {
            dropItem.Item.Container = dropItem.DropzoneIdentifier;

            var indexOffset = 0;
            int currentCategory;
            int newSequence;

            _checklistCategories.UpdateOrder(dropItem, item => item.Sequence, indexOffset);

            currentCategory = dropItem.Item.JobCategoryStructureId;
            newSequence = dropItem.IndexInZone + 1;

            JobCategoryStructure dbCategory = await ChecklistService.GetCategoryById(currentCategory);
            _checklistCategory = dbCategory;
            _checklistCategory.Sequence = newSequence;

            UpdateSequence(currentCategory, _checklistCategory);
        }

        // Update sequence
        void UpdateSequence(int currentCategory, JobCategoryStructure checklistCategory)
        {
            ChecklistService.UpdateCategorySequence(currentCategory, checklistCategory);
        }
    }
}
