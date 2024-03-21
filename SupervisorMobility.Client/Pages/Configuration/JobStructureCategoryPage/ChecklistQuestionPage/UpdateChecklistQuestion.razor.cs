using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.JobStructureCategoryPage.ChecklistQuestionPage
{
    public partial class UpdateChecklistQuestion
    {
        // Parameters
        [Parameter]
        public int categoryId { get; set; }

        [Parameter]
        public int questionId { get; set; }

        public List<Pillar> _pillars { get; set; } = new();

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>();

        // Objects
        JobCategoryStructure _checklistCategory = new();
        public ChecklistQuestion _question { get; set; } = new();
        public List<QuestionType> _questionTypes { get; set; } = new();


        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _questionTypes = await QuestionTypeService.GetQuestionTypes();

        }

        protected override async Task OnParametersSetAsync()
        {
            ChecklistQuestion dbQuestion = await JobStructureCategoriesService.GetQuestionById(categoryId, questionId);
            _checklistCategory = await JobStructureCategoriesService.GetCategoryById(categoryId);
            _question = dbQuestion;
            _question.Pillars = _question.Pillars ?? new List<int?>();
            _pillars = await PillarsService.GetPillars();


            _links = new List<BreadcrumbItem>
                     {
                        new BreadcrumbItem(text: Localizer["home"], href: "/"),
                        new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                        new BreadcrumbItem(text: Localizer["jobstructure"], href: $"/checklistcategories"),
                        new BreadcrumbItem(text: _checklistCategory.Description, href: $"/checklistcategories/category/{_checklistCategory.JobCategoryStructureId}"),
                        new BreadcrumbItem("UpdateQuestion", href: "", disabled: true),
                     };
            BreadcrumbService.UpdateBreadcrumbs(_links);
        }

        // Update question
        async void UpdateQuestionAsync()
        {

            Console.WriteLine("aaa");
            foreach (var pilar in _question.Pillars)
            {
                Console.WriteLine(pilar);

            }
            var result = await JobStructureCategoriesService.UpdateQuestion(categoryId, _question);


            if(result != null)
            {
            NavigationManager.NavigateTo($"checklistcategories/category/{categoryId}");
            }
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
                if (!_question.Pillars.Contains(pillarId))
                {
                    _question.Pillars.Add(pillarId);
                }
            }
            else
            {
                _question.Pillars.Remove(pillarId);
            }
        }
    }
}
