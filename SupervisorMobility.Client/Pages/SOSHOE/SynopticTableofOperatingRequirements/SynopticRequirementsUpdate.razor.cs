// - Core .NET imports
using System.Globalization;

// - Third-party imports
using MudBlazor;
using Microsoft.JSInterop;

// - Custom project imports
using SupervisorMobility.Client.Pages.SOSHOE.SynopticTableofOperatingRequirements.Modals;
using SupervisorMobility.Client.Pages.SOSHOE.SynopticTableofOperatingRequirements.Collections.Skill;
using SupervisorMobility.Client.Pages.SOSHOE.SynopticTableofOperatingRequirements.Collections.Knowledge;

/// <summary>
/// Component for updating a Synoptic Table of Operating Requirements (STRO) within the SOSHOE module.
/// Handles loading of existing STRO data, user authentication, hub and distribution selection,
/// updating difficulty levels, and saving changes back to the backend service.
/// Also manages UI interactions such as breadcrumbs, loading indicators, and dialogs for selecting SOS hubs.
/// </summary>
namespace SupervisorMobility.Client.Pages.SOSHOE.SynopticTableofOperatingRequirements
{
    public partial class SynopticRequirementsUpdate
    {
        //+====================== INPUT ======================+\\
        [Parameter]
        public int? SynopticRequirementsId { get; set; }

        //+================ BREADCRUMB LINKS =================+\\
        private List<BreadcrumbItem> _links;

        //+=============== LOADING INDICATORS ================+\\
        public bool ShowLoading = true;
        public bool LoadingDistributions { get; set; } = false;
        private IList<string> _sourceMsgLoading = new List<string>();

        //+==================== USER LOGIN ===================+\\
        public User user = new();
        public bool IsLoggedIn = false;

        //+============== SYNOPTIC REQUIREMENTS ===============+\\
        SOSSynopticTableofOperatingRequirements _sosSynopticRequeriments { get; set; } = new();

        //+================== HUB ANS LISTs ===================+\\
        List<SOSHub> AvailableSosHubs = new();
        SOSHub _soshub { get; set; } = new();
        IEnumerable<SOSHubDtoList> SOSHubList = new List<SOSHubDtoList>();


        //+==================== KNOWLEDGE =====================+\\
        private List<Knowledge> _KnowledgeGeneral = new();
        public List<KnowledgeDynamicDTO> KnowledgeDynamicDTO = new List<KnowledgeDynamicDTO>();

        //+====================== SKILL =======================+\\
        private List<Skill> _SkillGeneral = new();
        public List<SkillDynamicDTO> SkillDynamicDTO = new List<SkillDynamicDTO>();

        //+============= ESTEBLISHED CONDITIONS ===============+\\
        public List<EstablishedConditionDynamicDTO> EstablishedConditionDynamicDTO = new List<EstablishedConditionDynamicDTO>();

        //+=============== INSURANCE FEATURES =================+\\
        public List<InsuranceFeaturesDynamicDTO> InsuranceFeaturesDynamicDTO = new List<InsuranceFeaturesDynamicDTO>();

        // =================================================== \\
        //&============ COMPONENT INITIALIZATION =============&\\
        // =================================================== \\

        /// <summary>
        /// Performs component initialization, including loading messages, breadcrumbs,
        /// user verification, and fetching synoptic requirements and hubs.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected async override Task OnInitializedAsync()
        {
            PopulateLoadingMessages();
            InitializeBreadcrumbs();

            if (!await CheckUserLoginAsync())
                return;

            await LoadKnowlege();
            await LoadSkills();
            await LoadSynopticRequirementsAndHubsAsync();

            ShowLoading = false;
            StateHasChanged();
        }

        #region Component Initialization Helpers

        /// <summary>
        /// Populates the loading messages for the UI.
        /// </summary>
        private void PopulateLoadingMessages()
        {
            _sourceMsgLoading = Enumerable.Range(1, 11).Select(i => Localizer1[$"Loading{i}"].Value).ToList();
        }

        /// <summary>
        /// Initializes the breadcrumb navigation links.
        /// </summary>
        private void InitializeBreadcrumbs()
        {
            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(Localizer["homeSOSHOE"], href: "/soshoe"),
                new BreadcrumbItem(Localizer["SynopticRequirements"], href: "/soshoe/SynopticRequirements"),
                new BreadcrumbItem(Localizer["SynopticRequirementsDetails"], href: "/soshoe/SynopticRequirements", disabled: true)
            };

            BreadcrumbService.UpdateBreadcrumbs(_links);
        }

        /// <summary>
        /// Checks if the user is logged in and redirects if not.
        /// </summary>
        /// <returns>True if the user is logged in; otherwise, false.</returns>
        private async Task<bool> CheckUserLoginAsync()
        {
            IsLoggedIn = await JSRuntime.InvokeAsync<bool>("localStorage.hasOwnProperty", "user");
            if (!IsLoggedIn)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add("Error: You have to log in", Severity.Error);
                NavigationManager.NavigateTo("/");
            }

            return IsLoggedIn;
        }

        /// <summary>
        /// Loads the synoptic requirements and associated hubs from the backend.
        /// </summary>
        private async Task LoadSynopticRequirementsAndHubsAsync()
        {
            _sosSynopticRequeriments = await SynopticRequirementsService.GetSOSSynopticTableofOperatingRequirements((int)SynopticRequirementsId!, true, true, true);

            _soshub = await SOSHubServices.GetSOSHub((int)_sosSynopticRequeriments.SOSHubId!, true, true, includePeople: true, includeInformation: true, includeModel: true);

            var FilterSOSHubs = (await SOSHubServices.GetAllSOSHub(includeSOSDistribution: true)).Where(s => s.DistributionId == _soshub.DistributionId && s.SOSHubId != _soshub.SOSHubId).ToList();
            AvailableSosHubs = CleanSOSHubs(FilterSOSHubs);

            await VerifySOSHubsSR(_sosSynopticRequeriments);
        }

        /// <summary>
        /// Cleans the SOSHubs by keeping only the matching SOSDistribution per hub.
        /// </summary>
        /// <param name="allFilterSosHubs">List of SOSHubs to clean.</param>
        private List<SOSHub> CleanSOSHubs(List<SOSHub> allFilterSosHubs)
        {
            foreach (var hub in allFilterSosHubs)
            {
                var matchedDistribution = hub.SOSDistribution?.FirstOrDefault(s => s.SOSHubId == hub.SOSHubId);
                hub.SOSDistribution = matchedDistribution != null ? new List<SOSDistribution> { matchedDistribution } : new List<SOSDistribution>();
            }

            return allFilterSosHubs.Where(h => h.SOSDistribution?.Any() == true).ToList();
        }


        /// <summary>
        /// Loads knowledge data asynchronously from the service and assigns it to the general knowledge field.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task LoadKnowlege()
        {
            // Fetch knowledge data from the service
            _KnowledgeGeneral = await KnowledgeServices.GetKnowledges();
        }


        /// <summary>
        /// Loads skills data asynchronously from the service and assigns it to the general skills field.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task LoadSkills()
        {
            // Fetch skills data from the service
            _SkillGeneral = await SkillServices.GetSkills();
        }


        /// <summary>
        /// Verifies the SOS hubs associated with the synoptic requirements and fills distributions if any.
        /// </summary>
        /// <param name="STRO">The synoptic table of operating requirements to verify.</param>
        private async Task VerifySOSHubsSR(SOSSynopticTableofOperatingRequirements STRO)
        {
            if (STRO.SOSHubs?.Count() > 0)
            {
                SOSHubList = STRO.SOSHubs.Select(a => new SOSHubDtoList { Folio = a.Folio, ProcessSheet = a.ProcessSheet, Selected = true, SOSHubId = a.SOSHubId }).ToList();
                await FillDistributions(SOSHubList);
            }
        }

        #endregion


        // =================================================== \\
        //&================ GENERAL COMPONENT ================&\\
        // =================================================== \\

        /// <summary>
        /// Attempts to retrieve a logbook entry at a specific index from the synoptic requirements logbooks.
        /// The index is counted from the end of the list (inverted order).
        /// </summary>
        /// <param name="index">The zero-based index of the logbook entry to retrieve.</param>
        /// <param name="item">Outputs the logbook entry if found; otherwise, null.</param>
        /// <returns>True if the entry exists at the specified index; otherwise, false.</returns>
        public bool TryGetSynopticRequirementsLogbooksElementAtIndex(int index, out SOSSynopticRequirementsLogbook? item)
        {
            item = null;

            var logbooks = _sosSynopticRequeriments?.SynopticRequirementsLogbooks;
            if (logbooks == null || !logbooks.Any()) return false;

            // NOTE: Index is counted from the end of the list
            int invertedIndex = logbooks.Count - 1 - index;
            if (invertedIndex >= 0 && invertedIndex < logbooks.Count)
            {
                item = logbooks?.ElementAt(invertedIndex);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Navigates to the details page of a specific hub (Hoe) by its ID.
        /// </summary>
        /// <param name="HoeId">The ID of the hub to view.</param>
        private void HoeDetails(int HoeId)
        {
            NavigationManager.NavigateTo($"/soshoe/Hub/Details/{HoeId}");
        }

        /// <summary>
        /// Formats a nullable <see cref="DateTime"/> as "dd/MM/yyyy hh:mm:ss tt" in uppercase.
        /// Returns an empty string if the date is null.
        /// </summary>
        /// <param name="date">The date to format.</param>
        /// <returns>Formatted date string or empty if null.</returns>
        private static string DateFormat(DateTime? date)
        {
            if (!date.HasValue) return "";

            // NOTE: Use the current culture, fallback to "es-MX" if unavailable
            string language = CultureInfo.CurrentCulture.Name ?? "es-MX";
            CultureInfo cultureInfo = new CultureInfo(language);

            return date.Value.ToString("dd/MM/yyyy hh:mm:ss tt", cultureInfo).ToUpper();
        }


        // =================================================== \\
        //&===== DIALOG SELECT DISTRIBUTION BY SOSHUB ========&\\
        // =================================================== \\

        /// <summary>
        /// Opens a dialog to select SOS hubs and updates the selected list.
        /// </summary>
        private async Task OpenDialogListSOSHub()
        {
            var simpleList = BuildSOSHubList();
            var parameters = new DialogParameters { { "listSOSHub", simpleList } };
            var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true };

            try
            {
                var dialog = await DialogService.ShowAsync<SelectSosHubCollectorModal_Modal>("", parameters, options);
                var result = await dialog.Result;

                // NOTE: Update SOSHubList and distributions only if dialog was not canceled
                if (!result.Canceled && result.Data is IEnumerable<SOSHubDtoList> selectedHubs)
                {
                    SOSHubList = selectedHubs;
                    await FillDistributions(SOSHubList);
                }
            }
            catch (Exception ex)
            {
                // NOTE: Log error if dialog fails to open
                Console.Error.WriteLine($"Error opening dialog: {ex.Message}");
            }
        }


        // =================================================== \\
        //&======== FUNCTIONS TO FILL DATA OF SCRO ===========&\\
        // =================================================== \\

        /// <summary>
        /// Builds a list of SOSHub DTOs, marking each as selected if it exists in the current SOSHubList.
        /// </summary>
        /// <returns>List of <see cref="SOSHubDtoList"/> SOSHubs.</returns>
        private List<SOSHubDtoList> BuildSOSHubList()
        {
            return AvailableSosHubs.Select(item => new SOSHubDtoList
            {
                SOSHubId = item.SOSHubId,
                Folio = item.Folio,
                ProcessSheet = item.ProcessSheet,
                OperationNameDistribution = item.SOSDistribution?.FirstOrDefault()?.OperationName,
                Selected = SOSHubList.Any(sos => sos.SOSHubId == item.SOSHubId)
            }).ToList();
        }

        /// <summary>
        /// Fills distributions for the given SOS hubs, separating those with and without valid distributions.
        /// Updates the component state accordingly.
        /// </summary>
        /// <param name="sosHubList">The list of SOS hubs to process.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task FillDistributions(IEnumerable<SOSHubDtoList> sosHubList)
        {
            LoadingDistributions = true;

            var distributions = new List<SOSDistribution>();
            var hubsWithoutDistribution = new List<SOSHubDtoList>();
            var hubsWithDistribution = new List<SOSHubDtoList>();

            StateHasChanged();

            foreach (var hub in sosHubList)
            {
                var distribution = await SOSDistributionServices.GetSOSDistributionBySosHub(hub.SOSHubId, includeSOS: true, includeCollections: true);

                // NOTE: Separate hubs that have valid distributions from those that don't
                if (distribution == null || !(distribution.Analyses?.Any() == true || distribution.Sequences?.Any() == true))
                {
                    hubsWithoutDistribution.Add(hub);
                }
                else
                {
                    hubsWithDistribution.Add(hub);
                    distributions.Add(distribution);
                }
            }

            LoadingDistributions = false;

            // NOTE: Notify user about hubs without distributions
            if (hubsWithoutDistribution.Count > 0) ShowMessageNotDistribution(hubsWithoutDistribution);

            // Update component state with distributions and hubs that have valid data
            SetDistributionSTRO(distributions, hubsWithDistribution);

            // NOTE: Add dynamic knowledge filters for each distribution
            AddFiltersDynamicKnowledge(hubsWithDistribution, _KnowledgeGeneral);

            // NOTE: Add dynamic skill filters for each distribution
            AddFiltersDynamicSkill(hubsWithDistribution, _SkillGeneral);

            // NOTE: Add dynamic input for each section in each distribution
            SetEstablishedConditionsDynamic();

            // NOTE: Add dynamic input for each section in each distribution
            SetInsuranceFeaturesDynamic();
        }



        /// <summary>
        /// Updates the synoptic requirements with selected SOS hubs and their corresponding distributions 
        /// and difficulty levels.
        /// </summary>
        /// <param name="distributionsComplete">List of complete distributions for SOS hubs.</param>
        /// <param name="selectedSosHubsDto">List of selected SOS hubs provided as DTOs.</param>
        private void SetDistributionSTRO(List<SOSDistribution> distributionsComplete, List<SOSHubDtoList> selectedSosHubsDto)
        {
            var selectedSosHubs = new List<SOSHub>();
            var requirementDifficulties = new List<SOSSynopticTableRequirementOperationDifficulty>();

            foreach (var hubDto in selectedSosHubsDto)
            {

                var sosHub = AvailableSosHubs.FirstOrDefault(s => s.SOSHubId == hubDto.SOSHubId);
                if (sosHub == null) continue;

                var distribution = distributionsComplete.FirstOrDefault(d => d.SOSHubId == sosHub.SOSHubId);
                if (distribution == null) continue;

                // Attach the found distribution to the hub
                sosHub.SOSDistribution = new List<SOSDistribution> { distribution };
                selectedSosHubs.Add(sosHub);

                // NOTE: If no difficulty exists for this hub, defaults are applied
                var difficulty = _sosSynopticRequeriments.RequirementDifficulties?.FirstOrDefault(r => r.SOSHubId == sosHub.SOSHubId);

                requirementDifficulties.Add(new SOSSynopticTableRequirementOperationDifficulty
                {
                    Id = difficulty?.Id ?? 0,
                    DifficultyLevel = difficulty?.DifficultyLevel ?? DifficultyLevel.C,
                    SOSHubId = sosHub.SOSHubId,
                    SOSSynopticTableofOperatingRequirementsId = difficulty?.SOSSynopticTableofOperatingRequirementsId ?? _sosSynopticRequeriments.SOSSynopticTableofOperatingRequirementsId
                });
            }

            // Update the main synoptic requirements with hubs and difficulties
            _sosSynopticRequeriments.SOSHubs = selectedSosHubs;
            _sosSynopticRequeriments.RequirementDifficulties = requirementDifficulties;

            StateHasChanged();
        }

        /// <summary>
        /// Builds dynamic knowledge filters for the selected SOS hubs based on their existing knowledge requirements.
        /// Updates the <c>KnowledgeDynamicDTO</c> collection accordingly.
        /// </summary>
        /// <param name="selectedSosHubsDto">The list of selected SOS hubs.</param>
        /// <param name="Knowledges">The complete list of available knowledge items.</param>
        private void AddFiltersDynamicKnowledge(List<SOSHubDtoList> selectedSosHubsDto, List<Knowledge> Knowledges)
        {
            var copyKnowledges = _sosSynopticRequeriments.SOSSTROKnowledge;

            KnowledgeDynamicDTO = selectedSosHubsDto.Select(a =>
            {
                // Extract knowledge IDs already assigned to this hub
                var knowledgeIds = copyKnowledges?.Where(k => k.SOSHubId == a.SOSHubId).Select(ka => ka.KnowledgeId).ToList() ?? new List<int>();

                // Build the dynamic DTO with unassigned knowledge as filter options
                return new KnowledgeDynamicDTO
                {
                    SOSHubId = a.SOSHubId,
                    SelectedKnowledgeName = string.Empty,
                    KnowledgeIds = knowledgeIds,
                    FilteredKnowledge = Knowledges.Where(k => !knowledgeIds.Contains(k.Id)).Select(k => new Knowledge { Id = k.Id, Name = k.Name }).ToList()
                };
            }).ToList();
        }


        /// <summary>
        /// Builds dynamic skill filters for the selected SOS hubs based on their existing skill requirements.
        /// Updates the <c>SkillDynamicDTO</c> collection accordingly.
        /// </summary>
        /// <param name="selectedSosHubsDto">The list of selected SOS hubs.</param>
        /// <param name="Skills">The complete list of available skill items.</param>
        private void AddFiltersDynamicSkill(List<SOSHubDtoList> selectedSosHubsDto, List<Skill> Skills)
        {
            var copySkills = _sosSynopticRequeriments.SOSSTROSkill;

            SkillDynamicDTO = selectedSosHubsDto.Select(a =>
            {
                // Extract skill IDs already assigned to this hub
                var SkillIds = copySkills?.Where(k => k.SOSHubId == a.SOSHubId).Select(ka => ka.SkillId).ToList() ?? new List<int>();

                // Build the dynamic DTO with unassigned skills as filter options
                return new SkillDynamicDTO
                {
                    SOSHubId = a.SOSHubId,
                    SelectedSkillName = string.Empty,
                    SkillIds = SkillIds,
                    FilteredSkill = Skills.Where(k => !SkillIds.Contains(k.Id)).Select(k => new Skill { Id = k.Id, Name = k.Name }).ToList()
                };
            }).ToList();
        }


        /// <summary>
        /// Displays a warning message for SOS hubs without associated distributions
        /// and removes them from the current hub list.
        /// </summary>
        /// <param name="hubsWithoutDistribution">List of SOS hubs missing distributions.</param>
        private void ShowMessageNotDistribution(List<SOSHubDtoList> hubsWithoutDistribution)
        {
            var distributionList = string.Join(", ", hubsWithoutDistribution.Select(s => s.Folio));

            // Remove hubs without distribution from the current list
            SOSHubList = SOSHubList.Where(s => !hubsWithoutDistribution.Any(n => n.SOSHubId == s.SOSHubId)).ToList();

            // NOTE: Warning is displayed at the top center of the screen for better visibility
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add($"The following data collectors do not have an associated distribution and were not added: {distributionList}", Severity.Warning);
        }


        // =================================================== \\
        //&======== FUNCTIONS TO GENERAL SCRO TABLE ==========&\\
        // =================================================== \\

        /// <summary>
        /// Retrieves all distributions associated with the current synoptic requirements.
        /// </summary>
        /// <returns>A list of <see cref="SOSDistribution"/> objects, or null if not available.</returns>
        public List<SOSDistribution> GetDistributions()
        {
            // NOTE: Uses SelectMany to flatten distributions from multiple hubs
            return _sosSynopticRequeriments?.SOSHubs?.SelectMany(s => s.SOSDistribution).ToList();
        }

        /// <summary>
        /// Builds and returns the ordered list of operation sequences for a given distribution.
        /// </summary>
        /// <param name="distribution">The distribution containing sequences and analyses.</param>
        /// <returns>A list of <see cref="SOSDistributionOperationSequence"/> objects.</returns>
        private List<SOSDistributionOperationSequence> BuildOperationSequences(SOSDistribution distribution)
        {
            // Order existing sequences by SequenceId, or use empty if none exist
            var sequences = (distribution.SOSDistributionOperationSequence ?? Enumerable.Empty<SOSDistributionOperationSequence>()).OrderBy(s => s.SequenceId).ToList();
            int expectedCount = (distribution.Analyses?.Count() ?? 0) + (distribution.Sequences?.Count() ?? 0);

            // NOTE: Fill missing sequences with placeholders to match expected count
            int missing = expectedCount - sequences.Count;
            if (missing > 0) sequences.AddRange(Enumerable.Range(0, missing).Select(_ => new SOSDistributionOperationSequence()));

            return sequences;
        }

        /// <summary>
        /// Removes a distribution and its related data from the synoptic requirements.
        /// Updates SOS hubs, requirement difficulties, and dynamic filters accordingly.
        /// </summary>
        /// <param name="item">The distribution to remove.</param>
        private void RemoveDistribution(SOSDistribution item)
        {
            // Remove the hub reference from synoptic requirements
            _sosSynopticRequeriments.SOSHubs = _sosSynopticRequeriments?.SOSHubs?.Where(s => s.SOSHubId != item.SOSHubId);

            // Remove requirement difficulties associated with the hub
            _sosSynopticRequeriments!.RequirementDifficulties = _sosSynopticRequeriments.RequirementDifficulties?.Where(r => r.SOSHubId != item.SOSHubId);

            // Update hub list in UI to exclude the removed hub
            SOSHubList = SOSHubList.Where(s => s.SOSHubId != item.SOSHubId).ToList();

            // Update dynamic filters for knowledge and skills if needed
            KnowledgeDynamicDTO = KnowledgeDynamicDTO.Where(s => s.SOSHubId != item.SOSHubId).ToList();
            SkillDynamicDTO = SkillDynamicDTO.Where(s => s.SOSHubId != item.SOSHubId).ToList();

            // update dynamic inputs for established conditions
            SetEstablishedConditionsDynamic();

            // update dynamic inputs for insurance features
            SetInsuranceFeaturesDynamic();

            // Refresh the UI state
            StateHasChanged();
        }


        /// <summary>
        /// Determines whether a sequence or analysis should be rendered at the given row index.
        /// </summary>
        /// <param name="distribution">The distribution containing sequences and analyses.</param>
        /// <param name="indexRow">The row index to check.</param>
        /// <returns>True if rendering should occur at the given row; otherwise, false.</returns>
        public bool shouldRenderSequenceOrAnalyses(SOSDistribution distribution, int indexRow)
        {
            List<int> listSA = GenerateArraySeqAndAnalyses(distribution);

            int cumulative = 0;
            foreach (var rowSpan in listSA)
            {
                // NOTE: Rendering is triggered only at the first row of each span
                if (indexRow == cumulative)
                {
                    return true;
                }

                cumulative += rowSpan;
            }

            return false;
        }

        /// <summary>
        /// Calculates the row span for a given row index in a distribution.
        /// </summary>
        /// <param name="distribution">The distribution containing sequences and analyses.</param>
        /// <param name="indexRow">The row index to calculate span for.</param>
        /// <returns>The row span at the specified index, or 0 if the index is not valid.</returns>
        public int CalculateRowSpan(SOSDistribution distribution, int indexRow)
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

        /// <summary>
        /// Returns the Folio of the analysis or sequence corresponding to a given row index.
        /// </summary>
        /// <param name="distribution">The distribution containing sequences and analyses.</param>
        /// <param name="indexRow">The row index to check.</param>
        /// <returns>
        /// The Folio string of the analysis or sequence at the given index,
        /// or an empty string if not found.
        /// </returns>
        public string ShowSequenceOrAnalyses(SOSDistribution distribution, int indexRow)
        {
            var listSA = GenerateArraySeqAndAnalyses(distribution);
            int findIndex = FindIndexArray(listSA, indexRow);

            if (findIndex == -1) return string.Empty;

            int totalAnalyses = distribution.Analyses?.Count() ?? 0;

            if (findIndex < totalAnalyses)
            {
                // NOTE: Return Folio from analyses if the index is within analysis range
                var analysis = distribution.Analyses?.ElementAtOrDefault(findIndex);
                return analysis?.SOSHub?.Folio ?? string.Empty;
            }
            else
            {
                // NOTE: Return Folio from sequences if the index is beyond analyses
                int indexSequence = findIndex - totalAnalyses;
                var sequence = distribution.Sequences?.ElementAtOrDefault(indexSequence);
                return sequence?.SOSHub?.Folio ?? string.Empty;
            }
        }

        /// <summary>
        /// Generates a list of row spans for analyses and sequences in a distribution.
        /// This is used to determine how many rows each analysis or sequence occupies in a table.
        /// </summary>
        /// <param name="distribution">The distribution containing analyses, sequences, and operation sequences.</param>
        /// <returns>A list of integers representing the row spans for each analysis or sequence.</returns>
        private List<int> GenerateArraySeqAndAnalyses(SOSDistribution distribution)
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

                    // NOTE: Assign remaining count to last element, otherwise use baseValue
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

        /// <summary>
        /// Finds the index in a list of row spans that corresponds to a given row.
        /// </summary>
        /// <param name="listSA">The list of row spans.</param>
        /// <param name="indexRow">The row index to locate.</param>
        /// <returns>
        /// The index in <paramref name="listSA"/> where the row is located,
        /// or -1 if the row is outside the range.
        /// </returns>
        private int FindIndexArray(List<int> listSA, int indexRow)
        {
            int cumulative = 0;
            for (int i = 0; i < listSA.Count; i++)
            {
                if (indexRow == cumulative)
                {
                    return i;
                }

                cumulative += listSA[i];
            }

            return -1;
        }

        /// <summary>
        /// Returns the CSS class for a step based on its position in the list.
        /// </summary>
        /// <param name="sections">The list of operation sequences.</param>
        /// <param name="i">The index of the current step.</param>
        /// <returns>
        /// A string representing the CSS class for the step.
        /// "syntable__c--nt" if it is the last step, otherwise "syntable__c--nbt".
        /// </returns>
        public string GetStepClass(List<SOSDistributionOperationSequence> sections, int i)
        {
            // NOTE: Determine class based on whether this is the last element
            return (i == sections.Count - 1) ? " syntable__c--nt" : " syntable__c--nbt";
        }


        /// <summary>
        /// Retrieves the section corresponding to the given operation sequence within a distribution.
        /// </summary>
        /// <param name="distribution">The distribution containing analyses and sequences.</param>
        /// <param name="operationSequence">The operation sequence whose section is being retrieved.</param>
        /// <returns>
        /// The <see cref="Section"/> matching the operation sequence's SectionId.
        /// Returns a default empty Section if not found.
        /// </returns>
        public Section GetStepSection(SOSDistribution distribution, SOSDistributionOperationSequence operationSequence)
        {
            // Combine sections from analyses and sequences
            List<Section> sections = distribution.Analyses!.SelectMany(a => a.SOSHub?.Sections ?? Enumerable.Empty<Section>()).Concat(distribution.Sequences!.SelectMany(s => s.SOSHub?.Sections ?? Enumerable.Empty<Section>())).ToList();

            // NOTE: Find the section matching the operation sequence
            Section? findStep = sections.FirstOrDefault(s => s.SectionId == operationSequence.SectionId);
            return findStep ?? new Section { Step = "" };
        }

        /// <summary>
        /// Retrieves all critical points from the analyses of the section associated with the operation sequence.
        /// </summary>
        /// <param name="operationSequence">The operation sequence containing the section.</param>
        /// <returns>
        /// A list of critical point strings. Returns an empty list if no critical points are found.
        /// </returns>
        public List<string> GetCriticalPoints(SOSDistributionOperationSequence operationSequence)
        {
            // NOTE: Flatten all critical points from analyses if they exist
            return operationSequence?.Section?.Analyses?.Where(a => a?.CriticalPoints != null).SelectMany(a => a.CriticalPoints).ToList() ?? new List<string>();
        }

        /// <summary>
        /// Retrieves the difficulty level for a given distribution.
        /// </summary>
        /// <param name="distribution">The distribution to check for difficulty level.</param>
        /// <returns>
        /// The <see cref="DifficultyLevel"/> of the distribution. Returns <c>DifficultyLevel.A</c> if not found.
        /// </returns>
        private DifficultyLevel GetDifficultyLevel(SOSDistribution distribution)
        {
            // NOTE: Default to DifficultyLevel.A if no specific difficulty is assigned
            var difficulty = _sosSynopticRequeriments.RequirementDifficulties?.FirstOrDefault(r => r.SOSHubId == distribution.SOSHubId);
            return difficulty?.DifficultyLevel ?? DifficultyLevel.A;
        }

        /// <summary>
        /// Updates the difficulty level of a specific distribution.
        /// </summary>
        /// <param name="distribution">The distribution whose difficulty level will be changed.</param>
        /// <param name="level">The new difficulty level to assign.</param>
        public void ChangeLevelDistribution(SOSDistribution distribution, DifficultyLevel level)
        {
            // NOTE: Find the corresponding difficulty record for the distribution
            var findDistributionDifficultyLevel = _sosSynopticRequeriments.RequirementDifficulties!.FirstOrDefault(r => r.SOSHubId == distribution.SOSHubId);
            if (findDistributionDifficultyLevel == null) return;

            // Update the difficulty level
            findDistributionDifficultyLevel.DifficultyLevel = level;

        }

        /// <summary>
        /// Retrieves the training time for a specific distribution.
        /// </summary>
        /// <param name="distribution">The distribution for which the training time is requested.</param>
        /// <returns>
        /// The training time in days. Returns 0 if no matching SOSHub is found.
        /// </returns>
        public int GetTrainingTime(SOSDistribution distribution)
        {
            return distribution.SOSHubs!.FirstOrDefault(s => s.SOSHubId == distribution.SOSHubId)?.TrainingTime ?? 0;
        }

        // =================================================== \\
        //&============ FUNCTIONS TO OPERATIONS ==============&\\
        // =================================================== \\

        /// <summary>
        /// Retrieves an existing operation sequence for a given SOS Hub and section.
        /// If not found, creates a new operation sequence and adds it to the current synoptic requirements.
        /// </summary>
        /// <param name="sosHubId">The ID of the SOS Hub.</param>
        /// <param name="sectionId">The ID of the section.</param>
        /// <returns>
        /// Returns the existing or newly created <see cref="SOSSynopticRequirementsOperationSequence"/>.
        /// </returns>
        private SOSSynopticRequirementsOperationSequence GetOperationSequence(int sosHubId, int sectionId)
        {
            // NOTE: Try to find an existing sequence matching the SOS Hub and section
            var existingSequence = _sosSynopticRequeriments?.SOSSynopticRequirementsOperationSequence?.FirstOrDefault(seq => seq.SosHubId == sosHubId && seq.SectionId == sectionId);
            if (existingSequence != null) return existingSequence;

            // NOTE: Attempt to find the section from the related SOS Hub's distribution
            var section = _sosSynopticRequeriments?.SOSHubs?.FirstOrDefault(h => h.SOSHubId == sosHubId)?.SOSDistribution?.FirstOrDefault(d => d.SOSHubId == sosHubId)?.SOSDistributionOperationSequence?.FirstOrDefault(opSeq => opSeq.SectionId == sectionId);

            // NOTE: Return an empty sequence if the section is not found
            if (section == null) return new SOSSynopticRequirementsOperationSequence();

            // NOTE: Create a new operation sequence based on found section details
            var newSequence = new SOSSynopticRequirementsOperationSequence
            {
                Sequence = section.SequenceId,
                SectionId = section.SectionId,
                SosHubId = sosHubId,
                Section = section.Section,
                OperationPersonText = section.Section?.Step,
                OperationMachineText = section.Section?.Step,
                IsOperationPersonRequired = true,
                IsOperationMachineRequired = false,
                IsActive = true,
                SOSSynopticTableofOperatingRequirementsId = _sosSynopticRequeriments!.SOSSynopticTableofOperatingRequirementsId
            };

            // NOTE: Add newly created sequence to the current synoptic requirements
            _sosSynopticRequeriments.SOSSynopticRequirementsOperationSequence?.Add(newSequence);

            return newSequence;
        }

        /// <summary>
        /// Sets the OperationPersonText for a specific SOS Hub and section,
        /// validating the input value before updating.
        /// </summary>
        /// <param name="sosHubId">The ID of the SOS Hub.</param>
        /// <param name="sectionId">The ID of the section.</param>
        /// <param name="value">The new text value to set.</param>
        private void SetOperationPersonText(int sosHubId, int sectionId, string value)
        {
            const int MinLength = 4;

            // NOTE: Validate input value is not null, empty, or whitespace
            if (string.IsNullOrWhiteSpace(value))
            {
                Snackbar.Add("El valor no puede estar vac�o.", Severity.Warning);
                return;
            }

            // NOTE: Find the operation sequence entry for the given SOS Hub and section
            var item = _sosSynopticRequeriments?.SOSSynopticRequirementsOperationSequence?.FirstOrDefault(s => s.SosHubId == sosHubId && s.SectionId == sectionId);
            if (item == null)
            {
                Snackbar.Add("No se encontr� el registro correspondiente.", Severity.Error);
                return;
            }

            // NOTE: Validate minimum length requirement for the value
            if (value.Length < MinLength)
            {
                Snackbar.Add($"El valor no puede tener menos de {MinLength} caracteres.", Severity.Warning);
                return;
            }

            // NOTE: Update OperationPersonText if new value is longer or null
            if (item.OperationPersonText == null || value.Length >= item.OperationPersonText.Length)
            {
                item.OperationPersonText = value;
            }
        }

        private void SetOperationMachineRequired(int sosHubId, int sectionId, bool? value)
        {
            if (value == null) return;
            // NOTE: Locate the operation sequence for the given SOS Hub and section
            var item = _sosSynopticRequeriments?.SOSSynopticRequirementsOperationSequence?.FirstOrDefault(s => s.SosHubId == sosHubId && s.SectionId == sectionId);
            if (item == null)
            {
                Snackbar.Add("No se encontro el registro correspondiente.", Severity.Error);
                return;
            }

            var valueCast = (bool)value;

            if (!valueCast) item.OperationMachineText = "";
            item.IsOperationMachineRequired = valueCast;
        }

        /// <summary>
        /// Sets the OperationMachineText for a specific SOS Hub and section,
        /// validating the input value before updating.
        /// </summary>
        /// <param name="sosHubId">The ID of the SOS Hub.</param>
        /// <param name="sectionId">The ID of the section.</param>
        /// <param name="value">The new text value to set.</param>
        private void SetOperationMachineText(int sosHubId, int sectionId, string value)
        {
            const int MinLength = 4;

            // NOTE: Validate that value is not null, empty, or whitespace
            if (string.IsNullOrWhiteSpace(value))
            {
                Snackbar.Add("El valor no puede estar vac�o.", Severity.Warning);
                return;
            }

            // NOTE: Locate the operation sequence for the given SOS Hub and section
            var item = _sosSynopticRequeriments?.SOSSynopticRequirementsOperationSequence?.FirstOrDefault(s => s.SosHubId == sosHubId && s.SectionId == sectionId);
            if (item == null)
            {
                Snackbar.Add("No se encontro el registro correspondiente.", Severity.Error);
                return;
            }

            // NOTE: Ensure value meets minimum length requirement
            if (value.Length < MinLength)
            {
                Snackbar.Add($"El valor no puede tener menos de {MinLength} caracteres.", Severity.Warning);
                return;
            }

            // NOTE: Update OperationMachineText if new value is longer or null
            if (item.OperationMachineText == null || value.Length >= item.OperationMachineText.Length)
            {
                item.OperationMachineText = value;
            }
        }

        // =================================================== \\
        //&============ FUNCTIONS TO KNOWLEDGES ==============&\\
        // =================================================== \\

        /// <summary>
        /// Gets the knowledge DTO for a SOS hub, or an empty instance if not found.
        /// </summary>
        /// <param name="sosHubId">The SOS hub ID.</param>
        /// <returns>The corresponding <c>KnowledgeDynamicDTO</c> or a new empty instance.</returns>
        private KnowledgeDynamicDTO GetKnowledgeDTO(int SOSHubId)
        {
            return KnowledgeDynamicDTO.FirstOrDefault(k => k.SOSHubId == SOSHubId) ?? new KnowledgeDynamicDTO { };
        }

        /// <summary>
        /// Checks if the selected knowledge exists in the filtered list.
        /// </summary>
        /// <param name="sosHubId">The SOS hub ID.</param>
        /// <returns><c>true</c> if exists; otherwise <c>false</c>.</returns>
        /// </returns>
        private bool IsExistingKnowledge(int SosHubId)
        {
            var dto = KnowledgeDynamicDTO.FirstOrDefault(k => k.SOSHubId == SosHubId);
            if (dto == null) return false;

            // Return false if filtered knowledge list or selected knowledge name is null/empty
            if (dto.FilteredKnowledge == null || string.IsNullOrWhiteSpace(dto.SelectedKnowledgeName)) return false;

            // Check if selected knowledge exists in the filtered list
            return dto.FilteredKnowledge.Any(t => t.Name.Equals(dto.SelectedKnowledgeName, StringComparison.OrdinalIgnoreCase));
        }


        /// <summary>
        /// Sets the selected knowledge name for a SOS hub.
        /// </summary>
        /// <param name="sosHubId">The SOS hub ID.</param>
        /// <param name="value">The knowledge name (empty if null).</param>
        private void SetSelectedKnowledge(int SOSHubId, string value)
        {
            var dto = KnowledgeDynamicDTO.FirstOrDefault(k => k.SOSHubId == SOSHubId);

            if (dto != null)
            {
                dto.SelectedKnowledgeName = value ?? string.Empty;
            }
        }

        /// <summary>
        /// Adds the selected knowledge to assigned list and removes it from filtered list.
        /// </summary>
        /// <param name="sosHubId">The SOS hub ID.</param>
        private void AddSelectedKnowledge(int SosHubId)
        {
            var dto = KnowledgeDynamicDTO.FirstOrDefault(k => k.SOSHubId == SosHubId);

            if (dto == null) return;

            // NOTE: Find the knowledge matching the selected name
            var knowledge = dto.FilteredKnowledge.FirstOrDefault(t => t.Name.Equals(dto.SelectedKnowledgeName, StringComparison.OrdinalIgnoreCase));

            if (knowledge == null) return;

            // NOTE: Add to assigned and remove from filtered list
            dto.KnowledgeIds.Add(knowledge.Id);
            dto.FilteredKnowledge.Remove(knowledge);

            // Reset selection
            dto.SelectedKnowledgeName = string.Empty;
            StateHasChanged();
        }

        /// <summary>
        /// Removes a knowledge from assigned list and adds it back to filtered list.
        /// </summary>
        /// <param name="sosHubId">The SOS hub ID.</param>
        /// <param name="knowledgeId">The knowledge ID to remove.</param>
        private void RemoveKnowledge(int SosHubId, int idKnowledge)
        {
            var dto = KnowledgeDynamicDTO.FirstOrDefault(k => k.SOSHubId == SosHubId);
            if (dto == null) return;

            // NOTE: Find the knowledge in the general list
            var knowledge = _KnowledgeGeneral.FirstOrDefault(t => t.Id == idKnowledge);
            if (knowledge == null) return;

            // Remove from assigned and add back to filtered list
            dto.KnowledgeIds.Remove(knowledge.Id);
            dto.FilteredKnowledge.Add(knowledge);

            // Reset selection
            dto.SelectedKnowledgeName = string.Empty;
            StateHasChanged();
        }

        // -============ DYNAMIC FILTER HELPERS =============-+\\

        /// <summary>
        /// Searches for knowledge names containing the given value.
        /// </summary>
        /// <param name="value">The search term.</param>
        /// <param name="sosHubId">The SOS hub ID.</param>
        /// <returns>Matching knowledge names or empty array.</returns>
        private async Task<IEnumerable<string>> SearchKnowledge(string value, int SosHubId)
        {
            if (string.IsNullOrWhiteSpace(value)) return Array.Empty<string>();

            var dto = GetKnowledgeDTO(SosHubId);
            if (dto == null || dto.FilteredKnowledge == null) return Array.Empty<string>();

            // NOTE: Filter knowledge names case-insensitively
            var filtered = dto.FilteredKnowledge.Where(k => k.Name.Contains(value, StringComparison.OrdinalIgnoreCase)).Select(k => k.Name).ToList() ?? new List<string>();

            // NOTE: Small delay to simulate async operation for UI responsiveness
            await Task.Delay(50);

            return filtered;
        }

        /// <summary>
        /// Opens a modal to add a new knowledge item for a SOS hub.
        /// </summary>
        /// <param name="sosHubId">The SOS hub ID.</param>
        private async Task OpenAddKnowledgeModal(int SosHubId)
        {
            var dto = KnowledgeDynamicDTO.FirstOrDefault(k => k.SOSHubId == SosHubId);

            var parameters = new DialogParameters { { "KnowledgeName", dto?.SelectedKnowledgeName ?? "" } };
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

            // NOTE: Show modal dialog
            var dialog = DialogService.Show<AddKnowledge_modal>("Crear Habilidad", parameters, options);
            var result = await dialog.Result;

            // NOTE: If user confirms, add new knowledge to general list and update all filtered lists
            if (!result.Canceled && result.Data != null)
            {
                var newKnowledge = (Knowledge)result.Data;
                _KnowledgeGeneral.Add(newKnowledge);
                KnowledgeDynamicDTO.ForEach(a => a.FilteredKnowledge.Add(newKnowledge));
            }
        }


        // =================================================== \\
        //&============== FUNCTIONS TO SKILLS ================&\\
        // =================================================== \\

        /// <summary>
        /// Gets the Skill DTO for a SOS hub, or an empty instance if not found.
        /// </summary>
        /// <param name="sosHubId">The SOS hub ID.</param>
        /// <returns>The corresponding <c>SkillDynamicDTO</c> or a new empty instance.</returns>
        private SkillDynamicDTO GetSkillDTO(int SOSHubId)
        {
            return SkillDynamicDTO.FirstOrDefault(k => k.SOSHubId == SOSHubId) ?? new SkillDynamicDTO { };
        }

        /// <summary>
        /// Checks if the selected Skill exists in the filtered list.
        /// </summary>
        /// <param name="sosHubId">The SOS hub ID.</param>
        /// <returns><c>true</c> if exists; otherwise <c>false</c>.</returns>
        /// </returns>
        private bool IsExistingSkill(int SosHubId)
        {
            var dto = SkillDynamicDTO.FirstOrDefault(k => k.SOSHubId == SosHubId);
            if (dto == null) return false;

            // Return false if filtered Skill list or selected Skill name is null/empty
            if (dto.FilteredSkill == null || string.IsNullOrWhiteSpace(dto.SelectedSkillName)) return false;

            // Check if selected Skill exists in the filtered list
            return dto.FilteredSkill.Any(t => t.Name.Equals(dto.SelectedSkillName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Sets the selected Skill name for a SOS hub.
        /// </summary>
        /// <param name="sosHubId">The SOS hub ID.</param>
        /// <param name="value">The Skill name (empty if null).</param>
        private void SetSelectedSkill(int SOSHubId, string value)
        {
            var dto = SkillDynamicDTO.FirstOrDefault(k => k.SOSHubId == SOSHubId);

            if (dto != null)
            {
                dto.SelectedSkillName = value ?? string.Empty;
            }
        }


        /// <summary>
        /// Adds the selected Skill to assigned list and removes it from filtered list.
        /// </summary>
        /// <param name="sosHubId">The SOS hub ID.</param>
        private void AddSelectedSkill(int SosHubId)
        {
            var dto = SkillDynamicDTO.FirstOrDefault(k => k.SOSHubId == SosHubId);
            if (dto == null) return;

            // NOTE: Find the Skill matching the selected name
            var Skill = dto.FilteredSkill.FirstOrDefault(t => t.Name.Equals(dto.SelectedSkillName, StringComparison.OrdinalIgnoreCase));
            if (Skill == null) return;

            // NOTE: Add to assigned and remove from filtered list
            dto.SkillIds.Add(Skill.Id);
            dto.FilteredSkill.Remove(Skill);

            // Reset selection
            dto.SelectedSkillName = string.Empty;
            StateHasChanged();
        }

        /// <summary>
        /// Removes a Skill from assigned list and adds it back to filtered list.
        /// </summary>
        /// <param name="sosHubId">The SOS hub ID.</param>
        /// <param name="SkillId">The Skill ID to remove.</param>
        private void RemoveSkill(int SosHubId, int idSkill)
        {
            var dto = SkillDynamicDTO.FirstOrDefault(k => k.SOSHubId == SosHubId);
            if (dto == null) return;

            // NOTE: Find the Skill in the general list
            var Skill = _SkillGeneral.FirstOrDefault(t => t.Id == idSkill);
            if (Skill == null) return;

            // Remove from assigned and add back to filtered list
            dto.SkillIds.Remove(Skill.Id);
            dto.FilteredSkill.Add(Skill);

            // Reset selection
            dto.SelectedSkillName = string.Empty;
            StateHasChanged();
        }

        // -============ DYNAMIC FILTER HELPERS =============-+\\

        /// <summary>
        /// Searches for Skill names containing the given value.
        /// </summary>
        /// <param name="value">The search term.</param>
        /// <param name="sosHubId">The SOS hub ID.</param>
        /// <returns>Matching Skill names or empty array.</returns>
        private async Task<IEnumerable<string>> SearchSkill(string value, int SosHubId)
        {
            if (string.IsNullOrWhiteSpace(value)) return Array.Empty<string>();

            var dto = GetSkillDTO(SosHubId);
            if (dto == null || dto.FilteredSkill == null) return Array.Empty<string>();

            // NOTE: Filter Skill names case-insensitively
            var filtered = dto.FilteredSkill.Where(k => k.Name.Contains(value, StringComparison.OrdinalIgnoreCase)).Select(k => k.Name).ToList() ?? new List<string>();

            // NOTE: Small delay to simulate async operation for UI responsiveness
            await Task.Delay(50);

            return filtered;
        }

        /// <summary>
        /// Opens a modal to add a new Skill item for a SOS hub.
        /// </summary>
        /// <param name="sosHubId">The SOS hub ID.</param>
        private async Task OpenAddSkillModal(int SosHubId)
        {
            var dto = SkillDynamicDTO.FirstOrDefault(k => k.SOSHubId == SosHubId);

            var parameters = new DialogParameters { { "SkillName", dto?.SelectedSkillName ?? "" } };
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

            // NOTE: Show modal dialog
            var dialog = DialogService.Show<AddSkill_modal>("Crear Habilidad", parameters, options);
            var result = await dialog.Result;

            // NOTE: If user confirms, add new Skill to general list and update all filtered lists
            if (!result.Canceled && result.Data != null)
            {
                var newSkill = (Skill)result.Data;
                _SkillGeneral.Add(newSkill);
                SkillDynamicDTO.ForEach(a => a.FilteredSkill.Add(newSkill));
            }
        }


        // =================================================== \\
        //&============= ESTABLISHED CONDITIONS ==============&\\
        // =================================================== \\

        /// <summary>
        /// Sets the established conditions dynamically for each SOS hub's first distribution.
        /// Aggregates them into <see cref="EstablishedConditionDynamicDTO"/> objects.
        /// </summary>
        private void SetEstablishedConditionsDynamic()
        {
            // NOTE: Exit early if there are no SOS hubs
            if (_sosSynopticRequeriments?.SOSHubs == null) return;

            var copyEstablishedConditions = _sosSynopticRequeriments.EstablishedConditions ?? new List<EstablishedConditions>();
            var generalEConditions = new List<EstablishedConditionDynamicDTO>();

            foreach (var hub in _sosSynopticRequeriments.SOSHubs)
            {
                // NOTE: Only consider the first distribution of the hub
                var firstDistribution = hub?.SOSDistribution?.FirstOrDefault();
                if (firstDistribution == null) continue;

                var sectionsDistribution = BuildOperationSequences(firstDistribution);

                // NOTE: Map each section to a dynamic DTO with its established conditions
                var setECDynamicDistribution = sectionsDistribution.Select(section =>
                {
                    var establishedForSection = copyEstablishedConditions.Where(ec => ec.SectionId == section.SectionId).ToList();

                    return new EstablishedConditionDynamicDTO
                    {
                        SectionId = section.SectionId ?? 0,
                        InputEstablishedCondition = string.Empty,
                        EstablishedConditions = establishedForSection
                    };
                }
                );

                generalEConditions.AddRange(setECDynamicDistribution);
            }

            // NOTE: Assign the aggregated dynamic conditions to the property
            EstablishedConditionDynamicDTO = generalEConditions;
        }

        /// <summary>
        /// Retrieves the dynamic established condition for a given section.
        /// Returns a new <see cref="EstablishedConditionDynamicDTO"/> if none exists for the section.
        /// </summary>
        /// <param name="sectionId">The identifier of the section.</param>
        /// <returns>The <see cref="EstablishedConditionDynamicDTO"/> for the specified section.</returns>
        private EstablishedConditionDynamicDTO GetEstablishedCondition(int sectionId)
        {
            // NOTE: Return the first matching DTO or a new instance if not found
            return EstablishedConditionDynamicDTO.FirstOrDefault(e => e.SectionId == sectionId) ?? new EstablishedConditionDynamicDTO();
        }


        /// <summary>
        /// Removes an established condition from the provided dynamic DTO.
        /// Can remove by condition ID or by condition string if ID is zero.
        /// </summary>
        /// <param name="EstablishedConditionsDynamic">The dynamic DTO containing established conditions.</param>
        /// <param name="IdECondition">The ID of the condition to remove. If zero, the <paramref name="Condition"/> string is used.</param>
        /// <param name="Condition">The condition string to remove if <paramref name="IdECondition"/> is zero.</param>
        private void RemoveEstablishedCondition(EstablishedConditionDynamicDTO EstablishedConditionsDynamic, int IdECondition, string Condition)
        {
            // NOTE: Remove by ID if provided, otherwise remove by condition string
            if (IdECondition != 0)
            {
                EstablishedConditionsDynamic.EstablishedConditions.RemoveAll(e => e.Id == IdECondition);
            }
            else
            {
                EstablishedConditionsDynamic.EstablishedConditions.RemoveAll(e => e.Condition == Condition);
            }
        }

        /// <summary>
        /// Handles changes to an established condition and validates the new value.
        /// Updates the condition if it meets the minimum length, otherwise shows a warning.
        /// </summary>
        /// <param name="item">The established condition being updated.</param>
        /// <param name="value">The new value for the condition.</param>
        /// <param name="minLength">The minimum required length for the condition. Default is 2.</param>
        private void OnConditionChanged(EstablishedConditions item, string value, int minLength = 2)
        {
            if (string.IsNullOrEmpty(value) || value.Length < minLength)
            {
                Snackbar.Add($"El valor debe tener al menos {minLength} caracteres.", Severity.Warning);

                // NOTE: Truncate existing value if it exists
                if (!string.IsNullOrEmpty(item.Condition))
                {
                    item.Condition = new string(item.Condition.Take(minLength).ToArray());
                }

                return;
            }

            // NOTE: Update the condition with the new valid value
            item.Condition = value;
        }

        /// <summary>
        /// Changes the input value of the established condition for a given section.
        /// If the section does not exist, the method exits silently.
        /// </summary>
        /// <param name="sectionId">The identifier of the section to update.</param>
        /// <param name="value">The new input value for the condition.</param>
        public void ChangeEstablishedCondition(int sectionId, string value)
        {
            // NOTE: Find the section by ID; exit if not found
            var section = EstablishedConditionDynamicDTO.FirstOrDefault(e => e.SectionId == sectionId);
            if (section == null) return;

            // NOTE: Update the input value safely
            section.InputEstablishedCondition = value ?? string.Empty;
        }

        /// <summary>
        /// Adds a new established condition to the specified section.
        /// Validates that the input is not empty and prevents duplicate conditions.
        /// </summary>
        /// <param name="sectionId">The identifier of the section to add the condition to.</param>
        public void AddEstablishedCondition(int sectionId)
        {
            // NOTE: Find the section by ID; exit if not found
            var ECondition = EstablishedConditionDynamicDTO.FirstOrDefault(e => e.SectionId == sectionId);
            if (ECondition == null) return;

            var valueNewECondition = ECondition.InputEstablishedCondition;

            // NOTE: Notify and exit if input is empty
            if (string.IsNullOrWhiteSpace(valueNewECondition))
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                Snackbar.Add("No puede agregar una condicion vacia, ingrese texto", Severity.Info);
                return;
            }

            // NOTE: Check for duplicate conditions (case-insensitive)
            var existEstablishedCondition = ECondition.EstablishedConditions.Any(a => string.Equals(a.Condition, valueNewECondition, StringComparison.OrdinalIgnoreCase));
            if (!existEstablishedCondition)
            {
                // NOTE: Add new condition and reset input
                ECondition.InputEstablishedCondition = string.Empty;
                ECondition.EstablishedConditions.Add(new EstablishedConditions { Id = 0, Condition = valueNewECondition, SectionId = sectionId, SOSSynopticTableofOperatingRequirementsId = _sosSynopticRequeriments.SOSSynopticTableofOperatingRequirementsId });
                StateHasChanged();

            }
            else
            {
                // NOTE: Notify if the condition already exists
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                Snackbar.Add("Ya existe esta condicion establecida para estos puntos criticos", Severity.Warning);
            }
        }


        // =================================================== \\
        //&================ INSURANCE FEATURE ================&\\
        // =================================================== \\

        /// <summary>
        /// Sets the insurances features dynamically for each SOS hub's first distribution.
        /// Aggregates them into <see cref="InsuranceFeaturesDynamicDTO"/> objects.
        /// </summary>
        private void SetInsuranceFeaturesDynamic()
        {
            // NOTE: Exit early if there are no SOS hubs
            if (_sosSynopticRequeriments?.SOSHubs == null) return;

            var copyInsuranceFeatures = _sosSynopticRequeriments.InsuranceFeatures ?? new List<InsuranceFeatures>();
            var generalEFeatures = new List<InsuranceFeaturesDynamicDTO>();

            foreach (var hub in _sosSynopticRequeriments.SOSHubs)
            {
                // NOTE: Only consider the first distribution of the hub
                var firstDistribution = hub?.SOSDistribution?.FirstOrDefault();
                if (firstDistribution == null) continue;

                var sectionsDistribution = BuildOperationSequences(firstDistribution);

                // NOTE: Map each section to a dynamic DTO with its insurances features
                var setEFDynamicDistribution = sectionsDistribution.Select(section =>
                {
                    var establishedForSection = copyInsuranceFeatures.Where(ec => ec.SectionId == section.SectionId).ToList();

                    return new InsuranceFeaturesDynamicDTO
                    {
                        SectionId = section.SectionId ?? 0,
                        InputInsuranceFeatures = string.Empty,
                        InsuranceFeatures = establishedForSection
                    };
                }
                );

                generalEFeatures.AddRange(setEFDynamicDistribution);
            }

            // NOTE: Assign the aggregated dynamic insurances to the property
            InsuranceFeaturesDynamicDTO = generalEFeatures;
        }

        /// <summary>
        /// Retrieves the dynamic insurance feature for a given section.
        /// Returns a new <see cref="InsuranceFeaturesDynamicDTO"/> if none exists for the section.
        /// </summary>
        /// <param name="sectionId">The identifier of the section.</param>
        /// <returns>The <see cref="InsuranceFeaturesDynamicDTO"/> for the specified section.</returns>
        private InsuranceFeaturesDynamicDTO GetInsuranceFeatures(int sectionId)
        {
            // NOTE: Return the first matching DTO or a new instance if not found
            return InsuranceFeaturesDynamicDTO.FirstOrDefault(e => e.SectionId == sectionId) ?? new InsuranceFeaturesDynamicDTO();
        }


        /// <summary>
        /// Removes an insurance feature from the provided dynamic DTO.
        /// Can remove by insurance ID or by insurance string if ID is zero.
        /// </summary>
        /// <param name="InsuranceFeaturesDynamic">The dynamic DTO containing insurances features.</param>
        /// <param name="IdECondition">The ID of the insurance to remove. If zero, the <paramref name="Condition"/> string is used.</param>
        /// <param name="Condition">The insurance string to remove if <paramref name="IdECondition"/> is zero.</param>
        private void RemoveInsuranceFeatures(InsuranceFeaturesDynamicDTO InsuranceFeaturesDynamic, int IdEInsurance, string Insurance)
        {
            // NOTE: Remove by ID if provided, otherwise remove by insurance string
            if (IdEInsurance != 0)
            {
                InsuranceFeaturesDynamic.InsuranceFeatures.RemoveAll(e => e.Id == IdEInsurance);
            }
            else
            {
                InsuranceFeaturesDynamic.InsuranceFeatures.RemoveAll(e => e.Insurance == Insurance);
            }
        }

        /// <summary>
        /// Handles changes to an insurance feature and validates the new value.
        /// Updates the insurance if it meets the minimum length, otherwise shows a warning.
        /// </summary>
        /// <param name="item">The insurance feature being updated.</param>
        /// <param name="value">The new value for the insurance.</param>
        /// <param name="minLength">The minimum required length for the insurance. Default is 2.</param>
        private void OnInsuranceChanged(InsuranceFeatures item, string value, int minLength = 2)
        {
            if (string.IsNullOrEmpty(value) || value.Length < minLength)
            {
                Snackbar.Add($"El valor debe tener al menos {minLength} caracteres.", Severity.Warning);

                // NOTE: Truncate existing value if it exists
                if (!string.IsNullOrEmpty(item.Insurance))
                {
                    item.Insurance = new string(item.Insurance.Take(minLength).ToArray());
                }

                return;
            }

            // NOTE: Update the insurance with the new valid value
            item.Insurance = value;
        }

        /// <summary>
        /// Changes the input value of the insurance feature for a given section.
        /// If the section does not exist, the method exits silently.
        /// </summary>
        /// <param name="sectionId">The identifier of the section to update.</param>
        /// <param name="value">The new input value for the insurance.</param>
        public void ChangeInsuranceFeature(int sectionId, string value)
        {
            // NOTE: Find the section by ID; exit if not found
            var section = InsuranceFeaturesDynamicDTO.FirstOrDefault(e => e.SectionId == sectionId);
            if (section == null) return;

            // NOTE: Update the input value safely
            section.InputInsuranceFeatures = value ?? string.Empty;
        }

        /// <summary>
        /// Adds a new insurance features to the specified section.
        /// Validates that the input is not empty and prevents duplicate insurance.
        /// </summary>
        /// <param name="sectionId">The identifier of the section to add the insurance to.</param>
        public void AddInsuranceFeatures(int sectionId)
        {
            // NOTE: Find the section by ID; exit if not found
            var ECondition = InsuranceFeaturesDynamicDTO.FirstOrDefault(e => e.SectionId == sectionId);
            if (ECondition == null) return;

            var valueNewEFeature = ECondition.InputInsuranceFeatures;

            // NOTE: Notify and exit if input is empty
            if (string.IsNullOrWhiteSpace(valueNewEFeature))
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                Snackbar.Add("No puede agregar una condicion vacia, ingrese texto", Severity.Info);
                return;
            }

            // NOTE: Check for duplicate insurance (case-insensitive)
            var existInsuranceFeatures = ECondition.InsuranceFeatures.Any(a => string.Equals(a.Insurance, valueNewEFeature, StringComparison.OrdinalIgnoreCase));
            if (!existInsuranceFeatures)
            {
                // NOTE: Add new insurance and reset input
                ECondition.InputInsuranceFeatures = string.Empty;
                ECondition.InsuranceFeatures.Add(new InsuranceFeatures { Id = 0, Insurance = valueNewEFeature, SectionId = sectionId, SOSSynopticTableofOperatingRequirementsId = _sosSynopticRequeriments.SOSSynopticTableofOperatingRequirementsId });
                StateHasChanged();

            }
            else
            {
                // NOTE: Notify if the insurance already exists
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                Snackbar.Add("Ya existe esta caracteristica para estas caracteristicas de aseguramiento", Severity.Warning);
            }
        }


        // =================================================== \\
        //&============ FUNCTIONS TO UPDATE SCRO =============&\\
        // =================================================== \\

        /// <summary>
        /// Updates the Synoptic Table of Operating Requirements (STRO) and provides user feedback via a snackbar.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateSTRO()
        {
            // Prepare the data transfer object for updating STRO
            var stroUpdateDto = FormatterSendDataSTRO(_sosSynopticRequeriments, KnowledgeDynamicDTO, SkillDynamicDTO, EstablishedConditionDynamicDTO, InsuranceFeaturesDynamicDTO);

            // Call service to update the Synoptic Table
            var updateResult = await SynopticRequirementsService.UpdateSOSSynopticTableofOperatingRequirements(stroUpdateDto);

            // Clear previous notifications and configure snackbar position
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopRight;

            if (updateResult == null)
            {
                Snackbar.Add("Failed to update Synoptic Table of Operation Requirements. Please reload and try again.", Severity.Error);
            }
            else
            {
                Snackbar.Add("Synoptic Table of Operation Requirements updated successfully.", Severity.Success);
            }

            NavigationManager.NavigateTo($"soshoe/SynopticRequirements/Details/{stroUpdateDto.SOSSynopticTableofOperatingRequirementsId}");
        }

        /// <summary>
        /// Maps a <c>SOSSynopticTableofOperatingRequirements</c> entity to a DTO suitable for update operations.
        /// Combines existing entity data with dynamic knowledge, skill, and established condition filters.
        /// </summary>
        /// <param name="STRO">The source Synoptic Table of Operating Requirements entity.</param>
        /// <param name="knowledgeDynamicDTOs">Dynamic knowledge filters to include in the DTO.</param>
        /// <param name="skillDynamicDTOs">Dynamic skill filters to include in the DTO.</param>
        /// <param name="EstablishedConditionDynamicDTOs">Dynamic established condition filters to include in the DTO.</param>
        /// <returns>A DTO containing the necessary fields for updating the STRO.</returns>
        private static SOSSynopticTableofOperatingRequirementsForUpdateDto FormatterSendDataSTRO(SOSSynopticTableofOperatingRequirements STRO, List<KnowledgeDynamicDTO> knowledgeDynamicDTOs, List<SkillDynamicDTO> skillDynamicDTOs, List<EstablishedConditionDynamicDTO> EstablishedConditionDynamicDTOs,List<InsuranceFeaturesDynamicDTO> InsuranceFeaturesDynamicDTOs)
        {
            return new SOSSynopticTableofOperatingRequirementsForUpdateDto
            {
                SOSSynopticTableofOperatingRequirementsId = STRO.SOSSynopticTableofOperatingRequirementsId,
                SOSSynopticRequirementsOperationSequence = STRO.SOSSynopticRequirementsOperationSequence,
                InternalControlNumber = STRO.InternalControlNumber,
                ProcessName = STRO.ProcessName,
                CreatorId = STRO.CreatorId,
                ReviewerId = STRO.ReviewerId,
                ApproverId = STRO.ApproverId,
                IsActive = STRO.IsActive,
                SOSHubId = STRO.SOSHubId,

                // NOTE: Include all associated hub IDs
                SOSHubIds = STRO?.SOSHubs?.Select(s => s.SOSHubId).ToList(),

                // NOTE: Copy current requirement difficulties
                RequirementDifficulties = STRO?.RequirementDifficulties?.ToList(),

                // NOTE: Map dynamic knowledge IDs to the DTO
                SOSSTROKnowledge = knowledgeDynamicDTOs.SelectMany(a => a.KnowledgeIds.Select(k => new SOSSTROKnowledgeHub { Id = 0, KnowledgeId = k, SOSHubId = a.SOSHubId })).ToList(),

                // NOTE: Map dynamic skill IDs to the DTO
                SOSSTROSkill = skillDynamicDTOs.SelectMany(a => a.SkillIds.Select(s => new SOSSTROSkillHub { Id = 0, SkillId = s, SOSHubId = a.SOSHubId })).ToList(),

                // NOTE: Map dynamic established conditions to the DTO
                EstablishedConditions = EstablishedConditionDynamicDTOs.SelectMany(e => e.EstablishedConditions.Select(ec => new EstablishedConditions { Id = ec.Id, Condition = ec.Condition, SectionId = ec.SectionId, SOSSynopticTableofOperatingRequirementsId = ec.SOSSynopticTableofOperatingRequirementsId })).ToList(),

                // NOTE: Map dynamic insurances features to the DTO
                InsuranceFeatures = InsuranceFeaturesDynamicDTOs.SelectMany(e => e.InsuranceFeatures.Select(ec => new InsuranceFeatures { Id = ec.Id, Insurance = ec.Insurance, SectionId = ec.SectionId, SOSSynopticTableofOperatingRequirementsId = ec.SOSSynopticTableofOperatingRequirementsId })).ToList()
            };
        }
    }
}