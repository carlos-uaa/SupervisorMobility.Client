using Microsoft.JSInterop;

namespace SupervisorMobility.Client.Shared
{
    public partial class DeleteCategoryButton
    {
        [CascadingParameter]
        public int CategoryId { get; set; }

        async void DeleteCategory()
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this category?");

            if (confirm)
            {
                await ChecklistService.DeleteCategory(CategoryId);
                NavigationManager.NavigateTo($"checklistcategories");
            }
        }
    }
}
