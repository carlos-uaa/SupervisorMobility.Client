using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class UpdateChecklistQuestion
    {
        [Parameter]
        public int categoryId { get; set; }

        [Parameter]
        public int questionId { get; set; }

        ChecklistCategory _checklistCategory = new();
        public ChecklistQuestion _question { get; set; } = new();
        public List<QuestionType> _questionTypes { get; set; } = new();

        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Checklist categories", href: "/checklistcategories"),
            new BreadcrumbItem("CategoryDetail", href: ""),
            new BreadcrumbItem("UpdateQuestion", href: "", disabled: true),
        };

        protected async override Task OnInitializedAsync()
        {
            _questionTypes = await QuestionTypeService.GetQuestionTypes();
        }

        protected override async Task OnParametersSetAsync()
        {
            ChecklistQuestion dbQuestion = await ChecklistService.GetQuestionById(categoryId, questionId);
            _checklistCategory = await ChecklistService.GetCategoryById(categoryId);
            _question = dbQuestion;
        }

        void UpdateQuestionAsync()
        {
            ChecklistService.UpdateQuestion(categoryId, _question);
            NavigationManager.NavigateTo($"checklistcategories/category/{categoryId}");
        }

        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"checklistcategories/category/{categoryId}");
        }
    }
}
