namespace SupervisorMobility.Client.Pages
{
    public partial class CreateChecklistQuestion
    {
        [Parameter]
        public int categoryId { get; set; }

        ChecklistQuestion _question = new();
        public List<QuestionType> _questionTypes { get; set; } = new();

        protected async override Task OnInitializedAsync()
        {
            _questionTypes = await QuestionTypeService.GetQuestionTypes();
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
