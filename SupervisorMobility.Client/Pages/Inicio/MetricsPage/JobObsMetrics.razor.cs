using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities.ChartData;
using SupervisorMobility.Client.Data.Entities.PaginationEntities;

namespace SupervisorMobility.Client.Pages.Inicio.MetricsPage
{
    public partial class JobObsMetrics
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links;
        private IList<string> _sourceMsgLoading = new List<string>();

        [Inject]
        private IBreadcrumbService BreadcrumbService { get; set; }

        private string[] labels { get; set; }
        private int[] data { get; set; }

        private string[] labelsT { get; set; }
        StackedBarChartData[] datasetsT;
        private readonly string[] LabelsToStack = { "ILU Training_F" };
        private readonly string[] LabelsToStackOnto = { "ILU Training" };
        private readonly string[] LabelColors = { "#e74c3c", "#f1c40f", "#3498db", "#2ecc71" };

        private int total { get; set; }
        int TotalItems { get; set; }

        int reportType { get; set; }
        public List<JobObservation> _jobObservations { get; set; } = new();

        private MudTable<JobObservation> JobsTable;

        private MetricsFiltersDto filters = new MetricsFiltersDto
        {
            today = DateTime.Today.Date,
            inferiorDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
            superiorDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1)
        };

        bool isLoading = true;

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
            new BreadcrumbItem(text: Localizer["home"], href: "/"),
            new BreadcrumbItem(text: Localizer["Reports"], href: "/reports"),
            new BreadcrumbItem(text: Localizer["jobObservation"], href: "/reports/jobobservation", disabled: true)
        };

            BreadcrumbService.UpdateBreadcrumbs(_links);

            await Recount();

            isLoading = false;

            StateHasChanged();

            await RenderCharts();

            StateHasChanged();
        }

        private async Task Recount()
        {
            total = await MetricService.GetTotalJobObs(filters);

            var rawStatusData = await MetricService.GetJobsStatusChartData(filters);
            var rawTypeData = await MetricService.GetJobsTypeChartData(filters);

            List<string> tempLabels = new List<string>();
            List<int> tempValues = new List<int>();

            foreach (var item in rawStatusData)
            {
                tempLabels.Add(Localizer[item.Key]);
                tempValues.Add(item.Value);
            }

            labels = tempLabels.ToArray();
            data = tempValues.ToArray();


            tempLabels.Clear(); tempValues.Clear();

            int i = 0;
            int rawDataWithoutFinished = rawTypeData.Where(p => !LabelsToStack.Contains(p.Key)).Count();
            datasetsT = new StackedBarChartData[rawTypeData.Count];
            foreach (var item in rawTypeData)
            {
                var temparray = new int[rawDataWithoutFinished];
                bool InStackList = LabelsToStack.Contains(item.Key);
                bool InStackOntoList = LabelsToStackOnto.Contains(item.Key);

                if (!InStackList)
                {
                    tempLabels.Add(Localizer[item.Key]);
                    temparray[i] = item.Value;
                }
                else
                    temparray[i - 1] = item.Value;

                datasetsT[i] = new StackedBarChartData
                {
                    label = Localizer[item.Key],
                    data = temparray,
                    backgroundColor = LabelColors[i],
                    stack = !InStackList || !InStackOntoList ? "default" : "training"
                };

                i++;
            }

            labelsT = tempLabels.ToArray();
        }


        private async Task RenderCharts()
        {
            await JS.InvokeVoidAsync("renderPieChart", "myPieChart", new
            {
                labels = labels,
                datasets = new[]
                {
                new {
                    data = data,
                    backgroundColor = new[] { "#4A90E2", "#7ED321", "#F5A623", "#9B59B6", "#2ECC71", "#E74C3C" }
                }
            },
                total = total // ?? precomputed total
            });
            await JS.InvokeVoidAsync("renderStackedBarChart", "myStackedBarChart", new
            {
                labels = labelsT,
                datasets = datasetsT,
                total = total // ?? precomputed total
            });
        }

        async Task<TableData<JobObservation>> LoadJobObs(TableState state)
        {
            (int Total, List<JobObservation> JobObservations, JOCountPaginationDto Count) response = new();

            switch (reportType)
            {
                case 0:
                    response = await JobObservationService.GetLateJobObservationsByFilters(filters.today, filters.inferiorDate, filters.superiorDate, filters.plantId, filters.areaId,
                        filters.distributionId, filters.operationId, null, state.Page + 1, state.PageSize, (int)state.SortDirection, state.SortLabel);
                    break;
                case 1:
                    response = await JobObservationService.GetReprogrammedJobObservationsByFilters(filters.inferiorDate, filters.superiorDate, filters.plantId, filters.areaId,
                        filters.distributionId, filters.operationId, null, state.Page + 1, state.PageSize, (int)state.SortDirection, state.SortLabel);
                    break;
            }
            
            var pps = response.JobObservations.ToArray();

            _jobObservations = pps.ToList();

            TotalItems = response.Total;

            StateHasChanged();

            return new TableData<JobObservation>() { Items = pps, TotalItems = response.Total };
        }

        async Task applyFilters(MetricsFiltersDto newFilters)
        {
            filters = newFilters;
            await Recount();
            await RenderCharts();
            await JobsTable.ReloadServerData();
            StateHasChanged();
        }
    }
}