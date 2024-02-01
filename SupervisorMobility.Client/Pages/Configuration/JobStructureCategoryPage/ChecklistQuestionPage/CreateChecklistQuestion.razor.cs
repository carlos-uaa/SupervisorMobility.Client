using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.JobStructureCategoryPage.ChecklistQuestionPage
{
    public partial class CreateChecklistQuestion
    {
        // Parameters
        [Parameter]
        public int categoryId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "/"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Job Structure Category", href: "/checklistcategories"),
            new BreadcrumbItem("CategoryDetail", href: ""),
            new BreadcrumbItem("New question", href: "", disabled: true),
        };

        // Objects
        JobCategoryStructure _checklistCategory = new();
        ChecklistQuestion _question = new();
        public List<QuestionType> _questionTypes { get; set; } = new();
        public List<Pillar> _pillars { get; set; } = new();

        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _questionTypes = await QuestionTypeService.GetQuestionTypes();
            _checklistCategory = await JobStructureCategoriesService.GetCategoryById(categoryId);
            _pillars = await PillarsService.GetPillars();
        }

        // Create question
        async void CreateQuestionAsync()
        {
            _question.IsActive = true;
            var result = await JobStructureCategoriesService.CreateQuestion(categoryId, _question);
            NavigationManager.NavigateTo($"checklistcategories/category/{categoryId}");
        }

        // Cancel submit form
        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"checklistcategories/category/{categoryId}");
        }
    }
}
