using MudBlazor;
using SupervisorMobility.Client.Data.Entities.QuestionHelperEntities;

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
        QuestionType questionType { get; set; }
        public List<int> allowableQT { get; set; } = new();

        public List<(List<string> questions, List<string> actions)> _actions = new();
        Dictionary<Guid, (Dictionary<Guid, QuestionData> Questions, Dictionary<Guid, ActionData> Actions)> selectedData;
        List<Guid> SelectedDataIds = new();
        List<(List<Guid> QID, List<Guid> AID)> SelectedDataInnerIds = new();
        public Dictionary<int, int> Indexes { get; set; } = new();
        private bool _dependendcy = false;
        private static readonly HashSet<string> CodesToShowOptions = new() { "MC", "TF", "MCM" };
        private static readonly HashSet<string> CodesToAllowMoreOptions = new() { "MC", "MCM" };
        private static readonly HashSet<string> OperationsThatAddValueField = new() { "SET", "DBLOPT" };

        // Initialization
        protected async override Task OnInitializedAsync()
        {
        }

        protected override async Task OnParametersSetAsync()
        {
            ChecklistQuestion dbQuestion = await JobStructureCategoriesService.GetQuestionById(categoryId, questionId);
            _checklistCategory = await JobStructureCategoriesService.GetCategoryIncludingQuestions(categoryId);
            _question = dbQuestion;
            _question.Prompt = _question.Prompt.Replace("\\n","\n");
            _question.PromptEN = _question.PromptEN.Replace("\\n","\n");
            _question.Pillars = _question.Pillars ?? new List<int?>();
            _pillars = await PillarsService.GetPillars();


            _questionTypes = await QuestionTypeService.GetQuestionTypes();
            //Must be the same code for MultipleChoice and Yes/No as in db 
            allowableQT = _questionTypes.Where(x => x.Code == "MC" || x.Code == "TF").Select(x => x.QuestionTypeId).ToList();
            if (_question.TypeId != 0)
                questionType = _questionTypes.First(x => x.QuestionTypeId == _question.TypeId);

            for (int i = 0; i < _checklistCategory.ChecklistQuestions.Count; i++)
            {
                Indexes[_checklistCategory.ChecklistQuestions.ElementAt(i).QuestionID] = i + 1;
            }

            _links = new List<BreadcrumbItem>
                     {
                        new BreadcrumbItem(text: Localizer["home"], href: "/"),
                        new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                        new BreadcrumbItem(text: Localizer["jobstructure"], href: $"/checklistcategories"),
                        new BreadcrumbItem(text: _checklistCategory.Description, href: $"/checklistcategories/category/{_checklistCategory.JobCategoryStructureId}"),
                        new BreadcrumbItem("UpdateQuestion", href: "", disabled: true),
                     };
            BreadcrumbService.UpdateBreadcrumbs(_links);

            _question.Options = _question.Options??new();
            _question.Actions = _question.Actions??new();

            selectedData = new();
            if (_question.Actions != null && _question.Actions.Any())
            {
                _dependendcy = true;
                foreach (var (item, index) in _question.Actions.Select((item, index) => (item, index)))
                {
                    var act = item.Split("℘");
                    List<string> newQuestions = act[0].Split("⁂").ToList();
                    List<string> newActions = act[1].Split("⁂").ToList();
                    _actions.Add((newQuestions, newActions));

                    var temp1 = Guid.NewGuid();
                    SelectedDataIds.Add(temp1);
                    SelectedDataInnerIds.Add((new List<Guid> {  }, new List<Guid> {  }));
                    selectedData[temp1] = (new Dictionary<Guid, QuestionData>(), new Dictionary<Guid, ActionData>()) ;

                    for (int i = 0; i < newQuestions.Count; i++)
                    {
                        var temp = newQuestions[i].Split("§", StringSplitOptions.RemoveEmptyEntries);
                        var QidTemp = Guid.NewGuid();
                        SelectedDataInnerIds[index].QID.Add(QidTemp);
                        selectedData[temp1].Questions[QidTemp] = new QuestionData { 
                            QuestionId = temp.Length > 0 ? Int32.Parse(temp[0]) : 0,
                            Comparator = temp.Length > 1 ? temp[1] : "",
                            QstOption = temp.Length > 2 ? temp[2] : ""
                        };
                    }
                    for (int i = 0; i < newActions.Count; i++)
                    {
                        var temp = newActions[i].Split("§", StringSplitOptions.RemoveEmptyEntries);
                        var AidTemp = Guid.NewGuid();
                        SelectedDataInnerIds[index].AID.Add(AidTemp);
                        selectedData[temp1].Actions[AidTemp] = new ActionData {
                            Operation = temp.Length > 0 ? temp[0] : "",
                            Value = temp.Length > 1 ? temp[1] : ""
                        };
                    }
                }
            }
        }

        // Update question
        async void UpdateQuestionAsync()
        {

            Console.WriteLine("aaa");
            foreach (var pilar in _question.Pillars)
            {
                Console.WriteLine(pilar);

            }
            _question.Prompt = _question.Prompt.Replace("\n", "\\n");
            _question.PromptEN = _question.PromptEN.Replace("\n", "\\n");

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
        private void OnTypeSelected(QuestionType value)
        {

            // Perform a custom action when the type is selected
            if (value.Code == "TF")
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
            selectedData[SelectedDataIds[index]].Actions[SelectedDataInnerIds[index].AID[aIndex]].Value = value;
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
            var temp = Guid.NewGuid();
            SelectedDataInnerIds[index].QID.Add(temp);
            selectedData[SelectedDataIds[index]].Questions.Add(temp, new QuestionData());

            _actions[index].questions.Add("");
        }
        void RemoveQuestion(int index, int qIdx)
        {
            _actions[index].questions.RemoveAt(qIdx);
            selectedData[SelectedDataIds[index]].Questions.Remove(SelectedDataInnerIds[index].QID[qIdx]);
            SelectedDataInnerIds[index].QID.RemoveAt(qIdx);
        }
        void AddAnotherToAction(int index)
        {
            var temp = Guid.NewGuid();
            SelectedDataInnerIds[index].AID.Add(temp);
            selectedData[SelectedDataIds[index]].Actions.Add(temp, new ActionData());

            _actions[index].actions.Add("");
        }
        void RemoveAction(int index, int aIdx)
        {
            _actions[index].actions.RemoveAt(aIdx);
            selectedData[SelectedDataIds[index]].Actions.Remove(SelectedDataInnerIds[index].AID[aIdx]);
            SelectedDataInnerIds[index].AID.RemoveAt(aIdx);
        }
        private void AddAction()
        {
            List<string> newQuestions = new List<string> { "" };
            List<string> newActions = new List<string> { "" };

            var temp = Guid.NewGuid();
            SelectedDataIds.Add(temp);
            selectedData[temp] = (new Dictionary<Guid, QuestionData>(), new Dictionary<Guid, ActionData>());

            var temp2 = Guid.NewGuid();
            var temp3 = Guid.NewGuid();
            SelectedDataInnerIds.Add((new List<Guid> { temp2 }, new List<Guid> { temp3 }));
            selectedData[temp].Questions.Add(temp2, new QuestionData());
            selectedData[temp].Actions.Add(temp3, new ActionData());

            _actions.Add((newQuestions, newActions));
        }

        private void RemoveAction(int index)
        {
            _actions.RemoveAt(index);
            selectedData.Remove(SelectedDataIds[index]);
            SelectedDataIds.RemoveAt(index);
        }
    }
    
}
