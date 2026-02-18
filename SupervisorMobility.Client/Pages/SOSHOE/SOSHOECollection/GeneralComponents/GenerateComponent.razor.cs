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


        public AnalysisSequencesDto _selections = new AnalysisSequencesDto();
        public List<SOSHub> _soSHubSelections = new List<SOSHub>();
        private int _lastSelectedIndex = -1;

        // Calculamos el DistributionId dinámicamente, solo para Distribution (cases 3, 33)
        // Para CSPC (cases 7, 77) siempre devuelve 0 para permitir selecciones
        private int GetDistributionId()
        {
            // Si estamos en CSPC (cases 7 o 77), devolver 0 incluso si Distribution existe
            if (State?.SelectedIndex == 7 || State?.SelectedIndex == 77)
                return 0;
            
            // Solo para Distribution (cases 3 o 33), devolver el ID si existe
            if (State?.Hub?.SOSDistribution != null && State.Hub.SOSDistribution.Count > 0)
            {
                return State.Hub.SOSDistribution.First().SOSDistributionId;
            }
            return 0;
        }

        // Calcula el SynopticRequirementsId dinámicamente, similar a GetDistributionId()
        private int GetSynopticRequirementsId()
        {
            // Si estamos en CSRO (cases 6 o 66), devolver el ID si ya existe
            if (State?.Hub?.SOSSynopticOperatingRequirements != null && State.Hub.SOSSynopticOperatingRequirements.Count > 0)
            {
                return State.Hub.SOSSynopticOperatingRequirements.First().SOSSynopticTableofOperatingRequirementsId;
            }
            return 0;
        }

        // Determina si estamos viendo un CSRO existente (readonly) o creando uno nuevo (editable)
        private bool IsCSROReadOnly()
        {
            // Si estamos en paso 6 o 66 (CSRO) y ya existe CSRO cargado, es readonly
            if ((State?.SelectedIndex == 6 || State?.SelectedIndex == 66) &&
                State?.SynopticTableOfOpertingRequirements != null && 
                State.SynopticTableOfOpertingRequirements.Count > 0)
            {
                return true;
            }
            return false;
        }

        // Determina si estamos viendo un CSPC existente (readonly) o creando uno nuevo (editable)
        private bool IsCSPCReadOnly()
        {
            // Si estamos en paso 7 o 77 (CSPC) y ya existe CSPC cargado, es readonly
            if ((State?.SelectedIndex == 7 || State?.SelectedIndex == 77) &&
                State?.SynopticTableOfControlPoints != null && 
                State.SynopticTableOfControlPoints.Count > 0)
            {
                return true;
            }
            return false;
        }

        protected override async Task OnInitializedAsync()
        {
            State.IsLoading = true;

            // Sincronizamos los parámetros recibidos con nuestro StateContainer
            State.Hub = _sosHub;
            State.SelectedIndex = selectedIndexPageGenerate;
            State.LogedUser = user;
            _lastSelectedIndex = selectedIndexPageGenerate;

            // Aquí llamarías solo a la carga inicial mínima necesaria
            await LoadInitialData();

            // Cargar selecciones iniciales basado en la sección que se abre
            await LoadInitialSelections();

            State.IsLoading = false;
        }

        private async Task LoadInitialSelections()
        {
            Console.WriteLine($"[GenerateComponent LoadInitialSelections] SelectedIndex={State.SelectedIndex}");
            
            if (State.SelectedIndex == 3)
            {
                // Entrando directo a SelectAnalisis&Sequences para Distribution
                if (State?.Hub?.SOSDistribution != null && State.Hub.SOSDistribution.Count > 0)
                {
                    var distribution = State.Hub.SOSDistribution.First();
                    _selections = new AnalysisSequencesDto
                    {
                        AnalysisSelected = distribution.Analyses?.ToList() ?? new List<SOSAnalysis>(),
                        SequencesSelected = distribution.Sequences?.ToList() ?? new List<SOSSequence>()
                    };

                }
                else
                {
                    _selections = new AnalysisSequencesDto
                    {
                        AnalysisSelected = new List<SOSAnalysis>(),
                        SequencesSelected = new List<SOSSequence>()
                    };

                }
            }
            else if (State.SelectedIndex == 6)
            {
                // Entrando directo a SelectSOSHubs para CSRO
                var csroCount = State?.SynopticTableOfOpertingRequirements != null ? State.SynopticTableOfOpertingRequirements.Count : 0;
                Console.WriteLine($"[GenerateComponent LoadInitialSelections] CSRO - SynopticTableOfOpertingRequirements Count={csroCount}");
                // Si ya existe CSRO, cargar sus selecciones de SOSHubs
                if (State?.SynopticTableOfOpertingRequirements != null && State.SynopticTableOfOpertingRequirements.Count > 0)
                {
                    var csro = State.SynopticTableOfOpertingRequirements.First();
                    _soSHubSelections = csro.SOSHubs?.ToList() ?? new List<SOSHub>();
                    Console.WriteLine($"[GenerateComponent LoadInitialSelections] CSRO existente encontrado, SOSHubs cargados={_soSHubSelections.Count}");
                }
                else
                {
                    _soSHubSelections = new List<SOSHub>();
                    Console.WriteLine($"[GenerateComponent LoadInitialSelections] CSRO nuevo, _soSHubSelections vacío");
                }
            }
            else if (State.SelectedIndex == 7)
            {
                // Entrando directo a SelectAnalisis&Sequences para CSPC
                // Si ya existe CSPC, cargar sus selecciones desde State.SynopticTableOfControlPoints (misma fuente que usa CSPCSection)
                if (State?.SynopticTableOfControlPoints != null && State.SynopticTableOfControlPoints.Count > 0)
                {
                    var cspc = State.SynopticTableOfControlPoints.First();
                    _selections = new AnalysisSequencesDto
                    {
                        AnalysisSelected = cspc.Analyses?.ToList() ?? new List<SOSAnalysis>(),
                        SequencesSelected = cspc.Sequences?.ToList() ?? new List<SOSSequence>()
                    };

                }
                else
                {
                    _selections = new AnalysisSequencesDto
                    {
                        AnalysisSelected = new List<SOSAnalysis>(),
                        SequencesSelected = new List<SOSSequence>()
                    };

                }
            }
            else
            {
                // Para otras secciones, inicializar vacío
                _selections = new AnalysisSequencesDto
                {
                    AnalysisSelected = new List<SOSAnalysis>(),
                    SequencesSelected = new List<SOSSequence>()
                };
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            // Detectar cambios en SelectedIndex y resetear estado de selecciones
            if (State != null && _lastSelectedIndex != State.SelectedIndex)
            {
                
                // Resetear SIEMPRE _selections cuando cambia la sección
                // Luego cargar datos específicos según la sección
                
                if (State.SelectedIndex == 3)
                {
                    // Entrando a SelectAnalisis&Sequences paso previo a Distribution
                    // Si hay Distribution existente, cargar sus datos
                    if (State?.Hub?.SOSDistribution != null && State.Hub.SOSDistribution.Count > 0)
                    {
                        var distribution = State.Hub.SOSDistribution.First();
                        _selections = new AnalysisSequencesDto
                        {
                            AnalysisSelected = distribution.Analyses?.ToList() ?? new List<SOSAnalysis>(),
                            SequencesSelected = distribution.Sequences?.ToList() ?? new List<SOSSequence>()
                        };

                    }
                    else
                    {
                        // Nueva Distribution, vaciar selecciones
                        _selections = new AnalysisSequencesDto
                        {
                            AnalysisSelected = new List<SOSAnalysis>(),
                            SequencesSelected = new List<SOSSequence>()
                        };

                    }
                }
                else if (State.SelectedIndex == 6)
                {
                    // Entrando a SelectSOSHubs paso previo a CSRO
                    var csroParamCount = State?.SynopticTableOfOpertingRequirements != null ? State.SynopticTableOfOpertingRequirements.Count : 0;
                    Console.WriteLine($"[GenerateComponent OnParametersSetAsync] Case 6 - CSRO, SynopticTableOfOpertingRequirements Count={csroParamCount}");
                    // Si ya existe CSRO, cargar sus selecciones de SOSHubs
                    if (State?.SynopticTableOfOpertingRequirements != null && State.SynopticTableOfOpertingRequirements.Count > 0)
                    {
                        var csro = State.SynopticTableOfOpertingRequirements.First();
                        _soSHubSelections = csro.SOSHubs?.ToList() ?? new List<SOSHub>();
                        Console.WriteLine($"[GenerateComponent OnParametersSetAsync] CSRO existente, SOSHubs cargados={_soSHubSelections.Count}");
                    }
                    else
                    {
                        // Nuevo CSRO, vaciar selecciones
                        _soSHubSelections = new List<SOSHub>();
                        Console.WriteLine($"[GenerateComponent OnParametersSetAsync] CSRO nuevo, _soSHubSelections vacío");
                    }
                }
                else if (State.SelectedIndex == 7)
                {
                    // Entrando a SelectAnalisis&Sequences paso previo a CSPC
                    // Si ya existe CSPC, cargar sus selecciones desde State.SynopticTableOfControlPoints (misma fuente que usa CSPCSection)
                    if (State?.SynopticTableOfControlPoints != null && State.SynopticTableOfControlPoints.Count > 0)
                    {
                        var cspc = State.SynopticTableOfControlPoints.First();
                        _selections = new AnalysisSequencesDto
                        {
                            AnalysisSelected = cspc.Analyses?.ToList() ?? new List<SOSAnalysis>(),
                            SequencesSelected = cspc.Sequences?.ToList() ?? new List<SOSSequence>()
                        };

                    }
                    else
                    {
                        // Nuevo CSPC, vaciar selecciones
                        _selections = new AnalysisSequencesDto
                        {
                            AnalysisSelected = new List<SOSAnalysis>(),
                            SequencesSelected = new List<SOSSequence>()
                        };

                    }
                }
                
                _lastSelectedIndex = State.SelectedIndex;
                
                // Forzar actualización del componente para que refleje cambios en _selections
                StateHasChanged();
            }
        }

        private async Task LoadInitialData()
        {
            
            try
            {
                State.IsLoading = true;
                
                // 1. Asegurar que tenemos el Hub cargado con todas sus relaciones
                // Siempre recargamos para asegurar que tenemos Equipment, Tools y Models (Productos)
                if (SOSHubId != 0)
                {

                    State.Hub = await SOSHubService.GetSOSHub(SOSHubId, 
                        includeAnalysesBkup: true, 
                        includeSections: true, 
                        includeImages: true, 
                        includeVideos: true, 
                        includeCommentaries: true, 
                        includeTools: true, 
                        includeEquipments: true, 
                        includeMaterials: true, 
                        includeInformation: true, 
                        includePeople: true, 
                        includeDocuments: true, 
                        includeModel: true, 
                        includeCollections: true, 
                        includePeopleCollections: true, 
                        includePats: true);
                    

                }
                else if (_sosHub != null)
                {
                    // Si _sosHub ya viene, pero sospechamos que le faltan datos, lo recargamos por ID
                    if (_sosHub.SOSHubId != 0)
                    {
                        State.Hub = await SOSHubService.GetSOSHub(_sosHub.SOSHubId, 
                            includeAnalysesBkup: true, 
                            includeSections: true, 
                            includeImages: true, 
                            includeVideos: true, 
                            includeCommentaries: true, 
                            includeTools: true, 
                            includeEquipments: true, 
                            includeMaterials: true, 
                            includeInformation: true, 
                            includePeople: true, 
                            includeDocuments: true, 
                            includeModel: true, 
                            includeCollections: true, 
                            includePeopleCollections: true, 
                            includePats: true);
                        

                    }
                    else
                    {

                        State.Hub = _sosHub;
                    }
                }

                // 2. Mapeo de Listas (Relaciones Uno a Muchos)
                if (State.Hub != null)
                {
                    // Usamos ?? new() para asegurar que nunca sean nulas al llegar al componente hijo
                    State.Pats = State.Hub.PATs?.ToList() ?? new();
                    State.Analyses = State.Hub.SOSAnalysis?.ToList() ?? new(); // En tu Hub se llama SOSAnalysis
                    State.Combinations = State.Hub.SOSCombination?.ToList() ?? new();
                    State.Distributions = State.Hub.SOSDistribution?.ToList() ?? new();
                    State.Flows = State.Hub.SOSFlow?.ToList() ?? new();
                    State.Sequences = State.Hub.SOSSequence?.ToList() ?? new();
                    State.SynopticTableOfControlPoints = State.Hub.SOSSynopticControlPoints?.ToList() ?? new();
                    State.SynopticTableOfOpertingRequirements = State.Hub.SOSSynopticOperatingRequirements?.ToList() ?? new();



                    // Si existe distribución, sus datos se cargarán dinámicamente en OnParametersSetAsync
                    if (State.Hub.SOSDistribution != null && State.Hub.SOSDistribution.Count > 0)
                    {
                        var distribution = State.Hub.SOSDistribution.First();

                    }

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
            var hubSelCount = _soSHubSelections != null ? _soSHubSelections.Count : 0;
            var analCount = _selections?.AnalysisSelected != null ? _selections.AnalysisSelected.Count : 0;
            var seqCount = _selections?.SequencesSelected != null ? _selections.SequencesSelected.Count : 0;
            Console.WriteLine($"[GenerateComponent GoNext] SelectedIndex={State.SelectedIndex}, _soSHubSelections Count={hubSelCount}, _selections Analysis={analCount}, Sequences={seqCount}");

            // Para CSRO (case 6), validar selección de SOSHubs
            if (State.SelectedIndex == 6)
            {
                if (_soSHubSelections == null || !_soSHubSelections.Any())
                {
                    Console.WriteLine($"[GenerateComponent GoNext] CSRO - No hay SOSHubs seleccionados, bloqueando navegación");
                    Snackbar.Add("Please select at least one Collector (SOSHub) to proceed.", Severity.Warning);
                    return;
                }
                Console.WriteLine($"[GenerateComponent GoNext] CSRO - Navegando de case 6 a case 66 con {_soSHubSelections.Count} SOSHubs");
                State.SelectedIndex = 66;
                return;
            }

            // Para Distribution (case 3) y CSPC (case 7), validar selección de Analysis/Sequences
            if (_selections == null || 
                ((_selections.AnalysisSelected != null && _selections.AnalysisSelected.Count <= 0) && 
                (_selections.SequencesSelected != null && _selections.SequencesSelected.Count <= 0)))
            {
                Snackbar.Add("Please select at least one Analysis or Sequence to proceed.", Severity.Warning);
                return;
            }

            if (State.SelectedIndex < 9)
                if (State.SelectedIndex == 3)
                    State.SelectedIndex = 33;
                else if(State.SelectedIndex == 7)
                    State.SelectedIndex = 77;
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