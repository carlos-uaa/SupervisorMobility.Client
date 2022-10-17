namespace SupervisorMobility.Client.Shared
{
    public partial class ChecklistCategoryQuestions
    {
        [CascadingParameter]
        public int CategoryId { get; set; }

        public ChecklistCategory _checklistCategory { get; set; } = new();

        protected async override Task OnInitializedAsync()
        {
            _checklistCategory = await ChecklistService.GetCategoryIncludingQuestions(CategoryId);
        }

        void CreateQuestion(int categoryId)
        {
            NavigationManager.NavigateTo($"checklistcategories/category/{categoryId}/createquestion");
        }

        void UpdateQuestion(int categoryId, int questionId)
        {
            NavigationManager.NavigateTo($"checklistcategories/category/{categoryId}/updatequestion/{questionId}");
        }
    }
}
