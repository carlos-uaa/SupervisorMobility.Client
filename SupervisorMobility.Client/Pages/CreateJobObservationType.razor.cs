using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class CreateJobObservationType
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Job observation types", href: "/jobobservationtypes"),
            new BreadcrumbItem("New job observation type", href: "", disabled: true)
        };

        // Objects
        JobObservationType _jobObservationType = new();

        // Create job observation type
        async void CreateJobObservationTypeAsync()
        {
            var result = await JobObservationTypeService.CreateJobObservationType(_jobObservationType);
            NavigationManager.NavigateTo($"jobobservationtypes");
        }
    }
}
