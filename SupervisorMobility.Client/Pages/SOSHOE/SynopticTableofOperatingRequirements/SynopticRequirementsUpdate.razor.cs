using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities.SOS_Process;

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

        //UserLogin
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        //SynopticRequirements
        SOSSynopticTableofOperatingRequirements _sosSynopticRequeriments { get; set; } = new();
        SOSHub _soshub { get; set; } = new();
        Distribution _distribution { get; set; } = new();
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
                _soshub = await sosHubService.GetSOSHub( (int)_sosSynopticRequeriments.SOSHubId, true, true, includePeople: true, includeInformation: true, includeModel: true);
                _distribution = await DistributionService.GetDistributionWithCollections((int)_soshub.PlantId, (int)_soshub.AreaId, (int)_soshub.DistributionId);

                AvailableAnalyses = await SOSAnalysisServices.GetAllSOSAnalysisByDistribution((int)_soshub?.DistributionId, includeSOS: true);
                AvailableSequences = await SOSSequenceServices.GetAllSOSSequenceByDistribution((int)_soshub?.DistributionId, includeSOS: true);

                int secuenceInt = 0;

                foreach (var analysis in _sosSynopticRequeriments.Analyses)
                {
                    Console.WriteLine($"Analysis: {analysis.SOSAnalysisId}");
                    foreach (Section sect in analysis.SOSHub.Sections)
                    {
                        Console.WriteLine($"{analysis.SOSAnalysisId} Sec: {sect.Step}");

                        if (_sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence != null &&
                            _sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence.Any(seq => seq.SectionId == sect.SectionId))
                        {
                            _combinedItems.Add(
                                new DropItem
                                {
                                    Name = sect.Step,
                                    Type = $"SOSAnalysis",
                                    Zone = $"CombinedZone",
                                    //Identifier = $"CombinedZone",
                                    Identifier = $"Analysis_{analysis.SOSAnalysisId}",
                                    section = sect,
                                    Sequence = secuenceInt
                                }
                            );
                        }
                        else
                        {
                            _combinedItems.Add(
                                new DropItem
                                {
                                    Name = sect.Step,
                                    Type = $"SOSAnalysis",
                                    Zone = $"Analysis_{analysis.SOSAnalysisId}",
                                    Identifier = $"Analysis_{analysis.SOSAnalysisId}",
                                    section = sect,
                                    Sequence = secuenceInt
                                }
                                );

                        }
                        secuenceInt++;
                    }
                }

                foreach (var sequence in _sosSynopticRequeriments.Sequences)
                {
                    //Console.WriteLine(JsonSerializer.Serialize(sequence));

                    //int temp = AvailableSequences.FindIndex(a => a.SOSSequenceId == sequence.SOSSequenceId);
                    //AvailableSequences[temp].SOSHub = sequence.SOSHub;

                    foreach (Section sect in sequence.SOSHub.Sections)
                    {
                        if (_sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence != null &&
                            _sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence.Any(seq => seq.SectionId == sect.SectionId))
                        {
                            _combinedItems.Add(
                                new DropItem
                                {
                                    Name = sect.Step,
                                    Type = $"SOSSequence",
                                    Zone = $"CombinedZone",
                                    //Identifier = $"CombinedZone",
                                    Identifier = $"Sequence_{sequence.SOSSequenceId}",
                                    section = sect,
                                    Sequence = secuenceInt
                                }
                            );
                        }
                        else
                        {
                            _combinedItems.Add(
                                new DropItem
                                {
                                    Name = sect.Step,
                                    Type = $"SOSSequence",
                                    Zone = $"Sequence_{sequence.SOSSequenceId}",
                                    Identifier = $"Sequence_{sequence.SOSSequenceId}",
                                    section = sect,
                                    Sequence = secuenceInt
                                }
                            );
                        }
                        secuenceInt++;
                    }

                }

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
        List<SOSAnalysis> AvailableAnalyses = new();
        List<SOSSequence> AvailableSequences = new();

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

        private void VerifyItemsSequence()
        {
            if (_sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence == null)
            {
                _sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence = new List<SOSSynopticRequirementsOperationSequence>();

                foreach (var item in _combinedItems.Where(i => i.Zone == "CombinedZone"))
                {
                    if (item.section != null)
                    {
                        var operationSequence = new SOSSynopticRequirementsOperationSequence
                        {
                            SectionId = item.section.SectionId,
                            Section = item.section,
                            Sequence = item.Sequence,
                           
                            IsActive = true
                        };
                        _sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence.Add(operationSequence);
                    }
                }
            }
            else
            {
                // Añadir los que faltan y actualizar secuencia
                foreach (var item in _combinedItems.Where(i => i.Zone == "CombinedZone"))
                {
                    if (!_sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence.Any(t => t.SectionId == item.section.SectionId))
                    {
                        var operationSequence = new SOSSynopticRequirementsOperationSequence
                        {
                            SectionId = item.section.SectionId,
                            Section = item.section,
                            Sequence = item.Sequence,
                           
                            IsActive = true
                        };

                        _sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence.Add(operationSequence);
                    }
                    else
                    {
                        var existingOperation = _sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence.FirstOrDefault(t => t.SectionId == item.section.SectionId);
                        if (existingOperation != null)
                        {
                            existingOperation.Sequence = item.Sequence;
                        }
                    }
                }

                // Eliminar los que ya no están en _combinedItems
                var validSectionIds = _combinedItems
                    .Where(i => i.Zone == "CombinedZone" && i.section != null)
                    .Select(i => i.section.SectionId)
                    .ToHashSet();

                _sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence =
                    _sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence
                        .Where(seq => validSectionIds.Contains(seq.SectionId ?? 0))
                        .OrderBy(seq => seq.Sequence)
                        .ToList();
            }
        }

        #endregion


    }
}