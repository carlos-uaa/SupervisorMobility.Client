using MudBlazor;
using System.Text.RegularExpressions;
using System.Text;
using SupervisorMobility.Client.Data.Entities.SOS_Process;
using System.Globalization;
using DocumentFormat.OpenXml.Vml.Spreadsheet;

namespace SupervisorMobility.Client.Pages.SOSHOE.DistributionPage
{
    public partial class DistributionDetails
    {
        [Parameter]
        public int? DistributionId { get; set; }

        SOSDistribution _sosDistribution { get; set; } = new();
        private List<SOSDistributionLogbook> mostRecentLogs = new List<SOSDistributionLogbook>();
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

        private string[] additionalTimes = new string[] { "0", "0", "0", "0", "0" };
        private string[] cycleTimes = new string[] { "0", "0", "0", "0", "0" };
        private string[] applicationModels = new string[] { "", "", "", "", "" };

        private string[] takeQuantity = new string[] { "0", "0", "0", "0", "0" };
        private string[] takeTime = new string[] { "0", "0", "0", "0", "0", "0" };
        private string[] leaveQuantity = new string[] { "0", "0", "0", "0", "0" };
        private string[] leaveTime = new string[] { "0", "0", "0", "0", "0", "0" };
        private string[] stepsQuantity = new string[] { "0", "0", "0", "0", "0" };
        private string[] stepsTime = new string[] { "0", "0", "0", "0", "0", "0" };

        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>();

        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>
             {
                  new BreadcrumbItem(text: Localizer["homeSOSHOE"], href: "/soshoe"),
                 new BreadcrumbItem(text: Localizer["HOESOSDistributionTitle"], href: "/soshoe/Distribution"),
                 new BreadcrumbItem(text: Localizer["Details"], href: "/soshoe/Distribution/Details", disabled: true)
                        };
            BreadcrumbService.UpdateBreadcrumbs(_links);

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

            _sosDistribution = await SOSDistributionServices.GetSOSDistribution((int)DistributionId, true, true, true, true, true, includeTimes: true, includeCollections: true);
            if (_sosDistribution.DistributionLogbooks != null)
            {
                mostRecentLogs = _sosDistribution.DistributionLogbooks
                    .OrderByDescending(log => log.SOSDistributionLogbookId)
                    .Take(Math.Min(3, _sosDistribution.DistributionLogbooks.Count))
                    .OrderBy(log => log.SOSDistributionLogbookId)
                    .ToList();

                logCount = mostRecentLogs.Count;
                totalLogbooks = _sosDistribution.DistributionLogbooks.Count;

            }

            remainingLogs = 3 - logCount;
            logbookIds = mostRecentLogs.Select(log => log.SOSDistributionLogbookId).ToList();



            if (_sosDistribution.Illustrations != null && _sosDistribution.Illustrations.Any())
            {

                foreach (var distributionImage in _sosDistribution.Illustrations)
                {
                    var image = await SOSDistributionServices.ShowIlustrationSOSDistribution(distributionImage.FileUploadId);
                    capturedImages.Add(image);
                }
            }
            cycleId = _sosDistribution.SOSHubs?.FirstOrDefault()?.TrainingTime ?? 0;

            //creacion artificial
            //if (_sosDistribution.Times == null)
            //{
            //    _sosDistribution.Times = new List<SOSTime>();
            //    foreach (Section section in _sosDistribution.SOSHub.Sections)
            //    {
            //        SOSTime newitem = new SOSTime();

            //        newitem.SectionId = section.SectionId;
            //        newitem.IsActive = true;
            //        newitem.Time = "0";

            //        _sosDistribution.Times.Add(newitem);
            //    }
            //}
            //else
            //{
            //    //iterar sobre existentes para a�adir casos faltantes de haber
            //    foreach (Section section in _sosDistribution.SOSHub.Sections)
            //    {
            //        if (!_sosDistribution.Times.Any(t => t.SectionId == section.SectionId))
            //        {
            //            SOSTime newitem = new SOSTime();

            //            newitem.SectionId = section.SectionId;
            //            newitem.IsActive = true;
            //            newitem.Time = "";

            //            _sosDistribution.Times.Add(newitem);
            //        }
            //    }
            //}
            var sosDistributionAdditionalTime = _sosDistribution.SOSDistributionAdditionalTime;


            var tempAdditionalTimes = _sosDistribution.AdditionalTime?.Split("�") ?? new string[0];
            var tempCycleTimes = _sosDistribution.CycleTime?.Split("�") ?? new string[0];
            var tempApplicationModels = _sosDistribution.AplicationModels?.Split("�") ?? new string[0];

            var tempTakeQuantity = sosDistributionAdditionalTime?.TakeQuantity?.Split('�') ?? new string[0];
            var tempTakeTime = sosDistributionAdditionalTime?.TakeTime?.Split('�') ?? new string[0];
            var tempLeaveQuantity = sosDistributionAdditionalTime?.LeaveQuantity?.Split('�') ?? new string[0];
            var tempLeaveTime = sosDistributionAdditionalTime?.LeaveTime?.Split('�') ?? new string[0];
            var tempStepsQuantity = sosDistributionAdditionalTime?.StepsQuantity?.Split('�') ?? new string[0];
            var tempStepsTime = sosDistributionAdditionalTime?.StepsTime?.Split('�') ?? new string[0];

            for (int i = 0; i < 5; i++)
            {
                additionalTimes[i] = i < tempAdditionalTimes.Length && !string.IsNullOrWhiteSpace(tempAdditionalTimes[i]) && !tempAdditionalTimes[i].Contains("§") ? tempAdditionalTimes[i] : "0";
                cycleTimes[i] = i < tempCycleTimes.Length && !string.IsNullOrWhiteSpace(tempCycleTimes[i]) && !tempCycleTimes[i].Contains("§") ? tempCycleTimes[i] : "0";
                applicationModels[i] = i < tempApplicationModels.Length && !string.IsNullOrWhiteSpace(tempApplicationModels[i]) && !tempApplicationModels[i].Contains("§") ? tempApplicationModels[i] : "";
                takeQuantity[i] = i < tempTakeQuantity.Length && !string.IsNullOrWhiteSpace(tempTakeQuantity[i]) && !tempTakeQuantity[i].Contains("§") ? tempTakeQuantity[i] : "0";
                leaveQuantity[i] = i < tempLeaveQuantity.Length && !string.IsNullOrWhiteSpace(tempLeaveQuantity[i]) && !tempLeaveQuantity[i].Contains("§") ? tempLeaveQuantity[i] : "0";
                stepsQuantity[i] = i < tempStepsQuantity.Length && !string.IsNullOrWhiteSpace(tempStepsQuantity[i]) && !tempStepsQuantity[i].Contains("§") ? tempStepsQuantity[i] : "0";
            }

            for (int i = 0; i < 6; i++)
            {
                takeTime[i] = i < tempTakeTime.Length && !string.IsNullOrWhiteSpace(tempTakeTime[i]) && !tempTakeTime[i].Contains("§") ? tempTakeTime[i] : "0";
                leaveTime[i] = i < tempLeaveTime.Length && !string.IsNullOrWhiteSpace(tempLeaveTime[i]) && !tempLeaveTime[i].Contains("§") ? tempLeaveTime[i] : "0";
                stepsTime[i] = i < tempStepsTime.Length && !string.IsNullOrWhiteSpace(tempStepsTime[i]) && !tempStepsTime[i].Contains("§") ? tempStepsTime[i] : "0";
            }

            _sosDistribution.SOSDistributionOperationSequence = _sosDistribution.SOSDistributionOperationSequence
           .OrderBy(t => t.SequenceId)
           .ToList();

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
        


        public static string ReasonFormat(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            if (!input.StartsWith("("))
            {
                input = "(" + input;
            }

            if (!input.EndsWith(")"))
            {
                input = input + ")";
            }

            return input;
        }

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

        private void ClosevisibleExportDocumentDialog()
        {
            visibleExportDocument = false;
        }

        private void GoEditSosHub()
        {
            NavigationManager.NavigateTo($"/soshoe/Hub/Update/{(int)_sosDistribution.SOSHubId}");
        }

        bool visibleExportDocument = false;

        SOSHub _sosHub { get; set; } = new(); 
        private async void GoDownloadSosHub()
        {
            await Exportation.ExportDistributionToExcel(DistributionId.Value);
        }
        private async void DownloadExcel()
        {
            SnackbarService.Add("Comprobando Documento", Severity.Info);
            _sosHub = await SOSHubServices.GetSOSHub((int)_sosDistribution.SOSHubId, true, true, true, true, true, true, true, true, includeModel: true, includePeople: true, includeDocuments: true, includeCollections: true, includePats: true, includeInformation: true);

            if (HasEmptyRequiredCollections(_sosHub))
            {
                visibleExportDocument = true;

                SnackbarService.Add("Ooops. Collector Incomplete!", Severity.Error, config =>
                {
                    config.Action = "Go Edit Collector";
                    config.ActionColor = Color.Primary;
                    config.Onclick = snackbar =>
                    {
                        GoEditSosHub();
                        return Task.CompletedTask;
                    };
                });
                StateHasChanged();
                return;
            }
            else
            {
                SnackbarService.Clear();
                SnackbarService.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                SnackbarService.Add("Generando documento", Severity.Success);
                await Exportation.ExportDistributionToExcel(DistributionId.Value);
            }

        }

        private bool HasEmptyRequiredCollections(SOSHub hub)
        {
            if (hub == null)
                return true;


            // Validar AppliedModels
            if (hub.AppliedModels == null || !hub.AppliedModels.Any())
                return true;

            // Validar Images
            if (hub.Images == null || !hub.Images.Any())
                return true;

            // Validar Videos
            if (hub.Videos == null || !hub.Videos.Any())
                return true;

            // Validar SafetyEquipment
            if (hub.SafetyEquipment == null || !hub.SafetyEquipment.Any())
                return true;

            // Validar ToolsUsed
            if (hub.ToolsUsed == null || !hub.ToolsUsed.Any())
                return true;

            // Validar MaterialsUsed
            if (hub.MaterialsUsed == null || !hub.MaterialsUsed.Any())
                return true;

            // Validar Plant
            if (hub.Plant == null)
                return true;

            // Validar Area
            if (hub.Area == null)
                return true;

            // Validar Distribution
            if (hub.Distribution == null)
                return true;
            
            if (hub.ApproverOwners == null)
                return true;
            
            if (hub.ReviewerEditors == null)
                return true;



            // Si ninguna est� vac�a o nula, retorna false
            return false;
        }



    }
}