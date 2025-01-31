using Microsoft.JSInterop;
using MudBlazor;


namespace SupervisorMobility.Client.Pages.SOSHOE.CombinationPage
{
    public partial class CombinationUpdate
    {
        [Parameter]
        public int? CombinationId { get; set; }

        SOSCombination _sosCombination { get; set; } = new();
        private List<SOSCombinationLogbook> mostRecentLogs = new List<SOSCombinationLogbook>();
        private int logCount = 0;
        private int totalLogbooks = 0;
        private int remainingLogs = 0;
        private List<int> logbookIds = new List<int>();

        int cycleId = 0;
        private List<string> capturedImages = new List<string>();


        //Show evidence
        private DialogOptions dialogEvidenceOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };

        private bool visibleEvidence = false;

        private int photoIndex = 0;


        //Commentaries and Logbok
        private DialogOptions dialogCommentariesOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };
        private DialogOptions dialogLogbookOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };

        private bool visibleCommentaries = false;
        private bool visibleLogbook = false;

        //Loading
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<MudBlazor.Color> _Colors = new List<MudBlazor.Color>() { MudBlazor.Color.Default, MudBlazor.Color.Primary, MudBlazor.Color.Secondary, MudBlazor.Color.Success, MudBlazor.Color.Info, MudBlazor.Color.Default, MudBlazor.Color.Primary, MudBlazor.Color.Secondary, MudBlazor.Color.Success, MudBlazor.Color.Info };
        public bool ShowLoading = true;
        public bool UpdateButton = false;

        private double _CellSize { get; set; } = 0.02;
        private double _CellSize_Slider { get; set; } = 0.02;
        private double _HalfCellSize { get; set; } = 0.02;
        private int _Celdas { get; set; } = 100;
        private double result_tackTime;
        string[] _labels_CellSize = new string[] { "0.02", "0.05", "0.1", "0.2", "0.5", "1.0", "2.0", "5.0" };
        protected async override Task OnInitializedAsync()
        {
            _sourceMsgLoading.Add($"{Localizer1["Loading1"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading2"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading3"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading4"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading5"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading6"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading7"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading8"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading9"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading10"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading11"]}");

            _sosCombination = await SOSCombinationServices.GetSOSCombination((int)CombinationId, true, true, true, true, true, true);
            if (_sosCombination.CombinationLogbooks != null)
            {
                mostRecentLogs = _sosCombination.CombinationLogbooks
                    .OrderByDescending(log => log.SOSCombinationLogbookId)
                    .Take(Math.Min(3, _sosCombination.CombinationLogbooks.Count))
                    .OrderBy(log => log.SOSCombinationLogbookId)
                    .ToList();

                logCount = mostRecentLogs.Count;
                totalLogbooks = _sosCombination.CombinationLogbooks.Count;

            }

            remainingLogs = 3 - logCount;
            logbookIds = mostRecentLogs.Select(log => log.SOSCombinationLogbookId).ToList();

            //Validacion de que se creen los tiempos en caso de que no esten

            foreach (var section in _sosCombination.SOSHub?.Sections)
            {
                if (!_sosCombination.SOSCombinationOperationSequence.Any(sc => sc.SectionId == section.SectionId))
                {
                    SOSCombinationOperationSequence newToAdd = new SOSCombinationOperationSequence();
                    newToAdd.SectionId = section.SectionId;
                    newToAdd.ProcessName = section.Step;
                    newToAdd.IsActive = true;

                    _sosCombination.SOSCombinationOperationSequence.Add(newToAdd);
                }
            }


            if (_sosCombination.Illustrations != null && _sosCombination.Illustrations.Any())
            {

                foreach (var combinationImage in _sosCombination.Illustrations)
                {
                    var image = await SOSCombinationServices.ShowIlustrationSOSCombination(combinationImage.FileUploadId);
                    capturedImages.Add(image);
                }
            }
            cycleId = _sosCombination.SOSHub?.TrainingTime != null ? GetCycleId(_sosCombination.SOSHub.TrainingTime) : 0;

            UpdateTableValues();




            ShowLoading = false;
            StateHasChanged();
        }

        private List<string> GetRevisionNumbers()
        {
            List<string> revisionNumbers = new List<string> { "", "", "" };

            if (totalLogbooks <= 3)
            {
                for (int i = 0; i < totalLogbooks; i++)
                {
                    if (i == 0)
                    {
                        revisionNumbers[0] = "N";
                    }
                    else
                    {
                        revisionNumbers[i] = (i).ToString();
                    }
                }
            }
            else
            {
                revisionNumbers[0] = (totalLogbooks - 3).ToString();
                revisionNumbers[1] = (totalLogbooks - 2).ToString();
                revisionNumbers[2] = (totalLogbooks - 1).ToString();
            }

            return revisionNumbers;
        }

        public static int GetCycleId(string trainingTime)
        {
            string cycleIdString = trainingTime.Split(' ').First();

            if (int.TryParse(cycleIdString, out int cycleId))
            {
                return cycleId;
            }
            else
            {
                return 0;
            }
        }
        //protected override void OnAfterRender(bool firstRender)
        //{
        //    IndexCombination = 0;
        //}

        //protected override void OnParametersSet()
        //{
        //    IndexCombination = 0;
        //}


        private void OpenEvidenceDialog(int index)
        {
            photoIndex = index;
            visibleEvidence = true;

        }

        private void OpenCommentariesDialog()
        {
            visibleCommentaries = true;

        }

        private void CloseCommentariesDialog()
        {
            visibleCommentaries = false;
        }

        private void OpenLogbookDialog()
        {
            visibleLogbook = true;

        }

        private void CloseLogbookDialog()
        {
            visibleLogbook = false;
        }

        private async void DownloadExcel()
        {
            //await Exportation.ExportCombinationToExcel(CombinationId.Value);
        }

        private async Task MoveStepsProcess(int index, int direction)
        {

            int newIndex = index + direction;

            if (newIndex < 0 || newIndex >= _sosCombination.SOSCombinationOperationSequence.Count)
            {
                Console.WriteLine("Into Return move");

                return;
            }

            var StepsProcess = _sosCombination.SOSCombinationOperationSequence.ToList();
            var temp = StepsProcess[index];
            StepsProcess[index] = StepsProcess[newIndex];
            StepsProcess[newIndex] = temp;

            for (int i = 0; i < StepsProcess.Count; i++)
            {
                StepsProcess[i].SequenceId = i + 1;
            }

            _sosCombination.SOSCombinationOperationSequence = StepsProcess;
           
        }

        private async Task UpdateCombination()
        {
            Snackbar.Clear();
            UpdateButton = true;
        
            var result = await SOSCombinationServices.UpdateSOSCombination(_sosCombination);

            if (result != null)
            {
                Snackbar.Add($"Combination Updated!", Severity.Info);

                _sosCombination = result;
                //_ = await UploadEvidence();

                NavigationManager.NavigateTo("/SOSHOE/Combination");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Error al actualizar!");
            
            UpdateButton = false;

        }

        private void UpdateTableValues()
        {
            switch (_CellSize_Slider)
            {
                case 12.5:
                    _CellSize = 0.05;
                    break;
                case 25:
                    _CellSize = 0.1;
                    break;
                case 37.5:
                    _CellSize = 0.2;
                    break;
                case 50:
                    _CellSize = 0.5;
                    break;
                case 62.5:
                    _CellSize = 1.0;
                    break; 
                case 75:
                    _CellSize = 2.0;
                    break; 
                case 87.5:
                    _CellSize = 5.0;
                    break;
                default: _CellSize = 0.02;
                    break;
            }


            double tackTime = 0;
            double.TryParse(_sosCombination.TackTime?.Replace(",", ".") ?? "0", System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tackTime);

            if (tackTime > 0)
            {
                double decimalPart = tackTime - Math.Floor(tackTime);

                if (decimalPart <= 0.5)
                {
                    result_tackTime = (tackTime == 0.5) ? 1.0 : Math.Floor(tackTime) + 0.5;
                }
                else
                {
                    result_tackTime = Math.Ceiling(tackTime); 
                }

                _Celdas = (int)(result_tackTime / _CellSize);
                _HalfCellSize =  Math.Round(_CellSize / 2, 3); 
            }
            else
            {
                _HalfCellSize = Math.Round(_CellSize / 2, 3);
                _Celdas = 100;
            }
        }
    }
}