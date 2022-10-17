using Microsoft.JSInterop;

namespace SupervisorMobility.Client.Shared
{
    public partial class DeleteQuestionButton
    {
        [CascadingParameter(Name = "CategoryId")]
        public int categoryId { get; set; }

        [CascadingParameter(Name = "QuestionId")]
        public int questionId { get; set; }

        async void DeleteQuestion()
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this question?");

            if (confirm)
            {
                await ChecklistService.DeleteQuestion(categoryId, questionId);
                NavigationManager.NavigateTo($"checklistcategories/category/{categoryId}", forceLoad: true);
            }
        }
    }
}
