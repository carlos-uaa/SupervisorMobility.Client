namespace SupervisorMobility.Client.Shared
{
    public partial class ChecklistCategoryInfo
    {
        [CascadingParameter]
        public int CategoryId { get; set; }

        public ChecklistCategory _checklistCategory { get; set; } = new();

        protected async override Task OnInitializedAsync()
        {
            _checklistCategory = await ChecklistService.GetCategoryIncludingQuestions(CategoryId);
        }
    }
}
