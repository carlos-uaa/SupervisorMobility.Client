using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Services.PATService;
using System;
using static MudBlazor.CategoryTypes;

namespace SupervisorMobility.Client.Pages.SOSProgramPage
{
    public partial class SOSProgramIndex
    {

        private List<SOSReviewProgram> _SosReviewList { get; set; } = new();
        private string searchString1 = "";

        private List<BreadcrumbItem> _links;
        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["sosProgram"], href: "/", disabled: true),
            };

            logged = await HasPropertyAsync();
            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error You have to log in", Severity.Error);
                NavigationManager.NavigateTo($"/");
            }
            else
            {
                await GetUserAsync();

                _SosReviewList = await SOSReviewServices.GetAllSOSReviews(true);

                StateHasChanged();
            }



        }

        //Local storage user
        private async Task GetUserAsync()
        {
            if (!await TryGetAsync())
            {
                user = new();
            }
        }

        private async Task<bool> TryGetAsync()
        {
            bool hasProperty = await HasPropertyAsync();
            if (hasProperty)
            {
                json = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user");
                user = JsonSerializer.Deserialize<User>(json) ?? new();

            }
            return hasProperty;
        }

        private async Task<bool> HasPropertyAsync()
            => await JSRuntime.InvokeAsync<bool>("localStorage.hasOwnProperty", "user");


        // Create pat
        void CreateSosReview()
        {
            NavigationManager.NavigateTo($"sosProgram/createSosReview");
        }

        // Delete pat
        async Task DeleteSosReview(int patId)
        {
            //_PATs.RemoveAll(pat => pat.PATid == patId);
            //await PATsServices.DeletePat(patId);

            visibleDelete = false;
        }

        // Pat details
        void SosReviewDetails(int sosReviewId)
        {
            NavigationManager.NavigateTo($"sosDetails/{sosReviewId}");
        }

        //filter function
        private bool FilterFunc1(SOSReviewProgram element) => FilterFunc(element, searchString1);

        private bool FilterFunc(SOSReviewProgram element, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.SOSid.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Status.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.UserA.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.UserB.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.UserC.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Plant.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Area.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.AplicationYear.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.CreationDate.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.EditionDate.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.ApprovalDate.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        //Delete SOS Review
        private bool visibleDelete = false;
        public int deleteSosReviewId = 0;
        private void OpenDeleteDialog(int deleteId)
        {
            deleteSosReviewId = deleteId;
            visibleDelete = true;
        }
        void CloseDeleteModal() => visibleDelete = false;
        private DialogOptions dialogDeleteOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, Position = DialogPosition.TopCenter, DisableBackdropClick = true, CloseButton = true };



    }
}