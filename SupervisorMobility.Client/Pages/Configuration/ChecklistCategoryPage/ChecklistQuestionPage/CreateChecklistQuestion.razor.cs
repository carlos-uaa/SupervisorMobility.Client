using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.ChecklistCategoryPage.ChecklistQuestionPage
{
    public partial class CreateChecklistQuestion
    {
        // Parameters
        [Parameter]
        public int categoryId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Checklist categories", href: "/checklistcategories"),
            new BreadcrumbItem("CategoryDetail", href: ""),
            new BreadcrumbItem("New question", href: "", disabled: true),
        };

        // Objects
        ChecklistCategory _checklistCategory = new();
        ChecklistQuestion _question = new();
        public List<QuestionType> _questionTypes { get; set; } = new();

        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _questionTypes = await QuestionTypeService.GetQuestionTypes();
            _checklistCategory = await ChecklistService.GetCategoryById(categoryId);
        }

        // Create question
        async void CreateQuestionAsync()
        {
            var result = await ChecklistService.CreateQuestion(categoryId, _question);
            NavigationManager.NavigateTo($"checklistcategories/category/{categoryId}");
        }

        // Cancel submit form
        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"checklistcategories/category/{categoryId}");
        }
    }
}
