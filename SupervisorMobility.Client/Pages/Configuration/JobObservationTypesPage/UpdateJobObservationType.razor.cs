using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.JobObservationTypesPage
{
    public partial class UpdateJobObservationType
    {
        // Parameters
        [Parameter]
        public int JobObservationTypeId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links;
        // Objects
        public JobObservationType _jobObservationType { get; set; } = new();

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            JobObservationType dbJobObservationType = await JobObservationTypeService.GetJobObservationTypeById(JobObservationTypeId);
            _jobObservationType = dbJobObservationType;

            _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(text: Localizer["home"], href: "/"),
            new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
            new BreadcrumbItem(text: Localizer["JOTTitle"], href: "/jobobservationtypes"),
            new BreadcrumbItem("UpdateJobObservationType", href: "", disabled: true)
        };
        }

        // Update job observation type
        void UpdateJobObservationTypeAsync()
        {
            JobObservationTypeService.UpdateJobObservationType(_jobObservationType);
            NavigationManager.NavigateTo($"jobobservationtypes");
        }
    }
}
