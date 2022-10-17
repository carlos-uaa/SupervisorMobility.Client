using MudBlazor;
using MudBlazor.Utilities;

namespace SupervisorMobility.Client.Pages
{
    public partial class ChecklistCategoriesSequence
    {
        public List<ChecklistCategory> _checklistCategories { get; set; } = new();
        public ChecklistCategory _checklistCategory { get; set; } = new();

        protected async override Task OnInitializedAsync()
        {
            _checklistCategories = await ChecklistService.GetChecklistCategories();
        }

        async void ItemUpdated(MudItemDropInfo<ChecklistCategory> dropItem)
        {
            dropItem.Item.Container = dropItem.DropzoneIdentifier;

            var indexOffset = 0;
            int currentCategory;
            int newSequence;

            _checklistCategories.UpdateOrder(dropItem, item => item.Sequence, indexOffset);

            currentCategory = dropItem.Item.ChecklistCategoryId;
            newSequence = dropItem.IndexInZone + 1;

            ChecklistCategory dbCategory = await ChecklistService.GetCategoryById(currentCategory);
            _checklistCategory = dbCategory;
            _checklistCategory.Sequence = newSequence;

            UpdateSequence(currentCategory, _checklistCategory);
        }

        void UpdateSequence(int currentCategory, ChecklistCategory checklistCategory)
        {
            ChecklistService.UpdateCategorySequence(currentCategory, checklistCategory);
        }
    }
}
