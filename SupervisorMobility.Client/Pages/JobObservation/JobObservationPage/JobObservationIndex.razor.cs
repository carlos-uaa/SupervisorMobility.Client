using MudBlazor;
using static MudBlazor.CategoryTypes;

namespace SupervisorMobility.Client.Pages.JobObservation.JobObservationPage
{
    public partial class JobObservationIndex
    {

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Job Observation", href: "", disabled: true)
        };

        private List<BreadcrumbItem> _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Category 1", href: "jobobservation"),
            new BreadcrumbItem("Category 2", href: null, disabled: true),
            new BreadcrumbItem("Category 3", href: null, disabled: true),
            new BreadcrumbItem("Category 4", href: null, disabled: true),
            new BreadcrumbItem("Category 5", href: null, disabled : true),
            new BreadcrumbItem("Category 6", href: null, disabled : true),
        };

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

        string[] headings = { "Preparación de la Observacíon", "Respuestas y comentarios",  };
        string[] rows = {
        @"1.Question answer",
        @"2.Question answer",
        @"3.Question answer",
        @"4.Question answer ",
        @"5.Question answer",
        @"6.Question answer ",
    };

    }
}
