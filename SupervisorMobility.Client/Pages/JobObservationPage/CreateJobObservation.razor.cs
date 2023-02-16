using Microsoft.JSInterop;
using MudBlazor;

namespace SupervisorMobility.Client.Pages.JobObservationPage
{
    public partial class CreateJobObservation
    {

        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();
        List<Distribution> _distributions = new();
        List<Operation> _operations = new();
        public JobObservation _jobObservation { get; set; } = new();

        string[] models = new string[5] { "P71A", "X247", "P71A", "X247", "P71A" };
        string[] cicles = new string[5] { "1 min", "2 min", "3 min", "4 min", "5 min" };

        public string placeholder = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
          "sed do eiusmod tempor incididuntut labore et dolore magna aliqua. Ut enim ad minim " +
          "veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo coe velit esse cillum";

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Job Observation", href: "", disabled: true)
        };

        protected async override Task OnInitializedAsync()
        {
            _jobObservation.DateStart = DateTime.Now;
            _jobObservation.DateEnd = DateTime.Now;
            _jobObservation.Option = 3;
            _plants = await PlantServices.GetPlants();
        }
        private async void ShowAreas()
        {
            _areas = await AreaServices.GetAreas(_jobObservation.PlantId);
        }

        private async void ShowDistributions()
        {
            _distributions = await DistributionService.GetDistributions(_jobObservation.PlantId, _jobObservation.AreaId);
        }
        private async void ShowOperations()
        {
            _operations = await OperationService.GetOperations(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId);
        }

        private async Task CreateNewJobObservation()
        {


            _jobObservation.Models = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
            _jobObservation.Cicles= cicles[0] + "|" + cicles[1] + "|" + cicles[2] + "|" + cicles[3] + "|" + cicles[4];
            //Console.WriteLine(_jobObservation.PlantId);
            //Console.WriteLine(_jobObservation.AreaId);
            //Console.WriteLine(_jobObservation.DistributionId);
            //Console.WriteLine(_jobObservation.OperationId);
            //Console.WriteLine(_jobObservation.IsActive);
            //Console.WriteLine(_jobObservation.DateStart);
            //Console.WriteLine(_jobObservation.DateEnd);
            //Console.WriteLine(_jobObservation.Observer);
            //Console.WriteLine(_jobObservation.Operator);
            //Console.WriteLine(_jobObservation.Option);
            //Console.WriteLine(_jobObservation.Anomaly);
            //Console.WriteLine(_jobObservation.Time1HOE);
            //Console.WriteLine(_jobObservation.Time2HOE);
            //Console.WriteLine(_jobObservation.Models);
            //Console.WriteLine(_jobObservation.Cicles);
            //Console.WriteLine(_jobObservation.SArea);
            //Console.WriteLine(_jobObservation.QArea);
            //Console.WriteLine(_jobObservation.DArea);
            //Console.WriteLine(_jobObservation.CArea);
            //Console.WriteLine(_jobObservation.OthersArea);
            //Console.WriteLine(_jobObservation.IdentifiedActivity);
            //Console.WriteLine(_jobObservation.SsvCommentary);
            //Console.WriteLine(_jobObservation.OperatorCommentary);
            //Console.WriteLine(_jobObservation.SsvSignature);
            //Console.WriteLine(_jobObservation.OperatorSignature);

            var result = await JobObservationService.CreateJobObservation(_jobObservation);
            if (result != null)
            {
                NavigationManager.NavigateTo("/jobobservation");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Error en los datos!"); // Alert

        }

        void CancelCreateJobObservation()
        {
            NavigationManager.NavigateTo("/jobobservation");
        }


        //async void CreateNewAssyChartAsync()
        //{
        //    var result = await AssyChartServices.CreateAssyChart(_newassychart);
        //    if (result != null)
        //        NavigationManager.NavigateTo("/assychart");
        //    else
        //        await JsRuntime.InvokeVoidAsync("alert", "Error en los datos!"); // Alert
        //}

    }
}
