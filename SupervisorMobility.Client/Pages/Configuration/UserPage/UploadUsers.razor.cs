using global::System;
using global::System.Collections.Generic;
using global::System.Linq;
using global::System.Threading.Tasks;
using global::Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using SupervisorMobility.Client;
using SupervisorMobility.Client.Shared;
using SupervisorMobility.Client.Services;
using SupervisorMobility.Client.Data.Resources;
using Microsoft.Extensions.Localization;
using BlazorCameraStreamer;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using System.Net.Http.Headers;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace SupervisorMobility.Client.Pages.Configuration.UserPage
{
    public partial class UploadUsers
    {
        private static string DefaultDragClass = "d-flex flex-column align-center justify-center relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full z-10";
        private string DragClass = DefaultDragClass;
        private List<FileToDisplay> fileNames = new List<FileToDisplay>();
        private class FileToDisplay
        {
            public string name { get; set; }
            public string ftype { get; set; }
            public string message { get; set; }
        }

        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>()
        {
            Color.Default,
            Color.Primary,
            Color.Secondary,
            Color.Success,
            Color.Info,
            Color.Default,
            Color.Primary,
            Color.Secondary,
            Color.Success,
            Color.Info
        };
        bool ShowLoading = true;
        private int uploadtype = 1;
        private string FileName;
        private bool isOkFile = false;
        private string ErrorMessageToDisplay;
        private string FileErrorMesage;
        private IBrowserFile? FileSource;
        private bool ToCreate = false;
        private bool onloadfile = false;
        private bool AreasManagerVisibleDialog = false;
        private bool showResume = false;
        private bool activeUpload = false;
        private bool showTableInUI = false;
        private List<string[]> DataInDocument = new List<string[]>();
        private List<User> dataToShowInTable = new();
        private FileUpload uploadResult = new();
        private UsersUploadResult commitUsersResult = new();
        private List<Plant> _plants = new List<Plant>();
        private List<List<Area>> _areas = new List<List<Area>>();
        private Dictionary<int, Dictionary<int, List<Distribution>>> _distributions = new Dictionary<int, Dictionary<int, List<Distribution>>>();
        private List<Data.Entities.Group> _groups = new List<Data.Entities.Group>();
        private List<User> _allUsers = new List<User>();
        private User selectedSSVOfList = new();
        private User selectedSupervisorOfList = new();
        //Dialog Manage Areas Variables
        private User AuxDialogUser = new();
        private User MasterUserSuperior = new();
        private Area selectedAreaOfList = new Area();
        private bool ActiveAddArea = true;
        private bool HaveSuperiors = false;
        List<Area> _areasManager = null;
        //confirmation message edit info
        MudMessageBox mbox { get; set; }

        //Logged User
        private string json = string.Empty;
        public User loggedUser = new();
        public bool logged = false;
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
            ShowLoading = true;
            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer1["home"], href: "/"),
                new BreadcrumbItem(text: Localizer1["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: Localizer1["UsersTitle"], href: "/usersmanagement"),
                new BreadcrumbItem(text: Localizer1["UsersUploadTitle"], href: "", disabled: true),
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
                if (loggedUser.UserType == 3)
                {
                    uploadtype = 5;
                }

                StateHasChanged();
                _plants = await PlantsServices.GetPlants();
                foreach (var plant in _plants)
                {
                    var areas = await AreasServices.GetAreas(plant.PlantId);
                    _areas.Add(areas);
                    var areaDistributions = new Dictionary<int, List<Distribution>>();
                    foreach (var area in areas)
                    {
                        var distributions = await DistributionServices.GetDistributions(plant.PlantId, area.AreaId);
                        areaDistributions.Add(area.AreaId, distributions);
                    }

                    _distributions.Add(plant.PlantId, areaDistributions);
                }

                _groups = await GroupServices.GetGroups();
                _allUsers = await UsersServices.GetUsers(true, true);
                selectedSSVOfList = null;
                selectedSupervisorOfList = null;
            }

            ShowLoading = false;
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
                json = await JS.InvokeAsync<string>("localStorage.getItem", "user");
                loggedUser = JsonSerializer.Deserialize<User>(json) ?? new();
            }

            return hasProperty;
        }

        private async Task<bool> HasPropertyAsync() => await JS.InvokeAsync<bool>("localStorage.hasOwnProperty", "user");
        private List<BreadcrumbItem> _links;
        string ClassForCards = "";
        private async void GetPanelPositionClass(MouseEventArgs e)
        {
            var clickY = e.ClientY;
            var screenHeight = await JS.InvokeAsync<int>("getWindowInnerHeight");
            if (clickY < screenHeight / 2)
            {
                ClassForCards = "onexpanded-down";
            }
            else
            {
                ClassForCards = "onexpanded-up";
            }

            StateHasChanged();
        }

        private async Task AssignType(int value)
        {
            ShowLoading = true;
            StateHasChanged();
            try
            {
                switch (value)
                {
                    case 1:
                        _allUsers = await UsersServices.GetUsers(true, false);
                        await JS.InvokeVoidAsync("console.log", "carga tdos los usuarios");
                        await JS.InvokeVoidAsync("console.log", _allUsers);
                        ShowLoading = false;
                        break;
                    case 2:
                        _allUsers = await UsersServices.GetUsersByType(3, true, false);
                        ShowLoading = false;
                        break;
                    case 3:
                        _allUsers = await UsersServices.GetUsersByType(2, true, false);
                        ShowLoading = false;
                        break;
                    case 4:
                        _allUsers = await UsersServices.GetUsersByType(2, true, false);
                        _allUsers.AddRange(await UsersServices.GetUsersByType(4, false, false));
                        ShowLoading = false;
                        break;
                    case 5:
                    case 6:
                        _allUsers = await UsersServices.GetUsersByType(3, true, false);
                        ShowLoading = false;
                        break;
                }

                uploadtype = value;
                await CancelFunction();
            }
            catch(Exception ex)
            {

            }
            finally
            {
            
            StateHasChanged();
            }
        }

        private DialogOptions dialogDeleteOptions = new()
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.ExtraSmall,
            FullWidth = true,
            Position = DialogPosition.TopCenter,
            DisableBackdropClick = true,
            CloseButton = true
        };
        private async Task<bool> OpenMessageConfirmation()
        {
            bool? result = await mbox.Show(dialogDeleteOptions);
            return result == null ? false : true;
        }

        private async Task UpdateSelectSuperiorContextItem(User ItemContext)
        {
            if (ItemContext.Distribution != null)
            {
                await JS.InvokeVoidAsync("console.log","que cono?");
                var areaIdOfDistribution = ItemContext.Distribution?.AreaId;
                ItemContext.Area = ItemContext.Superior?.Areas?.FirstOrDefault(a => a.AreaId == areaIdOfDistribution);
                ItemContext.AreaId = ItemContext.Area?.AreaId;
                
            }
            else
            {
                ItemContext.Area = ItemContext.Superior?.Area;
                ItemContext.AreaId = ItemContext.Superior?.AreaId;

            }

            ItemContext.SuperiorId = ItemContext.Superior?.UserId;
            ItemContext.PlantId = ItemContext.Superior?.PlantId;
            ItemContext.GroupId = ItemContext.Superior?.GroupId;
            ItemContext.Group = ItemContext.Superior?.Group;
            ItemContext.Plant = ItemContext.Superior?.Plant;

            await JS.InvokeVoidAsync("console.log", ItemContext.Plant, ItemContext.PlantId);

            if (uploadtype == 5)
            {
                if (_distributions[(int)ItemContext.PlantId][(int)ItemContext.AreaId].FindIndex(a => a.DistributionId == ItemContext.DistributionId) == -1)
                {
                   
                    ItemContext.Distribution = null;
                    ItemContext.DistributionId = null;
                }
            }

            StateHasChanged();
        }

        private void RemoveSelectedSuperiorContextItem(User ItemContext)
        {
            switch (uploadtype)
            {
                case 3:
                case 5:
                    ItemContext.Superior = null;
                    ItemContext.SuperiorId = null;
                    ItemContext.Plant = null;
                    ItemContext.PlantId = null;
                    ItemContext.Area = null;
                    ItemContext.AreaId = null;
                    ItemContext.Group = null;
                    ItemContext.GroupId = null;
                    ItemContext.DistributionId = null;
                    ItemContext.Distribution = null;
                    break;
            }

            StateHasChanged();
        }

        private void updateSelectDistributionContextItem(User ItemContext)
        {
            ItemContext.DistributionId = ItemContext.Distribution?.DistributionId;
        }

        private void RemoveSelectedDistributionContextItem(User ItemContext)
        {
            ItemContext.DistributionId = null;
            ItemContext.Distribution = null;
            StateHasChanged();
        }

        private async void updateSelectPlantContextItem(User ItemContext)
        {
            ItemContext.PlantId = ItemContext.Plant?.PlantId;
            switch (uploadtype)
            {
                case 2:
                    ItemContext.Areas = new List<Area>();
                    break;
                case 3:
                case 5:
                    bool result = await OpenMessageConfirmation();
                    if (result)
                    {
                        RemoveSelectedSuperiorContextItem(ItemContext);
                    }

                    break;
                case 4:
                case 6:
                    bool? resultMsg = await DialogService.ShowMessageBox("Information", "This field is inherited from the superior, it is necessary to change the supervisor.!", yesText: "Ok!");
                    StateHasChanged();
                    break;
                default:
                    ItemContext.PlantId = null;
                    ItemContext.Plant = null;
                    break;
            }

            StateHasChanged();
        }

        private async void RemoveSelectedPlantContextItem(User ItemContext)
        {
            switch (uploadtype)
            {
                case 3:
                case 5:
                    bool result = await OpenMessageConfirmation();
                    if (result)
                    {
                        RemoveSelectedSuperiorContextItem(ItemContext);
                    }

                    break;
                case 4:
                case 6:
                    bool? resultMsg = await DialogService.ShowMessageBox("Information", "This field is inherited from the superior, it is necessary to change the supervisor.!", yesText: "Ok!");
                    StateHasChanged();
                    break;
                default:
                    ItemContext.PlantId = null;
                    ItemContext.Plant = null;
                    break;
            }

            StateHasChanged();
        }

        private async void RemoveSelectedAreaContextItem(User ItemContext)
        {
            switch (uploadtype)
            {
                case 3:
                case 5:
                    bool result = await OpenMessageConfirmation();
                    if (result)
                    {
                        RemoveSelectedSuperiorContextItem(ItemContext);
                    }

                    break;
                case 4:
                case 6:
                    bool? resultMsg = await DialogService.ShowMessageBox("Information", "This field is inherited from the superior, it is necessary to change the supervisor.!", yesText: "Ok!");
                    StateHasChanged();
                    break;
                default:
                    ItemContext.AreaId = null;
                    ItemContext.Area = null;
                    break;
            }

            StateHasChanged();
        }

        private void updateSelectAreaContextItem(User ItemContext)
        {
            ItemContext.AreaId = ItemContext.Area?.AreaId;
        }

        private void updateSelectGroupContextItem(User ItemContext)
        {
            ItemContext.GroupId = ItemContext.Group?.GroupId;
            StateHasChanged();
        }

        private async void RemoveSelectedGroupContextItem(User ItemContext)
        {
            switch (uploadtype)
            {
                case 3:
                case 5:
                    bool result = await OpenMessageConfirmation();
                    if (result)
                    {
                        ItemContext.GroupId = ItemContext.Group?.GroupId;
                        RemoveSelectedSuperiorContextItem(ItemContext);
                    }

                    break;
                case 4:
                case 6:
                    bool? resultMsg = await DialogService.ShowMessageBox("Information", "This field is inherited from the superior, it is necessary to change the supervisor.!", yesText: "Ok!");
                    StateHasChanged();
                    break;
                default:
                    ItemContext.GroupId = null;
                    ItemContext.Group = null;
                    break;
            }

            StateHasChanged();
        }

        private void DeleteAreaFromContextItem(Area AreaItemContext, User ContextItemUser)
        {
            ContextItemUser.Areas.Remove(AreaItemContext);
            StateHasChanged();
        }

        private void DeleteAreaFromContextItemList(Area AreaItemContext)
        {
            AuxDialogUser.Areas.Remove(AreaItemContext);
            _areasManager.Add(AreaItemContext);
            StateHasChanged();
        }

        private void AddAreaToItemContextList(Area AreaItemContext)
        {
            if (AuxDialogUser.Areas == null)
            {
                AuxDialogUser.Areas = new List<Area>();
            }

            if (selectedAreaOfList != null && !AuxDialogUser.Areas.Contains(AreaItemContext))
            {
                AuxDialogUser.Areas.Add(AreaItemContext);
                _areasManager.Remove(AreaItemContext);
                //reset value from selector
                selectedAreaOfList = null;
                ActiveAddArea = true;
            }

            StateHasChanged();
        }

        private void OnSelectedAreaToManageFunction()
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

        private void UpdateSelectSSVforSupervisors()
        {
            MasterUserSuperior = selectedSSVOfList;
            foreach (User element in dataToShowInTable)
            {
                element.Superior = selectedSSVOfList;
                element.SuperiorId = selectedSSVOfList.UserId;
                element.PlantId = selectedSSVOfList.PlantId;
                element.Plant = selectedSSVOfList.Plant;
                element.GroupId = selectedSSVOfList.GroupId;
                element.Group = selectedSSVOfList.Group;
            }

            StateHasChanged();
        }

        private void UpdateSelectSupervisorForOperators()
        {
            MasterUserSuperior = selectedSupervisorOfList;
            Console.WriteLine($"Entra en funcion updatew supervisor");
            Console.WriteLine(_distributions[(int)MasterUserSuperior.PlantId][(int)MasterUserSuperior.Areas.FirstOrDefault().AreaId]);
            foreach (User element in dataToShowInTable)
            {
                element.Superior = selectedSupervisorOfList;
                element.SuperiorId = selectedSupervisorOfList.UserId;
                element.PlantId = selectedSupervisorOfList.PlantId;
                element.Plant = selectedSupervisorOfList.Plant;
                element.Area = selectedSupervisorOfList.Area;
                element.AreaId = selectedSupervisorOfList.AreaId;
                element.GroupId = selectedSupervisorOfList.GroupId;
                element.Group = selectedSupervisorOfList.Group;
                Console.WriteLine($"Plant: {element.PlantId} {element.Plant.Description}");
                Console.WriteLine(_distributions[(int)element.PlantId][(int)element.AreaId].FindIndex(a => a.DistributionId == element.DistributionId));
                if (element.PlantId != null && element.AreaId != null && uploadtype == 6)
                    if (_distributions[(int)element.PlantId][(int)element.AreaId].FindIndex(a => a.DistributionId == element.DistributionId) == -1)
                    {
                        element.Distribution = null;
                        element.DistributionId = null;
                    }
            }

            StateHasChanged();
        }

        private async void OnInputFileChanged(InputFileChangeEventArgs e)
        {
            onloadfile = true;
            await Clear();
            FileName = e.File.Name;
            System.Text.RegularExpressions.Regex fileExtentionExcel = new System.Text.RegularExpressions.Regex(@".+\.xls", System.Text.RegularExpressions.RegexOptions.Compiled);
            System.Text.RegularExpressions.Regex fileExtentionCSV = new System.Text.RegularExpressions.Regex(@".+\.csv", System.Text.RegularExpressions.RegexOptions.Compiled);
            if (!fileExtentionExcel.IsMatch(FileName) && !fileExtentionCSV.IsMatch(FileName))
            {
                fileNames.Add(new FileToDisplay() { name = FileName, ftype = "otro", message = "This not a XLSX/csv file " });
                FileErrorMesage = "This not a XLSX/CSV File";
                FileName = string.Empty;
                FileSource = null;
                ToCreate = false;
            }
            else
            {
                FileErrorMesage = string.Empty;
                FileSource = e.File;
                ToCreate = true;
                var fileContent = new StreamContent(FileSource.OpenReadStream(FileSource.Size));
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(FileSource.ContentType);
                MemoryStream ms = new MemoryStream();
                await fileContent.CopyToAsync(ms);
                var outputFileString = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            
                if (fileExtentionCSV.IsMatch(e.File.Name))
                {
                    foreach (var item in outputFileString.Split(Environment.NewLine))
                    {
                        var addtolist = SplitCSV(item.ToString());
                        if (!addtolist.All(string.IsNullOrWhiteSpace))
                        {
                            DataInDocument.Add(addtolist);
                        }
                    }

                    //DataInDocument.RemoveAt(1);
                    fileNames.Add(new FileToDisplay() { name = FileName, ftype = "csv" });
                }

                if (fileExtentionExcel.IsMatch(FileName))
                {
                    try
                    {
                        DataInDocument = await GetDataTableFromExcel(e.File);
                        isOkFile = true;
                        fileNames.Add(new FileToDisplay() { name = FileName, ftype = "xls" });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

                //foreach (var item in DataInDocument)
                //{
                //    Console.WriteLine($"Linea:");
                //    foreach (var element in item)
                //    {
                //        Console.Write($",{element}");
                //    }
                //    Console.WriteLine($" Fin Linea");
                //}

                switch (uploadtype)
                {
                    case 1:
                        string[] lineall = DataInDocument[0];
                        if (lineall[0] != "UserId")
                        {
                            isOkFile = false;
                            showTableInUI = false;
                            ErrorMessageToDisplay = "This document does not belong to this section. ";
                        }
                        else
                        {
                            DataInDocument.RemoveAt(0);
                            isOkFile = true;
                            dataToShowInTable = await CreateUsersAllArray(DataInDocument);
                        }

                        break;
                    case 2:
                        string[] lineSSV = DataInDocument[0];
                        if (lineSSV[0] != "UserId SSV")
                        {
                            isOkFile = false;
                            showTableInUI = false;
                            ErrorMessageToDisplay = "This document does not belong to this section. ";
                        }
                        else
                        {
                            DataInDocument.RemoveAt(0);
                            dataToShowInTable = await CreateUsersSSV(DataInDocument);
                            isOkFile = true;
                        }

                        break;
                    case 3:
                    case 4:
                        string[] lineSup = DataInDocument[0];
                        if (lineSup[0] != "UserId Supervisor")
                        {
                            isOkFile = false;
                            showTableInUI = false;
                            ErrorMessageToDisplay = "This document does not belong to this section. ";
                        }
                        else
                        {
                            isOkFile = true;
                            DataInDocument.RemoveAt(0);
                            dataToShowInTable = CreateUsersSupervisors(DataInDocument);
                            
                            if (uploadtype == 4)
                            {
                                foreach (User element in dataToShowInTable)
                                {
                                    element.Superior = selectedSSVOfList != null ? selectedSSVOfList : null;
                                    element.SuperiorId = selectedSSVOfList != null ? selectedSSVOfList.UserId : null;
                                    element.PlantId = selectedSSVOfList != null ? selectedSSVOfList.PlantId : null;
                                    element.Plant = selectedSSVOfList != null ? selectedSSVOfList.Plant : null;
                                    element.GroupId = selectedSSVOfList != null ? selectedSSVOfList.GroupId : null;
                                    element.Group = selectedSSVOfList != null ? selectedSSVOfList.Group : null;
                                }
                            }
                        }

                        break;
                    case 5:
                    case 6:
                        string[] lineOpe = DataInDocument[0];
                        if (lineOpe[0] != "UserId Operator")
                        {
                            isOkFile = false;
                            showTableInUI = false;
                            ErrorMessageToDisplay = "This document does not belong to this section. ";
                        }
                        else
                        {
                            isOkFile = true;
                            DataInDocument.RemoveAt(0);
                            await JS.InvokeVoidAsync("console.log", "Despues de crear los supervisores");
                            dataToShowInTable = CreateUsersOperators(DataInDocument);
                            if (uploadtype == 6)
                            {
                                foreach (User element in dataToShowInTable)
                                {
                                    element.Superior = selectedSupervisorOfList != null ? selectedSupervisorOfList : null;
                                    element.SuperiorId = selectedSupervisorOfList != null ? selectedSupervisorOfList.SuperiorId : null;
                                    element.Area = selectedSupervisorOfList != null ? selectedSupervisorOfList.Area : null;
                                    element.AreaId = selectedSupervisorOfList != null ? selectedSupervisorOfList.AreaId : null;
                                    element.PlantId = selectedSupervisorOfList != null ? selectedSupervisorOfList.PlantId : null;
                                    element.Plant = selectedSupervisorOfList != null ? selectedSupervisorOfList.Plant : null;
                                    element.GroupId = selectedSupervisorOfList != null ? selectedSupervisorOfList.GroupId : null;
                                    element.Group = selectedSupervisorOfList != null ? selectedSupervisorOfList.Group : null;
                                    if (element.PlantId != null && element.AreaId != null)
                                        if (_distributions[(int)element.PlantId][(int)element.AreaId].FindIndex(a => a.DistributionId == element.DistributionId) == -1)
                                        {
                                            element.Distribution = null;
                                            element.DistributionId = null;
                                        }
                                }
                            }
                        }

                        break;
                }

            }

                if (isOkFile)
                {
                    showTableInUI = true;
                    ErrorMessageToDisplay = "";
                }

            onloadfile = false;
            StateHasChanged();
        }

        public async Task<List<User>> CreateUsersAllArray(List<string[]> DataInDocument)
        {
            List<User> ListtoReturn = new List<User>();
            foreach (string[] row in DataInDocument)
            {
                await JS.InvokeVoidAsync("console.log", row);
                bool allEqual = row.All(item => item.Equals("�"));
                if (allEqual)
                {
                    // Todos los elementos son iguales a "�"
                    Console.WriteLine("Todos los elementos son iguales a '�'.");
                    break;
                }

                try
                {
                    await JS.InvokeVoidAsync("console.log", "entre a la creacion del usuario");
                    var ToInsertIntoList = new User();
                    try
                    {
                        ToInsertIntoList.UserType = row[5] != "�" ? int.Parse(row[5]) : -1;
                    }
                    catch (Exception ex)
                    {
                        Console.Write($"{ex.Message}");
                    }

                    if (ToInsertIntoList.UserType == -1)
                    {
                        showTableInUI = false;
                        isOkFile = false;
                        ErrorMessageToDisplay = "Document Incomplet Some Field User Type";
                        break;
                    }

                    switch (ToInsertIntoList.UserType)
                    {
                        case 1:
                            try
                            {
                                ToInsertIntoList.UserId = row[0] != "�" ? int.Parse(row[0]) : -1;
                                try
                                {
                                    ToInsertIntoList.ObjectId = row[1] != "�" ? row[1] : row[-403];
                                }
                                catch (Exception ex)
                                {
                                    isOkFile = false;
                                    showTableInUI = false;
                                    ErrorMessageToDisplay = "Document Incomplet Some Principal user@compasdcpcs.local For Admin";
                                    break;
                                }

                                try
                                {
                                    ToInsertIntoList.Name = row[3] != "�" ? row[3] : row[-403];
                                }
                                catch (Exception ex)
                                {
                                    isOkFile = false;
                                    showTableInUI = false;
                                    ErrorMessageToDisplay = "Document Incomplet Some Name  For Admin";
                                    break;
                                }

                                try
                                {
                                    ToInsertIntoList.Email = row[4] != "�" ? row[4] : row[-403];
                                }
                                catch (Exception ex)
                                {
                                    isOkFile = false;
                                    showTableInUI = false;
                                    ErrorMessageToDisplay = "Document Incomplet Some Email For Admin";
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                isOkFile = false;
                                Console.WriteLine($"Case 1 => Create All Users:{ex.Message}");
                                showTableInUI = false;
                                break;
                            }

                            break;
                        case 2:
                            try
                            {
                                ToInsertIntoList.UserId = row[0] != "�" ? int.Parse(row[0]) : -1;
                                try
                                {
                                    ToInsertIntoList.ObjectId = row[1] != "�" ? row[1] : row[-403];
                                }
                                catch (Exception ex)
                                {
                                    showTableInUI = false;
                                    ErrorMessageToDisplay = "Document Incomplet Some Principal user@compasdcpcs.local For SSV";
                                    isOkFile = false;
                                    break;
                                }

                                try
                                {
                                    ToInsertIntoList.Name = row[3] != "�" ? row[3] : row[-403];
                                }
                                catch (Exception ex)
                                {
                                    showTableInUI = false;
                                    ErrorMessageToDisplay = $"Document Incomplet Some Name For SSV With PrincipalName: {ToInsertIntoList.ObjectId}";
                                    isOkFile = false;
                                    break;
                                }

                                try
                                {
                                    ToInsertIntoList.Email = row[4] != "�" ? row[4] : row[-403];
                                }
                                catch (Exception ex)
                                {
                                    showTableInUI = false;
                                    ErrorMessageToDisplay = $"Document Incomplet Some Email For SSV with Name {ToInsertIntoList.Name}";
                                    isOkFile = false;
                                    break;
                                }

                                try
                                {
                                    if (row[7].Contains(','))
                                    {
                                        string[]? SplitedSubordinates = row[7] != "�" ? row[7].Split(',') : null;
                                        if (SplitedSubordinates != null)
                                        {
                                            if (ToInsertIntoList.Subordinates != null)
                                            {
                                                foreach (var item in SplitedSubordinates)
                                                {
                                                    if (_allUsers.FindIndex(u => u.UserId == int.Parse(item)) != -1)
                                                        ToInsertIntoList.Subordinates.Add(_allUsers.Find(u => u.UserId == int.Parse(item)));
                                                }
                                            }
                                            else
                                            {
                                                ToInsertIntoList.Subordinates = new List<User>();
                                                foreach (var item in SplitedSubordinates)
                                                {
                                                    if (_allUsers.FindIndex(u => u.UserId == int.Parse(item)) != -1)
                                                        ToInsertIntoList.Subordinates.Add(_allUsers.Find(u => u.UserId == int.Parse(item)));
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (row[7] != "�")
                                        {
                                            if (ToInsertIntoList.Subordinates != null)
                                            {
                                                if (_allUsers.FindIndex(u => u.UserId == int.Parse(row[7])) != -1)
                                                    ToInsertIntoList.Subordinates.Add(_allUsers.Find(u => u.UserId == int.Parse(row[7])));
                                            }
                                            else
                                            {
                                                ToInsertIntoList.Subordinates = new List<User>();
                                                if (_allUsers.FindIndex(u => u.UserId == int.Parse(row[7])) != -1)
                                                    ToInsertIntoList.Subordinates.Add(_allUsers.Find(u => u.UserId == int.Parse(row[7])));
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    showTableInUI = false;
                                    ErrorMessageToDisplay = $"Document Error, One or More Subirdinates ID's For SSV with Email{ToInsertIntoList.Email}";
                                    isOkFile = false;
                                    break;
                                }

                                try
                                {
                                    ToInsertIntoList.PlantId = row[8] != "�" ? int.Parse(row[8]) : int.Parse(row[-403]);
                                    try
                                    {
                                        ToInsertIntoList.Plant = _plants.Find(p => p.PlantId == ToInsertIntoList.PlantId);
                                    }
                                    catch (Exception ex)
                                    {
                                        ErrorMessageToDisplay = $"Document Incorrect Some Plant Id For SSV whit email {ToInsertIntoList.Email}";
                                        isOkFile = false;
                                        showTableInUI = false;
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    showTableInUI = false;
                                    ErrorMessageToDisplay = $"Document Incomplet Some Plant For SSV whit email {ToInsertIntoList.Email}";
                                    isOkFile = false;
                                    break;
                                }

                                //subordinados
                                try
                                {
                                    if (row[9].Contains(','))
                                    {
                                        string[]? SplitedAreas = row[9] != "�" ? row[9].Split(',') : null;
                                        if (SplitedAreas != null)
                                        {
                                            if (ToInsertIntoList.Areas != null)
                                            {
                                                foreach (var item in SplitedAreas)
                                                {
                                                    ToInsertIntoList.Areas.Add(_areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == int.Parse(item)));
                                                }
                                            }
                                            else
                                            {
                                                ToInsertIntoList.Areas = new List<Area>();
                                                foreach (var item in SplitedAreas)
                                                {
                                                    ToInsertIntoList.Areas.Add(_areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == int.Parse(item)));
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (row[9] != "�")
                                        {
                                            if (ToInsertIntoList.Areas != null)
                                            {
                                                ToInsertIntoList.Areas.Add(_areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == int.Parse(row[9])));
                                            }
                                            else
                                            {
                                                ToInsertIntoList.Areas = new List<Area>();
                                                ToInsertIntoList.Areas.Add(_areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == int.Parse(row[9])));
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    showTableInUI = false;
                                    ErrorMessageToDisplay = $"Document Incomplet, Areas For SSV with Email{ToInsertIntoList.Email}";
                                    isOkFile = false;
                                    break;
                                }

                                try
                                {
                                    ToInsertIntoList.GroupId = row[10] != "�" ? int.Parse(row[10]) : int.Parse(row[-403]);
                                    try
                                    {
                                        ToInsertIntoList.Group = _groups.Find(p => p.GroupId == ToInsertIntoList.GroupId);
                                    }
                                    catch (Exception ex)
                                    {
                                        ErrorMessageToDisplay = $"Document Incorrect Some Group Id For SSV whit email {ToInsertIntoList.Email}";
                                        isOkFile = false;
                                        showTableInUI = false;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    showTableInUI = false;
                                    isOkFile = false;
                                    ErrorMessageToDisplay = "Document Incomplet Some Group For SSV";
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Case 2 => Create All Users:{ex.Message}");
                                showTableInUI = false;
                                isOkFile = false;
                            }

                            break;
                        case 3:
                            try
                            {
                                ToInsertIntoList.UserId = row[0] != "�" ? int.Parse(row[0]) : -1;
                                try
                                {
                                    ToInsertIntoList.ObjectId = row[1] != "�" ? row[1] : row[-403];
                                }
                                catch (Exception ex)
                                {
                                    showTableInUI = false;
                                    ErrorMessageToDisplay = "Document Incomplet Some Principal user@compasdcpcs.local For Supervisor";
                                    isOkFile = false;
                                }

                                try
                                {
                                    ToInsertIntoList.Name = row[3] != "�" ? row[3] : row[-403];
                                }
                                catch (Exception ex)
                                {
                                    showTableInUI = false;
                                    ErrorMessageToDisplay = $"Document Incomplet Some Name For Supervisor With PrincipalName: {ToInsertIntoList.ObjectId}";
                                    isOkFile = false;
                                    break;
                                }

                                try
                                {
                                    ToInsertIntoList.Email = row[4] != "�" ? row[4] : row[-403];
                                }
                                catch (Exception ex)
                                {
                                    showTableInUI = false;
                                    ErrorMessageToDisplay = $"Document Incomplet Some Email For Supervisor with Name: {ToInsertIntoList.Name}";
                                    isOkFile = false;
                                    break;
                                }

                                try
                                {
                                    await JS.InvokeVoidAsync("console.log", ToInsertIntoList);
                                    await JS.InvokeVoidAsync("console.log", $"Superior Id Raw Value: {row[6]}");
                                    
                                    ToInsertIntoList.SuperiorId = row[6] != "�" ? int.Parse(row[6]) : int.Parse(row[-403]);
                                    try
                                    {
                                        ToInsertIntoList.Superior = _allUsers.Find(p => p.UserId == ToInsertIntoList.SuperiorId);
                                        if (ToInsertIntoList.Superior != null)
                                        {
                                            try
                                            {
                                                ToInsertIntoList.PlantId = ToInsertIntoList.Superior?.PlantId;
                                                ToInsertIntoList.Plant = ToInsertIntoList.Superior?.Plant;
                                            }
                                            catch (Exception ex)
                                            {
                                                ErrorMessageToDisplay = $"Incorrect document, Review Superior's information, the superior's plant id for SSV with email: {ToInsertIntoList.Email}";
                                                isOkFile = false;
                                                showTableInUI = false;
                                                break;
                                            }

                                            try
                                            {
                                                ToInsertIntoList.Group = ToInsertIntoList.Superior?.Group;
                                                ToInsertIntoList.GroupId = ToInsertIntoList.Superior?.GroupId;
                                            }
                                            catch (Exception ex)
                                            {
                                                ErrorMessageToDisplay = $"Incorrect document, Review Superior's information, the superior's Group id for SSV with email: {ToInsertIntoList.Email}";
                                                isOkFile = false;
                                                showTableInUI = false;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            ErrorMessageToDisplay = $"Incorrect document, the superior id for SSV with email: {ToInsertIntoList.Email}";
                                            isOkFile = false;
                                            showTableInUI = false;
                                            break;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ErrorMessageToDisplay = $"Incorrect document, the superior id for SSV with email: {ToInsertIntoList.Email}";
                                        isOkFile = false;
                                        showTableInUI = false;
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    showTableInUI = false;
                                    ErrorMessageToDisplay = $"Document Incomplet Some Superior For Supervisor whit email {ToInsertIntoList.Email}";
                                    isOkFile = false;
                                    break;
                                }

                                try
                                {
                                    if (row[7].Contains(','))
                                    {
                                        string[]? SplitedSubordinates = row[7] != "�" ? row[7].Split(',') : null;
                                        if (SplitedSubordinates != null)
                                        {
                                            if (ToInsertIntoList.Subordinates != null)
                                            {
                                                foreach (var item in SplitedSubordinates)
                                                {
                                                    if (_allUsers.FindIndex(u => u.UserId == int.Parse(item)) != -1)
                                                        ToInsertIntoList.Subordinates.Add(_allUsers.Find(u => u.UserId == int.Parse(item)));
                                                }
                                            }
                                            else
                                            {
                                                ToInsertIntoList.Subordinates = new List<User>();
                                                foreach (var item in SplitedSubordinates)
                                                {
                                                    if (_allUsers.FindIndex(u => u.UserId == int.Parse(item)) != -1)
                                                        ToInsertIntoList.Subordinates.Add(_allUsers.Find(u => u.UserId == int.Parse(item)));
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (row[7] != "�")
                                        {
                                            if (ToInsertIntoList.Subordinates != null)
                                            {
                                                if (_allUsers.FindIndex(u => u.UserId == int.Parse(row[7])) != -1)
                                                    ToInsertIntoList.Subordinates.Add(_allUsers.Find(u => u.UserId == int.Parse(row[7])));
                                            }
                                            else
                                            {
                                                ToInsertIntoList.Subordinates = new List<User>();
                                                if (_allUsers.FindIndex(u => u.UserId == int.Parse(row[7])) != -1)
                                                    ToInsertIntoList.Subordinates.Add(_allUsers.Find(u => u.UserId == int.Parse(row[7])));
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    showTableInUI = false;
                                    ErrorMessageToDisplay = $"Document Error, One or More Subirdinates ID's For SSV with Email{ToInsertIntoList.Email}";
                                    isOkFile = false;
                                    break;
                                }

                                try
                                {
                                    if (row[9].Contains(','))
                                    {
                                        string[]? SplitedAreas = row[9] != "�" ? row[9].Split(',') : null;
                                        if (SplitedAreas != null)
                                        {
                                            if (ToInsertIntoList.Areas != null)
                                            {
                                                foreach (var item in SplitedAreas)
                                                {
                                                    ToInsertIntoList.Areas.Add(_areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == int.Parse(item)));
                                                }
                                            }
                                            else
                                            {
                                                ToInsertIntoList.Areas = new List<Area>();
                                                foreach (var item in SplitedAreas)
                                                {
                                                    ToInsertIntoList.Areas.Add(_areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == int.Parse(item)));
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (row[9] != "�")
                                        {
                                            if (ToInsertIntoList.Areas != null)
                                            {
                                                ToInsertIntoList.Areas.Add(_areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == int.Parse(row[9])));
                                            }
                                            else
                                            {
                                                ToInsertIntoList.Areas = new List<Area>();
                                                ToInsertIntoList.Areas.Add(_areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == int.Parse(row[9])));
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    showTableInUI = false;
                                    ErrorMessageToDisplay = $"Document Incomplet, Areas For SV with Email{ToInsertIntoList.Email}";
                                    isOkFile = false;
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Case 3 => Create All Users:{ex.Message}");
                                showTableInUI = false;
                                isOkFile = false;
                                break;
                            }
                            await JS.InvokeVoidAsync("console.log", ToInsertIntoList);
                            ListtoReturn.Add(ToInsertIntoList);
                            break;
                        case 4:
                            try
                            {
                                ToInsertIntoList.UserId = row[0] != "�" ? int.Parse(row[0]) : -1;
                                try
                                {
                                    ToInsertIntoList.Payroll = row[2] != "�" ? int.Parse(row[2]) : int.Parse(row[-403]);
                                }
                                catch (Exception ex)
                                {
                                    showTableInUI = false;
                                    ErrorMessageToDisplay = $"Document Incomplet Some Payroll For Operator";
                                    isOkFile = false;
                                    break;
                                }

                                try
                                {
                                    ToInsertIntoList.Name = row[3] != "�" ? row[3] : row[-403];
                                }
                                catch (Exception ex)
                                {
                                    showTableInUI = false;
                                    ErrorMessageToDisplay = $"Document Incomplet Some Name For Operator With payroll: {ToInsertIntoList.Payroll}";
                                    isOkFile = false;
                                    break;
                                }

                                try
                                {
                                    ToInsertIntoList.SuperiorId = row[6] != "�" ? int.Parse(row[6]) : int.Parse(row[-403]);
                                    try
                                    {
                                       
                                        ToInsertIntoList.Superior = _allUsers.Find(p => p.UserId == ToInsertIntoList.SuperiorId);
                                        if (ToInsertIntoList.Superior != null)
                                        {
                                            try
                                            {
                                                ToInsertIntoList.PlantId = ToInsertIntoList.Superior?.PlantId;
                                                ToInsertIntoList.Plant = ToInsertIntoList.Superior?.Plant;
                                            }
                                            catch (Exception ex)
                                            {
                                                ErrorMessageToDisplay = $"Incorrect document, Review Superior's information, the superior's plant id for SSV with email: {ToInsertIntoList.Email}";
                                                isOkFile = false;
                                                showTableInUI = false;
                                                break;
                                            }

                                            try
                                            {
                                                ToInsertIntoList.AreaId = ToInsertIntoList.Superior?.Areas.FirstOrDefault().AreaId;
                                                ToInsertIntoList.Area = ToInsertIntoList.Superior?.Area;
                                            }
                                            catch (Exception ex)
                                            {
                                                ErrorMessageToDisplay = $"Incorrect document, Review Superior's information, the superior's area id for SSV with email: {ToInsertIntoList.Email}";
                                                isOkFile = false;
                                                showTableInUI = false;
                                                break;
                                            }

                                            try
                                            {
                                                ToInsertIntoList.Group = ToInsertIntoList.Superior?.Group;
                                                ToInsertIntoList.GroupId = ToInsertIntoList.Superior?.GroupId;
                                            }
                                            catch (Exception ex)
                                            {
                                                ErrorMessageToDisplay = $"Incorrect document, Review Superior's information, the superior's Group id for SSV with email: {ToInsertIntoList.Email}";
                                                isOkFile = false;
                                                showTableInUI = false;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            isOkFile = false;
                                            showTableInUI = false;
                                            ErrorMessageToDisplay = $"Document Incorrect, Superior not found For Operator whit Name {ToInsertIntoList.Name}";
                                            break;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        isOkFile = false;
                                        showTableInUI = false;
                                        ErrorMessageToDisplay = $"Document Incorrect, Superior not found For Operator whit Name {ToInsertIntoList.Name}";
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    showTableInUI = false;
                                    ErrorMessageToDisplay = $"Document Incorrect Some Superior Id For Operator whit Name {ToInsertIntoList.Name}";
                                    isOkFile = false;
                                    break;
                                }

                                try
                                {
                                    ToInsertIntoList.DistributionId = row[11] != "�" ? int.Parse(row[11]) : -1;
                                    if (ToInsertIntoList.DistributionId != -1)
                                    {
                                        try
                                        {
                                            ToInsertIntoList.Distribution = _distributions[(int)ToInsertIntoList.PlantId][(int)ToInsertIntoList.AreaId].Find(d => d.DistributionId == ToInsertIntoList.DistributionId);
                                        }
                                        catch (Exception ex)
                                        {
                                            isOkFile = false;
                                            showTableInUI = false;
                                            ErrorMessageToDisplay = $"Document Incorrect, Some DistributionId For Operator whit Name {ToInsertIntoList.Name}";
                                            break;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    isOkFile = false;
                                    showTableInUI = false;
                                    ErrorMessageToDisplay = $"Document Incorrect, Some Distribution Id not exist For Operator whit Name {ToInsertIntoList.Name}";
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Case 4 => Create All Users:{ex.Message}");
                                isOkFile = false;
                                showTableInUI = false;
                            }

                            //AREAS COLLECTION
                            try
                            {
                                if (row[10].Contains(','))
                                {
                                    string[]? SplitedAreas = row[10] != "�" ? row[10].Split(',') : null;
                                    if (SplitedAreas != null)
                                    {
                                        if (ToInsertIntoList.Areas != null)
                                        {
                                            foreach (var item in SplitedAreas)
                                            {
                                                ToInsertIntoList.Areas.Add(_areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == int.Parse(item)));
                                            }
                                        }
                                        else
                                        {
                                            ToInsertIntoList.Areas = new List<Area>();
                                            foreach (var item in SplitedAreas)
                                            {
                                                ToInsertIntoList.Areas.Add(_areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == int.Parse(item)));
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (row[10] != "�" && row[10] != "")
                                    {
                                        if (ToInsertIntoList.Areas != null)
                                        {
                                            ToInsertIntoList.Areas.Add(_areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == int.Parse(row[10])));
                                        }
                                        else
                                        {
                                            ToInsertIntoList.Areas = new List<Area>();
                                            ToInsertIntoList.Areas.Add(_areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == int.Parse(row[10])));
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                showTableInUI = false;
                                ErrorMessageToDisplay = $"Document Incomplet, Areas For Operator with Name {ToInsertIntoList.Name}";
                                isOkFile = false;
                                break;
                            }

                            break;
                    }

                    if (!showTableInUI)
                    {
                        break;
                    }
                    await JS.InvokeVoidAsync("console.log", ToInsertIntoList);
                    ListtoReturn.Add(ToInsertIntoList);
                   
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                    isOkFile = false;
                    showTableInUI = false;
                }
            }

            Console.WriteLine(ErrorMessageToDisplay);
            return ListtoReturn;
        }

        public async Task<List<User>> CreateUsersSSV(List<string[]> DataInDocument)
        {
            List<User> ListtoReturn = new List<User>();
            foreach (string[] row in DataInDocument)
            {

                try
                {
                    var ToInsertIntoList = new User();
                    ToInsertIntoList.UserId = row[0] != "�" ? int.Parse(row[0]) : -1;
                    try
                    {
                        ToInsertIntoList.ObjectId = row[6] != "�" ? row[6] : "";
                    }
                    catch (Exception ex)
                    {
                        showTableInUI = false;
                        ErrorMessageToDisplay = "Document Incomplet Some Principal user@compasdcpcs.local For SSV";
                        isOkFile = false;
                        break;
                    }

                    try
                    {
                        ToInsertIntoList.Name = row[1] != "�" ? row[1] : row[-403];
                    }
                    catch (Exception ex)
                    {
                        showTableInUI = false;
                        ErrorMessageToDisplay = $"Document Incomplet Some Name For SSV With PrincipalName: {ToInsertIntoList.ObjectId}";
                        isOkFile = false;
                        break;
                    }

                    try
                    {
                        ToInsertIntoList.Email = row[2] != "�" ? row[2] : "";
                    }
                    catch (Exception ex)
                    {
                    //showTableInUI = false;
                    //ErrorMessageToDisplay = $"Document Incomplet Some Email For SSV with Name {ToInsertIntoList.Name}";
                    //isOkFile = false;
                    //break;
                    }

                    try
                    {
                        ToInsertIntoList.PlantId = row[3] != "�" ? int.Parse(row[3]) : -1;
                        if (ToInsertIntoList.PlantId != -1)
                        {
                            try
                            {
                                ToInsertIntoList.Plant = _plants.Find(p => p.PlantId == ToInsertIntoList.PlantId);
                            }
                            catch (Exception ex)
                            {
                                ErrorMessageToDisplay = $"Document Incorrect ID Plant For SSV whit Name {ToInsertIntoList.Name}";
                                isOkFile = false;
                                showTableInUI = false;
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        showTableInUI = false;
                        ErrorMessageToDisplay = $"Document Incomplet Some Plant ID For SSV whit Name {ToInsertIntoList.Name}";
                        isOkFile = false;
                        break;
                    }

                    //AREAS
                    try
                    {
                        if (row[5].Contains(','))
                        {
                            string[]? SplitedAreas = row[5] != "�" ? row[5].Split(',') : null;
                            if (SplitedAreas != null)
                            {
                                if (ToInsertIntoList.Areas != null)
                                {
                                    foreach (var item in SplitedAreas)
                                    {
                                        ToInsertIntoList.Areas.Add(_areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == int.Parse(item)));
                                    }
                                }
                                else
                                {
                                    ToInsertIntoList.Areas = new List<Area>();
                                    foreach (var item in SplitedAreas)
                                    {
                                        ToInsertIntoList.Areas.Add(_areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == int.Parse(item)));
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (row[5] != "�")
                            {
                                if (ToInsertIntoList.Areas != null)
                                {
                                    ToInsertIntoList.Areas.Add(_areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == int.Parse(row[5])));
                                }
                                else
                                {
                                    ToInsertIntoList.Areas = new List<Area>();
                                    ToInsertIntoList.Areas.Add(_areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == int.Parse(row[5])));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        showTableInUI = false;
                        ErrorMessageToDisplay = $"Document Incomplet/FormatError, Areas For SSV with Email{ToInsertIntoList.Email}";
                        isOkFile = false;
                        break;
                    }

                    try
                    {
                        ToInsertIntoList.GroupId = row[4] != "�" ? int.Parse(row[4]) : -1;
                        if (ToInsertIntoList.GroupId != -1)
                        {
                            try
                            {
                                ToInsertIntoList.Group = _groups.Find(p => p.GroupId == ToInsertIntoList.GroupId);
                            }
                            catch (Exception ex)
                            {
                                ErrorMessageToDisplay = $"Document Incorrect Some Group Id For SSV whit email {ToInsertIntoList.Email}";
                                isOkFile = false;
                                showTableInUI = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        showTableInUI = false;
                        isOkFile = false;
                        ErrorMessageToDisplay = $"Document Incomplet/FormatError Some Group For SSV whit name {ToInsertIntoList.Name}";
                    }

                    ////////////////////////////
                    await JS.InvokeVoidAsync("console.log", ToInsertIntoList);
                    ListtoReturn.Add(ToInsertIntoList);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"User Id:{ex.Message}");
                    showTableInUI = false;
                    isOkFile = false;
                    Console.WriteLine($"{ex.Message}");
                    showTableInUI = false;
                }


            } //end for

            return ListtoReturn;
        }

        public List<User> CreateUsersSupervisors(List<string[]> DataInDocument)
        {
            List<User> ListtoReturn = new List<User>();
            foreach (string[] row in DataInDocument)
            {
                try
                {
                    var ToInsertIntoList = new User();
                    ToInsertIntoList.UserId = row[0] != "�" ? int.Parse(row[0]) : -1;
                    ToInsertIntoList.UserType = 3; // Supervisor
                    ToInsertIntoList.Name = row[1] != "�" ? row[1] : "";
                    ToInsertIntoList.Email = row[2] != "�" ? row[2] : "";
                    ToInsertIntoList.SuperiorId = row[3] != "�" ? int.Parse(row[3]) : -1;
                    ToInsertIntoList.ObjectId = row[5] != "�" ? row[5] : "";
                    try
                    {
                        if (ToInsertIntoList.SuperiorId != -1)
                        {
                            ToInsertIntoList.Superior = _allUsers.Find(u => u.UserId == ToInsertIntoList.SuperiorId);
                            ToInsertIntoList.PlantId = ToInsertIntoList.Superior?.PlantId;
                            ToInsertIntoList.GroupId = ToInsertIntoList.Superior?.GroupId;
                            ToInsertIntoList.Plant = _plants.Find(e => e.PlantId == ToInsertIntoList.PlantId);
                            ToInsertIntoList.Group = _groups.Find(e => e.GroupId == ToInsertIntoList.GroupId);
                        }
                        else
                        {
                            ToInsertIntoList.Superior = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error in try to assign info from supperior");
                    }

                    //AREAS COLLECTION (row[4])
                    try
                    {
                        if (row[4] != "�" && row[4] != "" && row[4] != "-1")
                        {
                            if (row[4].Contains(','))
                            {
                                string[]? SplitedAreas = row[4].Split(',');
                                if (SplitedAreas != null)
                                {
                                    if (ToInsertIntoList.Areas != null)
                                    {
                                        foreach (var item in SplitedAreas)
                                        {
                                            ToInsertIntoList.Areas.Add(_areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == int.Parse(item)));
                                        }
                                    }
                                    else
                                    {
                                        ToInsertIntoList.Areas = new List<Area>();
                                        foreach (var item in SplitedAreas)
                                        {
                                            ToInsertIntoList.Areas.Add(_areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == int.Parse(item)));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (ToInsertIntoList.Areas != null)
                                {
                                    ToInsertIntoList.Areas.Add(_areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == int.Parse(row[4])));
                                }
                                else
                                {
                                    ToInsertIntoList.Areas = new List<Area>();
                                    ToInsertIntoList.Areas.Add(_areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == int.Parse(row[4])));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error in try to assign areas");
                    }

                    ListtoReturn.Add(ToInsertIntoList);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                    showTableInUI = false;
                }
            }

            return ListtoReturn;
        }

        public List<User> CreateUsersOperators(List<string[]> DataInDocument)
        {
            List<User> ListtoReturn = new List<User>();
            foreach (string[] row in DataInDocument)
            {
                //try
                //{
                var ToInsertIntoList = new User();
                ToInsertIntoList.UserId = row[0] != "�" ? int.Parse(row[0]) : -1;
                ToInsertIntoList.UserType = 4; // Operator
                ToInsertIntoList.Payroll = row[1] != "�" ? int.Parse(row[1]) : -1;
                ToInsertIntoList.Name = row[2] != "�" ? row[2] : "";
                ToInsertIntoList.DistributionId = row[3] != "�" ? int.Parse(row[3]) : -1;
                ToInsertIntoList.SuperiorId = row[4] != "�" ? int.Parse(row[4]) : -1;
                try
                {
                    if (ToInsertIntoList.SuperiorId != -1)
                    {
                        ToInsertIntoList.Superior = _allUsers.Find(u => u.UserId == ToInsertIntoList.SuperiorId);
                        ToInsertIntoList.PlantId = ToInsertIntoList.Superior?.PlantId;
                        ToInsertIntoList.AreaId = ToInsertIntoList.Superior?.AreaId;
                        ToInsertIntoList.GroupId = ToInsertIntoList.Superior?.GroupId;
                        ToInsertIntoList.Plant = _plants.Find(e => e.PlantId == ToInsertIntoList.PlantId);
                        ToInsertIntoList.Group = _groups.Find(e => e.GroupId == ToInsertIntoList.GroupId);
                        ToInsertIntoList.Area = _areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == ToInsertIntoList.AreaId);
                    }
                    else
                    {
                        ToInsertIntoList.Superior = null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in try to assign info from supperior");
                }

                try
                {
                    if (ToInsertIntoList.PlantId != null && ToInsertIntoList.AreaId != null)
                    {
                        if (ToInsertIntoList.DistributionId != -1)
                        {
                            ToInsertIntoList.Distribution = _distributions[(int)ToInsertIntoList.PlantId][(int)ToInsertIntoList.AreaId].FirstOrDefault(d => d.DistributionId == ToInsertIntoList.DistributionId);
                        }
                    }
                    else
                    {
                        foreach (var plant in _distributions)
                        {
                            foreach (var area in plant.Value)
                            {
                                foreach (var dist in area.Value)
                                {
                                    if (dist.DistributionId == ToInsertIntoList.DistributionId)
                                    {
                                        ToInsertIntoList.Distribution = dist;
                                        break;
                                    }
                                }

                                if (ToInsertIntoList.Distribution != null)
                                    break;
                            }

                            if (ToInsertIntoList.Distribution != null)
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in try to assign Distribution");
                }

                //AREAS COLLECTION
                try
                {
                    if (row.Length > 5 && row[5] != null && row[5] != "�" && row[5] != "")
                    {
                        if (row[5].Contains(','))
                        {
                            string[]? SplitedAreas = row[5].Split(',');
                            if (SplitedAreas != null)
                            {
                                if (ToInsertIntoList.Areas != null)
                                {
                                    foreach (var item in SplitedAreas)
                                    {
                                        ToInsertIntoList.Areas.Add(_areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == int.Parse(item)));
                                    }
                                }
                                else
                                {
                                    ToInsertIntoList.Areas = new List<Area>();
                                    foreach (var item in SplitedAreas)
                                    {
                                        ToInsertIntoList.Areas.Add(_areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == int.Parse(item)));
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (ToInsertIntoList.Areas != null)
                            {
                                ToInsertIntoList.Areas.Add(_areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == int.Parse(row[5])));
                            }
                            else
                            {
                                ToInsertIntoList.Areas = new List<Area>();
                                ToInsertIntoList.Areas.Add(_areas[_plants.FindIndex(e => e.PlantId == ToInsertIntoList.PlantId)].Find(a => a.AreaId == int.Parse(row[5])));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in try to assign Areas collection");
                }

                ListtoReturn.Add(ToInsertIntoList);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($" general exepction: {ex.Message}");
            //    showTableInUI = false;
            //}
            }

            return ListtoReturn;
        }

        public static async Task<List<string[]>> GetDataTableFromExcel(IBrowserFile file)
        {
            List<string[]> DataToReturn = new List<string[]>();
            List<string> RowsInFile = new List<string>();
            using (MemoryStream memStream = new MemoryStream())
            {
                await file.OpenReadStream(file.Size).CopyToAsync(memStream);
                using (XLWorkbook workBook = new XLWorkbook(memStream, XLEventTracking.Disabled))
                {
                    //Read the first Sheet from Excel file.
                    IXLWorksheet workSheet = workBook.Worksheet(1);
                    //Loop through the Worksheet rows.
                    foreach (IXLRow row in workSheet.Rows())
                    {
                        if (!row.IsEmpty())
                        {
                            RowsInFile.Clear();
                            foreach (IXLCell cell in row.Cells(1, 13))
                            {
                                string toinsert = "�";
                                // Verificar si la celda no est� vac�a antes de obtener su valor
                                if (!cell.IsEmpty())
                                {
                                    toinsert = cell.Value.ToString();
                                }

                                RowsInFile.Add(toinsert);
                            }

                            DataToReturn.Add(RowsInFile.ToArray());
                        }
                    }
                }
            }

            return DataToReturn;
        }

        private string[] SplitCSV(string input)
        {
            //Excludes commas within quotes
            System.Text.RegularExpressions.Regex csvSplit = new System.Text.RegularExpressions.Regex("(?:^|,)(\"(?:[^\"]+|\"\")*\"|[^,]*)", System.Text.RegularExpressions.RegexOptions.Compiled);
            List<string> list = new List<string>();
            string curr = string.Empty;
            foreach (System.Text.RegularExpressions.Match match in csvSplit.Matches(input))
            {
                curr = match.Value;
                if (0 == curr.Length)
                    list.Add("");
                list.Add(curr.TrimStart(','));
            }

            list.RemoveAt(0);
            return list.ToArray();
        }

        private async Task CancelFunction()
        {
            FileName = string.Empty;
            ErrorMessageToDisplay = string.Empty;
            DataInDocument.Clear();
            dataToShowInTable.Clear();
            showTableInUI = false;
            FileSource = null;
            await Clear();
        }

        private async Task Clear()
        {
            fileNames.Clear();
            dataToShowInTable.Clear();
            ClearDragClass();
            activeUpload = false;
            showResume = false;
            showTableInUI = false;
            DataInDocument.Clear();
            await Task.Delay(100);
        }

        private void SetDragClass()
        {
            DragClass = $"{DefaultDragClass} mud-border-primary";
        }

        private void ClearDragClass()
        {
            DragClass = DefaultDragClass;
        }

        async void UploadFunction()
        {
            activeUpload = true;
            using var content = new MultipartFormDataContent();
            if (FileSource is not null)
            {
                foreach (var item in dataToShowInTable)
                {
                    item.CreatedDate = DateTime.Now;
                    item.LastUpdated = DateTime.Now;
                    item.IsActive = true;
                    switch (uploadtype)
                    {
                        case 2:
                            item.UserType = 2;
                            break;
                        case 3:
                        case 4:
                            item.UserType = 3;
                            break;
                        case 5:
                        case 6:
                            item.UserType = 4;
                            break;
                    }
                }

                switch (uploadtype)
                {
                    case 1:
                        var fileContent = new StreamContent(FileSource.OpenReadStream(FileSource.Size));
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(FileSource.ContentType);
                        content.Add(content: fileContent, name: "\"file\"", fileName: FileSource.Name);
                        var fileUploadResult = await FileUpDoServices.UploadUsers(content);
                        if (fileUploadResult is not null)
                        {
                            uploadResult = fileUploadResult;
                            UsersUploadResult newDataResults = await UsersServices.ProccedToUploadUsers(uploadResult);
                            if (newDataResults is not null)
                            {
                                //tiene resultados
                                ErrorMessageToDisplay = "Upload Data Succesfull";
                                await Clear();
                                showResume = true;
                                FileSource = null;
                                commitUsersResult = newDataResults;
                                base.StateHasChanged();
                            }
                            else
                            {
                                ErrorMessageToDisplay = "Fail, Upload Data, pls Call for admin";
                            }
                        }
                        else
                        {
                            ErrorMessageToDisplay = "Fail Upload file to system, pls Call for admin";
                        }

                        break;
                    case 2:
                    case 3:
                    case 5:
                        UsersUploadResult ResultMassiveUpload = await UsersServices.UploadUsers(dataToShowInTable);
                        await JS.InvokeVoidAsync("console.log", dataToShowInTable);
                        if (ResultMassiveUpload is not null)
                        {
                            //tiene resultados
                            ErrorMessageToDisplay = "Upload Data Succesfull";
                            await Clear();
                            showResume = true;
                            FileSource = null;
                            commitUsersResult = ResultMassiveUpload;
                            base.StateHasChanged();
                        }
                        else
                        {
                            ErrorMessageToDisplay = "Fail, Upload Data, pls Call for admin";
                        }

                        break;
                    case 4:
                    case 6:
                        UsersUploadResult ResultMassiveUploadToSuperior = await UsersServices.UploadUsersToSuperior(dataToShowInTable, MasterUserSuperior);
                        if (ResultMassiveUploadToSuperior is not null)
                        {
                            //tiene resultados
                            ErrorMessageToDisplay = "Upload Data Succesfull";
                            await Clear();
                            showResume = true;
                            FileSource = null;
                            commitUsersResult = ResultMassiveUploadToSuperior;
                            base.StateHasChanged();
                        }
                        else
                        {
                            ErrorMessageToDisplay = "Fail, Upload Data, pls Call for admin";
                        }

                        break;
                }
            }
            else
            {
                ErrorMessageToDisplay = "Fail, Upload Data, pls Call for admin";
            }
        }

        private async Task DownloadAllFormat()
        {
            await UsersServices.DownloadAllUsersFormat();
        }

        private async Task DownloadSSVFormat()
        {
            await UsersServices.DownloadSSVFormat();
        }

        private async Task DownloadSupervisorsFormat()
        {
            await UsersServices.DownloadSupervisorsFormat();
        }

        private async Task DownloadOperatorsFormat()
        {
            await UsersServices.DownloadOperatorsFormat();
        }

        private async Task DownloadAllExample()
        {
            await FileUpDoServices.DownloadAllUsersFormat();
        }

        private async Task DownloadSSVExample()
        {
            await FileUpDoServices.DownloadSSVFormat();
        }

        private async Task DownloadSVExample()
        {
            await FileUpDoServices.DownloadSupervisorsFormat();
        }

        private async Task DownloadOperatorsExample()
        {
            await FileUpDoServices.DownloadOperatorsFormat();
        }

        private async void OpenDialog(User ContextItem)
        {
            AreasManagerVisibleDialog = true;
            
            await JS.InvokeVoidAsync("console.log", $"[OpenDialog] UserType: {ContextItem.UserType}, Superior: {ContextItem.Superior?.Name}, SuperiorId: {ContextItem.Superior?.UserId}");
            await JS.InvokeVoidAsync("console.log", $"[OpenDialog] _allUsers count: {_allUsers?.Count}");
            
            // Para Supervisores (UserType 3), cargar SOLO las áreas del superior (SSV)
            // Para Operadores (UserType 4), cargar SOLO las áreas del superior (Supervisor)
            // Igual que CreateUsers.razor
            if ((ContextItem.UserType == 3 || ContextItem.UserType == 4) && ContextItem.Superior != null)
            {
                await JS.InvokeVoidAsync("console.log", "[OpenDialog] Entrando a filtro de supervisor/operador");
                
                // Buscar el superior en _allUsers para obtener sus áreas completas
                // porque ContextItem.Superior puede no tener las Areas cargadas
                var superiorFromList = _allUsers.FirstOrDefault(u => u.UserId == ContextItem.Superior.UserId);
                
                await JS.InvokeVoidAsync("console.log", $"[OpenDialog] Superior encontrado: {superiorFromList?.Name}, Areas count: {superiorFromList?.Areas?.Count}");
                
                if (superiorFromList?.Areas != null && superiorFromList.Areas.Count > 0)
                {
                    await JS.InvokeVoidAsync("console.log", $"[OpenDialog] Usando {superiorFromList.Areas.Count} áreas del superior");
                    // Usar las áreas del superior encontrado en _allUsers
                    _areasManager = new List<Area>(superiorFromList.Areas);
                }
                else if (ContextItem.Superior.Areas != null && ContextItem.Superior.Areas.Count > 0)
                {
                    await JS.InvokeVoidAsync("console.log", "[OpenDialog] Fallback: usando áreas de ContextItem.Superior");
                    // Fallback: usar las áreas del ContextItem.Superior directamente
                    _areasManager = new List<Area>(ContextItem.Superior.Areas);
                }
                else
                {
                    await JS.InvokeVoidAsync("console.log", "[OpenDialog] Superior sin áreas, lista vacía");
                    // Si el superior no tiene áreas, lista vacía
                    _areasManager = new List<Area>();
                }
            }
            else
            {
                await JS.InvokeVoidAsync("console.log", "[OpenDialog] Usando TODAS las áreas de la planta (SSV u otro)");
                // Para SSV (UserType 2) u otros, cargar todas las áreas de la planta
                _areasManager = new List<Area>(_areas[_plants.FindIndex(e => e.PlantId == ContextItem.PlantId)]);
            }
            
            // Remover las áreas ya asignadas de la lista de disponibles
            if (ContextItem.Areas?.Count > 0)
            {
                if (_areasManager?.Count > 0)
                {
                    foreach (Area item in ContextItem.Areas)
                    {
                        if (_areasManager.FindIndex(i => i.AreaId == item.AreaId) != -1)
                        {
                            _areasManager.RemoveAt(_areasManager.FindIndex(i => i.AreaId == item.AreaId));
                        }
                    }
                }
            }

            AuxDialogUser = ContextItem;
            StateHasChanged();
        }

        void CloseDialog()
        {
            AreasManagerVisibleDialog = false;
            StateHasChanged();
        }

        private DialogOptions dialogOptions = new()
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Large,
            FullWidth = true,
            DisableBackdropClick = true,
            CloseButton = true
        };
    }
}