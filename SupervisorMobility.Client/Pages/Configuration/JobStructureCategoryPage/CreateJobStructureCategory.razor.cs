using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.JobStructureCategoryPage
{
    public partial class CreateJobStructureCategory
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "/"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Checklist categories", href: "/checklistcategories"),
            new BreadcrumbItem("New category", href: "", disabled: true),
        };
        public List<JobCategoryStructure> _checklistCategories { get; set; } = new();

        // Objects
        JobCategoryStructure _checklistCategory = new();

        protected override async Task OnParametersSetAsync()
        {
            _checklistCategories = await JobStructureCategoriesService.GetChecklistCategories();
            _checklistCategory.Type = (StructureType)(-1);
        }

            // Create checklist category
            async void CreateCategoryAsync()
        {
            _checklistCategory.IsActive = true;
            var result = await JobStructureCategoriesService.CreateCategory(_checklistCategory);
            NavigationManager.NavigateTo($"checklistcategories");
        }

        private bool HasTitularCategory()
        {
            return _checklistCategories.Any(c => c.Type == StructureType.Titular );
        }
        private bool HasLUPCategory()
        {
            return _checklistCategories.Any(c => c.Type == StructureType.LUP );
        }
        private bool HasTimerCategory()
        {
            return _checklistCategories.Any(c => c.Type == StructureType.Timer);
        }
        private bool HasSignatureCategory()
        {
            return _checklistCategories.Any(c => c.Type == StructureType.Signature);
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
