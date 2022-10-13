namespace SupervisorMobility.Client.Pages
{
    public partial class ChecklistCategoryDetail
    {
        [Parameter]
        public int CategoryId { get; set; }

        ChecklistCategory _checklistCategory = new();

        protected override async Task OnParametersSetAsync()
        {
            _checklistCategory = await ChecklistService.GetCategoryById(CategoryId);
        }
    }
}
