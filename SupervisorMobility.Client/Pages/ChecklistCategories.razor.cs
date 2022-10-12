namespace SupervisorMobility.Client.Pages
{
    public partial class ChecklistCategories
    {
        public List<ChecklistCategory> _checklistCategories { get; set; } = new();

        protected async override Task OnInitializedAsync()
        {
            _checklistCategories = await ChecklistService.GetChecklistCategories();
        }
    }
}
