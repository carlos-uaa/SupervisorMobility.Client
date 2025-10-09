// - Core .NET imports
using System.Globalization;

// - Third-party imports
using MudBlazor;
using Microsoft.JSInterop;

namespace SupervisorMobility.Client.Pages.SOSHOE.SynopticTableofOperatingRequirements
{


    public partial class SynopticRequirementsDetails
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
        public bool IsDownload = false;

        //+==================== USER LOGIN ===================+\\
        public User user = new();
        public bool IsLoggedIn = false;

        //+============== SYNOPTIC REQUIREMENTS ===============+\\
        SOSSynopticTableofOperatingRequirements _sosSynopticRequeriments { get; set; } = new();

        //+================== HUB ANS LISTs ===================+\\
        SOSHub _soshub { get; set; } = new();

        //+==================== KNOWLEDGE =====================+\\
        private List<Knowledge> _KnowledgeGeneral = new();

        //+====================== SKILL =======================+\\
        private List<Skill> _SkillGeneral = new();


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

            await FillDistributions(_sosSynopticRequeriments);
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
        /// Navigates to the update page of a specific STRO by its ID.
        /// </summary>
        /// <param name="SynopticId">The ID of the STRO to view.</param>
        private void UpdateSynopticRequirements(int SynopticId)
        {
            NavigationManager.NavigateTo($"soshoe/SynopticRequirements/Update/{SynopticId}");
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
        //&======== FUNCTIONS TO FILL DATA OF SCRO ===========&\\
        // =================================================== \\

        /// <summary>
        /// Fills distributions for the given SOS hubs, separating those with and without valid distributions.
        /// Updates the component state accordingly.
        /// </summary>
        /// <param name="sosHubList">The list of SOS hubs to process.</param>
        private async Task FillDistributions(SOSSynopticTableofOperatingRequirements SOSSynopticRequeriments)
        {
            LoadingDistributions = true;

            IEnumerable<int> SOSHubsId = SOSSynopticRequeriments.SOSHubs!.Select(s => s.SOSHubId);

            var distributions = new List<SOSDistribution>();
            var hubsWithoutDistribution = new List<SOSDistribution>();

            StateHasChanged();

            foreach (var HubId in SOSHubsId)
            {
                var distribution = await SOSDistributionServices.GetSOSDistributionBySosHub(HubId, includeSOS: true, includeCollections: true);

                // NOTE: Separate hubs that have valid distributions from those that don't
                if (distribution == null || !(distribution.Analyses?.Any() == true || distribution.Sequences?.Any() == true))
                {
                    hubsWithoutDistribution.Add(distribution!);
                }
                else
                {
                    distributions.Add(distribution);
                }
            }

            LoadingDistributions = false;

            var SOSHubsInSTRO = _sosSynopticRequeriments.SOSHubs!.ToList();

            // NOTE: Notify user about hubs without distributions
            if (hubsWithoutDistribution.Count > 0) ShowMessageNotDistribution(hubsWithoutDistribution);
            SetDistributionSTRO(distributions, SOSHubsInSTRO);
        }

        /// <summary>
        /// Updates the synoptic requirements with selected SOS hubs and their corresponding distributions 
        /// and difficulty levels.
        /// </summary>
        /// <param name="distributionsComplete">List of complete distributions for SOS hubs.</param>
        /// <param name="selectedSosHubsDto">List of selected SOS hubs provided as DTOs.</param>
        private void SetDistributionSTRO(List<SOSDistribution> distributionsComplete, List<SOSHub> SOSHubsInSTRO)
        {
            var selectedSosHubs = new List<SOSHub>();

            foreach (var SOSHub in SOSHubsInSTRO)
            {
                var distribution = distributionsComplete.FirstOrDefault(d => d.SOSHubId == SOSHub.SOSHubId);
                if (distribution == null) continue;

                // Attach the found distribution to the hub
                SOSHub.SOSDistribution = new List<SOSDistribution> { distribution };
                selectedSosHubs.Add(SOSHub);
            }

            // Update the main synoptic requirements with hubs and difficulties
            _sosSynopticRequeriments.SOSHubs = selectedSosHubs;

            StateHasChanged();
        }

        /// <summary>
        /// Displays a warning message for SOS hubs without associated distributions 
        /// and removes them from the current hub list.
        /// </summary>
        /// <param name="hubsWithoutDistribution">List of SOS hubs missing distributions.</param>
        private void ShowMessageNotDistribution(List<SOSDistribution> Distributions)
        {
            var distributionList = string.Join(", ", Distributions.SelectMany(s => s.SOSHubs!.Select(a => a.Folio)));

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

        /// <summary>
        /// Retrieves an existing operation sequence for a given SOS Hub and section.
        /// Returns a new empty sequence if none is found.
        /// </summary>
        /// <param name="sosHubId">The ID of the SOS Hub.</param>
        /// <param name="sectionId">The ID of the section.</param>
        /// <returns>
        /// The existing <see cref="SOSSynopticRequirementsOperationSequence"/> 
        /// matching the given SOS Hub and section, or a new empty sequence if not found.
        /// </returns>
        private SOSSynopticRequirementsOperationSequence GetOperationSequence(int sosHubId, int sectionId)
        {
            // NOTE: Return the first matching sequence or a new instance if none exists
            return _sosSynopticRequeriments?.SOSSynopticRequirementsOperationSequence?.FirstOrDefault(s => s.SosHubId == sosHubId && s.SectionId == sectionId) ?? new SOSSynopticRequirementsOperationSequence();
        }

        /// <summary>
        /// Gets all skills associated with a specific SOS hub.
        /// </summary>
        /// <param name="sosHubId">The SOS hub ID.</param>
        /// <returns>A list of <c>SOSSTROSkillHub</c> for the given SOS hub, or an empty list if none exist.</returns>
        public List<SOSSTROSkillHub> GetSkills(int sosHubId)
        {
            return _sosSynopticRequeriments.SOSSTROSkill?.Where(a => a.SOSHubId == sosHubId).ToList() ?? new List<SOSSTROSkillHub>();
        }

        /// <summary>
        /// Gets all knowledges associated with a specific SOS hub.
        /// </summary>
        /// <param name="sosHubId">The SOS hub ID.</param>
        /// <returns>A list of <c>SOSSTROKnowledgeHub</c> for the given SOS hub, or <c>null</c> if none exist.</returns>
        public List<SOSSTROKnowledgeHub> GetKnowledges(int sosHubId)
        {
            return _sosSynopticRequeriments.SOSSTROKnowledge?.Where(a => a.SOSHubId == sosHubId).ToList();
        }

        /// <summary>
        /// Retrieves all established conditions for a specific section.
        /// Returns an empty list if no conditions exist for the section.
        /// </summary>
        /// <param name="sectionId">The identifier of the section.</param>
        /// <returns>A list of <see cref="EstablishedConditions"/> for the specified section.</returns>
        private List<EstablishedConditions> GetEstablishedCondition(int sectionId)
        {
            // NOTE: Return all conditions for the section or an empty list if none exist
            return _sosSynopticRequeriments?.EstablishedConditions?.Where(e => e.SectionId == sectionId).ToList() ?? new List<EstablishedConditions>();
        }

        /// <summary>
        /// Retrieves all insurance features for a specific section.
        /// Returns an empty list if no insurance exist for the section.
        /// </summary>
        /// <param name="sectionId">The identifier of the section.</param>
        /// <returns>A list of <see cref="EstablishedConditions"/> for the specified section.</returns>
        private List<InsuranceFeatures> GetInsuranceFeatures(int sectionId)
        {
            // NOTE: Return all conditions for the section or an empty list if none exist
            return _sosSynopticRequeriments?.InsuranceFeatures?.Where(e => e.SectionId == sectionId).ToList() ?? new List<InsuranceFeatures>();
        }


        //&===================== FUNCTIONS FOR DOWNLOAD FORMAT =====================&\\
        private async Task DownloadSTOR()
        {
            try
            {
                IsDownload = true;

                if (SynopticRequirementsId == null) throw new InvalidOperationException("SynopticRequirementsId is null.");
                await SynopticRequirementsService.GenerateExcelSTOperatingRequirements((int)SynopticRequirementsId, _sosSynopticRequeriments.ProcessName);

                IsDownload = false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error while downloading STOR: {ex.Message}");
                IsDownload = false;
            }
        }




    }
}