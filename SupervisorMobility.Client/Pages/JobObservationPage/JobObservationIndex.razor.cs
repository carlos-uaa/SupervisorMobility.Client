using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Pages.JobObservationPage.Categories.Category1Page;
using static MudBlazor.CategoryTypes;

namespace SupervisorMobility.Client.Pages.JobObservationPage
{
    public partial class JobObservationIndex
    {


        public List<ChecklistCategory> _checklistCategories { get; set; } = new();

        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _checklistCategories = await ChecklistService.GetChecklistCategories();
        }

        public int category = 1;


        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Job Observation", href: "", disabled: true)
        };

        private List<BreadcrumbItem> _items;
        public JobObservationIndex()
        {
            _items = new List<BreadcrumbItem>()
            {
                new BreadcrumbItem("Category 1", href: "jobobservation"),
                new BreadcrumbItem("Category 2", href: null, disabled: false),
                new BreadcrumbItem("Category 3", href: null, disabled: false),
                new BreadcrumbItem("Category 4", href: null, disabled: false),
                new BreadcrumbItem("Category 5", href: null, disabled: false),
                new BreadcrumbItem("Category 6", href: null, disabled: false),
                //new BreadcrumbItem(categoria, href: null, disabled : (bool)_checklistCategories.FirstOrDefault().IsActive),
            };

        }

        public List<string> header = new List<string> { "Area", "Distribuidor", "Producto" };

        DisplayNameLabelClass model = new();

        public class DisplayNameLabelClass
        {
            public DateTime? Date { get; set; }
            public bool Boolean { get; set; }
            public string String { get; set; }
        }

        private string stringValue { get; set; }
        private Drink enumValue { get; set; } = Drink.HotWater;
        public enum Drink { Tea, SparklingWater, SoftDrink, Cider, Beer, Wine, Moonshine, Wodka, Cola, GreeTea, FruitJuice, Lemonade, HotWater, SpringWater, IceWater, }

        string[] headings = { "Preparación de la Observacíon", "Respuestas y comentarios", };
        string[] rows =
        {
            @"1.1. Los estándares estan completos y actualizados (HOE, Estado de referencia de 5S, etc. Icluyendo la pasada observación de operación  (S/N)|Si",
            @"1.2. ¿Cuál es nivel de ILU del operador?  ¿Está el entrenamiento alineado con el Cuadro de requisitos de Operaicón ? (S/N)| No, Operador de recien ingreso",
            @"1.3. Verificar  ""Documentación de Seguridad y Ergonomía"" están actualizados (S/N) ?| Si ",
            @"1.4. ¿Hay algún problema de seguridad y ergonomía identificado? ¿Si existe indicar, cuál?|NO",
            @"1.5. ¿Hay algún problema de Calidad en la estación de trabajo recientemente? Si existe , ¿Cuál?|Si  Holgura fender ",
            @"1.6. ¿Cuál es la prioridad KPI a mejorarse para el la estación de trabajo o Zona de trabajo?|DPHU ",
        };

        private bool HideCategory1 { get; set; } = false;
        private bool HideCategory2 { get; set; } = true;
        private bool HideCategory3 { get; set; } = true;
        private bool HideCategory4 { get; set; } = true;
        private bool HideCategory5 { get; set; } = true;
        private bool HideCategory6 { get; set; } = true;

        public void ShowCategory(string category)
        {
            switch (category)
            {
                case "Category 1": HideCategory1 = false;
                    HideCategory2 = true;
                    HideCategory3 = true;
                    HideCategory4 = true;
                    HideCategory5 = true;
                    HideCategory6 = true;
                    break;
                case "Category 2": HideCategory2 = false;
                    HideCategory1 = true;
                    HideCategory3 = true;
                    HideCategory4 = true;
                    HideCategory5 = true;
                    HideCategory6 = true;
                    break;
                case "Category 3": HideCategory3 = false;
                    HideCategory1 = true;
                    HideCategory2 = true;
                    HideCategory4 = true;
                    HideCategory5 = true;
                    HideCategory6 = true;
                    break;
                case "Category 4": HideCategory4 = false;
                    HideCategory1 = true;
                    HideCategory2 = true;
                    HideCategory3 = true;
                    HideCategory5 = true;
                    HideCategory6 = true; break;
                case "Category 5": HideCategory5 = false;
                    HideCategory1 = true;
                    HideCategory2 = true;
                    HideCategory3 = true;
                    HideCategory4 = true;
                    HideCategory6 = true; break;
                case "Category 6": HideCategory6 = false;
                    HideCategory1 = true;
                    HideCategory2 = true;
                    HideCategory3 = true;
                    HideCategory4 = true;
                    HideCategory5 = true; 
                    break;

        }

    }




}
}
