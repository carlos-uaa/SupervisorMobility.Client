using Microsoft.JSInterop;
using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.UserPage
{
    public partial class EditUsers
    {
        [Parameter]
        public int userId { get; set; }

        [Inject] private IDialogService DialogService { get; set; }

        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, DisableBackdropClick = true, CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = true, };

        private List<BreadcrumbItem> _links;




        private User _user = new User();
        private User _usercopy = new User();
        private User? _AssignAuxSuperior = null;
        private User? selectedSeniorSupervisorOfList = null;
        private User? selectedSupervisorOfList = null;
        private User? selectedOperatorOfList = null;
        private Area? selectedAreaOfList = new Area();
        private Area Reassign = new Area();

        private List<User> Managers = new List<User>();
        private List<User> SeniorsSupervisors = new List<User>();
        private List<User> Supervisors = new List<User>();
        private List<User> Operators = new List<User>();

        private List<User> _ReassignedUsers = new List<User>();
        private List<User> _ReassignedUsersAreas = new List<User>();

        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();
        List<Area> _areasManager = new List<Area>();
        List<Distribution> _distributions = new();
        List<Group> _groups { get; set; } = new();

        private string json = string.Empty;
        public User LogedUser = new();

        private int UserUpdateOption = 0;
        //auxdistribution no null

        private int auxPlant = 0;
        private int auxArea = 0;
        private int auxDepa = 0;
        private int auxDistribution = 0;
        private int auxGroup = 0;

        private bool superior = false;
        private bool enableSave = false;

        //Var aux para controlar los switch de permisos
        private bool IsAdmin = false;
        private bool IsSeniorSupervisor = false;
        private bool IsSupervisor = false;
        private bool IsOperator = false;
        private bool IsManager = false;
        private bool IsLineSupport = false;
        private bool IsHeadCount = false;
        private bool IsRTC = false;

        //Var On/Off Button Add
        private bool ActiveAddArea = true;
        private bool ActiveAddSubordinated = true;

        //aux strings show info
        private string plantInfo = "First select a superior";
        private string areaInfo = "First select a superior";
        private string grupInfo = "First select a superior";

        private bool isOpen = false;
        private bool showui = false;
        private bool ReasignUsersView = false;

        private int ReasignUsersTypeView = 0;


        private bool ReasignUsersOptionsView = false;
        private bool ReasignUsersAreaOptionsView = false;
        private bool CompleteAreasView = false;

        private int optionSuboridinatesReasign = 0;
        private int optionAreasReasign = 0;

        //Seleccion de Opcion de reasignacion
        private TaskCompletionSource<bool> TaskComplete = new TaskCompletionSource<bool>();


        MudMessageBox ReasignUsersBox { get; set; }

        List<Department> _departments = new();
        public int departmentId = 0;

        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(text: @Localizer["home"], href: "/"),
            new BreadcrumbItem(text: @Localizer["configuration"], href: "/configuration"),
            new BreadcrumbItem(text: @Localizer["UsersTitle"], href: "/usersmanagement"),
            new BreadcrumbItem(text: @Localizer["UsersEdit"], href: "", disabled: true),
        };
            _user = await UsersServices.GetUserAndCollection(userId);
            _usercopy = ObjectCloner.ObjectCloner.DeepClone<User>(_user);

            showui = true;

            json = await JS.InvokeAsync<string>("localStorage.getItem", "user");
            LogedUser = JsonSerializer.Deserialize<User>(json) ?? new();

            auxDepa = _user.DepartmentId ?? 0;
            auxPlant = _user.PlantId != null ? (int)_user.PlantId : 0;
            auxArea = _user.AreaId != null ? (int)_user.AreaId : 0;
            auxGroup = _user.GroupId != null ? (int)_user.GroupId : 0;
            _departments = await DepartmentServices.GetDepartments();
            if (_user.UserType == 4 && _user.DepartmentId != 0 && _user.DepartmentId != null)
            {
                departmentId = (int)_user.DepartmentId;
            }

            _plants = await PlantServices.GetPlants();
            if (auxPlant != 0)
            {
                _areas = await AreaServices.GetAreas(auxPlant);
            }
            _groups = await GroupServices.GetGroups();

            IsAdmin = (_user.UserType == 1);
            IsSeniorSupervisor = (_user.UserType == 2);
            IsSupervisor = (_user.UserType == 3);
            IsOperator = (_user.UserType == 4);
            IsManager = (_user.UserType == 5);
            IsLineSupport = (_user.UserType == 6);
            IsHeadCount = (_user.UserType == 7);
            IsRTC = (_user.UserType == 8);

            selectedSeniorSupervisorOfList = new User();
            selectedSupervisorOfList = new User();
            selectedOperatorOfList = new User();
            selectedAreaOfList = new Area();

            Reassign.AreaId = -2;

            switch (_user.UserType)
            {
                case 2:
                    if (LogedUser.UserType == 1)
                    {
                        SeniorsSupervisors = await UsersServices.GetUsersByType(2, true, false);
                        Supervisors = await UsersServices.GetUsersByType(3, true, false);
                    }
                    else if (_user.PlantId != null && _user.PlantId != 0)
                    {
                        SeniorsSupervisors = await UsersServices.GetUsersByUserTypeInPlant((int)_user.PlantId, 2, true, false);
                        Supervisors = await UsersServices.GetUsersByUserTypeInPlant((int)_user.PlantId, 3, true, false);
                    }
                    break;
                case 4:
                    if (LogedUser.UserType == 1)
                    {
                        Supervisors = await UsersServices.GetUsersByType(3, true, false);
                    }
                    else
                    {
                        Supervisors = await UsersServices.GetUsersByUserTypeInPlant((int)_user.PlantId, 3, true, false);
                    }
                    break;
                case 3:
                    Supervisors = await UsersServices.GetUsersByType(3, true, false);
                    if (LogedUser.UserType == 1)
                    {
                        SeniorsSupervisors = await UsersServices.GetUsersByType(2, true, false);

                        if (_user.PlantId != null)
                        {
                            Operators = await UsersServices.GetUsersByUserTypeInPlant((int)_user.PlantId, 4, true, false);
                        }
                        else
                        {
                            Operators = await UsersServices.GetUsersByType(4, true, false);
                        }
                    }
                    else
                    {
                        SeniorsSupervisors = await UsersServices.GetUsersByUserTypeInPlant((int)_user.PlantId, 2, true, false);
                        if (_user.AreaId != null && _user.PlantId != null)
                        {
                            Operators = await UsersServices.GetUsersByUserTypeInPlantAndArea((int)_user.PlantId, (int)_user.AreaId, 4, true, false);
                        }
                        else if (_user.PlantId != null)
                        {
                            Operators = await UsersServices.GetUsersByUserTypeInPlant((int)_user.PlantId, 4, true, false);
                        }
                        else
                        {
                            Operators = await UsersServices.GetUsersByType(4, true, false);
                        }

                    }
                    break;
                case 5:
                    if (LogedUser.UserType == 1)
                    {
                        SeniorsSupervisors = await UsersServices.GetUsersByType(2, true, false);
                    }
                    else
                    {
                        SeniorsSupervisors = await UsersServices.GetUsersByUserTypeInPlant((int)_user.PlantId, 2, true, false);
                    }
                    break;

            }

            //Eliminar duplicados de la lista y al usuario en caso de ser necesario
            switch (_user.UserType)
            {
                //3
                case 1:
                case 5:
                    //admin o Gerente
                    foreach (User user in SeniorsSupervisors.ToList())
                    {
                        if (_user.Subordinates?.ToList().FindIndex(u => u.UserId == user.UserId) != -1)
                        {
                            SeniorsSupervisors.Remove(user);
                        }
                    }

                    break;
                //SSV
                case 2:
                    auxGroup = _user.GroupId != null ? (int)_user.GroupId : 0;

                    if (auxPlant != 0)
                    {
                        _areasManager?.Clear();
                        _areasManager = await AreaServices.GetAreas(auxPlant);

                        foreach (Area areaM in _areasManager.ToList())
                        {
                            if (_user.Areas?.ToList().FindIndex(a => a.AreaId == areaM.AreaId) != -1)
                            {
                                _areasManager.Remove(areaM);
                            }
                        }



                        foreach (User user in Supervisors.ToList())
                        {
                            if (_user.Subordinates?.ToList().FindIndex(u => u.UserId == user.UserId) != -1)
                            {
                                Supervisors.Remove(user);
                            }
                        }

                    }
                    break;
                //SV
                case 3:


                    if (_user.Superior != null)
                    {
                        superior = true;
                    }

                    plantInfo = _user.Plant?.Description;
                    areaInfo = _user.Area?.Description;
                    grupInfo = _user.Group?.Description;

                    foreach (User user in Operators.ToList())
                    {
                        if (_user.Subordinates?.ToList().FindIndex(u => u.UserId == user.UserId) != -1)
                        {
                            Operators.Remove(user);
                            Console.WriteLine($"{user.Name} - ID: {user.UserId} Type:{user.UserType}");

                        }
                    }
                    Console.WriteLine(Operators.Count);
                    break;
                //OPerador
                case 4:
                    auxPlant = _user.PlantId != null ? (int)_user.PlantId : 0;
                    auxArea = _user.AreaId != null ? (int)_user.AreaId : 0;
                    auxGroup = _user.GroupId != null ? (int)_user.GroupId : 0;
                    auxDistribution = _user.DistributionId != null ? (int)_user.DistributionId : 0;


                    if (_user.Superior != null)
                    {
                        superior = true;
                    }

                    plantInfo = _user.Plant?.Description;
                    areaInfo = _user.Area?.Description;
                    grupInfo = _user.Group?.Description;


                    if (auxPlant != 0 && auxArea != 0)
                    {
                        _distributions = await DistributionsServices.GetDistributions(auxPlant, auxArea);
                    }
                    break;
                case 6:
                case 7:
                    auxGroup = _user.GroupId != null ? (int)_user.GroupId : 0;

                    if (auxPlant != 0)
                    {
                        _areasManager?.Clear();
                        _areasManager = await AreaServices.GetAreas(auxPlant);

                        foreach (Area areaM in _areasManager.ToList())
                        {
                            if (_user.Areas?.ToList().FindIndex(a => a.AreaId == areaM.AreaId) != -1)
                            {
                                _areasManager.Remove(areaM);
                            }
                        }
                    }
                    break;

            }

            if (SeniorsSupervisors?.FindIndex(u => u.UserId == _user.UserId) != -1)
            {
                SeniorsSupervisors.RemoveAt(SeniorsSupervisors.FindIndex(u => u.UserId == _user.UserId));
            }

            if (Supervisors?.FindIndex(u => u.UserId == _user.UserId) != -1)
            {
                Supervisors.RemoveAt(Supervisors.FindIndex(u => u.UserId == _user.UserId));
            }

            if (Operators?.FindIndex(u => u.UserId == _user.UserId) != -1)
            {
                Operators.RemoveAt(Operators.FindIndex(u => u.UserId == _user.UserId));
            }

            if (Managers?.FindIndex(u => u.UserId == _user.UserId) != -1)
            {
                Managers.RemoveAt(Managers.FindIndex(u => u.UserId == _user.UserId));
            }

        }



        private async Task<IEnumerable<User>> SearchSSV(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            // await Task.Delay(1000);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return SeniorsSupervisors;

            return SeniorsSupervisors.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task<IEnumerable<User>> SearchSV(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            // await Task.Delay(1000);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return Supervisors;

            return Supervisors.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task<IEnumerable<User>> SearchOP(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            // await Task.Delay(1000);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return Operators;

            return Operators.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task<IEnumerable<Distribution>> SearchDistribution(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            // await Task.Delay(1000);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return _distributions;

            return _distributions.Where(x => x.Description.Contains(value, StringComparison.InvariantCultureIgnoreCase) || x.Code.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task<IEnumerable<User>> SearchManager(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            // await Task.Delay(1000);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return Managers;

            return Managers.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }



        private async Task setActiveUser(int userType)
        {
            var auxId = _user.UserId;
            var auxObjectId = _user.ObjectId;
            var auxName = _user.Name;
            var auxCreateDate = _user.CreatedDate;

            _user = new();

            _user.UserId = auxId;
            _user.ObjectId = auxObjectId;
            _user.Name = auxName;
            _user.CreatedDate = auxCreateDate;
            _user.IsActive = true;

            // resets de variables

            auxPlant = 0;
            auxArea = 0;
            auxGroup = 0;
            auxDistribution = 0;
            plantInfo = "";
            areaInfo = "";
            grupInfo = "";
            selectedSeniorSupervisorOfList = new User();
            selectedSupervisorOfList = new User();
            selectedOperatorOfList = new User();
            selectedAreaOfList = new Area();


            IsAdmin = userType == 1 && !IsAdmin;
            IsSeniorSupervisor = userType == 2 && !IsSeniorSupervisor;
            IsSupervisor = userType == 3 && !IsSupervisor;
            IsOperator = userType == 4 && !IsOperator;
            IsManager = userType == 5 && !IsManager;
            IsLineSupport = userType == 6 && !IsLineSupport;
            IsHeadCount = userType == 7 && !IsHeadCount;
            IsRTC = userType == 8 && !IsRTC;

            _user.UserType = !IsAdmin && !IsSeniorSupervisor && !IsSupervisor && !IsOperator && !IsManager && !IsLineSupport && !IsHeadCount && !IsRTC ? 0 : userType;


            ShowAreasInvokeByPlantSelector();

            switch (userType)
            {
                case 1: RemoveSuperiorUser(); break;
                case 2: RemoveSuperiorUser(); break;
                case 3:
                    if (LogedUser.UserType == 2)
                    {
                        _user.SuperiorId = LogedUser.UserId;
                        _user.Superior = LogedUser;
                        superior = true;
                        _user.PlantId = _user.Superior?.PlantId;
                        _user.GroupId = _user.Superior?.GroupId;

                        auxPlant = (int)_user.Superior?.PlantId;
                        auxGroup = (int)_user.Superior?.GroupId;

                        plantInfo = _user.Superior?.Plant?.Description;
                        grupInfo = _user.Superior?.Group?.Description;

                    }
                    else
                    {
                        auxPlant = 0;
                        auxArea = 0;
                        _user.GroupId = 0;
                    }
                    break;
                case 4:
                    if (LogedUser.UserType == 3)
                    {
                        auxPlant = (int)LogedUser.PlantId;
                        auxArea = (int)LogedUser.AreaId;
                        auxGroup = (int)LogedUser.GroupId;

                        _user.SuperiorId = LogedUser.UserId;
                        _user.Superior = LogedUser;
                        superior = true;

                        _user.PlantId = _user.Superior?.PlantId;
                        _user.AreaId = _user.Superior?.AreaId;
                        _user.GroupId = _user.Superior?.GroupId;

                        plantInfo = _user.Superior?.Plant?.Description;
                        areaInfo = _user.Superior?.Area?.Description;
                        grupInfo = _user.Superior?.Group?.Description;

                        if (auxPlant != 0 && auxArea != 0)
                        {
                            _distributions = await DistributionsServices.GetDistributions(auxPlant, auxArea);
                        }
                    }
                    else
                    {
                        auxPlant = 0;
                        auxArea = 0;
                        _user.GroupId = 0;
                        auxDepa = 0;
                    }
                    break;
                case 5: RemoveSuperiorUser(); break;
                case 6: RemoveSuperiorUser(); break;
                case 7: RemoveSuperiorUser(); break;

            }

            enableSave = false;
            StateHasChanged();
        }

        private async void ShowAreas()
        {
            _user.AreaId = 0;
            _areas = await AreaServices.GetAreas((int)_user.PlantId);
        }

        private async void ShowAreasInvokeByPlantSelector()
        {
            Managers.Clear();
            SeniorsSupervisors.Clear();
            Supervisors.Clear();
            Operators.Clear();

            switch (_user.UserType)
            {
                case 2:
                    if (LogedUser.UserType == 1)
                    {
                        Managers = await UsersServices.GetUsersByType(5, true, false);
                    }
                    else if (_user.PlantId != null && _user.PlantId != 0)
                    {
                        Managers = await UsersServices.GetUsersByUserTypeInPlant((int)_user.PlantId, 5, true, false);
                    }

                    if (LogedUser.UserType == 1)
                    {
                        Supervisors = await UsersServices.GetUsersByType(3, true, false);
                    }
                    else if (_user.PlantId != 0 && _user.PlantId != null)
                    {
                        Supervisors = await UsersServices.GetUsersByUserTypeInPlant((int)_user.PlantId, 3, true, false);
                    }

                    break;

                case 3:
                    if (LogedUser.UserType == 1)
                    {
                        SeniorsSupervisors = await UsersServices.GetUsersByType(2, true, false);
                        if (_user.AreaId != null && _user.PlantId != null && _user.AreaId != 0 && _user.PlantId != 0)
                        {
                            Operators = await UsersServices.GetUsersByUserTypeInPlantAndArea((int)_user.PlantId, (int)_user.AreaId, 4, true, false);
                        }
                        else if (_user.PlantId != null && _user.PlantId != 0)
                        {
                            Operators = await UsersServices.GetUsersByUserTypeInPlant((int)_user.PlantId, 4, true, false);
                        }
                        else
                        {
                            Operators = await UsersServices.GetUsersByType(4, true, false);
                        }
                    }
                    else
                    {
                        SeniorsSupervisors = await UsersServices.GetUsersByUserTypeInPlant((int)_user.PlantId, 2, true, false);
                        if (_user.AreaId != null && _user.PlantId != null && _user.AreaId != 0 && _user.PlantId != 0)
                        {
                            Operators = await UsersServices.GetUsersByUserTypeInPlantAndArea((int)_user.PlantId, (int)_user.AreaId, 4, true, false);
                        }
                        else if (_user.PlantId != null && _user.PlantId != 0)
                        {
                            Operators = await UsersServices.GetUsersByUserTypeInPlant((int)_user.PlantId, 4, true, false);
                        }
                        else
                        {
                            Operators = await UsersServices.GetUsersByType(4, true, false);
                        }
                    }
                    break;
                case 4:

                    if (LogedUser.UserType == 1)
                    {
                        Supervisors = await UsersServices.GetUsersByType(3, true, false);
                    }
                    else if (_user.PlantId != 0 && _user.PlantId != null)
                    {
                        Supervisors = await UsersServices.GetUsersByUserTypeInPlant((int)_user.PlantId, 3, true, false);
                    }
                    break;
                case 5:
                    if (LogedUser.UserType == 1)
                    {
                        SeniorsSupervisors = await UsersServices.GetUsersByType(2, true, false);
                    }
                    else if (_user.PlantId != null && _user.PlantId != 0)
                    {
                        SeniorsSupervisors = await UsersServices.GetUsersByUserTypeInPlant((int)_user.PlantId, 2, true, false);
                    }
                    else
                    {
                        SeniorsSupervisors = await UsersServices.GetUsersByType(2, true, false);
                    }
                    break;

            }

            if (SeniorsSupervisors.FindIndex(u => u.UserId == _user.UserId) != -1)
            {
                SeniorsSupervisors.RemoveAt(SeniorsSupervisors.FindIndex(u => u.UserId == _user.UserId));
            }

            if (Supervisors.FindIndex(u => u.UserId == _user.UserId) != -1)
            {
                Supervisors.RemoveAt(Supervisors.FindIndex(u => u.UserId == _user.UserId));
            }

            if (Operators.FindIndex(u => u.UserId == _user.UserId) != -1)
            {
                Operators.RemoveAt(Operators.FindIndex(u => u.UserId == _user.UserId));
            }

            if (Managers.FindIndex(u => u.UserId == _user.UserId) != -1)
            {
                Managers.RemoveAt(Managers.FindIndex(u => u.UserId == _user.UserId));
            }


            auxArea = 0;
            auxDistribution = 0;

            if (auxPlant != 0)
            {
                _user.Subordinates?.Clear();
                _user.Areas?.Clear();

                _areas.Clear();
                _areasManager?.Clear();
                _areas = await AreaServices.GetAreas(auxPlant);
                _areasManager = await AreaServices.GetAreas(auxPlant);
            }



            if (auxPlant != 0 && auxArea != 0)
            {
                ShowDistributionInvokeByAreasSelector();
            }

            StateHasChanged();
        }

        private async void ShowDistributionInvokeByAreasSelector()
        {
            _user.DistributionId = 0;

            if (_user.UserType == 3)
            {
                Operators?.Clear();

                if (LogedUser.UserType == 1)
                {
                    Operators = await UsersServices.GetUsersByUserTypeInPlant(auxPlant, 4, true, false);
                }
                else
                {
                    Operators = await UsersServices.GetUsersByUserTypeInPlantAndArea(auxPlant, auxArea, 4, true, false);
                }
            }



            if (auxPlant != 0 && auxArea != 0 && _user.UserType == 4)
            {
                _distributions = await DistributionsServices.GetDistributions(auxPlant, auxArea);
            }

            StateHasChanged();
        }




        private async void OnSelectedAreaToManageFunction()
        {
            if (selectedAreaOfList != new Area())
            {
                ActiveAddArea = false;

            }
            else
            {
                ActiveAddArea = true;
            }
        }

        private async void SetDistribution(Distribution element)
        {
            auxDistribution = element.DistributionId;
        }


        private async void OnSelectedSubordinatedFunction(User element, int type)
        {
            switch (type)
            {
                case 2:
                    //ssv
                    selectedSeniorSupervisorOfList = element;
                    break;
                case 3:
                    //SV
                    selectedSupervisorOfList = element;
                    break;
                case 4:
                    //SV
                    selectedOperatorOfList = element;
                    break;
            }

            if (selectedSupervisorOfList != new User() || selectedSeniorSupervisorOfList != new User() || selectedOperatorOfList != new User())
            {
                ActiveAddSubordinated = false;
            }
            else
            {
                ActiveAddSubordinated = true;
            }

        }





        private async Task SetSuperiorToUser(User ItemSelected)
        {
            _user.Superior = ItemSelected;

            if (_user.UserType != 1)
            {
                _user.SuperiorId = _user.Superior.SuperiorId;
                superior = true;
            }


            //Idemtificar user
            switch (_user.UserType)
            {
                case 2: break;
                case 3:
                    _user.PlantId = _user.Superior?.PlantId;
                    _user.GroupId = _user.Superior?.GroupId;

                    auxPlant = (int)_user.Superior?.PlantId;
                    auxGroup = (int)_user.Superior?.GroupId;

                    plantInfo = _user.Superior?.Plant?.Description;
                    grupInfo = _user.Superior?.Group?.Description;

                    break;
                case 4:
                    _user.PlantId = _user.Superior?.PlantId;
                    //agrega el area
                    _user.AreaId = _user.Superior?.AreaId;
                    _user.GroupId = _user.Superior?.GroupId;

                    auxPlant = (int)_user.Superior?.PlantId;
                    //agrega el area
                    auxArea = (int)_user.Superior?.AreaId;
                    auxGroup = (int)_user.Superior?.GroupId;

                    plantInfo = _user.Superior?.Plant?.Description;
                    //agrega el area
                    areaInfo = _user.Superior?.Area?.Description;
                    grupInfo = _user.Superior?.Group?.Description;

                    if (auxPlant != 0 && auxArea != 0)
                    {
                        _distributions = await DistributionsServices.GetDistributions(auxPlant, auxArea);
                    }
                    break;
            }




            StateHasChanged();
        }

        private void RemoveSuperiorUser()
        {
            auxDistribution = 0;
            _distributions = new();
            _user.SuperiorId = null;
            _user.Superior = null;
            superior = false;

            areaInfo = "";
            plantInfo = "";
            grupInfo = "";

            StateHasChanged();

        }

        private void DeleteAreaFromUserList(Area element)
        {
            _user.Areas.Remove(element);
            _areasManager.Add(element);
            StateHasChanged();
        }


        private void AddAreaToUserList(Area selection)
        {
            if (_user.Areas == null)
            {
                _user.Areas = new List<Area>();
            }

            if (selectedAreaOfList != null && !_user.Areas.Contains(selection))
            {
                _user.Areas.Add(selection);

                _areasManager.Remove(selection);
                //reset value from selector
                selectedAreaOfList = null;
                ActiveAddArea = true;
            }


            StateHasChanged();
        }


        private void DeleteSeniorSupervisorFromSubordinateList(User selection)
        {
            _user.Subordinates?.Remove(selection);
            Console.WriteLine($"Antes Senior {SeniorsSupervisors.Count}");
            SeniorsSupervisors.Add(selection);
            Console.WriteLine($"New Senior {SeniorsSupervisors.Count}");
            StateHasChanged();
        }

        private void AddSeniorSupervisorToSubordinateList(User selection)
        {
            if (_user.Subordinates == null)
            {
                _user.Subordinates = new List<User>();
            }


            if (selectedSeniorSupervisorOfList != null && !_user.Subordinates.Contains(selection))
            {
                _user.Subordinates.Add(selection);
                SeniorsSupervisors.Remove(selection);
                selectedSeniorSupervisorOfList = new();
                ActiveAddSubordinated = true;
            }

            StateHasChanged();
        }


        private void DeleteSupervisorFromSubordinateList(User selection)
        {
            _user.Subordinates?.Remove(selection);
            Supervisors.Add(selection);
            StateHasChanged();
        }

        private void AddSupervisorToSubordinateList(User selection)
        {
            if (_user.Subordinates == null)
            {
                _user.Subordinates = new List<User>();
            }


            if (selectedSupervisorOfList != null && !_user.Subordinates.Contains(selection))
            {



                _user.Subordinates.Add(selection);

                Supervisors.Remove(selection);

                selectedSupervisorOfList = null;
                ActiveAddSubordinated = true;
            }
            StateHasChanged();
        }

        private void DeleteOperatorFromSubordinateList(User selection)
        {
            _user.Subordinates.Remove(selection);
            Operators.Add(selection);
            StateHasChanged();
        }

        private void AddOperatorToSubordinateList(User selection)
        {
            if (_user.Subordinates == null)
            {
                _user.Subordinates = new List<User>();
            }


            if (selectedOperatorOfList != null && !_user.Subordinates.Contains(selection))
            {
                _user.Subordinates.Add(selection);

                Operators.Remove(selection);

                selectedOperatorOfList = null;
                ActiveAddSubordinated = true;
            }
            StateHasChanged();
        }




        private void AssignAuxSuperiorFunction(User NewSuperior)
        {
            _AssignAuxSuperior = NewSuperior;
        }

        private async void RemoveSelectedAreaContextItem(User ItemContext)
        {

            ItemContext.AreaId = null;
            ItemContext.Area = null;

            StateHasChanged();
        }

        private void updateSelectAreaContextItem(User ItemContext)
        {
            ItemContext.AreaId = ItemContext.Area?.AreaId;
            StateHasChanged();

        }


        public void SetReasignSubordinatesOption(int Opcion)
        {
            ReasignUsersOptionsView = false;

            optionSuboridinatesReasign = Opcion;
            // Cambiar la variable y notificar a la tarea que está esperando

            TaskComplete.TrySetResult(true);
        }

        public void SetReasignAreaOption(int Opcion)
        {
            ReasignUsersAreaOptionsView = false;

            optionAreasReasign = Opcion;
            // Cambiar la variable y notificar a la tarea que está esperando

            TaskComplete.TrySetResult(true);
        }

        private Task EsperarCambioVariableAsync()
        {
            // Devolver la tarea que se completará cuando la variable cambie
            return TaskComplete.Task;
        }


        private async void ReasignUsersDialog()
        {
            ReasignUsersView = false;


            TaskComplete.TrySetResult(true);

            // var result = await UsersServices.PromoveUserAndAssignNewSuperior(userId, _user, _usercopy, _AssignAuxSuperior.UserId);

            // if (result)
            // {
            //     await JS.InvokeVoidAsync("alert", "Succesful Update!"); // Alert
            //     NavigationManager.NavigateTo($"/usersmanagement/DetailUser/{_user.UserId}");
            // }
            // else
            //     await JS.InvokeVoidAsync("alert", "Fallo Actualizacion!"); // Alert

        }

        private async void SetAreasSave()
        {

            switch (ReasignUsersTypeView)
            {

                case 1:
                    //A OTRO SUPERVISOR

                    bool todosPertenecen = true;


                    if (_AssignAuxSuperior.UserType == 2)
                    {
                        foreach (var sub in _usercopy.Subordinates)
                        {
                            if (sub.AreaId != null)
                            {
                                int AreaSearch = (int)sub.AreaId;

                                if (!(_AssignAuxSuperior.Areas.Select(area => area.AreaId).ToList().Contains(AreaSearch)))
                                {
                                    todosPertenecen = false;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var sub in _usercopy.Subordinates)
                        {
                            if (sub.AreaId != _AssignAuxSuperior.AreaId)
                            {
                                todosPertenecen = false;
                                break;
                            }
                        }
                    }

                    if (!todosPertenecen)
                    {
                        // alerta de que aun esta incompleto

                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Asigna un area correspondiente!", Severity.Warning);

                    }
                    else
                    {
                        CompleteAreasView = false;
                        TaskComplete.TrySetResult(true);
                    }

                    break;

                case 2:
                    //Completar a super actual usando subordindados de _usercopy

                    bool todosPertenecenAlActual = true;


                    if (_user.UserType == 2)
                    {
                        foreach (var sub in _usercopy.Subordinates)
                        {
                            if (sub.AreaId != null)
                            {
                                int AreaSearch = (int)sub.AreaId;

                                if (!(_user.Areas.Select(area => area.AreaId).ToList().Contains(AreaSearch)))
                                {
                                    todosPertenecenAlActual = false;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var sub in _usercopy.Subordinates)
                        {
                            if (sub.AreaId != _user.AreaId)
                            {
                                todosPertenecenAlActual = false;
                                break;
                            }
                        }
                    }


                    if (!todosPertenecenAlActual)
                    {
                        // alerta de que aun esta incompleto

                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Asigna un area correspondiente!", Severity.Warning);

                    }
                    else
                    {
                        CompleteAreasView = false;
                        TaskComplete.TrySetResult(true);
                    }

                    break;

                case 3:

                    break;
            }

            //si todo esta ok se puede cerrar, si algo falla, se muestra un mensaje de alerta

            // var result = await UsersServices.PromoveUserAndAssignNewSuperior(userId, _user, _usercopy, _AssignAuxSuperior.UserId);

            // if (result)
            // {
            //     await JS.InvokeVoidAsync("alert", "Succesful Update!"); // Alert
            //     NavigationManager.NavigateTo($"/usersmanagement/DetailUser/{_user.UserId}");
            // }
            // else
            //     await JS.InvokeVoidAsync("alert", "Fallo Actualizacion!"); // Alert

        }

        private async void UpdateUserAsync()
        {
            enableSave = true;

            _user.IsActive = true;
            _user.LastUpdated = DateTime.Now;

            _user.PlantId = auxPlant;
            _user.AreaId = auxArea;

            _user.GroupId = auxGroup;

            if (_user.Superior != null)
            {
                _user.SuperiorId = _user.Superior.UserId;
            }

            if(departmentId != 0)
            {
                _user.DepartmentId = departmentId;
            }

            switch (_user.UserType)
            {
                case 1:

                    break;

                case 2:
                    if (_user.PlantId == 0)
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{@Localizer["UsersFirstPlant"]}!", Severity.Warning);
                        enableSave = false;

                        return;
                    }

                    if (_user.GroupId == 0)
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{@Localizer["UsersFirstGroup"]}!", Severity.Warning);
                        enableSave = false;

                        return;
                    }

                    if (_user.Areas?.Count == 0)
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{@Localizer["UsersFirstArea"]}!", Severity.Warning);
                        enableSave = false;

                        return;
                    }
                    break;

                case 3:

                    if (_user.Superior == null)
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{@Localizer["UsersFirstSuperior"]}", Severity.Warning);
                        enableSave = false;
                        return;
                    }

                    if (auxArea == 0)
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{@Localizer["UsersFirstArea"]}!", Severity.Warning);
                        enableSave = false;

                        return;
                    }
                    break;

                case 4:
                    _user.DistributionId = auxDistribution;
                    _user.ObjectId = null;
                    if (_user.Payroll.ToString() == null || _user.Payroll.ToString() == "")
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{@Localizer["UsersFirstPayroll"]}!", Severity.Warning);
                        enableSave = false;
                        return;
                    }
                    if (_user.Superior == null)
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{@Localizer["UsersFirstSuperior"]}", Severity.Warning);
                        enableSave = false;
                        return;
                    }
                    break;

                case 5:
                    if (_user.PlantId == 0)
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{@Localizer["UsersFirstPlant"]}!", Severity.Warning);
                        enableSave = false;

                        return;
                    }
                    break;

                case 6:

                    break;


            }

            switch (_usercopy.UserType)
            {
                case 5:
                    if (Managers?.Count == 0)
                    {
                        if (LogedUser.UserType == 1)
                        {
                            Managers = await UsersServices.GetUsersByType(5, true, false);
                        }
                        else if (_user.PlantId != null && _user.PlantId != 0)
                        {
                            Managers = await UsersServices.GetUsersByUserTypeInPlant((int)_user.PlantId, 5, true, false);
                        }
                        else
                        {
                            Managers = await UsersServices.GetUsersByType(5, true, false);
                        }
                    }
                    break;
                case 2:
                    if (SeniorsSupervisors?.Count == 0)
                    {
                        if (LogedUser.UserType == 1)
                        {
                            SeniorsSupervisors = await UsersServices.GetUsersByType(2, true, false);
                        }
                        else if (_user.PlantId != null && _user.PlantId != 0)
                        {
                            SeniorsSupervisors = await UsersServices.GetUsersByUserTypeInPlant((int)_user.PlantId, 2, true, false);
                        }
                        else
                        {
                            SeniorsSupervisors = await UsersServices.GetUsersByType(2, true, false);
                        }
                    }
                    break;
                case 3:
                    if (Supervisors?.Count == 0)
                    {
                        if (LogedUser.UserType == 1)
                        {
                            Supervisors = await UsersServices.GetUsersByType(3, true, false);
                        }
                        else if (_user.PlantId != 0 && _user.PlantId != null)
                        {
                            Supervisors = await UsersServices.GetUsersByUserTypeInPlant((int)_user.PlantId, 3, true, false);
                        }
                    }
                    break;
            }


            if (Supervisors.Any(u => u.UserId == _user.UserId))
            {
                Supervisors.Remove(Supervisors.Find(u => u.UserId == _user.UserId));
            }

            if (SeniorsSupervisors.Any(u => u.UserId == _user.UserId))
            {
                SeniorsSupervisors.Remove(SeniorsSupervisors.Find(u => u.UserId == _user.UserId));
            }

            if (Managers.Any(u => u.UserId == _user.UserId))
            {
                Managers.Remove(Managers.Find(u => u.UserId == _user.UserId));
            }

            //Valida si hay un asenso
            if (_usercopy.UserType == 5 && (_user.UserType == 1 || _user.UserType == 6) && _usercopy.Subordinates?.Count > 0)
            {
                // promoción de Tipo 5 Manager a Tipo 1 Admin / Tipo 6 Final Line

                ReasignUsersOptionsView = true;
                // Pregunta de Reasignacion
                await EsperarCambioVariableAsync();
                TaskComplete = new TaskCompletionSource<bool>();
            }
            else if (_usercopy.UserType == 2 && _user.UserType == 5 && _usercopy.Subordinates?.Count > 0)
            {
                //  promoción de Tipo 2 SSV a Tipo 5 Manager
                ReasignUsersOptionsView = true;

                // Pregunta de Reasignacion
                await EsperarCambioVariableAsync();
            }
            else if (_usercopy.UserType == 3 && _user.UserType == 2 && _usercopy.Subordinates?.Count > 0)
            {
                //  promoción de Tipo 3 SV a Tipo 2 SV
                ReasignUsersOptionsView = true;

                // Pregunta de Reasignacion
                await EsperarCambioVariableAsync();
                TaskComplete = new TaskCompletionSource<bool>();
            }
            else if (_usercopy.UserType != _user.UserType && _usercopy.Subordinates?.Count > 0)
            {
                //  Alguna promocion
                ReasignUsersOptionsView = true;

                // Pregunta de Reasignacion
                await EsperarCambioVariableAsync();
                TaskComplete = new TaskCompletionSource<bool>();
            }


        //@*1 Reasignar a otro Supervisor(Heredar Propiedades de Nuevo Supervisor)*@
        //@*2 Reasignar a otro Supervisor(Mantener Propiedades de Usuarios)*@
        //@*3 Mantener Subordinados(Asignar Nueva Area)*@
        //@*4 Mantener Subordinados(Sin Asignar Area, Mantener Area anterior)*@
        //@*5 No Asignar a nadie(Los deja al aire)*@

        switch (optionSuboridinatesReasign)
            {
                case 1:
                //@*1 Reasignar a otro Supervisor(Heredar Propiedades de Nuevo Supervisor)*@
                ReasignUsersTypeView = 1;
                    ReasignUsersView = true;
                    StateHasChanged();
                    await EsperarCambioVariableAsync();
                    TaskComplete = new TaskCompletionSource<bool>();

                    //Ordena los SUBS para el nuevo Jefe
                    if (_usercopy.UserType == 2)
                    {
                        ///unica area o selecionar manual.. manual es completar area

                        CompleteAreasView = true;
                        await EsperarCambioVariableAsync();
                        TaskComplete = new TaskCompletionSource<bool>();
                    }

                    //Guardo los SUBS EN OTRO Listado para trabajar individualmente con ellos
                    foreach (User user in _usercopy.Subordinates)
                    {

                        user.SuperiorId = _AssignAuxSuperior.UserId;
                        user.PlantId = _AssignAuxSuperior.PlantId;
                        user.GroupId = _AssignAuxSuperior.GroupId;

                        if (_user.UserType == 3)
                        {
                            user.AreaId = _AssignAuxSuperior.AreaId;
                        }


                        _ReassignedUsers?.Add(user);
                    }
                    break;
                case 2:
                    ReasignUsersTypeView = 1;
                    ReasignUsersView = true;
                    StateHasChanged();
                    await EsperarCambioVariableAsync();
                    TaskComplete = new TaskCompletionSource<bool>();
                //@*2 Reasignar a otro Supervisor(Mantener Propiedades de Usuarios)*@
                foreach (User user in _usercopy.Subordinates)
                    {
                        user.SuperiorId = _AssignAuxSuperior.UserId;
                        _ReassignedUsers?.Add(user);
                    }

                    break;

                case 3:
                //@*3 Mantener Subordinados(Asignar Area)*@
                if (_usercopy.Subordinates?.Count > 0)
                    {
                        ReasignUsersTypeView = 2;
                        //Completar areas Seleccion de area solamente si son diferentes a las nuevas areas en caso de ser un SSV
                        //Se compruba si el area de los antiguos suborinados esta contenida en las nuevas areas

                        if (_user.UserType == 2)
                        {
                            //Upgrade a SSV
                            //sub anteriores son compatibles con las areas actuales?
                            if (!_usercopy.Subordinates.All(s => _user.Areas.Select(u => u.AreaId).ToList().Contains(s.AreaId ?? 0)))
                            {
                                //preguntar si automatico o manual

                                CompleteAreasView = true;
                                await EsperarCambioVariableAsync();
                                TaskComplete = new TaskCompletionSource<bool>();
                            }

                        }



                        foreach (User user in _usercopy.Subordinates)
                        {

                            user.SuperiorId = _user.UserId;

                            if (_user.UserType != 5)
                            {
                                user.PlantId = _user.PlantId;
                                user.GroupId = _user.GroupId;
                            }


                            if (_user.UserType == 3)
                            {
                                user.AreaId = _user.AreaId;
                            }

                            if (_user.Subordinates == null)
                            {
                                _user.Subordinates = new List<User>();
                            }

                            _user.Subordinates?.Add(user);
                        }
                    }
                    break;

                case 4:
                //@*4 Mantener Subordinados(Sin Asignar Area)*@
                foreach (User user in _usercopy.Subordinates)
                    {
                        user.SuperiorId = _user.UserId;

                        if (_user.Subordinates == null)
                        {
                            _user.Subordinates = new List<User>();
                        }
                        _user.Subordinates?.Add(user);
                    }
                    break;

                case 5:
                //@*5 No Asignar a nadie(Los deja al aire)*@
                //eliminar subordinados
                break;
            }

            //Valida que pertenezcan al area
            if (_user.Subordinates?.Count > 0 && optionSuboridinatesReasign != 4 && _user.UserType != 5)
            {

                bool todosPertenecen = true;

                if (_user.UserType == 2)
                {
                    foreach (var sub in _user.Subordinates)
                    {
                        if (sub.AreaId != null)
                        {
                            int AreaSearch = (int)sub.AreaId;

                            if (!(_user.Areas.Select(area => area.AreaId).ToList().Contains(AreaSearch)))
                            {
                                todosPertenecen = false;
                                break;
                            }
                        }
                    }
                }
                else if (_user.UserType != 2 && _user.Subordinates?.Count > 0)
                {
                    if (_user.Subordinates.Any(u => u.AreaId != _user.AreaId))
                    {
                        todosPertenecen = false;
                    }
                }



                if (!todosPertenecen)
                {
                    if (_user.UserType == 2)
                    {
                        foreach (var sub in _user.Subordinates)
                        {
                            if (sub.AreaId != null)
                            {
                                int AreaSearch = (int)sub.AreaId;

                                if (!(_user.Areas.Select(area => area.AreaId).ToList().Contains(AreaSearch)))
                                {
                                    if (_ReassignedUsersAreas == null)
                                    {
                                        _ReassignedUsersAreas = new List<User>();
                                    }
                                    User auxUserSave = ObjectCloner.ObjectCloner.DeepClone(sub);
                                    _ReassignedUsersAreas?.Add(auxUserSave);

                                }
                            }
                        }
                    }
                    else if (_user.Subordinates?.Count > 0)
                    {

                        foreach (var sub in _user.Subordinates)
                        {
                            if (sub.AreaId != _user.AreaId)
                            {
                                if (_ReassignedUsersAreas == null)
                                {
                                    _ReassignedUsersAreas = new List<User>();
                                }
                                User auxUserSave = ObjectCloner.ObjectCloner.DeepClone(sub);
                                _ReassignedUsersAreas?.Add(auxUserSave);

                            }
                        }

                    }

                    foreach (var sub in _ReassignedUsersAreas)
                    {
                        var ustToRemove = _user.Subordinates?.ToList().Find(u => u.UserId == sub.UserId);
                        _user.Subordinates.Remove(ustToRemove);
                    }

                    // Preguntar que hacer
                    ReasignUsersAreaOptionsView = true;

                    await EsperarCambioVariableAsync();
                    TaskComplete = new TaskCompletionSource<bool>();
                }


            }

        //@*Areas *@

        //@*1 Reasignar a otro Supervisor(Heredar Propiedades de Nuevo Supervisor)*@
        //@*2 Reasignar a otro Supervisor(Mantener Propiedades de Usuarios)*@
        //@*3 Mantener Subordinados(Asignar Nueva Area)*@
        //@*4 Mantener Subordinados(Sin Asignar Area, Mantener Area anterior)*@
        //@*5 No Asignar a nadie(Los deja al aire)*@
        switch (optionAreasReasign)
            {
                case 1:
                //@*1 Reasignar a otro Supervisor(Heredar Propiedades de Nuevo Supervisor)*@
                ReasignUsersTypeView = 3;

                    Console.WriteLine($"Count Sv {_ReassignedUsersAreas.Count}");


                    ReasignUsersView = true;
                    await EsperarCambioVariableAsync();
                    TaskComplete = new TaskCompletionSource<bool>();

                    if (_usercopy.UserType == 2)
                    {
                        //aqui se pregunta si manual o automatico
                        //aun no se implementa la seleccion, por ahora es manual


                        //se verifica si ya es compatible para evitar abrir dialog inecesario
                        if (!_ReassignedUsersAreas.All(s => _AssignAuxSuperior.Areas.Select(u => u.AreaId).ToList().Contains(s.AreaId ?? 0)))
                        {
                            //preguntar si automatico o manual

                            CompleteAreasView = true;
                            await EsperarCambioVariableAsync();
                            TaskComplete = new TaskCompletionSource<bool>();
                        }
                    }


                    foreach (var user in _ReassignedUsersAreas)
                    {
                        user.SuperiorId = _AssignAuxSuperior.UserId;
                    }


                    break;
                case 2:
                //@*2 Reasignar a otro Supervisor(Mantener Propiedades de Usuarios)*@



                ReasignUsersView = true;
                    await EsperarCambioVariableAsync();
                    TaskComplete = new TaskCompletionSource<bool>();

                    foreach (var user in _ReassignedUsersAreas)
                    {
                        user.SuperiorId = _AssignAuxSuperior.UserId;
                    }

                    break;
                case 3:
                //@*3 Mantener Subordinados(Asignar Nueva Area)*@

                ReasignUsersTypeView = 4;
                    //Completar areas Seleccion de area

                    if (_user.UserType == 2)
                    {
                        //Upgrade a SSV
                        //sub anteriores son compatibles con las areas actuales?

                        CompleteAreasView = true;
                        await EsperarCambioVariableAsync();
                        TaskComplete = new TaskCompletionSource<bool>();


                    }
                    else
                    {
                        foreach (var usr in _ReassignedUsersAreas)
                        {
                            usr.AreaId = _user.AreaId;
                            if (_user.Subordinates == null)
                            {
                                _user.Subordinates = new List<User>();
                            }
                            _user.Subordinates?.Add(usr);
                        }
                    }




                    break;

                case 4:
                //@*4 Mantener Subordinados(Sin Asignar Area, Mantener Area anterior)*@
                //continua con la edicion, hay que considerar el update
                foreach (var usr in _ReassignedUsersAreas)
                    {
                        usr.AreaId = _user.AreaId;
                        if (_user.Subordinates == null)
                        {
                            _user.Subordinates = new List<User>();
                        }
                        _user.Subordinates?.Add(usr);
                    }
                    //no hacemos nada
                    break;


                case 5:
                //@*3 No Asignar a nadie(Quitarlos del listado)*@

                //eliminar subordinados
                break;
            }


            if (_ReassignedUsers?.Count > 0)
            {
                foreach (var usr in _ReassignedUsers)
                {
                    if (usr.AreaId == 0 || usr.AreaId == null && usr.Area != null)
                    {
                        usr.AreaId = usr.Area.AreaId;
                    }
                }

                Console.WriteLine("Reasign Por Upgrade");
                //reasginacion de subordinados
                var ResponseReassignUser = await UsersServices.ReassignNewSuperior(_ReassignedUsers, optionSuboridinatesReasign);

                if (ResponseReassignUser)
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Reassign Ok", Severity.Warning);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error Por Upgrade", Severity.Warning);
                }

            }

            if (_ReassignedUsersAreas?.Count > 0 && optionAreasReasign < 3)
            {
                foreach (var usr in _ReassignedUsersAreas)
                {
                    if (usr.AreaId == 0 || usr.AreaId == null && usr.Area != null)
                    {
                        usr.AreaId = usr.Area.AreaId;
                    }
                }
                //reasginacion de subordinados
                Console.WriteLine("Reasign Por Areas");

                var ResponseReassignAreasUser = await UsersServices.ReassignNewSuperior(_ReassignedUsersAreas, optionAreasReasign);

                if (ResponseReassignAreasUser)
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Reassign By Area Ok", Severity.Warning);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error Update by Area User", Severity.Warning);
                }

            }

            //Actualizacion de usautio
            if (_user.Subordinates?.Count > 0)
                foreach (var usr in _user.Subordinates)
                {
                    if (usr.AreaId == 0 || usr.AreaId == null && usr.Area != null)
                    {
                        usr.AreaId = usr.Area.AreaId;
                    }
                    usr.SuperiorId = _user.UserId;
                }

            _user.Department = null;
            _user.DepartmentId = auxDepa;

            var ResponseUpdateUser = await UsersServices.UpdateUser(userId, _user, optionAreasReasign);

            if (ResponseUpdateUser)
            {
                if (_user.UserId == LogedUser.UserId)
                {
                    LogedUser = await UserServices.GetUserByObjectIdWithCollections(LogedUser.ObjectId);
                    if (LogedUser != null)
                    {
                        json = JsonSerializer.Serialize<User>(LogedUser);
                        await JS.InvokeVoidAsync("localStorage.setItem", "user", json);
                    }
                }
                await JS.InvokeAsync<string>("alert", "Succesful Update!"); // Alert
                NavigationManager.NavigateTo($"/usersmanagement/DetailUser/{_user.UserId}");
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error Update User", Severity.Warning);
            }




            // if (_usercopy.UserType > _user.UserType || (_user.UserType == 5 && _usercopy.UserType != 4))
            // {

            // Console.WriteLine($"if Orginal{_user.UserType} Copia {_usercopy.UserType}");

            // if (_usercopy.Subordinates?.Count > 0)
            // {

            //     bool? result = await ReasignUsersBox.Show();
            //     bool state = result == null ? false : true;

            //     if (state)
            //     {
            //         ReasignUsersView = true;
            //         StateHasChanged();
            //     }
            //     else
            //     {
            //         var ResponseUpdateUser = await UsersServices.UpdateUser(userId, _user);

            //         if (ResponseUpdateUser)
            //         {
            //             if (_user.UserId == LogedUser.UserId)
            //             {
            //                 LogedUser = await UserServices.GetUserByObjectIdWithCollections(LogedUser.ObjectId);
            //                 if (LogedUser != null)
            //                 {
            //                     json = JsonSerializer.Serialize<User>(LogedUser);
            //                     await JS.InvokeVoidAsync("localStorage.setItem", "user", json);
            //                 }
            //             }
            //             await JS.InvokeVoidAsync("alert", "Succesful Update!"); // Alert
            //             NavigationManager.NavigateTo($"/usersmanagement/DetailUser/{_user.UserId}");
            //             //NavigationManager.NavigateTo("/usersmanagement");
            //         }
            //         else
            //             await JS.InvokeVoidAsync("alert", "Fallo Actualizacion!"); // Alert

            //     }


            // }
            // else
            // {
            //     var ResponseUpdateUser = await UsersServices.UpdateUser(userId, _user);

            //     if (ResponseUpdateUser)
            //     {
            //         if (_user.UserId == LogedUser.UserId)
            //         {
            //             LogedUser = await UserServices.GetUserByObjectIdWithCollections(LogedUser.ObjectId);
            //             if (LogedUser != null)
            //             {
            //                 json = JsonSerializer.Serialize<User>(LogedUser);
            //                 await JS.InvokeVoidAsync("localStorage.setItem", "user", json);
            //             }
            //         }
            //         await JS.InvokeVoidAsync("alert", "Succesful Update!"); // Alert
            //         NavigationManager.NavigateTo($"/usersmanagement/DetailUser/{_user.UserId}");
            //         //NavigationManager.NavigateTo("/usersmanagement");
            //     }
            //     else
            //         await JS.InvokeVoidAsync("alert", "Fallo Actualizacion!"); // Alert
            // }

            //     StateHasChanged();
            // }
            // else
            // {
            // Console.WriteLine($"Else Orginal{_user.UserType} Copia {_usercopy.UserType}");

            // if (_user.Subordinates?.Any(u => u.AreaId == -2) == true)
            // {
            //     Console.WriteLine($"Reasignacion");

            //     ReasignUsersView = true;
            //     StateHasChanged();
            // }
            // else
            // {
            //     Console.WriteLine($"Sin reasignar");

            //     //no hay reasignacion

            //     bool noareaFound = false;

            //     //falta verificar que tipo de usuario es para investigar si los subs se comparan con las areas o nel
            //     switch (_user.UserType)
            //     {
            //         case 2:
            //             if (_user.Subordinates?.Count > 0)
            //                 foreach (var sub in _user.Subordinates)
            //                 {
            //                     if (_user.Areas?.ToList().FindIndex(a => a.AreaId == sub.AreaId) == -1)
            //                     {
            //                         noareaFound = true;
            //                         break;
            //                     }
            //                 }
            //             break;
            //         case 3:
            //             if (_user.Subordinates?.Count > 0)
            //                 foreach (var sub in _user.Subordinates)
            //                 {
            //                     if (_user.AreaId != sub.AreaId)
            //                     {
            //                         noareaFound = true;
            //                         break;
            //                     }
            //                 }
            //             break;
            //         case 5:
            //             if (_user.Subordinates?.Count > 0)
            //                 foreach (var sub in _user.Subordinates)
            //                 {
            //                     if (_user.PlantId != sub.PlantId)
            //                     {
            //                         noareaFound = true;
            //                         break;
            //                     }
            //                 }
            //             break;

            //     }



            //     if (noareaFound)
            //     {
            //         Console.WriteLine($"Todo Ok, pero las areas difieren");

            //         bool? result = null;

            //         if (_user.UserType == 5)
            //         {
            //             result = await DialogService.ShowMessageBox(
            //             $"Planta difiere",
            //             (MarkupString)$"la planta del sub difiere",
            //             yesText: $"{Localizer["Users_SubDistArea_Continue"]}!", noText: $"{Localizer["UEPromoContinueAssign"]} SSV", cancelText: $"{Localizer["Users_SubDistArea_Edit"]}!");

            //         }
            //         else
            //         {
            //             result = await DialogService.ShowMessageBox(
            //             $"{Localizer["Users_SubDistArea_Title"]}",
            //             (MarkupString)$"{Localizer["Users_SubDistArea_Body"]}",
            //             yesText: $"{Localizer["Users_SubDistArea_Continue"]}!", noText: $"{Localizer["UEPromoContinueAssign"]} SSV", cancelText: $"{Localizer["Users_SubDistArea_Edit"]}!");

            //         }



            //         switch (result)
            //         {
            //             case null:
            //                 //continuar con la edicion
            //                 enableSave = false;
            //                 break;
            //             case true:
            //                 //continuar sin reasignar nada
            //                 var ResponseUpdateUser = await UsersServices.UpdateUser(userId, _user);

            //                 if (ResponseUpdateUser)
            //                 {
            //                     if (_user.UserId == LogedUser.UserId)
            //                     {
            //                         LogedUser = await UserServices.GetUserByObjectIdWithCollections(LogedUser.ObjectId);
            //                         if (LogedUser != null)
            //                         {
            //                             json = JsonSerializer.Serialize<User>(LogedUser);
            //                             await JS.InvokeVoidAsync("localStorage.setItem", "user", json);
            //                         }
            //                     }
            //                     await JS.InvokeVoidAsync("alert", "Succesful Update!"); // Alert
            //                     NavigationManager.NavigateTo($"/usersmanagement/DetailUser/{_user.UserId}");
            //                     //NavigationManager.NavigateTo("/usersmanagement");
            //                 }
            //                 else
            //                     await JS.InvokeVoidAsync("alert", "Fallo Actualizacion!"); // Alert
            //                 break;

            //             case false:
            //                 //reasinar a otro SV
            //                 ReasignUsersView = true;
            //                 break;
            //         }

            //         StateHasChanged();

            //     }
            //     else
            //     {
            //         //todo ok
            //         Console.WriteLine($"Ninguna area difiere");


            //         var ResponseUpdateUser = await UsersServices.UpdateUser(userId, _user);

            //         if (ResponseUpdateUser)
            //         {
            //             if (_user.UserId == LogedUser.UserId)
            //             {
            //                 LogedUser = await UserServices.GetUserByObjectIdWithCollections(LogedUser.ObjectId);
            //                 if (LogedUser != null)
            //                 {
            //                     json = JsonSerializer.Serialize<User>(LogedUser);
            //                     await JS.InvokeVoidAsync("localStorage.setItem", "user", json);
            //                 }
            //             }
            //             await JS.InvokeVoidAsync("alert", "Succesful Update!"); // Alert
            //                                                                     //NavigationManager.NavigateTo("/usersmanagement");
            //             NavigationManager.NavigateTo($"/usersmanagement/DetailUser/{_user.UserId}");
            //         }
            //         else
            //             await JS.InvokeVoidAsync("alert", "Fallo Actualizacion!"); // Alert
            //                                                                        //}
            //                                                                        //else
            //                                                                        //{
            //                                                                        //    enableSave = false;
            //                                                                        //}
            //     }

            // }

            // if (ReasignUsersView)
            // {

            //     switch (_usercopy.UserType)
            //     {

            //         case 2:
            //             if (SeniorsSupervisors?.Count == 0)
            //             {
            //                 if (LogedUser.UserType == 1)
            //                 {
            //                     SeniorsSupervisors = await UsersServices.GetUsersByType(2, true, false);
            //                 }
            //                 else if (_user.PlantId != null && _user.PlantId != 0)
            //                 {
            //                     SeniorsSupervisors = await UsersServices.GetUsersByUserTypeInPlant((int)_user.PlantId, 2, true, false);
            //                 }
            //                 else
            //                 {
            //                     SeniorsSupervisors = await UsersServices.GetUsersByType(2, true, false);
            //                 }
            //             }
            //             break;
            //         case 3:
            //             if (Supervisors?.Count == 0)
            //             {
            //                 if (LogedUser.UserType == 1)
            //                 {
            //                     Supervisors = await UsersServices.GetUsersByType(3, true, false);
            //                 }
            //                 else if (_user.PlantId != 0 && _user.PlantId != null)
            //                 {
            //                     Supervisors = await UsersServices.GetUsersByUserTypeInPlant((int)_user.PlantId, 3, true, false);
            //                 }
            //             }
            //             break;
            //     }


            //     if (Supervisors.Any(u => u.UserId == _user.UserId))
            //     {
            //         Supervisors.Remove(Supervisors.Find(u => u.UserId == _user.UserId));
            //     }

            //     if (SeniorsSupervisors.Any(u => u.UserId == _user.UserId))
            //     {
            //         SeniorsSupervisors.Remove(SeniorsSupervisors.Find(u => u.UserId == _user.UserId));
            //     }

            //     if (Managers.Any(u => u.UserId == _user.UserId))
            //     {
            //         Managers.Remove(Managers.Find(u => u.UserId == _user.UserId));
            //     }

            // }


            // }



            enableSave = false;
            StateHasChanged();
        }


        private async void UpdateUserAdvanceAsync()
        {
            switch (UserUpdateOption)
            {
                case 1:
                    break;
                case 2:
                    break;

                case 3:
                    break;

                case 4:
                    break;


                default:

                    break;
            }
        }
    }
}