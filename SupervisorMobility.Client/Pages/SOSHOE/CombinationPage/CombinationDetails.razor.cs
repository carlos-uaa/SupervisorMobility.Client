using MudBlazor;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;
using SupervisorMobility.Client.Data.Entities.SOS_Process;
using Blazorise;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Wordprocessing;
using SupervisorMobility.Client.Data.Entities.IS;

namespace SupervisorMobility.Client.Pages.SOSHOE.CombinationPage
{
    public partial class CombinationDetails
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


        private double _CellSize { get; set; } = 0.02;
        private double _CellSize_Slider { get; set; } = 0.02;
        private double _HalfCellSize { get; set; } = 0.02;
        private int _Celdas { get; set; } = 100;
        private double result_tackTime;
        double targetCells = 60;
        string[] _labels_CellSize = new string[] { "0.02", "0.05", "0.1", "0.2", "0.5", "1.0", "2.0", "5.0" };
        private double _tackTimePosition = 0;
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

            if (_sosCombination.TackTime != "")
            {
                double closestCellSize = _labels_CellSize
                   .Select(double.Parse)
                   .OrderBy(cellSize => Math.Abs((result_tackTime / cellSize) - targetCells))
                   .First();

                _CellSize = closestCellSize;

                if (double.TryParse(_sosCombination.TackTime, out double tackTime))
                {
                    _tackTimePosition = tackTime;
                }
            }

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
                default:
                    _CellSize = 0.02;
                    break;
            }

            GetLastValidOperationIndex();


            if (result_tackTime > 0)
            {
                _Celdas = (int)(result_tackTime / _CellSize);
                _HalfCellSize = Math.Round(_CellSize / 2, 3);
            }
            else
            {
                _HalfCellSize = Math.Round(_CellSize / 2, 3);
                _Celdas = 100;
            }

            switch (_CellSize)
            {
                case 0.05:
                    _CellSize_Slider = 12.5;
                    break;
                case 0.1:
                    _CellSize_Slider = 25;
                    break;
                case 0.2:
                    _CellSize_Slider = 37.5;
                    break;
                case 0.5:
                    _CellSize_Slider = 50;
                    break;
                case 1.0:
                    _CellSize_Slider = 62.5;
                    break;
                case 2.0:
                    _CellSize_Slider = 75;
                    break;
                case 5.0:
                    _CellSize_Slider = 87.5;
                    break;
                default:
                    _CellSize_Slider = 0.02;
                    break;
            }
        }
        int lastValidOperationIndex = -1;
        private void GetLastValidOperationIndex()
        {
            lastValidOperationIndex = -1; // Inicializar a -1 para el caso en que no se encuentre ninguna operación válida

            foreach (var (operation, index) in _sosCombination.SOSCombinationOperationSequence.Select((operation, index) => (operation, index)))
            {
                if (string.IsNullOrEmpty(operation.ManualOperationTime) &&
                    string.IsNullOrEmpty(operation.ManualOperationTimeWithMachineInAutomatic) &&
                    string.IsNullOrEmpty(operation.AutomaticMachineOperationTime) &&
                    string.IsNullOrEmpty(operation.StepsToNextProcess) &&
                    string.IsNullOrEmpty(operation.PartsPerCycle))
                {
                    lastValidOperationIndex = index - 1;
                    break; // Salir del bucle una vez que se encuentra la primera operación vacía
                }
            }
        }

        private double CalculateSizeStep(double steps, double top)
        {
            double fullCells = Math.Floor(steps / _CellSize);
            double remainingSteps = steps - (fullCells * _CellSize);
            double result;

            if (remainingSteps > 0 && remainingSteps <= _HalfCellSize)
            {
                result = fullCells * 33 + _HalfCellSize;
            }
            else if (remainingSteps > _HalfCellSize)
            {
                result = (fullCells + 1) * 33;
            }
            else
            {
                result = fullCells * 33;
            }

            if (top == 31)
            {
                result += 15;
            }

            return result;
        }

        private double CalculateRotateAngle(double top, double sizeStep)
        {
            if (top == 40)
            {
                if (sizeStep > 90)
                {
                    return 0;
                }
                return 25;
            }
            else if (top == 31)
            {
                return 35;
            }
            // Agrega más condiciones según sea necesario
            return 25; // Valor por defecto
        }

    }
}