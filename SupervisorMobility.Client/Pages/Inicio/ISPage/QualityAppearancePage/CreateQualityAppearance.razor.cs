using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.IS;
using SupervisorMobility.Client.Services.SignatureImageService;

namespace SupervisorMobility.Client.Pages.Inicio.ISPage.QualityAppearancePage
{
    public partial class CreateQualityAppearance
    {
        private List<BreadcrumbItem> _links;
        Apearance _appeareance { get; set; } = new();
        public List<DataPanel> _dataPanelsCategories { get; set; } = new();
        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        public string signatureUser  = string.Empty;
        public bool isHeader = false;
        public int userType = 0;

        string partName = "FR DOOR INR RH";
        string partNumber = "A2477221001D";
        string programmed = "500";
        string inspector = "A. GARCIA";
        string hour = "22:45";
        string date = "14/02/24";

        string ssvImage = string.Empty;
        string svImage = string.Empty;
        string operatorImage = string.Empty;

        public class ItemModel
        {
            public string Commentary { get; set; }
        }

        List<ItemModel> items = new List<ItemModel>();


        public class SpecificationValues
        {
            public string MaterialSpecification { get; set; }
            public string PartThickness { get; set; }
            public string BoreholesQuantity { get; set; }
            public string Laminate { get; set; }
            public string PartNumberReleased { get; set; }
        }

        private List<SpecificationValues> specificationValues = new List<SpecificationValues>
    {
        new SpecificationValues
        {
            MaterialSpecification = "CR5",
            PartThickness = "0.77",
            BoreholesQuantity = "540",
            Laminate = "G1",
            PartNumberReleased = "01"
        },
        new SpecificationValues
        {
            MaterialSpecification = "CR5",
            PartThickness = "0.77",
            BoreholesQuantity = "540",
            Laminate = "G1",
            PartNumberReleased = "01"
        },
        new SpecificationValues
        {
            MaterialSpecification = "CR5",
            PartThickness = "0.77",
            BoreholesQuantity = "540",
            Laminate = "G1",
            PartNumberReleased = "01"
        },
        new SpecificationValues
        {
            MaterialSpecification = "CR5",
            PartThickness = "0.77",
            BoreholesQuantity = "540",
            Laminate = "G1",
            PartNumberReleased = "01"
        },
        new SpecificationValues
        {
            MaterialSpecification = "CR5",
            PartThickness = "0.77",
            BoreholesQuantity = "540",
            Laminate = "G1",
            PartNumberReleased = "01"
        },
        new SpecificationValues
        {
            MaterialSpecification = "CR5",
            PartThickness = "0.77",
            BoreholesQuantity = "540",
            Laminate = "G1",
            PartNumberReleased = "01"
        },
        new SpecificationValues
        {
            MaterialSpecification = "CR5",
            PartThickness = "0.77",
            BoreholesQuantity = "540",
            Laminate = "G1",
            PartNumberReleased = "01"
        },
        new SpecificationValues
        {
            MaterialSpecification = "CR5",
            PartThickness = "0.77",
            BoreholesQuantity = "540",
            Laminate = "G1",
            PartNumberReleased = "01"
        },
        new SpecificationValues
        {
            MaterialSpecification = "CR5",
            PartThickness = "0.77",
            BoreholesQuantity = "540",
            Laminate = "G1",
            PartNumberReleased = "01"
        },
    };

        string fracture = "OK";
        string radiusMalformation = "OK";
        string strikes = "OK";
        string thinning = "0.77";
        string lackOfMaterial = "OK";
        string corrosionControl = "v3";
        string fitting = "OK";
        string generalGrate = "0.18";
        string gap = "OK";
        string buttonMark = "V3";
        string mercedesMark = "V3";
        string burringHole = "V3";

        List<Product> _products { get; set; } = new();

        int productId = 0;

        List<User> _seniorSupervisors { get; set; } = new();
        List<User> _allSSVs { get; set; } = new();
        List<User> _allSupervisors { get; set; } = new();
        List<User> _supervisors { get; set; } = new();
        List<User> _operators = new();
        public List<User> operatorUsers = new();
        int ssvId = 0;
        int supervisorId = 0;
        int operatorId = 0;

        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem(text: Localizer["home"], href: "/"),
                    new BreadcrumbItem(text: Localizer["Appearance"], href: "", disabled: true)
                };
            BreadcrumbService.UpdateBreadcrumbs(_links);
            await GetUserAsync();
            logged = await HasPropertyAsync();
            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error You have to log in", Severity.Error);
                NavigationManager.NavigateTo($"/");
            }
            AddItem();
            _products = await ProductsService.GetProducts();
            _products = _products.OrderBy(p => p.Description).ToList();
            _dataPanelsCategories = await DataPanelServices.GetAllDataPanels();

            _seniorSupervisors = new();
            _allSSVs = await UsersService.GetUsersByType(2, true, false);
            _allSSVs = _allSSVs.OrderBy(s => s.Name).ToList();

            _seniorSupervisors = _allSSVs;
        }
        //Local storage user
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
                json = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user");
                user = JsonSerializer.Deserialize<User>(json) ?? new();
            }
            return hasProperty;
        }
        private async Task<bool> HasPropertyAsync()
            => await JSRuntime.InvokeAsync<bool>("localStorage.hasOwnProperty", "user");


        void AddItem()
        {
            items.Add(new ItemModel());
        }

        void RemoveItem(ItemModel item)
        {
            if (items.Count > 1)
            {
                items.Remove(item);
            }

        }

        private async void ShowSupervisors()
        {

            if (ssvId != 0)
            {
                User ssv = new();

                ssv = _seniorSupervisors.Where(ssv => ssv.UserId == ssvId).FirstOrDefault();

            supervisorId = 0;
            operatorId = 0;
                _supervisors = new();
                int ssvPlantId = (int)ssv.PlantId;
                _allSupervisors = await UsersService.GetUsersByUserTypeInPlant(ssvPlantId, 3, false, false);
                _allSupervisors = _allSupervisors.OrderBy(s => s.Name).ToList();
                foreach (var sv in _allSupervisors)
                {
                    if (sv.SuperiorId == ssvId)
                    {
                        _supervisors.Add(sv);
                    }
                }
            }
            StateHasChanged();
        }

        private async void ShowOperators()
        {
 
            if (user.UserType == 1 || user.UserType == 2)
            {
                _operators = await UsersService.GetSubordinates(supervisorId, false);
                _operators = _operators.OrderBy(o => o.Name).ToList();
            }
            operatorUsers = new();
            operatorId = 0;
            //operator User
            foreach (var operatorUser in _operators)
            {
                if (operatorUser.SuperiorId == supervisorId)
                {
                    operatorUsers.Add(operatorUser);
                }
            }
            StateHasChanged();
        }


        private string currentImage = "";

        private void HandleSignatureSaved()
        {
            currentImage = _signatureImageService.GetImage();

            switch (userType)
            {
                case 1: ssvImage = currentImage; break;
                case 2: svImage = currentImage; break;
                case 3: operatorImage = currentImage; break;
            }

            isHeader = false;
            userType = 0;
            currentImage = "";
        }

        private void HandleClearSignature()
        {
            currentImage = "";
        }



        private void SignAppearance(bool header, int user)
        {
            //SSV
            isHeader = header;
            userType = user;

            switch (userType)
            {
                case 1: signatureUser = "SSV Signature"; break;
                case 2: signatureUser = "SV Signature"; break;
                case 3: signatureUser = "Operator Signature"; break;
            }
            StateHasChanged();
        }
    }
}