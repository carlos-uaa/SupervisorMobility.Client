using MudBlazor;
using SupervisorMobility.Client.Data.Entities;

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


        private List<int> _idsAreas { get; set; } = new();
        private List<Area> _ExistAreas { get; set; } = new();
        protected async override Task OnInitializedAsync()
        {
            if (!ExpertiseTable.Any())
            {
                for (int i = 0; i < 5; i++)
                {
                    ExpertiseTable.Add( new());
                }
            }
            _LevelsILU = await ILUServices.GetLevelsILU();

            _idsAreas = ExpertiseTable.Where(e => e.Distribution != null).Select(e => e.Distribution.AreaId).Distinct().ToList();

            _ExistAreas = await AreaServices.GetAreasByIds(_idsAreas);

            Task<List<Plant>> plantsTask = null;
            Task<List<Area>> areasTask = null;
            if (user.UserType == 3)
            {
                plantId = (int)user.PlantId;
                areaId = (int)user.AreaId;
                _distributions = await DistributionService.GetDistributionsWithCollections(plantId, areaId);
            }
            else
            {
                if (user.UserType == 1 || user.UserType == 5)
                {
                    plantsTask = PlantServices.GetPlants();
                }

                if (user.UserType == 2 || user.UserType == 5)
                {
                    plantId = (int)user.PlantId;
                    areasTask = AreaServices.GetAreas(plantId);
                }
            }

            if (plantsTask != null)
            {
                _plants = (await plantsTask).OrderBy(p => p.Description).ToList();
            }

            if (areasTask != null)
            {
                _areas = (await areasTask).OrderBy(a => a.Description).ToList();
            }

            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private void DateChanged(DateRange range, int index)
        {
            ExpertiseTable[index].AcquisitionDate = range.Start;
            ExpertiseTable[index].EndDate = range.End;
            Upd.InvokeAsync((ExpertiseTable[index], index));
        }
        //private void TextChanged(string val, int idx)
        //{
        //    ExpertiseTable[idx].Description = val;
        //    Upd.InvokeAsync((ExpertiseTable[idx], idx));
        //}
        //private void LevelChanged(string val, int idx)
        //{
        //    ExpertiseTable[idx].ILULevelId = val;
        //    Upd.InvokeAsync((ExpertiseTable[idx], idx));
        //}

        private void Delete(int index)
        {
            //KnowledgeTable.RemoveAt(index);
            Del.InvokeAsync(index);
        }

        private void AddHere()
        {
            ILURegister newILU = new ILURegister();
            //KnowledgeTable.Add(niu);
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

            if (filterDate.HasValue)
            {
                var fechaFiltro = filterDate.Value.Date;
                query = query
                    .Where(e => e.AcquisitionDate.HasValue &&
                                e.AcquisitionDate.Value.Date == fechaFiltro);
            }

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                string term = searchString.Trim().ToLower();

                query = query.Where(e =>
                    // ID o código
                    e.ILURegisterid.ToString().Contains(term) ||
                    (e.DistributionId?.ToString().Contains(term) ?? false) ||
                    (e.Distribution?.Code?.ToLower().Contains(term) ?? false) ||
                    (e.Distribution?.Description?.ToLower().Contains(term) ?? false) ||

                    // Datos de Área
                    (e.Distribution != null &&
                     _ExistAreas.Any(a => a.AreaId == e.Distribution.AreaId &&
                                          (a.Description?.ToLower().Contains(term) ?? false))) ||

                    // Datos de Planta
                    (e.Distribution != null &&
                     _ExistAreas.Any(a => a.AreaId == e.Distribution.AreaId &&
                                          (a.Description?.ToLower().Contains(term) ?? false))) ||

                    // Fecha como texto
                    (e.AcquisitionDate?.ToString("dd/MM/yyyy").Contains(term) ?? false) ||
                    (e.AcquisitionDate?.ToString("MMMM").ToLower().Contains(term) ?? false) || // mes texto
                    (e.AcquisitionDate?.Month.ToString().Contains(term) ?? false) // mes número
                );
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
                ClearFilters();
                StateHasChanged();
                return;

            }

            areaId = 0;
            color = Color.Default;
            ClearFilters();


            _areas = await AreaServices.GetAreas(plantId);
            _areas = _areas.OrderBy(a => a.Description).ToList();

            StateHasChanged();
        }

        public void ActiveFilters()
        {
            filters = !filters;
            searchString = ""; 

            //idFilter = new();
            //distributionId = new();
            //operationFlag = false;
            //operationId = new();
            //filterDate = null;
            //operatorId = new();
            //statusId = new();

            //SelectTableEvent0.ReloadServerData();

            if (color == Color.Info)
            {
                color = Color.Default;
            }
            else
            {
                color = Color.Info;
            }

        }


        public void ClearFilters()
        {
            searchString = "";
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