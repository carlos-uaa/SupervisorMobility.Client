using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using System.Globalization;

namespace SupervisorMobility.Client.Pages.Inicio.HCIPage.Components
{
    public partial class Expertise
    {
        [Parameter]
        public bool details { get; set; }
        [Parameter]
        public List<ILURegister> ExpertiseTable { get; set; }


        [Parameter]
        public EventCallback<ILURegister> Add { get; set; }
        [Parameter]
        public EventCallback<int> Del { get; set; }
        [Parameter]
        public EventCallback<(ILURegister, int)> Upd { get; set; }

        [Parameter]
        public User user { get; set; } 


        private List<ILULevel> _LevelsILU { get; set; } = new();
        private ILURegister _newIlu { get; set; } = new();
        private List<ILURegister> _expertiseItems = new List<ILURegister>();

        private List<int> _idsAreas { get; set; } = new();
        private List<Area> _ExistAreas { get; set; } = new();
        protected async override Task OnInitializedAsync()
        {
            _LevelsILU = await ILUServices.GetLevelsILU();
            _idsAreas = ExpertiseTable.Where(e => e.Distribution != null).Select(e => e.Distribution.AreaId).Distinct().ToList();
            _ExistAreas = await AreaServices.GetAreasByIds(_idsAreas);
            _expertiseItems = GetGroupedExpertiseTable();

            Task<List<Plant>> plantsTask = null;
            Task<List<Area>> areasTask = null;
            if (user.UserType == 3)
            {
                plantId = (int)user.PlantId;
                areaId = user.Areas != null && user.Areas.Count > 0 ? user.Areas.FirstOrDefault().AreaId : 0;
                _distributions = await DistributionService.GetDistributionsWithCollections(plantId, areaId);
            }
            else
            {
                if (user.UserType == 1 || user.UserType == 5)
                    plantsTask = PlantServices.GetPlants();
                if (user.UserType == 2 || user.UserType == 5)
                {
                    plantId = (int)user.PlantId;
                    areasTask = AreaServices.GetAreas(plantId);
                }
            }

            if (plantsTask != null)
                _plants = (await plantsTask).OrderBy(p => p.Description).ToList();
            if (areasTask != null)
                _areas = (await areasTask).OrderBy(a => a.Description).ToList();

            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private void DateChanged(DateRange range, int index)
        {
            ExpertiseTable[index].AcquisitionDate = range.Start;
            ExpertiseTable[index].EndDate = range.End;
            Upd.InvokeAsync((ExpertiseTable[index], index));
        }


        private void Delete(int index)
        {
            Del.InvokeAsync(index);
        }

        private void AddHere()
        {
            ILURegister newILU = new ILURegister();
            Add.InvokeAsync(newILU);
        }

        private void updateILULevel(int auxIluLevelId)
        {
            _newIlu.ILULevelId = auxIluLevelId;
        }

        private List<ILURegister> GetGroupedExpertiseTable()
        {
            var query = ExpertiseTable.Where(e => e.isActive);

            if (plantId != 0)
            {
                var allowedAreaIds = _ExistAreas
                    .Where(a => a.PlantId == plantId)
                    .Select(a => a.AreaId)
                    .ToHashSet();

                query = query.Where(e => e.Distribution != null &&
                                         allowedAreaIds.Contains(e.Distribution.AreaId));
            }

            if (areaId != 0)
            {
                query = query.Where(e => e.Distribution != null && e.Distribution.AreaId == areaId);
            }

            if (distributionId != 0)
            {
                query = query.Where(e => e.Distribution != null && e.Distribution.DistributionId == distributionId);
            }


            if (filterDate.HasValue)
            {
                query = query.Where(e => e.AcquisitionDate.HasValue &&
                e.AcquisitionDate.Value.Date == filterDate.Value.Date);

                if (areaId == 0)
                {
                    _distributions = query.Select(e => e.Distribution).Where(d => d != null && d.IsActive == true).Distinct().ToList();
                }
            }

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                string term = searchString.Trim().ToLower();

                // Detectores previos
                bool isDateExact = DateTime.TryParseExact(searchString,
                    new[] { "yyyy-MM-dd", "dd/MM/yyyy", "MM/dd/yyyy", "yyyy/MM/dd", "dd.MM.yyyy", "MM.dd.yyyy" },
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate);

                bool isYear = int.TryParse(searchString, out int year);

                bool isMonth = false;
                DateTime parsedMonth = DateTime.MinValue;
                int monthNumber = 0;

                if (DateTime.TryParseExact(searchString, "MMMM", new CultureInfo("en-US"), DateTimeStyles.None, out parsedMonth) ||
                    DateTime.TryParseExact(searchString, "MMM", new CultureInfo("en-US"), DateTimeStyles.None, out parsedMonth) ||
                    DateTime.TryParseExact(searchString, "MMMM", new CultureInfo("es-ES"), DateTimeStyles.None, out parsedMonth) ||
                    DateTime.TryParseExact(searchString, "MMM", new CultureInfo("es-ES"), DateTimeStyles.None, out parsedMonth) ||
                    (int.TryParse(searchString, out monthNumber) && monthNumber >= 1 && monthNumber <= 12))
                {
                    isMonth = true;
                    if (monthNumber == 0) monthNumber = parsedMonth.Month;
                }

                bool isRange = false;
                DateTime startDate = DateTime.MinValue, endDate = DateTime.MinValue;
                if (searchString.Contains("to", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = searchString.Split("to", StringSplitOptions.TrimEntries);
                    if (parts.Length == 2 &&
                        DateTime.TryParseExact(parts[0], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate) &&
                        DateTime.TryParseExact(parts[1], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
                    {
                        isRange = true;
                    }
                }

                query = query.Where(e =>
                    // ID o código
                    e.ILURegisterid.ToString().Contains(term) ||
                    (e.DistributionId?.ToString().Contains(term) ?? false) ||
                    (e.Distribution?.Code?.ToLower().Contains(term) ?? false) ||
                    (e.Distribution?.Description?.ToLower().Contains(term) ?? false) ||

                    // Datos de Área
                    (e.Distribution != null &&
                    _ExistAreas.Any(a => a.AreaId == e.Distribution.AreaId && ((a.Description?.ToLower().Contains(term) ?? false) || (a.Code?.ToLower().Contains(term) ?? false)))) ||

                    

                    // Fecha en texto
                    (e.AcquisitionDate?.ToString("dd/MM/yyyy").Contains(term) ?? false) ||
                    (e.AcquisitionDate?.ToString("MMMM", new CultureInfo("es-ES")).ToLower().Contains(term) ?? false) ||
                    (e.AcquisitionDate?.Month.ToString().Contains(term) ?? false) ||

                    // --- Filtros avanzados ---
                    (isDateExact && e.AcquisitionDate.HasValue && e.AcquisitionDate.Value.Date == parsedDate.Date) ||
                    (isYear && e.AcquisitionDate.HasValue && e.AcquisitionDate.Value.Year == year) ||
                    (isMonth && e.AcquisitionDate.HasValue && e.AcquisitionDate.Value.Month == monthNumber) ||
                    (isRange && e.AcquisitionDate.HasValue &&
                        e.AcquisitionDate.Value.Date >= startDate.Date &&
                        e.AcquisitionDate.Value.Date <= endDate.Date)
                );
            }

            // Agrupar por DistributionId y categoría ILU
            if(statusId != 0)
            {
                query = query.Where(e => e.ILULevel.ILULevelId != null && e.ILULevel.ILULevelId == statusId);
                if (areaId == 0)
                {
                    _distributions = query.Select(e => e.Distribution).Where(d => d != null && d.IsActive == true).Distinct().ToList();
                }
            }


            return query
                .GroupBy(e => new { e.DistributionId, ILUCategory = GetILUCategory(e.ILULevel?.ILULevelCode) })
                .Select(g => g
                    .OrderByDescending(e => e.AcquisitionDate ?? DateTime.MinValue)
                    .First())
                .ToList();
        }

        private string GetILUCategory(string? iluLevelCode)
        {
            return iluLevelCode switch
            {
                "ITrainee" or "I" or "ILeader" or "LTrainee" or "LTraineeLeader" => "IGroup",
                "L" or "LLeader" or "UTrainee" or "ULeaderTrainee" => "LGroup",
                "U" or "ULeader" => "UGroup",
                _ => "Other"
            };
        }

        private string GetIluImage(int? levelId)
        {
            var level = _LevelsILU.FirstOrDefault(x => x.ILULevelId == levelId);
            if (level == null) return "Images/default.png";

            return level.ILULevelCode switch
            {
                "ITrainee" or "I" or "ILeader" or "LTrainee" or "LTraineeLeader" => "Images/I.png",
                "L" or "LLeader" or "UTrainee" or "ULeaderTrainee" => "Images/L.png",
                "U" or "ULeader" => "Images/U.png",
                _ => "Images/default.png"
            };
        }

        #region Filters

        //Filters
        public bool filters = false;
        private string searchString = "";
        public Color color = Color.Default;

        public int plantId;
        public int areaId;
        public int distributionId;

        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();
        List<Distribution> _distributions = new();

        public DateTime? filterDate = null;
        public int statusId;

        private async void ShowAreas()
        {

            if (plantId == 0)
            {
                areaId = 0;
                distributionId = 0;
                StateHasChanged();
                return;

            }
           
                areaId = 0;
                color = Color.Default;
                distributionId = 0;

            _areas = await AreaServices.GetAreas(plantId);
            _areas = _areas?.OrderBy(a => a.Description).ToList();

            StateHasChanged();
        }

        private async void ShowDist()
        {
            if (areaId == 0)
            {
                ShowAreas();
                return;
            }

            distributionId = 0;

            _distributions = await DistributionService.GetDistributionsWithCollections(plantId, areaId);

            StateHasChanged();
        }
        public void ActiveFilters()
        {
            filters = !filters;
            searchString = ""; 

            if (color == Color.Info)
                color = Color.Default;
            else
                color = Color.Info;
        }


        public void ClearFilters()
        {
            searchString = "";
            plantId = new();
            areaId= new();
            distributionId = new();
            filterDate = null;
            statusId = new();

            StateHasChanged();
        }

        private void OnDateChange(DateTime? date)
        {
            filterDate = date;
        }

     

        public string GetStatusLabel(int status)
        {
            return status switch
            {
                1 => "planned",
                2 => "inProgress",
                3 => "late",
                4 => "underReview",
                5 => "rejected",
                6 => "finished",
                _ => "",
            };
        }
        #endregion
    }
}