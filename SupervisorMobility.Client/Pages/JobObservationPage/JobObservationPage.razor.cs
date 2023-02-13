using Microsoft.JSInterop;
using MudBlazor;

namespace SupervisorMobility.Client.Pages.JobObservationPage
{
    public partial class JobObservationPage
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Job Observation", href: "/jobobservation", disabled: true),
        };

        //Objects
        private bool dense = false;
        private bool hover = true;
        private bool ronly = false;
        private string searchString = "";

        // Objects
        public List<JobObservation> _jobObservation { get; set; } = new();


        protected async override Task OnInitializedAsync()
        {
            _jobObservation = await JobObservationService.GetAllJobObservations();
        }


        async Task DeleteJobObservation(int jobObservationId)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this job observation?");

            if (confirm)
            {
                _jobObservation.RemoveAll(jobObservation => jobObservation.JobObservationId == jobObservationId);
                await JobObservationService.DeleteJobObservation(jobObservationId);
            }
        }

        void EditJobObservation(int jobObservationId)
        {
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{jobObservationId}");
        }

        void JobObservationDetails(int jobObservationId)
        {
            NavigationManager.NavigateTo($"jobobservation/details/{jobObservationId}");
        }




    }
}
