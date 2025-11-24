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
        /// <summary>
        /// variable para checar que el valor del id no sea nulo
        /// </summary>
        private bool isNull = false;

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
            cycleId = _sosCombination.SOSHub?.TrainingTime ?? 0;


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

        private async Task DownloadExcel()
        {
            if (CombinationId.HasValue)
            {
                isNull = false;
                await Exportation.ExportCombinationToExcel(CombinationId.Value);
                
            }
            else
            {
                isNull = true;
                return;

            }
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
            
            lastValidOperationIndex = _sosCombination.SOSCombinationOperationSequence.Count() - 1;
            //foreach (var (operation, index) in _sosCombination.SOSCombinationOperationSequence.Select((operation, index) => (operation, index)))
            //{
            //    if (string.IsNullOrEmpty(operation.ManualOperationTime) &&
            //        string.IsNullOrEmpty(operation.ManualOperationTimeWithMachineInAutomatic) &&
            //        string.IsNullOrEmpty(operation.AutomaticMachineOperationTime) &&
            //        string.IsNullOrEmpty(operation.StepsToNextProcess) &&
            //        string.IsNullOrEmpty(operation.PartsPerCycle))
            //    {
            //        lastValidOperationIndex = index - 1;
            //        break; // Salir del bucle una vez que se encuentra la primera operaci�n vac�a
            //    }
            //}
        }


        private (int top, double angle, int left, double width) CalculateTopParameters(double step)
        {
            // Valores exactos ordenados por step, usando valores absolutos en lugar de relativos a _CellSize
            var stepPresets = new (double step, int top, double angle, int left, double width)[]
            {
                (_CellSize*1, 28, 65, -10, 60),
                (_CellSize*1.5, 25, 55, -5, 70),
                (_CellSize*2, 29, 45, -5, 75),
                (_CellSize*2.5, 35, 40, -10, 85),
                (_CellSize*3, 20, 33, -5, 100),
                (_CellSize*3.5, 30, 30, -5, 110),
                (_CellSize*4, 30, 45, -6, 123),
                (_CellSize*4.5, 20, 25, -5, 137),
                (_CellSize*5, 35, 20, -5, 150),
                (_CellSize*5.1, 26, 20, -5, 160),
                (_CellSize*6, 30, 17, -5, 177),
                (_CellSize*6.5, 32, 15, -5, 190),
                (_CellSize*7,   25, 13, -5, 202),
                (_CellSize*7.5, 30, 11, -5, 215),
                (_CellSize*8,   28, 10, -5, 228),
                (_CellSize*8.5, 27, 9,  -5, 240),
                (_CellSize*9,   32, 8,  -5, 253),
                (_CellSize*9.5, 30, 7,  -5, 265),
                (_CellSize*10,  25, 6,  -5, 278),
                (_CellSize*10.5,28, 5.5,-5, 290),
                (_CellSize*11,  30, 5,  -5, 303),
                (_CellSize*11.5,32, 4.5,-5, 315),
                (_CellSize*12,  30, 4,  -5, 328),
                (_CellSize*13,  28, 3.5,-5, 353),
                (_CellSize*14,  30, 3,  -5, 378),
                (_CellSize*15,  32, 2.5,-5, 403)
            };

            // Funci�n de interpolaci�n
            static double Interpolate(double x, double x0, double y0, double x1, double y1) =>
                y0 + (x - x0) * (y1 - y0) / (x1 - x0);

            // Manejo de bordes
            if (step <= stepPresets[0].step) return (stepPresets[0].top, stepPresets[0].angle, stepPresets[0].left, stepPresets[0].width);
            if (step >= stepPresets[^1].step) return (stepPresets[^1].top, stepPresets[^1].angle, stepPresets[^1].left, stepPresets[^1].width);

            // Encontrar el intervalo adecuado
            for (int i = 0; i < stepPresets.Length - 1; i++)
            {
                if (step >= stepPresets[i].step && step < stepPresets[i + 1].step)
                {
                    double t = (step - stepPresets[i].step) / (stepPresets[i + 1].step - stepPresets[i].step);

                    // Para top y left, seleccionamos el m�s com�n entre los dos puntos
                    int top = stepPresets[i].top == stepPresets[i + 1].top ?
                        stepPresets[i].top :
                        (t > 0.5 ? stepPresets[i + 1].top : stepPresets[i].top);

                    int left = stepPresets[i].left == stepPresets[i + 1].left ?
                        stepPresets[i].left :
                        (t > 0.5 ? stepPresets[i + 1].left : stepPresets[i].left);

                    // Interpolaci�n para angle y width
                    double angle = Interpolate(step, stepPresets[i].step, stepPresets[i].angle,
                                             stepPresets[i + 1].step, stepPresets[i + 1].angle);

                    double width = Interpolate(step, stepPresets[i].step, stepPresets[i].width,
                                             stepPresets[i + 1].step, stepPresets[i + 1].width);

                    return (top, angle, left, width);
                }
            }

            return (stepPresets[0].top, stepPresets[0].angle, stepPresets[0].left, stepPresets[0].width);
        }

        private (int top, double angle, int left, double width) CalculateMiddleParameters(double step)
        {
            // Valores exactos ordenados por step, usando valores absolutos en lugar de relativos a _CellSize
            var stepPresets = new (double step, int top, double angle, int left, double width)[]
            {
                (_CellSize*1,   40, 55, -10, 45),
                (_CellSize*1.5, 40, 40, -5,  55),
                (_CellSize*2,   35, 33, -5,  60),
                (_CellSize*2.5, 40, 30, -5,  80),
                (_CellSize*3,   40, 25, -5,  92),
                (_CellSize*3.5, 35, 20, 0,   100),
                (_CellSize*4,   45, 20, -5,  120),
                (_CellSize*4.5, 35, 15, -5,  130),
                (_CellSize*5,   35, 15, -5,  140),
                (_CellSize*5.5, 40, 15, -5,  160),
                (_CellSize*6,   35, 12, 0,   170),
                (_CellSize*6.5, 40, 12, -5,  180),
                (_CellSize*7,   35, 10, -5,  195),
                (_CellSize*7.5, 40, 10, -5,  210),
                (_CellSize*8,   35, 9,  0,   225),
                (_CellSize*8.5, 40, 8,  -5,  240),
                (_CellSize*9,   35, 8,  -5,  255),
                (_CellSize*9.5, 40, 7,  -5,  270),
                (_CellSize*10,  35, 7,  -5,  285),
                (_CellSize*10.5,40, 6.5,-5,  300),
                (_CellSize*11,  35, 6,  -5,  315),
                (_CellSize*11.5,40, 6,  -5,  330),
                (_CellSize*12,  35, 5.5,-5,  345),
                (_CellSize*13,  40, 5,  -5,  375),
                (_CellSize*14,  35, 4.5,-5,  405),
                (_CellSize*15,  40, 4,  -5,  435),
                (_CellSize*15.5, 35, 3.8, -5, 450),
                (_CellSize*16,   40, 3.5, -5, 465),
                (_CellSize*16.5, 35, 3.3, -5, 480),
                (_CellSize*17,   40, 3.2, -5, 495),
                (_CellSize*17.5, 35, 3.0, -5, 510),
                (_CellSize*18,   40, 2.9, -5, 525),
                (_CellSize*18.5, 35, 2.8, -5, 540),
                (_CellSize*19,   40, 2.7, -5, 555),
                (_CellSize*19.5, 35, 2.6, -5, 570),
                (_CellSize*20,   40, 2.5, -5, 585),
                (_CellSize*20.5, 35, 2.45, -5, 600),
                (_CellSize*21,   40, 2.4, -5, 615),
                (_CellSize*21.5, 35, 2.35, -5, 630),
                (_CellSize*22,   40, 2.3, -5, 645),
                (_CellSize*22.5, 35, 2.25, -5, 660),
                (_CellSize*23,   40, 2.2, -5, 675),
                (_CellSize*23.5, 35, 2.15, -5, 690),
                (_CellSize*24,   40, 2.1, -5, 705),
                (_CellSize*24.5, 35, 2.05, -5, 720),
                (_CellSize*25,   40, 2.0, -5, 735),
                (_CellSize*25.5, 35, 1.95, -5, 750),
                (_CellSize*26,   40, 1.9, -5, 765),
                (_CellSize*26.5, 35, 1.85, -5, 780),
                (_CellSize*27,   40, 1.8, -5, 795),
                (_CellSize*27.5, 35, 1.75, -5, 810),
                (_CellSize*28,   40, 1.7, -5, 825),
                (_CellSize*28.5, 35, 1.65, -5, 840),
                (_CellSize*29,   40, 1.6, -5, 855),
                (_CellSize*29.5, 35, 1.55, -5, 870),
                (_CellSize*30,   40, 1.5, -5, 885)
            };

            // Funci�n de interpolaci�n
            static double Interpolate(double x, double x0, double y0, double x1, double y1) =>
                y0 + (x - x0) * (y1 - y0) / (x1 - x0);

            // Manejo de bordes
            if (step <= stepPresets[0].step) return (stepPresets[0].top, stepPresets[0].angle, stepPresets[0].left, stepPresets[0].width);
            if (step >= stepPresets[^1].step) return (stepPresets[^1].top, stepPresets[^1].angle, stepPresets[^1].left, stepPresets[^1].width);

            // Encontrar el intervalo adecuado
            for (int i = 0; i < stepPresets.Length - 1; i++)
            {
                if (step >= stepPresets[i].step && step < stepPresets[i + 1].step)
                {
                    double t = (step - stepPresets[i].step) / (stepPresets[i + 1].step - stepPresets[i].step);

                    // Para top y left, seleccionamos el m�s com�n entre los dos puntos
                    int top = stepPresets[i].top == stepPresets[i + 1].top ?
                        stepPresets[i].top :
                        (t > 0.5 ? stepPresets[i + 1].top : stepPresets[i].top);

                    int left = stepPresets[i].left == stepPresets[i + 1].left ?
                        stepPresets[i].left :
                        (t > 0.5 ? stepPresets[i + 1].left : stepPresets[i].left);

                    // Interpolaci�n para angle y width
                    double angle = Interpolate(step, stepPresets[i].step, stepPresets[i].angle,
                                             stepPresets[i + 1].step, stepPresets[i + 1].angle);

                    double width = Interpolate(step, stepPresets[i].step, stepPresets[i].width,
                                             stepPresets[i + 1].step, stepPresets[i + 1].width);

                    return (top, angle, left, width);
                }
            }

            return (stepPresets[0].top, stepPresets[0].angle, stepPresets[0].left, stepPresets[0].width);
        }


    }
}