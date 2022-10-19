namespace SupervisorMobility.Client.Pages
{
    public partial class UpdateChecklistQuestion
    {
        [Parameter]
        public int categoryId { get; set; }

        [Parameter]
        public int questionId { get; set; }

        public ChecklistQuestion _question { get; set; } = new();
        public List<QuestionType> _questionTypes { get; set; } = new();

        protected async override Task OnInitializedAsync()
        {
            _questionTypes = await QuestionTypeService.GetQuestionTypes();
        }

        protected override async Task OnParametersSetAsync()
        {
            ChecklistQuestion dbQuestion = await ChecklistService.GetQuestionById(categoryId, questionId);
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
