using FuzzyString;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Services.BreadcrumsService;
using SupervisorMobility.Client.Services.HCIService;
using SupervisorMobility.Client.Shared;
using static MudBlazor.CategoryTypes;

namespace SupervisorMobility.Client.Pages.Configuration.UserPage
{
    public partial class Users
    {
        [Inject]
        private IBreadcrumbService BreadcrumbService { get; set; }

        List<User> _users = new List<User>();
        List<User> allUsers = new List<User>();
        string searchString = "";

        //User
        private string json = string.Empty;
        public User loggedUser = new();

        private List<Data.Entities.HCI> _HCIs;

        private List<BreadcrumbItem> _links;

        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: @Localizer["home"], href: "/"),
                new BreadcrumbItem(text: @Localizer["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: @Localizer["UsersTitle"],href: "", disabled: true)
            };

            BreadcrumbService.UpdateBreadcrumbs(_links);

            await GetUserAsync();

            if (loggedUser != null)
            {
                if (loggedUser.UserType == 1)
                {
                    _users.AddRange(await UsersServices.GetUsersByType(1, false, false));
                    _users.AddRange(await UsersServices.GetUsersByType(5, true, false));
                    _users.AddRange(await UsersServices.GetUsersByType(6, true, false));
                    _users.AddRange(await UsersServices.GetUsersByType(7, true, false));
                    _users.AddRange(await UsersServices.GetUsersByType(2, true, false));
                    _users.AddRange(await UsersServices.GetUsersByType(3, true, false));
                    _users.AddRange(await UsersServices.GetUsersByType(4, true, false));
                }
                else
                {
                    switch (loggedUser.UserType)
                    {
                        case 5:
                            //Add loged
                            _users.Add(loggedUser);


                            foreach (User item in loggedUser.Subordinates)
                            {
                                //SSVs 
                                _users.Add(item);
                                var SVs = await UsersServices.GetSubordinates(item.UserId);
                                foreach (User Sub in SVs)
                                {
                                    //add SV
                                    _users.Add(Sub);
                                    //Operators
                                    _users.AddRange(Sub.Subordinates);
                                }
                            }
                            break;
                        case 2:
                            //SSV
                            _users.Add(loggedUser);

                            foreach (User item in loggedUser.Subordinates)
                            {
                                //add SV
                                _users.Add(item);
                                //Operators
                                _users.AddRange(await UsersServices.GetSubordinates(item.UserId));
                            }
                            break;
                        case 3:
                            _users.Add(loggedUser);
                            _users.AddRange(loggedUser.Subordinates);

                            break;
                    }
                }
            }

            _HCIs = await HCIsServices.GetHCIs();


        }

        //Local storage user
        private async Task GetUserAsync()
        {
            if (!await TryGetAsync())
                loggedUser = new();
        }

        private async Task<bool> TryGetAsync()
        {
            bool hasProperty = await HasPropertyAsync();
            if (hasProperty)
            {
                json = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user");
                loggedUser = JsonSerializer.Deserialize<User>(json) ?? new();
            }
            return hasProperty;
        }

        private async Task<bool> HasPropertyAsync()
            => await JSRuntime.InvokeAsync<bool>("localStorage.hasOwnProperty", "user");


        // Plant details
        void PlantDetails(int plantId)
        {
            NavigationManager.NavigateTo($"plants/{plantId}");
        }


        void EditUser(int userId)
        {
            NavigationManager.NavigateTo($"/usersmanagement/EditUser/{userId}");
        }

        async void DeleteUser(int iduser)
        {
            _users.RemoveAll(user => user.UserId == iduser);
            await UsersServices.DeleteUser(iduser);
            //await PlantService.DeletePlant(plantId);
            _users.Clear();


            allUsers = await UsersServices.GetUsers(true, false);


            await GetUserAsync();

            if (loggedUser != null)
            {
                if (loggedUser.UserType == 1)
                {
                    _users = allUsers;
                }
                else
                {

                    foreach (User usr in allUsers)
                    {
                        if (usr.UserId == loggedUser.UserId || usr.SuperiorId == loggedUser.UserId)
                        {
                            _users.Add(usr);
                        }

                    }

                }
            }


            visibleDelete = false;
            StateHasChanged();
        }

        void UserDetails(int userId)
        {
            NavigationManager.NavigateTo($"/usersmanagement/DetailUser/{userId}");
        }

        private bool FilterFunc(User element)
        {

            if (string.IsNullOrWhiteSpace(searchString))
                return true;

            if (element.UserId != 0)
            {
                if (element.UserId.ToString().Length >= searchString.Length)
                    if ((bool)element.UserId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                        return true;
            }

            if (element.ObjectId != null)
            {
                if (element.ObjectId.Length >= searchString.Length)
                    if ((bool)element.ObjectId?.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                        return true;
            }

            if (element.Email != null)
            {
                if (element.Email.Length >= searchString.Length)
                    if ((bool)element.Email?.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                        return true;
            }


            if (element.Payroll != null)
            {
                if (element.Payroll.ToString().Length >= searchString.Length)
                    if ((bool)element.Payroll?.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                        return true;
            }
            if (element.Plant != null)
            {
                if (element.Plant?.Description.ToString().Length >= searchString.Length)
                    if ((bool)element.Plant?.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                        return true;
            }
            if (element.Area != null)
            {
                if (element.Area?.Description.ToString().Length >= searchString.Length)
                    if ((bool)element.Area?.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                        return true;
            }
            if (element.Group != null)
            {
                if (element.Group?.Description.ToString().Length >= searchString.Length)
                    if ((bool)element.Group?.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                        return true;
            }

            if (element.Name.ToString().Length >= searchString.Length)
            {

                string[] partesBusqueda = searchString.ToLower().Split(' ');

                double simility = partesBusqueda.Select(part => element.Name.ToLower().JaccardDistance(part)).Average();


                // Puedes ajustar el umbral de similitud según tus necesidades
                if (simility > 0.95)
                {
                    Console.WriteLine($"{element.Name} = {simility}");
                    return true;
                }
            }

            if (element.Name.Length >= searchString.Length)
                if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;


            return false;
        }

        //Delete User
        private bool visibleDelete = false;
        public int deleteUserId = 0;
        private void OpenDeleteDialog(int deleteId)
        {
            deleteUserId = deleteId;
            visibleDelete = true;
        }
        void CloseDeleteModal() => visibleDelete = false;
        private DialogOptions dialogDeleteOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, Position = DialogPosition.TopCenter, DisableBackdropClick = true, CloseButton = true };

        private int selectedRowNumber = -1;
        private MudTable<User> SelectTableEvent;

        private void RowClickEvent(TableRowClickEventArgs<User> args)
        {
            var visibleItems = SelectTableEvent.FilteredItems.ToList();
            var rowIndex = visibleItems.IndexOf(args.Item);

            if (selectedRowNumber == rowIndex)
            {
                    NavigationManager.NavigateTo($"/usersmanagement/DetailUser/{args.Item.UserId}");
            }
            else
            {
                SelectTableEvent.SelectedItem = args.Item;
                selectedRowNumber = rowIndex;
                StateHasChanged();
            }
        }

        private void CreateHci(int UserId)
        {
        NavigationManager.NavigateTo($"/HCI/Create/?user={UserId}");
        }

        private string SelectedRowClassFunc(User element, int rowNumber)
        {
            var visibleItems = SelectTableEvent.FilteredItems.ToList();

            if (selectedRowNumber == rowNumber)
            {
                return "selected"; // Marca la fila seleccionada
            }
            else if (SelectTableEvent.SelectedItem != null && SelectTableEvent.SelectedItem.Equals(element))
            {
                selectedRowNumber = visibleItems.IndexOf(element);  // Usa el índice filtrado
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}