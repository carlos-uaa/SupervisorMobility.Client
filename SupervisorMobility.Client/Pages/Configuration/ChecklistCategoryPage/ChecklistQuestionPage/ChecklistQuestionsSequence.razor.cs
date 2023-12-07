using MudBlazor;
using MudBlazor.Utilities;

namespace SupervisorMobility.Client.Pages.Configuration.ChecklistCategoryPage.ChecklistQuestionPage
{
    public partial class ChecklistQuestionsSequence
    {

        // Parameters
        [Parameter]
        public int categoryId { get; set; }
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "/"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Checklist categories", href: "/checklistcategories"),
            new BreadcrumbItem("CategoryDetail", href: ""),
            new BreadcrumbItem("Sequence", href: "", disabled: true),
        };

        // Objects
        public ChecklistCategory _checklistCategoryAndQuestions { get; set; } = new();
        public ChecklistQuestion _checklistQuestion { get; set; } = new();


        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _checklistCategoryAndQuestions = await ChecklistService.GetCategoryIncludingQuestions(categoryId);
            Console.WriteLine(_checklistCategoryAndQuestions);
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

            ChecklistQuestion dbCategory = await ChecklistService.GetQuestionById(categoryId, currentQuestion);
            _checklistQuestion = dbCategory;
            _checklistQuestion.CategorySequence = newSequence;

            UpdateSequence(currentQuestion, _checklistQuestion);
        }

        // Update sequence
        void UpdateSequence(int currentQuestionId, ChecklistQuestion checklistQuestion)
        {
            ChecklistService.UpdateChecklistQuestionSequence(categoryId, currentQuestionId, checklistQuestion);
        }

        void GoToCategories()
        {
            NavigationManager.NavigateTo($"checklistcategories/category/{categoryId}");
        }

    }
}
