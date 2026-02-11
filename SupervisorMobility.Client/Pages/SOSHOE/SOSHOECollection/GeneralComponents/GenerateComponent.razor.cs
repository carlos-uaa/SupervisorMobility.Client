using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities.Dtos;
using SupervisorMobility.Client.Services.HCIService;
using SupervisorMobility.Client.Services.PlantService;
using SupervisorMobility.Client.Services.SOS_Services.SOSHubService;
using SupervisorMobility.Client.Pages.SOSHOE.SOSHOECollection.GeneralComponents.Sections;

namespace SupervisorMobility.Client.Pages.SOSHOE.SOSHOECollection.GeneralComponents
{
    public partial class GenerateComponent
    {
        [Inject] private SOSState State { get; set; } // Inyectamos el estado global
        [Inject] private ISOSHubService SOSHubService { get; set; } 
        [Inject] private IPlantService PlantService { get; set; } 
        [Inject] private IHCIService HCIService { get; set; } 
        [Inject] private ISnackbar Snackbar { get; set; }
        [Inject] private Microsoft.AspNetCore.Components.WebAssembly.Hosting.IWebAssemblyHostEnvironment HostEnvironment { get; set; }

        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        [Parameter] public int selectedIndexPageGenerate { get; set; }
        [Parameter] public SOSHub _sosHub { get; set; }
        [Parameter] public int SOSHubId { get; set; }
        [Parameter] public User user { get; set; } = new User();

        // Referencias por componente
        private PATSection PatSectionRef;
        private AnalysisSection AnalysisSectionRef;
        private CombinationSection CombinationSectionRef;
        private DistributionSection DistributionSectionRef;
        private FlowSection FlowSectionRef;
        private SequenceSection SequenceSectionRef;
        private CSROSection CSROSectionRef;
        private CSPCSection CSPCSectionRef;
        private HCISection HCISectionRef;

        protected override async Task OnInitializedAsync()
        {
            State.IsLoading = true;

            // Sincronizamos los parámetros recibidos con nuestro StateContainer
            State.Hub = _sosHub;
            State.SelectedIndex = selectedIndexPageGenerate;
            State.LogedUser = user;

            // Aquí llamarías solo a la carga inicial mínima necesaria
            await LoadInitialData();

            State.IsLoading = false;
        }

        private async Task LoadInitialData()
        {
            try
            {
                State.IsLoading = true;
                // 1. Asegurar que tenemos el Hub cargado
                // Si no viene por parámetro, lo buscamos por ID
                if (_sosHub == null || _sosHub.SOSHubId == 0)
                    if (SOSHubId != 0)
                        State.Hub = await SOSHubService.GetSOSHub(SOSHubId);
                    else
                    State.Hub = _sosHub;

                // 2. Mapeo de Listas (Relaciones Uno a Muchos)
                // Esto evita que los componentes hijos trabajen con objetos nulos
                if (State.Hub != null)
                {
                    // Usamos ?? new() para asegurar que nunca sean nulas al llegar al componente hijo
                    State.Pats = State.Hub.PATs?.ToList() ?? new();
                    State.Analyses = State.Hub.SOSAnalysis?.ToList() ?? new(); // En tu Hub se llama SOSAnalysis
                    State.Combinations = State.Hub.SOSCombination?.ToList() ?? new();
                    State.Distributions = State.Hub.SOSDistribution?.ToList() ?? new();
                    State.Flows = State.Hub.SOSFlow?.ToList() ?? new();
                    State.Sequences = State.Hub.SOSSequence?.ToList() ?? new();

                    // 3. Carga de HCI (Sigue siendo un objeto único basado en HciId)
                    if (State.Hub.HciId.HasValue && State.Hub.HciId > 0)
                        State.Hci = await HCIService.GetHCI(State.Hub.HciId.Value);
                }

                // 4. Cargar datos transversales (Listas que se usan en varios lugares)
                // Solo cargamos si el State aún no las tiene (para no repetir llamadas API)
                if (State.Plants == null || !State.Plants.Any())
                {
                    var plantsResponse = await PlantService.GetPlants();
                    State.Plants = plantsResponse.ToList();
                }

                // 5. Sincronizar el índice de navegación inicial
                State.Dev_env = HostEnvironment.IsDevelopment();
                State.SelectedIndex = selectedIndexPageGenerate;
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error initializing SOS Process: {ex.Message}", Severity.Error);
            }
            finally
            {
                State.IsLoading = false;
            }
        }

        private void GoNext()
        {
            // Lógica simple de incremento de índice
            if (State.SelectedIndex < 9)
            {
                State.SelectedIndex++;
                StateHasChanged();
            }
        }

        private void GoBack()
        {
            MudDialog.Close(DialogResult.Ok("Reopen"));
        }

        private void Close() => MudDialog.Cancel();

        // El padre ahora solo cambia un índice, no maneja la lógica interna de los pasos
        private void ChangeStep(int newStep) => State.SelectedIndex = newStep;

        private string GetStepTitle() => State.SelectedIndex switch
        {
            0 => "PAT Creation",
            1 => "SOS Analysis",
            2 => "Combination",
            6 => "SOS Cuadro Sinóptico de Requisitos de Operación",
            66 => "SOS Cuadro Sinóptico de Requisitos de Operación",
            77 => "SOS Cuadro Sinóptico de Puntos de Control",
            _ => "SOS Process"
        };

        private string GetStepIcon() => State.SelectedIndex switch
        {
            0 => Icons.Material.Filled.Assignment,
            1 => Icons.Material.Filled.Analytics,
            _ => Icons.Material.Filled.Settings
        };
    }
}