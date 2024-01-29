using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.JobStructureCategoryPage.ChecklistQuestionPage
{
    public partial class UpdateChecklistQuestion
    {
        // Parameters
        [Parameter]
        public int categoryId { get; set; }

        [Parameter]
        public int questionId { get; set; }

        public List<Pillar> _pillars { get; set; } = new();

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "/"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Job Structure Categorys", href: "/checklistcategories"),
            new BreadcrumbItem("CategoryDetail", href: ""),
            new BreadcrumbItem("UpdateQuestion", href: "", disabled: true),
        };

        // Objects
        JobCategoryStructure _checklistCategory = new();
        public ChecklistQuestion _question { get; set; } = new();
        public List<QuestionType> _questionTypes { get; set; } = new();

        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _questionTypes = await QuestionTypeService.GetQuestionTypes();

        }

        protected override async Task OnParametersSetAsync()
        {
            ChecklistQuestion dbQuestion = await ChecklistService.GetQuestionById(categoryId, questionId);
            _checklistCategory = await ChecklistService.GetCategoryById(categoryId);
            _question = dbQuestion;
            _pillars = await PillarsService.GetPillars();
        }

        // Update question
        void UpdateQuestionAsync()
        {
            ChecklistService.UpdateQuestion(categoryId, _question);
            NavigationManager.NavigateTo($"checklistcategories/category/{categoryId}");
        }

        // Cancel submit form
        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"checklistcategories/category/{categoryId}");
        }
    }
}
