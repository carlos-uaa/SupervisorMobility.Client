using Microsoft.JSInterop;
using MudBlazor;

namespace SupervisorMobility.Client.Pages.JobObservationPage
{
    public partial class UpdateJobObservation
    {

        [Parameter]
        public int JobObservationId { get; set; }
        public JobObservation _jobObservation { get; set; } = new();

        //Objects
        private bool dense = false;
        private bool hover = false;
        private bool ronly = false;

        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();
        List<Distribution> _distributions = new();
        List<Operation> _operations = new();

        public int plantId;
        public int areaId;
        public int distributionId;
        public int operationId;

        string[] models = new string[5];
        string[] cicles = new string[5];

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "/"),
            new BreadcrumbItem("Job Observation", href: "/jobobservation"),
            new BreadcrumbItem("Update Job Observation", href: "", disabled: true)
        };

        protected async override Task OnInitializedAsync()
        {
            _jobObservation = await JobObservationService.GetJobObservationById(JobObservationId);
            _plants = await PlantServices.GetPlants();
            _areas = await AreaServices.GetAreas(_jobObservation.PlantId);
            _distributions = await DistributionService.GetDistributions(_jobObservation.PlantId, _jobObservation.AreaId);
            _operations = await OperationService.GetOperations(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId);

            models = _jobObservation.Models.Split('|');
            cicles = _jobObservation.Cicles.Split('|');
        }
        private async void ShowAreas()
        {
            _jobObservation.AreaId = 0;
            _jobObservation.DistributionId = 0;
            _jobObservation.OperationId = 0;
            _areas = await AreaServices.GetAreas(_jobObservation.PlantId);
        }

        private async void ShowDistributions()
        {
            _jobObservation.DistributionId = 0;
            _jobObservation.OperationId = 0;
            _distributions = await DistributionService.GetDistributions(_jobObservation.PlantId, _jobObservation.AreaId);
        }
        private async void ShowOperations()
        {
            _jobObservation.OperationId = 0;
            _operations = await OperationService.GetOperations(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId);
        }

        private async Task CreateNewJobObservation()
        {

            _jobObservation.Models = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
            _jobObservation.Cicles = cicles[0] + "|" + cicles[1] + "|" + cicles[2] + "|" + cicles[3] + "|" + cicles[4];

            var result = await JobObservationService.UpdateJobObservation(_jobObservation);

            if (result)
            {
                await JSRuntime.InvokeVoidAsync("alert", "Job Observation Succesful Update!"); // Alert
                NavigationManager.NavigateTo("/jobobservation");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
        }

        void CancelUpdateJobObservation()
        {
            NavigationManager.NavigateTo("/jobobservation");
        }


    }
}
