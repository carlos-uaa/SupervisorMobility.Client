using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.Configuration.JobStructureCategoryPage
{
    public partial class JobStructureCategoryDetail
{
        // Parameters
        [Parameter]
        public int CategoryId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>();

        // Objects
        JobCategoryStructure _checklistCategory = new();
        public List<ChecklistQuestion> _checklistQuestions { get; set; } = new();
        string currentLanguage = "es-ES";

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            
            try
            {
                currentLanguage = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "i18nextLng");
                Console.WriteLine($" Current:'{currentLanguage}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Load Language: {ex.Message}");
            }
            _checklistCategory = await JobStructureCategoriesService.GetCategoryIncludingQuestions(CategoryId);

            if(_checklistCategory.Type != StructureType.Checklist)
            {
                //redirection 
                NavigationManager.NavigateTo($"checklistcategories");
            }

            _checklistQuestions = await JobStructureCategoriesService.GetChecklistQuestionsByCategoryId(CategoryId);

            _links = new List<BreadcrumbItem>
                     {
                         new BreadcrumbItem(text: Localizer["home"], href: "/"),
                         new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                         new BreadcrumbItem(text: Localizer["jobstructure"], href: $"/checklistcategories"),
                         new BreadcrumbItem(text: _checklistCategory.Description, href: $"/checklistcategories/category/{_checklistCategory.JobCategoryStructureId}"),
                     };
            BreadcrumbService.UpdateBreadcrumbs(_links);
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
                await JobStructureCategoriesService.DeleteQuestion(categoryId, questionId);
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
            if ($"{element.JobCategoryStructureId} {element.Prompt} {element.NotGood} {element.CategorySequence}".Contains(searchString))
                return true;
            return false;
        }

        private int selectedRowNumber = -1;
        private MudTable<ChecklistQuestion> SelectTableEvent;

        private void RowClickEvent(TableRowClickEventArgs<ChecklistQuestion> args)
        {
            var visibleItems = SelectTableEvent.FilteredItems.ToList();
            var rowIndex = visibleItems.IndexOf(args.Item);

            if (selectedRowNumber == rowIndex)
            {
                NavigationManager.NavigateTo($"checklistcategories/category/{CategoryId}/updatequestion/{args.Item.QuestionID}");
            }
            else
            {
                SelectTableEvent.SelectedItem = args.Item;
                selectedRowNumber = rowIndex;
                StateHasChanged();
            }

        }

        private string SelectedRowClassFunc(ChecklistQuestion element, int rowNumber)
        {
            var visibleItems = SelectTableEvent.FilteredItems.ToList();

            if (selectedRowNumber == rowNumber)
            {
                return "selected"; // Marca la fila seleccionada
            }
            else if (SelectTableEvent.SelectedItem != null && SelectTableEvent.SelectedItem.Equals(element))
            {
                selectedRowNumber = visibleItems.IndexOf(element);  // Usa el índice filtrado
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
