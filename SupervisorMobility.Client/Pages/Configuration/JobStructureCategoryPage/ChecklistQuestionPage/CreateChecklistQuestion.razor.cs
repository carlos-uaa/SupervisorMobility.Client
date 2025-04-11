using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using System.Reflection.Metadata;
using static MudBlazor.CategoryTypes;

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
        private bool _dependendcy = false;

        // Objects
        JobCategoryStructure _checklistCategory = new();
        ChecklistQuestion _question = new();
        public List<QuestionType> _questionTypes { get; set; } = new();
        public List<int> allowableQT { get; set; } = new();

        public List<(List<string> questions, List<string> actions)> _actions = new();
        Dictionary<int, (Dictionary<int, int> Questions, Dictionary<int, string> Actions)> selectedData;
        public Dictionary<int, int> Indexes { get; set; } = new();
        QuestionType questionType { get; set; }
        public List<Pillar> _pillars { get; set; } = new();
        public List<int?> SelectedPillarIds { get; set; } = new List<int?>();

        private IEnumerable<string> selectedItems = new List<string>();
        private bool _open = false;
        private static readonly HashSet<string> CodesToShowOptions = new() { "MC", "TF", "MCM" };
        private static readonly HashSet<string> CodesToAllowMoreOptions = new() { "MC", "MCM" };
        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _questionTypes = await QuestionTypeService.GetQuestionTypes();
            //Must be the same code for MultipleChoice and Yes/No as in db 
            allowableQT = _questionTypes.Where(x => x.Code == "MC" || x.Code == "TF").Select(x=>x.QuestionTypeId).ToList();
            _checklistCategory = await JobStructureCategoriesService.GetCategoryIncludingQuestions(categoryId);
            for (int i = 0; i < _checklistCategory.ChecklistQuestions.Count; i++)
            {
                Indexes[_checklistCategory.ChecklistQuestions.ElementAt(i).QuestionID] = i+1;
            }
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
            _question.Options = new();
            _question.Actions = new();
            selectedData = new();
        }

        async void CreateQuestionAsync()
        {
            _question.Prompt = _question.Prompt.Replace("\n","\\n");
            _question.PromptEN = _question.PromptEN.Replace("\n","\\n");
            _checked = true;
            _question.IsActive = true;
            _question.Pillars = SelectedPillarIds;

            _question.TypeId = questionType.QuestionTypeId;

            if (!_dependendcy)
            {
                _question.Actions = null;
            }
            else
            {
                foreach (var action in _actions)
                {
                    var questionString = string.Join("⁂", action.questions);
                    var actionString = string.Join("⁂", action.actions);
                    var finalString = string.Join("℘", questionString, actionString);
                    _question.Actions.Add(finalString);
                }
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
        private void TogglePopover()
        {
            selectedItems = new List<string>();
            _open = true;
        }

        private void HandleQuestionSelection(int qstnId, int index, int qIndex)
        {
            var temp = _actions[index].questions[qIndex].Split("§");
            if (temp.Length > 1)
            {
                temp[0] = qstnId.ToString();
                var temp2 = string.Join("§", temp);
                _actions[index].questions[qIndex] = temp2;
            }
            else
            {
                _actions[index].questions[qIndex] = $"{qstnId.ToString()}";
            }
        }
        private void HandleQuestionOperation(string operation, int index, int qIndex)
        {
            var temp = _actions[index].questions[qIndex].Split("§");
            if (temp.Length > 1)
            {
                temp[1] = operation;
                var temp2 = string.Join("§", temp);
                _actions[index].questions[qIndex] = temp2;
            }
            else
            {
                _actions[index].questions[qIndex] += $"§{operation}";
            }
        }
        private void HandleQuestionOption(string option, int index, int qIndex)
        {
            var temp = _actions[index].questions[qIndex].Split("§");
            if (temp.Length > 2)
            {
                temp[2] = option;
                var temp2 = string.Join("§", temp);
                _actions[index].questions[qIndex] = temp2;
            }
            else
            {
                _actions[index].questions[qIndex] += $"§{option}";
            }
        }
        private void HandleActionSelection(string action, int index, int aIndex)
        {
            _actions[index].actions[aIndex] = $"{action}";
        }
        private void HandleActionValueModification(string value, int index, int aIndex)
        {
            var temp = _actions[index].actions[aIndex].Split("§");
            if (temp.Length > 1)
            {
                temp[1] = value;
                var temp2 = string.Join("§", temp);
                _actions[index].actions[aIndex] = temp2;
            }
            else
            {
                _actions[index].actions[aIndex] += $"§{value}";
            }
        }

        void AddQuestionToAction(int index)
        {
            _actions[index].questions.Add("");
        }
        void AddAnotherToAction(int index)
        {
            _actions[index].actions.Add("");
        }

        private void OnTypeSelected(QuestionType value)
        {

            // Perform a custom action when the type is selected
            if(value.Code == "TF")
            {
                _question.Options = new List<string> { "YES", "NG", "N/A" };
            }
            else
            {
                _question.Options = new();
            }
        }
        void HandleValueChanged(string item, string newValue)
        {
            var index = _question.Options.IndexOf(item);

            if (index != -1) // Ensure the item exists in the list
            {
                _question.Options[index] = newValue;
            }
            _question.Options.First();
        }

        private void AddElement()
        {
            _question.Options?.Add(string.Empty);
        }

        private void RemoveElement(string index)
        {
            _question.Options?.Remove(index);
        }
        private void AddAction()
        {
            List<string> newQuestions = new List<string> { "" };
            List<string> newActions = new List<string> { "" };
            _actions.Add((newQuestions, newActions));
        }

        private void RemoveAction(int index)
        {
            _actions.RemoveAt(index);
        }

        //private async Task OpenDialog()
        //{
        //    var parameters = new DialogParameters
        //    {
        //        { "Questions", _checklistCategory.ChecklistQuestions.Where(p=>p.QuestionID != _question.QuestionID).ToList() },
        //        { "SelectedQuestion", _question.Dependencies != null ? _checklistCategory.ChecklistQuestions.Where(p=> _question.Dependencies.Any(d=>d.QuestionID == p.QuestionID)).ToList() : new() }
        //    };
        //    var dialogResult = await DialogService.Show<Components.DependenciesDialog>("Add Items", parameters).Result;

        //    if (!dialogResult.Canceled)
        //    {
        //        var temp = dialogResult.Data as List<ChecklistQuestion>;
        //        var temp2 = temp.Select(q => new ChecklistQuestionDependency { QuestionID = q.QuestionID }).ToList();
        //        foreach (var newDep in temp2)
        //        {
        //            var existingDep = _question.Dependencies.FirstOrDefault(d => d.QuestionID == newDep.QuestionID);

        //            if (existingDep == null)
        //            {
        //                _question.Dependencies.Add(newDep);
        //            }
        //        }
        //        var selectedDependencyQIDs = temp2.Select(d => d.QuestionID).ToList();
        //        _question.Dependencies.RemoveAll(d => !selectedDependencyQIDs.Contains(d.QuestionID));
        //    }
        //}
    }
}
