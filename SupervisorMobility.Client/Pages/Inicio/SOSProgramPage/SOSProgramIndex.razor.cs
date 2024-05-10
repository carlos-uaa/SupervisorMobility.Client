using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Services.BreadcrumsService;
using SupervisorMobility.Client.Services.PATService;
using System;
using static MudBlazor.CategoryTypes;

namespace SupervisorMobility.Client.Pages.Inicio.SOSProgramPage
{
    public partial class SOSProgramIndex
    {

        bool ShowLoading = true;

        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };

        private List<SOSReviewProgram>? _SosReviewList { get; set; } = new();
        private string searchString1 = "";

        private List<BreadcrumbItem> _links;
        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;
        [Inject]
        private IBreadcrumbService BreadcrumbService { get; set; }
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
                new BreadcrumbItem(text: Localizer["sosProgram"], href: "/", disabled: true),
            };

            BreadcrumbService.UpdateBreadcrumbs(_links);


            try
            {
                ShowLoading = true;

                logged = await HasPropertyAsync();
                if (logged)
                {
                    await GetUserAsync();
                    _SosReviewList = await SOSReviewServices.GetAllSOSReviews(true, true);


                    if (user.UserType == 2)
                    {
                                        _SosReviewList = _SosReviewList
                        .Where(s => s.Supervisors != null &&
                                    s.Supervisors.Any(x => user.Subordinates.Any(subordinate => subordinate.UserId == x.UserId)))
                        .ToList();

                        // Imprime información para depurar
                        foreach (var sosReviewProgram in _SosReviewList)
                        {
                            Console.WriteLine($"SOSReviewProgramId: {sosReviewProgram.Area.Description}");
                            foreach (var supervisor in sosReviewProgram.Supervisors)
                            {
                                Console.WriteLine($"Supervisor UserId: {supervisor.UserId}");
                            }
                        }
                    }
                    else if (user.UserType == 3)
                    {
                        HashSet<int> userIds = new HashSet<int>(_SosReviewList.SelectMany(s => s.Supervisors?.Select(u => u.UserId) ?? Enumerable.Empty<int>()));

                        _SosReviewList = _SosReviewList
                        .Where(s => s.Supervisors != null && s.Supervisors.Any(x => x.UserId == user.UserId))
                        .ToList();

                       

                    }
                    StateHasChanged();
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                ShowLoading = false;

            }



        }


        private async Task GetUserAsync()
        {
            if (!await TryGetAsync())
                user = new();
        }

        private async Task<bool> TryGetAsync()
        {
            bool hasProperty = await HasPropertyAsync();
            if (hasProperty)
            {
                json = await js.InvokeAsync<string>("localStorage.getItem", "user");
                user = JsonSerializer.Deserialize<User>(json) ?? new();


            }
            return hasProperty;
        }

        private async Task<bool> HasPropertyAsync()
            => await js.InvokeAsync<bool>("localStorage.hasOwnProperty", "user");


        private int selectedRowNumber = -1;
        private MudTable<SOSReviewProgram> SelectTableEvent;

        private void RowClickEvent(TableRowClickEventArgs<SOSReviewProgram> tableRowClickEventArgs)
        {
        }

        private string SelectedRowClassFunc(SOSReviewProgram element, int rowNumber)
        {
            if (selectedRowNumber == rowNumber)
            {
                selectedRowNumber = -1;
                if (SelectTableEvent.SelectedItem != null && SelectTableEvent.SelectedItem.Equals(element))
                {
                    NavigationManager.NavigateTo($"sosDetails/{element.SOSid}");

                }
                return string.Empty;
            }
            else if (SelectTableEvent.SelectedItem != null && SelectTableEvent.SelectedItem.Equals(element))
            {
                selectedRowNumber = rowNumber;
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }






        // Create pat
        void CreateSosReview()
        {
            NavigationManager.NavigateTo($"sosProgram/createSosReview");
        }

        // Delete pat
        async Task DeleteSosReview(int sosReviewId)
        {
            _SosReviewList.RemoveAll(sos => sos.SOSid == sosReviewId);
            await SOSReviewServices.DeleteSOSReview(sosReviewId);

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
            if (element.Supervisors.Any(u => u.Name.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase)))
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