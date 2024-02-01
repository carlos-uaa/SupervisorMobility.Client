using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.JobStructureCategoryPage
{
    public partial class UpdateJobStructureCategory
    {
        // Parameters
        [Parameter]
        public int CategoryId { get; set; }

        // Breadcrumb links 
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "/"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Checklist categories", href: "/checklistcategories"),
            new BreadcrumbItem("UpdateCategory", href: "", disabled: true),
        };

        // Objects
        public JobCategoryStructure _JobCategory { get; set; } = new();
        public List<JobCategoryStructure> _checklistCategories { get; set; } = new();
        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            _checklistCategories = await JobStructureCategoriesService.GetChecklistCategories();

            try
            {
                JobCategoryStructure dbCategory = _checklistCategories.Find(c => c.JobCategoryStructureId == CategoryId);
                _JobCategory = dbCategory;

            }catch (Exception ex)
            {
                Console.WriteLine("Error al seleccionar elemento de listado - Update Job structure");
                Console.WriteLine(ex.Message);
            }
        }

        // Update category
        void UpdateCategory()
        {
            JobStructureCategoriesService.UpdateCategory(_JobCategory);
            NavigationManager.NavigateTo($"checklistcategories");
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

        private bool HasTitularCategory()
        {
            return _checklistCategories.Any(c => c.Type == StructureType.Titular && c.JobCategoryStructureId != _JobCategory.JobCategoryStructureId);
        }
        private bool HasLUPCategory()
        {
            return _checklistCategories.Any(c => c.Type == StructureType.LUP && c.JobCategoryStructureId != _JobCategory.JobCategoryStructureId);
        } 
        private bool HasTimerCategory()
        {
            return _checklistCategories.Any(c => c.Type == StructureType.Timer && c.JobCategoryStructureId != _JobCategory.JobCategoryStructureId);
        } 
        private bool HasSignatureCategory()
        {
            return _checklistCategories.Any(c => c.Type == StructureType.Signature && c.JobCategoryStructureId != _JobCategory.JobCategoryStructureId);
        }
    }
}
