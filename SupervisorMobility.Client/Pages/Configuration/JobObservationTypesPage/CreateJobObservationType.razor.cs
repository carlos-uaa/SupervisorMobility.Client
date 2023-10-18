using MudBlazor;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.Configuration.JobObservationTypesPage
{
    public partial class CreateJobObservationType
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        // Objects
        JobObservationType _jobObservationType = new();

        protected async override Task OnInitializedAsync()
        {
         
            _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(text: Localizer["home"], href: "#"),
            new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
            new BreadcrumbItem(text: Localizer["JOTTitle"], href: "/jobobservationtypes"),
            new BreadcrumbItem(text: Localizer["JOTNew"], href: "", disabled: true)
        };
        }

        // Create job observation type
        async void CreateJobObservationTypeAsync()
        {_jobObservationType.IsActive = true;
            var result = await JobObservationTypeService.CreateJobObservationType(_jobObservationType);
            NavigationManager.NavigateTo($"jobobservationtypes");
        }
    }
}
