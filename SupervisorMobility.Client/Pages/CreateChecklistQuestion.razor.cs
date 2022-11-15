using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class CreateChecklistQuestion
    {
        [Parameter]
        public int categoryId { get; set; }

        ChecklistCategory _checklistCategory = new();
        ChecklistQuestion _question = new();
        public List<QuestionType> _questionTypes { get; set; } = new();

        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Checklist categories", href: "/checklistcategories"),
            new BreadcrumbItem("CategoryDetail", href: ""),
            new BreadcrumbItem("New question", href: "", disabled: true),
        };

        protected async override Task OnInitializedAsync()
        {
            _questionTypes = await QuestionTypeService.GetQuestionTypes();
            _checklistCategory = await ChecklistService.GetCategoryById(categoryId);
        }

        async void CreateQuestionAsync()
        {
            var result = await ChecklistService.CreateQuestion(categoryId, _question);
            NavigationManager.NavigateTo($"checklistcategories/category/{categoryId}");
        }
        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"checklistcategories/category/{categoryId}");
        }
    }
}
