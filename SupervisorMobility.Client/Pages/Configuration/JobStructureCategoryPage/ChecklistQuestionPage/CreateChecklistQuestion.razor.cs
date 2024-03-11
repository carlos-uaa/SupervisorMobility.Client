using MudBlazor;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.Configuration.JobStructureCategoryPage.ChecklistQuestionPage
{
    public partial class CreateChecklistQuestion
    {
        // Parameters
        [Parameter]
        public int categoryId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>();

        // Objects
        JobCategoryStructure _checklistCategory = new();
        ChecklistQuestion _question = new();
        public List<QuestionType> _questionTypes { get; set; } = new();
        public List<Pillar> _pillars { get; set; } = new();

        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _questionTypes = await QuestionTypeService.GetQuestionTypes();
            _checklistCategory = await JobStructureCategoriesService.GetCategoryById(categoryId);
            _pillars = await PillarsService.GetPillars();

            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: Localizer["jobstructure"], href: $"/checklistcategories"),
                new BreadcrumbItem(text: _checklistCategory.Description, href: $"/checklistcategories/category/{_checklistCategory.JobCategoryStructureId}"),
                new BreadcrumbItem("New question", href: "", disabled: true),
            };
            BreadcrumbService.UpdateBreadcrumbs(_links);

            _question.Pillars = new List<int> { 0 };
        }

        // Create question
        async void CreateQuestionAsync()
        {
            _question.IsActive = true;
            var result = await JobStructureCategoriesService.CreateQuestion(categoryId, _question);
            NavigationManager.NavigateTo($"checklistcategories/category/{categoryId}");
        }

        // Cancel submit form
        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"checklistcategories/category/{categoryId}");
        }

        //Add pillar to list
        void AddPillar()
        {
            _question.Pillars.Add(0);
        }

        // Remove pillar from list
        void RemovePillar(int index)
        {
            _question.Pillars.RemoveAt(index);
            if (!_question.Pillars.Any())
                _question.Pillars.Add(0);
        }
    }
}
