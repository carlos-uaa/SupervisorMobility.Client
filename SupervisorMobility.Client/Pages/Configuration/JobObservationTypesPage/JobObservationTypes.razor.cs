using Microsoft.JSInterop;
using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.JobObservationTypesPage
{
    public partial class JobObservationTypes
    {
        //Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Job Observation types", href: "", disabled: true)
        };

        // Objects
        public List<JobObservationType> _jobObservationTypes { get; set; } = new();
        JobObservationType _jobObservationType = new();

        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _jobObservationTypes = await JobObservationTypeService.GetJobObservationTypes();
        }

        // Create job observation type
        void CreateJobObservationType()
        {
            NavigationManager.NavigateTo($"jobobservationtypes/createjobobservationtype");
        }

        // Delete job observation type
        async Task DeleteJobObservationType(int jobObservationTypeId)
        {

            _jobObservationTypes.RemoveAll(jobObservationType => jobObservationType.JobObservationTypeId == jobObservationTypeId);
            await JobObservationTypeService.DeleteJobObservationType(jobObservationTypeId);

            visibleDelete = false;
        }

        // Update job observation type
        void UpdateJobObservationType(int jobObservationTypeId)
        {
            NavigationManager.NavigateTo($"jobobservationtypes/updatejobobservationtype/{jobObservationTypeId}");
        }

        private string searchString = "";

        private bool FilterFunc(JobObservationType element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.JobObservationTypeId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Code.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.JobObservationTypeId} {element.Code} {element.Description}".Contains(searchString))
                return true;
            return false;
        }


        //Delete Product
        private bool visibleDelete = false;
        public int deleteJobObservationTypetId = 0;
        private void OpenDeleteDialog(int deleteId)
        {
            deleteJobObservationTypetId = deleteId;
            visibleDelete = true;
        }
        void CloseDeleteModal() => visibleDelete = false;
        private DialogOptions dialogDeleteOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, Position = DialogPosition.TopCenter, DisableBackdropClick = true, CloseButton = true };


    }
}
