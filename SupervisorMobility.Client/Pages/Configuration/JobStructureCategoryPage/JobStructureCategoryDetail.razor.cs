using Microsoft.JSInterop;
using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.JobStructureCategoryPage
{
    public partial class JobStructureCategoryDetail
{
        // Parameters
        [Parameter]
        public int CategoryId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "/"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Checklist categories", href: "/checklistcategories"),
            new BreadcrumbItem("CategoryDetail", href: "", disabled: true),
        };

        // Objects
        JobCategoryStructure _checklistCategory = new();
        public List<ChecklistQuestion> _checklistQuestions { get; set; } = new();

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            _checklistCategory = await ChecklistService.GetCategoryIncludingQuestions(CategoryId);

            if(_checklistCategory.Type != StructureType.Checklist)
            {
                //redirection 
                NavigationManager.NavigateTo($"checklistcategories");
            }

            _checklistQuestions = await ChecklistService.GetChecklistQuestionsByCategoryId(CategoryId);
        }

        // Create question
        void CreateQuestion(int categoryId)
        {
            NavigationManager.NavigateTo($"checklistcategories/category/{categoryId}/createquestion");
        }
        void QuestionSequence(int categoryId)
        {
            NavigationManager.NavigateTo($"checklistcategories/category/{categoryId}/sequence");
        }
        // Delete question
        async Task DeleteQuestion(int categoryId, int questionId)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this question?");

            if (confirm)
            {
                _checklistQuestions.RemoveAll(question => question.QuestionID == questionId);
                await ChecklistService.DeleteQuestion(categoryId, questionId);
            }
        }

        // Update question
        void UpdateQuestion(int categoryId, int questionId)
        {
            NavigationManager.NavigateTo($"checklistcategories/category/{categoryId}/updatequestion/{questionId}");
        }

        private string searchString = "";

        private bool FilterFunc(ChecklistQuestion element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.QuestionID.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Prompt.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.NotGood.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.CategorySequence.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.ChecklistCategoryId} {element.Prompt} {element.NotGood} {element.CategorySequence}".Contains(searchString))
                return true;
            return false;
        }
    }
}
