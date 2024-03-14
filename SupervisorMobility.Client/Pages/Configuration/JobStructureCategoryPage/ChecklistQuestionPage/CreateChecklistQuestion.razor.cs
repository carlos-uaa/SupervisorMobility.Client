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

        private bool _checked = false;

        // Objects
        JobCategoryStructure _checklistCategory = new();
        ChecklistQuestion _question = new();
        public List<QuestionType> _questionTypes { get; set; } = new();
        public List<Pillar> _pillars { get; set; } = new();
        public List<int?> SelectedPillarIds { get; set; } = new List<int?>();


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

            _question.Pillars = new List<int?> { null };
        }

        async void CreateQuestionAsync()
        {
            _checked = true;
            _question.IsActive = true;
            _question.Pillars = SelectedPillarIds;

            Console.WriteLine("aaa");
            foreach(var pilar in _question.Pillars)
            {
                Console.WriteLine(pilar);

            }
            var result = await JobStructureCategoriesService.CreateQuestion(categoryId, _question);
            NavigationManager.NavigateTo($"checklistcategories/category/{categoryId}");
        }

        // Cancel submit form
        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"checklistcategories/category/{categoryId}");
        }


        private void HandlePillarCheckedChanged(bool isChecked, int pillarId)
        {
            if (isChecked)
            {
                if (!SelectedPillarIds.Contains(pillarId))
                {
                    SelectedPillarIds.Add(pillarId);
                }
            }
            else
            {
                SelectedPillarIds.Remove(pillarId);
            }
        }

    }
}
