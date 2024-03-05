using MudBlazor;
using MudBlazor.Utilities;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.Configuration.JobStructureCategoryPage.ChecklistQuestionPage
{
    public partial class ChecklistQuestionsSequence
    {

        // Parameters
        [Parameter]
        public int categoryId { get; set; }
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>();
        // Objects
        public JobCategoryStructure _checklistCategoryAndQuestions { get; set; } = new();
        public ChecklistQuestion _checklistQuestion { get; set; } = new();


        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _checklistCategoryAndQuestions = await JobStructureCategoriesService.GetCategoryIncludingQuestions(categoryId);
            Console.WriteLine(_checklistCategoryAndQuestions);
          
            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: Localizer["jobstructure"], href: $"/checklistcategories"),
                new BreadcrumbItem(text: _checklistCategoryAndQuestions.Description, href: $"/checklistcategories/category/{_checklistCategoryAndQuestions.JobCategoryStructureId}"),
                new BreadcrumbItem("Sequence", href: "", disabled: true),
            };
            BreadcrumbService.UpdateBreadcrumbs(_links);
        }

        // Drag and drop category
        async void ItemUpdated(MudItemDropInfo<ChecklistQuestion> dropItem)
        {
            dropItem.Item.Container = dropItem.DropzoneIdentifier;

            var indexOffset = 0;
            int currentQuestion;
            int newSequence;

            _checklistCategoryAndQuestions.ChecklistQuestions.UpdateOrder(dropItem, item => item.CategorySequence, indexOffset);

            currentQuestion = dropItem.Item.QuestionID;
            newSequence = dropItem.IndexInZone + 1;

            ChecklistQuestion dbCategory = await JobStructureCategoriesService.GetQuestionById(categoryId, currentQuestion);
            _checklistQuestion = dbCategory;
            _checklistQuestion.CategorySequence = newSequence;

            UpdateSequence(currentQuestion, _checklistQuestion);
        }

        // Update sequence
        void UpdateSequence(int currentQuestionId, ChecklistQuestion checklistQuestion)
        {
            JobStructureCategoriesService.UpdateChecklistQuestionSequence(categoryId, currentQuestionId, checklistQuestion);
        }

        void GoToCategories()
        {
            NavigationManager.NavigateTo($"checklistcategories/category/{categoryId}");
        }

    }
}
