using Microsoft.JSInterop;
using MudBlazor;


namespace SupervisorMobility.Client.Pages.Configuration.AssyChartPage
{
    public partial class CreateAssyChart
    {
        //objects
        AssyChart _newassychart = new();
        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();
        List<Product> _products = new();
        List<Distribution> _distributions { get; set; } = new();

        string[] _ccp { get; set; } = { "/01. PRESS ", "/01. PRESS/01. CCP ", "/02. BiW ", "/02. BiW/01. P71A ", "/02. BiW/01. P71A/01. IMP B ", "/02. BiW/01. P71A/02. ANTICORROSION ", "/02. BiW/01. P71A/03. PRECISION ", "/02. BiW/01. P71A/04. APARIENCIA ", "/02. BiW/01. P71A/05. TORQUE ", "/02. BiW/02. X247 ", "/02. BiW/02. X247/01. IMP B ", "/02. BiW/02. X247/02. ANTICORROSION ", "/02. BiW/02. X247/03. PRECISION ", "/02. BiW/02. X247/04. APARIENCIA ", "/02. BiW/02. X247/05. TORQUE ", "/02. BiW/03.N71A ", "/02. BiW/03.N71A/01. IMP B ", "/02. BiW/03.N71A/02. ANTICORROSION ", "/02. BiW/03.N71A/03. PRECISION ", "/02. BiW/03.N71A/04. APARIENCIA ", "/02. BiW/03.N71A/05. TORQUE ", "/03. PAINT ", "/03. PAINT/01. PRETRATAMIENTO "};
        string[] _hoe { get; set; } = {"/01. PRESS/04. MANTENIMIENTO ","/02. BODY/01. CALIDAD/01. P71A, N71A,  X247/01. LINEA ","/02. BODY/01. CALIDAD/01. P71A, N71A,  X247/01. LINEA/01. WELDING ","/02. BODY/01. CALIDAD/01. P71A, N71A,  X247/01. LINEA/02. GLUE ","/02. BODY/01. CALIDAD/01. P71A, N71A,  X247/01. LINEA/03. APPAREANCE ","/02. BODY/01. CALIDAD/01. P71A, N71A,  X247/01. LINEA/04. TORQUE ","/02. BODY/01. CALIDAD/01. P71A, N71A,  X247/01. LINEA/05. OTHER ","/02. BODY/01. CALIDAD/01. P71A, N71A,  X247/03. CMM ","/02. BODY/01. CALIDAD/01. P71A, N71A,  X247/03. CMM/01. LOM ","/02. BODY/01. CALIDAD/02. Normas de Inspeccion/01. P71A ","/02. BODY/01. CALIDAD/02. Normas de Inspeccion/01. P71A/01. SOLDADURA ","/02. BODY/01. CALIDAD/02. Normas de Inspeccion/01. P71A/02. SELLO ","/02. BODY/01. CALIDAD/02. Normas de Inspeccion/01. P71A/03. PRESICION ","/02. BODY/01. CALIDAD/02. Normas de Inspeccion/01. P71A/04. TORQUE ","/02. BODY/01. CALIDAD/02. Normas de Inspeccion/01. P71A/05. APARIENCIA ","/02. BODY/01. CALIDAD/02. Normas de Inspeccion/02. N71A ","/02. BODY/01. CALIDAD/02. Normas de Inspeccion/02. N71A/01. SOLDADURA ","/02. BODY/01. CALIDAD/02. Normas de Inspeccion/02. N71A/02. SELLO ","/02. BODY/01. CALIDAD/02. Normas de Inspeccion/02. N71A/03. PRESICION ","/02. BODY/01. CALIDAD/02. Normas de Inspeccion/02. N71A/04. TORQUE ","/02. BODY/01. CALIDAD/02. Normas de Inspeccion/02. N71A/05. APARIENCIA ","/02. BODY/01. CALIDAD/02. Normas de Inspeccion/03. X247 "};
        string[] _gos { get; set; } = { "/01. PRESS", "/01. PRESS/01. MANUFACTURA/01. X247", "/01. PRESS/01. MANUFACTURA", "/01. PRESS/01. MANUFACTURA/02. P71A", "/01. PRESS/01. MANUFACTURA/03. N71A", "/02. BiW", "/02. BiW/01. P71A", "/02. BiW/01. P71A/01. LWR BED (Z1)", "/02. BiW/01. P71A/01. LWR BED (Z1)/01.  AJ", "/02. BiW/01. P71A/01. LWR BED (Z1)/02. JR", "/02. BiW/01. P71A/01. LWR BED (Z1)/03. NSL", "/02. BiW/01. P71A/02. UPR BODY (Z2-Z2.3)", "/02. BiW/01. P71A/02. UPR BODY (Z2-Z2.3)/01. AJ", "/02. BiW/01. P71A/02. UPR BODY (Z2-Z2.3)/01. AJ/01. WH (LH)", "/02. BiW/01. P71A/02. UPR BODY (Z2-Z2.3)/01. AJ/02. WH (RH)", "/02. BiW/01. P71A/02. UPR BODY (Z2-Z2.3)/01. AJ/03. AJ ROOF", "/02. BiW/01. P71A/02. UPR BODY (Z2-Z2.3)/02. JR", "/02. BiW/01. P71A/02. UPR BODY (Z2-Z2.3)/02. JR/01. BS- REINF (LH)", "/02. BiW/01. P71A/02. UPR BODY (Z2-Z2.3)/02. JR/02. BS- REINF(RH)", "/02. BiW/01. P71A/02. UPR BODY (Z2-Z2.3)/03. NSL", "/02. BiW/01. P71A/02. UPR BODY (Z2-Z2.3)/03. NSL/01. BSM-BSO(LH)", "/02. BiW/01. P71A/02. UPR BODY (Z2-Z2.3)/03. NSL/02. BSM-BSO(RH)", "/02. BiW/01. P71A/02. UPR BODY (Z2-Z2.3)/04. NRL", "/02. BiW/01. P71A/03. HEM LINE (Z3)", "/02. BiW/01. P71A/03. HEM LINE (Z3)/01. AJ", "/02. BiW/01. P71A/03. HEM LINE (Z3)/02. JR", "/02. BiW/01. P71A/03. HEM LINE (Z3)/03. NSL", "/02. BiW/01. P71A/03. HEM LINE (Z3)/04. WELD NUT", "/02. BiW/01. P71A/04. METAL LINE (Z4)"};


        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Assy Chart", href: "/assychart"),
            new BreadcrumbItem("New Assy Chart", href: "", disabled: true),
        };

        //Inizialize
        protected async override Task OnInitializedAsync()
        {
            _plants = await PlantServices.GetPlants();
            _products = await ProductServices.GetProducts();
        }



        //Function Update Area on change plant select

        async void UpdateAreas()
        {
            _areas = await AreaServices.GetAreas(_newassychart.PlantId);
        }
        //Function Update Distributions on change Area select

        private async void UpdateDistributions()
        {
            _distributions = await DistributionServices.GetDistributions(_newassychart.PlantId, _newassychart.AreaId);
        }

        async void CreateNewAssyChartAsync()
        {
            //_newassychart.CreationDate = DateTime.Now;
            var result = await AssyChartServices.CreateAssyChart(_newassychart);
            if(result != null)
                NavigationManager.NavigateTo("/assychart");
            else
                await JsRuntime.InvokeVoidAsync("alert", "Error en los datos!"); // Alert
        }

        void CancelCreateAssyChart()
        {
            NavigationManager.NavigateTo("/assychart");
        }

    }
}
