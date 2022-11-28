using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class UpdateJobObservationType
    {
        // Parameters
        [Parameter]
        public int JobObservationTypeId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Job observation types", href: "/jobobservationtypes"),
            new BreadcrumbItem("UpdateJobObservationType", href: "", disabled: true)
        };

        // Objects
        public JobObservationType _jobObservationType { get; set; } = new();

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            JobObservationType dbJobObservationType = await JobObservationTypeService.GetJobObservationTypeById(JobObservationTypeId);
            _jobObservationType = dbJobObservationType;
        }

        // Update job observation type
        void UpdateJobObservationTypeAsync()
        {
            JobObservationTypeService.UpdateJobObservationType(_jobObservationType);
            NavigationManager.NavigateTo($"jobobservationtypes");
        }
    }
}
