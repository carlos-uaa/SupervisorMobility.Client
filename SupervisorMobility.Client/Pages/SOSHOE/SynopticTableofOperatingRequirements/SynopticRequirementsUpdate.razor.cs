using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Utilities;
using SupervisorMobility.Client.Data.Entities.SOS_Process;
using SupervisorMobility.Client.Data.Entities.SOS_Process.SOSSynopticTableRO;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using SupervisorMobility.Client.Pages.SOSHOE.SynopticTableofOperatingRequirements.Modals;
using System.Threading.Tasks;

namespace SupervisorMobility.Client.Pages.SOSHOE.SynopticTableofOperatingRequirements
{


    public partial class SynopticRequirementsUpdate
    {
        [Parameter]
        public int? SynopticRequirementsId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        //Loading
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<MudBlazor.Color> _Colors = new List<MudBlazor.Color>() { MudBlazor.Color.Default, MudBlazor.Color.Primary, MudBlazor.Color.Secondary, MudBlazor.Color.Success, MudBlazor.Color.Info, MudBlazor.Color.Default, MudBlazor.Color.Primary, MudBlazor.Color.Secondary, MudBlazor.Color.Success, MudBlazor.Color.Info };
        public bool ShowLoading = true;
        public bool LoadingDistributions { get; set; } = false;


        //UserLogin
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        //SynopticRequirements
        SOSSynopticTableofOperatingRequirements _sosSynopticRequeriments { get; set; } = new();
        SOSHub _soshub { get; set; } = new();
        Distribution _distribution { get; set; } = new();
        IEnumerable<SOSHubDtoList> SOSHubList = new List<SOSHubDtoList>();
        IEnumerable<SOSDristributionSTROTable> Distributions = new List<SOSDristributionSTROTable>();

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

            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["homeSOSHOE"], href: "/soshoe"),
                new BreadcrumbItem(text: Localizer["SynopticRequirements"], href: "/soshoe/SynopticRequirements"),
                new BreadcrumbItem(text: Localizer["SynopticRequirementsDetails"], href: "/soshoe/SynopticRequirements", disabled:true)
            };

            BreadcrumbService.UpdateBreadcrumbs(_links);


            logged = await HasPropertyAsync();

            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error You have to log in", Severity.Error);
                NavigationManager.NavigateTo($"/");
            }
            else
            {
                _sosSynopticRequeriments = await SynopticRequirementsService.GetSOSSynopticTableofOperatingRequirements((int)SynopticRequirementsId, true, true, true);
                _soshub = await SOSHubServices.GetSOSHub((int)_sosSynopticRequeriments.SOSHubId, true, true, includePeople: true, includeInformation: true, includeModel: true);

                AvailableSoshubs = await SOSHubServices.GetAllSOSHub();
                AvailableSoshubs = AvailableSoshubs.Where(s => s.DistributionId == _soshub.DistributionId).ToList();
                AvailableSoshubs.RemoveAll(s => s.SOSHubId == _soshub.SOSHubId);

            }


            ShowLoading = false;
            StateHasChanged();
        }

        #region UserLogin
        //Local storage user
        private async Task GetUserAsync()
        {
            if (!await TryGetAsync())
            {
                user = new();
            }
        }

        private async Task<bool> TryGetAsync()
        {
            bool hasProperty = await HasPropertyAsync();
            if (hasProperty)
            {
                json = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user");
                user = System.Text.Json.JsonSerializer.Deserialize<User>(json) ?? new();

            }
            return hasProperty;
        }

        private async Task<bool> HasPropertyAsync()
            => await JSRuntime.InvokeAsync<bool>("localStorage.hasOwnProperty", "user");


        #endregion

        #region SynopticRequirements
        private void UpdateSynopticRequirements(int SynopticId)
        {
            NavigationManager.NavigateTo($"soshoe/SynopticRequirements/Update/{SynopticId}");
        }



        #endregion

        #region SynopticRequirementsLogbook

        public bool TryGetSynopticRequirementsLogbooksElementAtIndex(int index, out SOSSynopticRequirementsLogbook? item)
        {
            item = null;
            if (_sosSynopticRequeriments.SynopticRequirementsLogbooks == null || _sosSynopticRequeriments.SynopticRequirementsLogbooks.Count == 0)
            {
                return false;
            }

            int invertedIndex = _sosSynopticRequeriments.SynopticRequirementsLogbooks.Count - 1 - index;

            if (invertedIndex >= 0 && invertedIndex < _sosSynopticRequeriments.SynopticRequirementsLogbooks.Count)
            {
                item = _sosSynopticRequeriments?.SynopticRequirementsLogbooks?.ElementAt(invertedIndex);
                return true;
            }

            return false;
        }
        #endregion

        #region Hoe
        private void HoeDetails(int HoeId)
        {
            NavigationManager.NavigateTo($"/soshoe/Hub/Details/{HoeId}");
        }
        #endregion

        #region Sequencesanalyses
        List<SOSHub> AvailableSoshubs = new();

        List<SOSAnalysis> AvailableAnalyses = new();
        List<SOSSequence> AvailableSequences = new();

        private string searchSosHub = "";
        private IEnumerable<SOSHub> FilteredSosHubs =>
            AvailableSoshubs.Where(op =>
                string.IsNullOrEmpty(searchSosHub) ||
                (op.Folio?.Contains(searchSosHub, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (op.ProcessSheet?.Contains(searchSosHub, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (op.OtherInformation?.Contains(searchSosHub, StringComparison.OrdinalIgnoreCase) ?? false));


        private string searchAnalysis = "";
        private IEnumerable<SOSAnalysis> FilteredAnalysis =>
            AvailableAnalyses.Where(op =>
                string.IsNullOrEmpty(searchAnalysis) ||
                (op.InternalControlNumber?.Contains(searchAnalysis, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (op.ProcessName?.Contains(searchAnalysis, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (op.OperationName?.Contains(searchAnalysis, StringComparison.OrdinalIgnoreCase) ?? false));

        private string searchSequence = "";
        private IEnumerable<SOSSequence> FilteredSequences =>
            AvailableSequences.Where(op =>
                string.IsNullOrEmpty(searchSequence) ||
                (op.InternalControlNumber?.Contains(searchSequence, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (op.ProcessName?.Contains(searchSequence, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (op.OperationName?.Contains(searchSequence, StringComparison.OrdinalIgnoreCase) ?? false));

        private List<DropItem> _combinedItems = new();

        public class DropItem
        {
            public string Name { get; init; }
            public string Type { get; init; } // SOSAnalysis or SOSSequenceId
            public string Zone { get; set; } // SOSAnalysis, SOSSequenceId, or Combined
            public string Identifier { get; set; }
            public int Sequence { get; set; }

            public Section section { get; set; }
        }


        private IEnumerable<SOSAnalysis> AnalysesSelected
        {
            get
            {
                return _sosSynopticRequeriments.Analyses;
            }

            set
            {
                _sosSynopticRequeriments.Analyses = value;
                CloseAnalysesSequences();
            }
        }

        private IEnumerable<SOSSequence> SequencesSelected
        {
            get
            {
                return _sosSynopticRequeriments.Sequences;
            }

            set
            {
                _sosSynopticRequeriments.Sequences = value;
                CloseAnalysesSequences();
            }
        }

        private void CloseAnalysesSequences()
        {



            int secuenceint = 0;

            foreach (var analysis in _sosSynopticRequeriments.Analyses)
            {
                foreach (Section sect in analysis.SOSHub.Sections)
                {
                    if (!_combinedItems.Any(i => i.Identifier == $"Analysis_{analysis.SOSAnalysisId}" && i.section == sect))
                    {
                        _combinedItems.Add(
                            new DropItem
                            {
                                Name = sect.Step,
                                Type = $"SOSAnalysis",
                                Zone = $"Analysis_{analysis.SOSAnalysisId}",
                                Identifier = $"Analysis_{analysis.SOSAnalysisId}",
                                section = sect,
                                Sequence = secuenceint
                            }
                            );
                        secuenceint++;
                    }
                }
            }

            foreach (var sequence in _sosSynopticRequeriments.Sequences)
            {
                foreach (Section sect in sequence.SOSHub.Sections)
                {
                    if (!_combinedItems.Any(i => i.Identifier == $"Sequence_{sequence.SOSSequenceId}" && i.section == sect))
                    {
                        _combinedItems.Add(
                        new DropItem
                        {
                            Name = sect.Step,
                            Type = $"SOSSequenceId",
                            Zone = $"Sequence_{sequence.SOSSequenceId}",
                            Identifier = $"Sequence_{sequence.SOSSequenceId}",
                            section = sect,
                            Sequence = secuenceint
                        }
                        );
                        secuenceint++;

                    }
                }
            }




            List<DropItem> ForRemove = new List<DropItem>();

            foreach (DropItem item in _combinedItems)
            {
                Console.WriteLine(JsonSerializer.Serialize(item));

                if (item.Identifier.Split('_').First() == "Analysis_" && item.Zone == "CombinedZone")
                {
                    //Analysis_
                    int id_item = int.Parse(item.Identifier.Split('_').Last());

                    if (!_sosSynopticRequeriments.Analyses.Any(a => a.SOSAnalysisId == id_item))
                    {
                        ForRemove.Add(item);
                    }
                }
                else if (item.Zone == "CombinedZone")
                {
                    int id_item = int.Parse(item.Identifier.Split('_').Last());

                    if (!_sosSynopticRequeriments.Sequences.Any(a => a.SOSSequenceId == id_item))
                    {
                        ForRemove.Add(item);
                    }
                }
            }

            foreach (var toRemove in ForRemove)
            {
                _combinedItems.Remove(toRemove);

            }


            VerifyItemsSequence();

            StateHasChanged();

        }

        private void CloseStructure()
        {
            VerifyItemsSequence();
            StateHasChanged();
        }

        private void VerifyItemsSequence()
        {
            if (_sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence == null)
            {
                _sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence = new List<SOSSynopticRequirementsOperationSequence>();

                //Esto es del drag and drop, se quita porque no se usa por ahora
                //foreach (var item in _combinedItems.Where(i => i.Zone == "CombinedZone"))
                //{
                //    if (item.section != null)
                //    {
                //        var operationSequence = new SOSSynopticRequirementsOperationSequence
                //        {
                //            SectionId = item.section.SectionId,
                //            Section = item.section,
                //            Sequence = item.Sequence,

                //            IsActive = true
                //        };
                //        _sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence.Add(operationSequence);
                //    }
                //}
                int sequ = 0;
                foreach (var dtCollect in _sosSynopticRequeriments.SOSHubs)
                {
                    var operationSequence = new SOSSynopticRequirementsOperationSequence
                    {
                        SOSHubId = dtCollect.SOSHubId,
                        SOSHub = dtCollect,
                        Sequence = sequ,

                        IsActive = true
                    };
                    sequ++;
                    _sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence.Add(operationSequence);
                }
            }
            else
            {
                // A�adir los que faltan y actualizar secuencia
                //foreach (var item in _combinedItems.Where(i => i.Zone == "CombinedZone"))
                //{
                //    if (!_sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence.Any(t => t.SectionId == item.section.SectionId))
                //    {
                //        var operationSequence = new SOSSynopticRequirementsOperationSequence
                //        {
                //            SectionId = item.section.SectionId,
                //            Section = item.section,
                //            Sequence = item.Sequence,

                //            IsActive = true
                //        };

                //        _sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence.Add(operationSequence);
                //    }
                //    else
                //    {
                //        var existingOperation = _sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence.FirstOrDefault(t => t.SectionId == item.section.SectionId);
                //        if (existingOperation != null)
                //        {
                //            existingOperation.Sequence = item.Sequence;
                //        }
                //    }
                //}


                int sequ = _sosSynopticRequeriments.SOSHubs.Count() + 1;
                foreach (SOSHub dtCollect in _sosSynopticRequeriments.SOSHubs)
                {
                    if (!_sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence.Any(t => t.SOSHubId == dtCollect.SOSHubId))
                    {
                        var operationSequence = new SOSSynopticRequirementsOperationSequence
                        {
                            SOSHubId = dtCollect.SOSHubId,
                            SOSHub = dtCollect,
                            Sequence = sequ,

                            IsActive = true
                        };
                        sequ++;
                        _sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence.Add(operationSequence);
                    }
                }

                // Eliminar los que ya no est�n en _combinedItems
                //var validSectionIds = _combinedItems
                //    .Where(i => i.Zone == "CombinedZone" && i.section != null)
                //    .Select(i => i.section.SectionId)
                //    .ToHashSet();

                //_sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence =
                //    _sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence
                //        .Where(seq => validSectionIds.Contains(seq.SectionId ?? 0))
                //        .OrderBy(seq => seq.Sequence)
                //        .ToList();

                var validHubIds = _sosSynopticRequeriments.SOSHubs
                  .Select(i => i.SOSHubId)
                  .ToHashSet();

                _sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence =
                    _sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence
                        .Where(seq => validHubIds.Contains(seq.SOSHubId ?? 0))
                        .OrderBy(seq => seq.Sequence)
                        .ToList();
            }
        }

        private void ItemUpdated(MudItemDropInfo<DropItem> dropItem)
        {
            dropItem.Item.Zone = dropItem.DropzoneIdentifier;


            int newIndex = dropItem.IndexInZone;


            _combinedItems.UpdateOrder(dropItem, item => item.Sequence, newIndex);
            Console.WriteLine("Combined: " + JsonSerializer.Serialize(_combinedItems.Where(i => i.Zone.Contains("OP")).OrderBy(s => s.Sequence)));


            //if (_sosDistribution.SOSDistributionOperationSequence == null)
            //    _sosDistribution.SOSDistributionOperationSequence = new List<SOSDistributionOperationSequence>();

            //// Elimina los que ya no est�n en _combinedItems
            //var validSectionIds = _combinedItems
            //    .Where(i => i.Zone == "CombinedZone" && i.section != null)
            //    .Select(i => i.section.SectionId)
            //    .ToHashSet();

            //_sosDistribution.SOSDistributionOperationSequence =
            //    _sosDistribution.SOSDistributionOperationSequence
            //        .Where(seq => validSectionIds.Contains(seq.SectionId ?? 0))
            //        .ToList();

            //// A�ade los que faltan
            //foreach (var item in _combinedItems.Where(i => i.Zone == "CombinedZone" && i.section != null))
            //{
            //    if (!_sosDistribution.SOSDistributionOperationSequence.Any(seq => seq.SectionId == item.section.SectionId))
            //    {
            //        _sosDistribution.SOSDistributionOperationSequence.Add(new SOSDistributionOperationSequence
            //        {
            //            SectionId = item.section.SectionId,
            //            Section = item.section,
            //            SequenceId = item.Sequence,
            //            Times = CreateTimeString("0", 0),
            //            IsActive = true
            //        });
            //    }
            //}

        }

        private string GetFormatedAnalisisText(Section section, int analisisIndex)
        {
            string BaseText = Regex.Replace(section.Analyses[analisisIndex].Text, @"\*", "").ToString();

            return BaseText;
        }

        private MarkupString GenerateHighlightedText(string text, List<string> criticalPoints)
        {
            if (string.IsNullOrEmpty(text) || criticalPoints == null || criticalPoints.Count == 0)
            {
                return new MarkupString(text);
            }

            var normalizedText = Normalize(text);
            var builder = new StringBuilder();
            var currentIndex = 0;

            foreach (var criticalPoint in criticalPoints)
            {
                var normalizedCriticalPoint = Normalize(criticalPoint);
                var match = Regex.Match(normalizedText, Regex.Escape(normalizedCriticalPoint), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

                if (match.Success)
                {
                    var startIndex = match.Index;
                    var endIndex = startIndex + criticalPoint.Length;

                    // Agregar el texto normal antes del punto cr�tico
                    builder.Append(text.Substring(currentIndex, startIndex - currentIndex));

                    // Agregar el punto cr�tico resaltado
                    builder.Append($"<mark>{text.Substring(startIndex, endIndex - startIndex)}</mark>");

                    currentIndex = endIndex;
                }
            }

            // Agregar el texto normal despu�s del �ltimo punto cr�tico
            builder.Append(text.Substring(currentIndex));

            return new MarkupString(builder.ToString());
        }

        private static string Normalize(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            return input.Normalize(NormalizationForm.FormD).Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).Aggregate(new StringBuilder(), (sb, c) => sb.Append(c)).ToString().ToLowerInvariant();
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


        #endregion
        private void ChangeOperationType(int index, TypeOperacion newType)
        {
            var item = _sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence.ElementAt(index);
            item.Type = newType;

            //if (newType == TypeOperacion.None)
            //{
            //    item.TypeText = string.Empty;
            //}


        }

        #region generalfunctions
        //&===================== FUNCTIONS FOR GENERAL COMPONENT =====================&\\

        /// <summary>
        /// Formats a nullable <see cref="DateTime"/> as "MONTH YEAR" in uppercase.
        /// Returns empty string if null.
        /// </summary>
        /// <param name="date">The date to format.</param>
        /// <returns>Formatted month and year string, or empty if null.</returns>
        private string DateFormat(DateTime? date)
        {
            if (!date.HasValue) return "";

            string language = CultureInfo.CurrentCulture.Name ?? "es-MX";
            CultureInfo cultureInfo = new CultureInfo(language);

            return date.Value.ToString("dd/MM/yyyy hh:mm:ss tt", cultureInfo).ToUpper();
        }


        #endregion

        #region downloadFormat
        //&===================== FUNCTIONS FOR DOWNLOAD FORMAT =====================&\\
        private async void DownloadSTOR()
        {
            await SynopticRequirementsService.GenerateExcelSTOperatingRequirements((int)SynopticRequirementsId);
        }

        #endregion

        private async Task OpenDialogListSOSHub()
        {
            IEnumerable<SOSHubDtoList> simpleList = BuildSOSHubList();
            var parameters = new DialogParameters { { "listSOSHub", simpleList } };
            var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true };

            try
            {
                var dialog = await DialogService.ShowAsync<SelectSosHubCollectorModal_Modal>("", parameters, options);
                var result = await dialog.Result;

                if (!result.Cancelled && result.Data is IEnumerable<SOSHubDtoList> data)
                {
                    SOSHubList = data;
                    await FillDistributions(SOSHubList);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error opening dialog: {ex.Message}");
            }
        }

        private List<SOSHubDtoList> BuildSOSHubList()
        {
            return AvailableSoshubs.Select(item => new SOSHubDtoList
            {
                SOSHubId = item.SOSHubId,
                Folio = item.Folio,
                ProcessSheet = item.ProcessSheet,
                Selected = SOSHubList.Any(sos => sos.SOSHubId == item.SOSHubId)
            }).ToList();
        }

        private async Task FillDistributions(IEnumerable<SOSHubDtoList> ListSOSHub)
        {
            var DistributionsLocal = new List<SOSDistribution>();
            var listSosNotDistribution = new List<SOSHubDtoList>();
            LoadingDistributions = true;
            StateHasChanged();
            for (int i = 0; i < ListSOSHub.Count(); i++)
            {
                var SOSHub = ListSOSHub.ElementAt(i);
                SOSDistribution? distributionComplete = await SOSDistributionServices.GetSOSDistributionBySosHub(SOSHub.SOSHubId, includeCollections: true);
                if (distributionComplete == null || (distributionComplete.Analyses.LongCount() == 0 && distributionComplete.Sequences.LongCount() == 0))
                {
                    listSosNotDistribution.Add(SOSHub);
                }
                else
                {
                    DistributionsLocal.Add(distributionComplete);
                }
            }
            LoadingDistributions = false;
            if (listSosNotDistribution.Count > 0) ShowMessageNotDistribution(listSosNotDistribution);
            SetDistributionSTROTable(DistributionsLocal);

        }

        private void SetDistributionSTROTable(List<SOSDistribution> DistributionsComplete)
        {
            Distributions = new List<SOSDristributionSTROTable>(
                DistributionsComplete.Select(d => new SOSDristributionSTROTable
                {
                    SOSDistributionId = d.SOSDistributionId,
                    InternalControlNumber = d.InternalControlNumber,
                    OperationName = d.OperationName,
                    ProcessName = d.ProcessName,
                    ControlNumber = d.ControlNumber,
                    CreatedAt = d.CreatedAt,
                    IsActive = d.IsActive,
                    SOSHubId = d.SOSHubId,
                    SOSHubs = d.SOSHubs,
                    SOSDistributionOperationSequence = d.SOSDistributionOperationSequence,
                    Analyses = d.Analyses?.Select(a => new SOSAnalysis
                    {
                        SOSAnalysisId = a.SOSAnalysisId,
                        InternalControlNumber = a.InternalControlNumber,
                        OperationName = a.OperationName,
                        ProcessName = a.ProcessName,
                        SOSHubId = a.SOSHubId,
                        SOSHub = new SOSHub
                        {
                            Folio = a.SOSHub.Folio,
                            ProcessSheet = a.SOSHub.ProcessSheet,
                            SourcePlan = a.SOSHub.SourcePlan,
                            Sections = a.SOSHub.Sections.Select(s => new Section
                            {
                                IsActive = s.IsActive,
                                SectionId = s.SectionId,
                                Step = s.Step,
                                IsMachineOperation = s.IsMachineOperation,
                                Analyses = s.Analyses.Select(anl => new Analysis
                                {
                                    AnalysisId = anl.AnalysisId,
                                    Uid = anl.Uid,
                                    Text = anl.Text,
                                    CriticalPoints = anl.CriticalPoints,
                                    Reasons = anl.Reasons,
                                    IsActive = anl.IsActive
                                }).ToList()
                            }).ToList()
                        }
                    }).ToList(),
                    Sequences = d.Sequences.Select(a => new SOSSequence
                    {
                        SOSSequenceId = a.SOSSequenceId,
                        InternalControlNumber = a.InternalControlNumber,
                        OperationName = a.OperationName,
                        ProcessName = a.ProcessName,
                        SOSHubId = a.SOSHubId,
                        SOSHub = new SOSHub
                        {
                            Folio = a.SOSHub.Folio,
                            ProcessSheet = a.SOSHub.ProcessSheet,
                            SourcePlan = a.SOSHub.SourcePlan,
                            Sections = a.SOSHub.Sections.Select(s => new Section
                            {
                                IsActive = s.IsActive,
                                SectionId = s.SectionId,
                                Step = s.Step,
                                IsMachineOperation = s.IsMachineOperation,
                                Analyses = s.Analyses.Select(anl => new Analysis
                                {
                                    AnalysisId = anl.AnalysisId,
                                    Uid = anl.Uid,
                                    Text = anl.Text,
                                    CriticalPoints = anl.CriticalPoints,
                                    Reasons = anl.Reasons,
                                    IsActive = anl.IsActive
                                }).ToList()
                            }).ToList()
                        }
                    }).ToList(),
                })
            );


            StateHasChanged();
        }

        private void ShowMessageNotDistribution(List<SOSHubDtoList> listSosNotDistribution)
        {
            string listDitribution = listDitribution = string.Join(", ", listSosNotDistribution.Select(s => s.Folio));

            SOSHubList = SOSHubList.Where(s => !listSosNotDistribution.Any(n => n.SOSHubId == s.SOSHubId)).ToList();

            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add($"The following data collectors do not have an associated distribution and were not added: {listDitribution}", Severity.Warning);
        }

        private void RemoveDistribution(SOSDristributionSTROTable item)
        {
            Distributions = Distributions.Where(d => d.SOSDistributionId != item.SOSDistributionId).ToList();
            SOSHubList = SOSHubList.Where(s => s.SOSHubId != item.SOSHubId).ToList();
            StateHasChanged();
        }


        private List<SOSDistributionOperationSequence> BuildOperationSequences(SOSDristributionSTROTable distribution)
        {
            var sequences = (distribution.SOSDistributionOperationSequence ?? Enumerable.Empty<SOSDistributionOperationSequence>()).OrderBy(s => s.SequenceId).ToList();
            int expectedCount = (distribution.Analyses?.Count() ?? 0) + (distribution.Sequences?.Count() ?? 0);

            int missing = expectedCount - sequences.Count;
            if (missing > 0)
            {
                sequences.AddRange(Enumerable.Range(0, missing).Select(_ => new SOSDistributionOperationSequence()));
            }

            foreach (var s in sequences)
            {
                Console.WriteLine(JsonSerializer.Serialize(s));
            }

            return sequences;
        }

        public bool shouldRenderSequenceOrAnalyses(SOSDristributionSTROTable distribution, int indexRow)
        {
            List<int> listSA = GenerateArraySeqAndAnalyses(distribution);
            Console.WriteLine(string.Join(", ", listSA));
            int cumulative = 0;
            foreach (var rowSpan in listSA)
            {
                if (indexRow == cumulative)
                {
                    return true;
                }

                cumulative += rowSpan;
            }

            return false;
        }

        public int CalculateRowSpan(SOSDristributionSTROTable distribution, int indexRow)
        {
            List<int> listSA = GenerateArraySeqAndAnalyses(distribution);

            int cumulative = 0;
            foreach (var rowSpan in listSA)
            {
                if (indexRow == cumulative)
                {
                    return rowSpan;
                }

                cumulative += rowSpan;
            }

            return 0;
        }

        public string ShowSequenceOrAnalyses(SOSDristributionSTROTable distribution, int indexRow)
        {
            var listSA = GenerateArraySeqAndAnalyses(distribution);
            int findIndex = FindIndexArray(listSA, indexRow);

            if (findIndex == -1) return string.Empty;

            int totalAnalyses = distribution.Analyses?.Count() ?? 0;

            if (findIndex < totalAnalyses)
            {
                var analysis = distribution.Analyses.ElementAtOrDefault(findIndex);
                return analysis?.SOSHub?.Folio ?? string.Empty;
            }
            else
            {
                int indexSequence = findIndex - totalAnalyses;
                var sequence = distribution.Sequences?.ElementAtOrDefault(indexSequence);
                return sequence?.SOSHub?.Folio ?? string.Empty;
            }
        }

        public bool StepIsOperationMachine(SOSDristributionSTROTable distribution, SOSDistributionOperationSequence operationSequence)
        {
            List<Section> sections = distribution.Analyses.SelectMany(a => a.SOSHub?.Sections ?? Enumerable.Empty<Section>()).Concat(distribution.Sequences.SelectMany(s => s.SOSHub?.Sections ?? Enumerable.Empty<Section>())).ToList();

            Section? findStep = sections.FirstOrDefault(s => s.SectionId == operationSequence.SectionId);
            return findStep?.IsMachineOperation ?? false;

        }

        public Section GetStepSection(SOSDristributionSTROTable distribution, SOSDistributionOperationSequence operationSequence)
        {
            List<Section> sections = distribution.Analyses.SelectMany(a => a.SOSHub?.Sections ?? Enumerable.Empty<Section>()).Concat(distribution.Sequences.SelectMany(s => s.SOSHub?.Sections ?? Enumerable.Empty<Section>())).ToList();

            Section? findStep = sections.FirstOrDefault(s => s.SectionId == operationSequence.SectionId);
            return findStep ?? new Section { Step = "", IsMachineOperation = false };
        }

        public List<string> GetCriticalPoints(SOSDistributionOperationSequence operationSequence)
        {
            return operationSequence?.Section?.Analyses?.Where(a => a?.CriticalPoints != null).SelectMany(a => a.CriticalPoints).ToList() ?? new List<string>();
        }


        private int FindIndexArray(List<int> listSA, int indexRow)
        {
            int cumulative = 0;
            for (int i = 0; i < listSA.Count; i++)
            {
                if (indexRow == cumulative)
                {
                    return i; // devuelve el índice del bloque
                }

                cumulative += listSA[i];
            }

            return -1;
        }


        private List<int> GenerateArraySeqAndAnalyses(SOSDristributionSTROTable distribution)
        {
            int totalSeqAndAna = (distribution.Analyses?.Count() ?? 0) + (distribution.Sequences?.Count() ?? 0);

            var sequences = distribution.SOSDistributionOperationSequence?.ToList() ?? new List<SOSDistributionOperationSequence>();

            if (totalSeqAndAna == 0) return new List<int>();

            var rowSpans = new List<int>(new int[totalSeqAndAna]);

            if (sequences.Count >= totalSeqAndAna)
            {
                int baseValue = (int)Math.Round((double)sequences.Count / totalSeqAndAna);
                int acumulated = 0;

                for (int i = 0; i < rowSpans.Count; i++)
                {
                    int remaining = sequences.Count - acumulated;

                    // Si es la última posición, asigna todo lo que quede.
                    int value = (i == rowSpans.Count - 1) ? remaining : Math.Min(baseValue, remaining);

                    rowSpans[i] = value;
                    acumulated += value;

                    if (remaining <= 0) break;
                }
            }
            else
            {
                rowSpans = rowSpans.Select(x => 1).ToList();
            }

            return rowSpans;
        }



        public string GetStepClass(List<SOSDistributionOperationSequence> sections, int i)
        {
            return (i == sections.Count - 1) ? " syntable__c--nt" : " syntable__c--nbt";
        }

        public int GetTrainingTime(SOSDristributionSTROTable distribution)
        {
            return distribution.SOSHubs.FirstOrDefault(s => s.SOSHubId == distribution.SOSHubId)?.TrainingTime ?? 0;
        }

        private string _selectedState;

        private readonly string[] _states = { "A", "B", "C", "D" };

    }
}