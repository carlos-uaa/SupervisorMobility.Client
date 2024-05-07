using static System.Net.WebRequestMethods;
using System.Text.Json.Serialization;
using static MudBlazor.CategoryTypes;
using SupervisorMobility.Client.Data.Entities;
using System.Runtime.CompilerServices;
using AutoMapper;
using MudBlazor;
using static SupervisorMobility.Client.Pages.Inicio.SOSProgramPage.SOS_Details;
using DocumentFormat.OpenXml.Presentation;
using System.Diagnostics;
using SupervisorMobility.Client.Pages.Inicio.SOSProgramPage;
using DocumentFormat.OpenXml.Bibliography;
using System.Net.Http.Headers;

namespace SupervisorMobility.Client.Services.SOS_Data_Service
{
    public class SOSDataService : ISOSDataService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly ISOSReviewService _ReviewService;
        private readonly IMapper _mapper;
        private readonly ISOSReviewService SOSServices;
        private readonly IJobObservationService JobObsServices;
        //Data
        //Control de jobs Observations
        public List<JobObservationNulls> _All_SOSJobobservation { get; set; } = new();
        //matris de datos general
        public Dictionary<(int, int), List<SOSRegisterJobObservation>?> SOS_Registers_Matrix { get; set; } = new Dictionary<(int, int), List<SOSRegisterJobObservation>?>();
        //relation User SV to Operation processed
        public Dictionary<int, SOSRegUserOperationRelationship?> SOS_Registers_UserOperationRelationship { get; set; } = new Dictionary<int, SOSRegUserOperationRelationship?>();
        //Resume Operations In Distribution
        public Dictionary<int, SosJobCount> OperationsInDistributionCount { get; set; } = new Dictionary<int, SosJobCount>();
       
        //Suggest
        //Control de jobs Observations suggestion
        public List<JobObservationNulls> _All_Suggested_SOSJobobservation { get; set; } = new();
        //Registro de SV - Operacion suggest
        public Dictionary<int, SOSRegUserOperationRelationship?> Suggested_SOS_Registers_UserOperationRelationship { get; set; } = new Dictionary<int, SOSRegUserOperationRelationship?>();
        //matris de datos suggestion general
        public Dictionary<(int, int), List<JobObservationNulls>?> Suggested_Registers_Matrix { get; set; } = new Dictionary<(int, int), List<JobObservationNulls>?>();
        //Jobs Externas a SOS Program de los SV (Planeadas y Programadas)
        public List<JobObservationNulls> _AnotherJobs { get; set; } = new();

        //RAW Data registers
        //Register Jobs without processed
        public List<SOSRegisterJobObservation> _SosRegisters { get; set; } = new();
        //Relation User SV to Operation without processed
        public List<SOSRegUserOperation> _SosRegistersrUserOperation { get; set; } = new();



        public List<Distribution> _distributions { get; set; } = new();
        public List<Operation> _All_Operations { get; set; } = new();


        public DateTime Startday { get; set; } = DateTime.Now;
        public int diasSeparate { get; set; } = 1;
        public int JobsPorDia { get; set; } = 1;
        public string jobCategoryStructureIds { get; set; } = "";

        public SOSDataService(HttpClient http, ISOSReviewService SosInject, IMapper MapInjec, ISOSReviewService sosInject, IJobObservationService jobObsServices)
        {
            SOSServices = sosInject;
            _mapper = MapInjec;
            _ReviewService = SosInject;
            _http = http;
            _options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
            };
            _options.Converters.Add(new IntToStringConverter());
            JobObsServices = jobObsServices;
        }

        public async Task<AsyncVoidMethodBuilder> SetSosJobObservation(int sos, List<Distribution> _distributions, string jobCategoryStructureIds)
        {
            _All_SOSJobobservation?.Clear();
            _All_Suggested_SOSJobobservation?.Clear();

            Suggested_SOS_Registers_UserOperationRelationship?.Clear();
            SOS_Registers_UserOperationRelationship?.Clear();

            SOS_Registers_Matrix?.Clear();
            Suggested_Registers_Matrix?.Clear();

            _SosRegisters?.Clear();
            _SosRegistersrUserOperation?.Clear();

            _AnotherJobs?.Clear();
            OperationsInDistributionCount?.Clear();

            _All_Operations?.Clear();

            SOSReviewProgram _sos_plan = await _ReviewService.GetSOSById(sos, true, true, true);

            this.jobCategoryStructureIds = jobCategoryStructureIds;
            this._distributions = _distributions;
            //Optenemos los registros relacionados al sos para asi crear data fantasma en la matriz de aquella que no existe
            //Solamente para tener render en ui
            //La data denota que algo no existe

            foreach (var item in _distributions)
            {
                this._All_Operations.AddRange(item.Operations);
            }

            this._SosRegisters = await SOSServices.GetSOSRegisters(_sos_plan.SOSid);
            //Referencia de jobs globales que esten dentro de la distribucion (Informativo solamente)
            this._AnotherJobs = _mapper.Map<List<JobObservationNulls>>(await JobObsServices.GetAllJobObservations(year: (int)_sos_plan.AplicationYear, SOSAnualId: _sos_plan.SOSid));


            foreach (var OpItem in _All_Operations)
            {
                for (int i = 1; i < 13; i++)
                {
                    int currentindex = i;
                    var matchingRegisters = _SosRegisters?
                       .Where(r => r.OperationId == OpItem.OperationId && r.Year <= _sos_plan.AplicationYear && r.Month == currentindex)
                       .ToList();

                    SOS_Registers_Matrix.Add((OpItem.OperationId, currentindex), matchingRegisters);
                }
            }

            foreach (var item in _SosRegisters)
            {
                _All_SOSJobobservation?.Add(item.JobObservation);
            }

            if (SOS_Registers_UserOperationRelationship?.Count == 0)
            {
                Console.WriteLine($"First Time: Create SOS_Registers_UserOperationRelationship");
                _SosRegistersrUserOperation = await SOSServices.GetSOSRegUserOperation(_sos_plan.SOSid);

                foreach (var item in _SosRegistersrUserOperation)
                {
                    SOSRegUserOperationRelationship regAux = new();
                    regAux.Register = item;
                    regAux.StateUpdate = false;
                    regAux.Exist = true;
                    SOS_Registers_UserOperationRelationship.Add((int)item.OperationId, regAux);
                }

                foreach (var op in _All_Operations)
                {
                    if (!SOS_Registers_UserOperationRelationship.TryGetValue(op.OperationId, out var context))
                    {
                        //si no existre se crea de manera artificial
                        SOSRegUserOperationRelationship regAux = new();
                        regAux.Register = new();
                        regAux.Exist = false;
                        regAux.StateUpdate = false;
                        SOS_Registers_UserOperationRelationship.Add(op.OperationId, regAux);
                    }
                }

            }
            foreach (var reg in _SosRegisters)
            {
                foreach (var dist in _distributions)
                {
                    if (dist.Operations.Any(o => o.OperationId == reg.OperationId))
                    {
                        if (OperationsInDistributionCount.ContainsKey(dist.DistributionId))
                        {
                            OperationsInDistributionCount[dist.DistributionId].TotalSosJobs++;
                            switch (reg.JobObservation.Status)
                            {
                                case 1:
                                    OperationsInDistributionCount[dist.DistributionId].sosplanned++;
                                    break;
                                case 2:
                                    OperationsInDistributionCount[dist.DistributionId].sosinProgress++;
                                    break;
                                case 3:
                                    OperationsInDistributionCount[dist.DistributionId].soslate++;
                                    break;
                                case 4:
                                    OperationsInDistributionCount[dist.DistributionId].sosunderReview++;
                                    break;
                                case 5:
                                    OperationsInDistributionCount[dist.DistributionId].sosrejected++;
                                    break;
                                case 6:
                                    OperationsInDistributionCount[dist.DistributionId].sosfinished++;
                                    break;
                                case 7:
                                    OperationsInDistributionCount[dist.DistributionId].sosprogramed++;
                                    break;
                            }

                        }
                        else
                        {
                            SosJobCount newReg = new SosJobCount();

                            newReg.TotalJobs = dist.Operations.Count();

                            newReg.TotalSosJobs++;
                            //1 @Localizer["planned"]
                            //2 @Localizer["inProgress"]
                            //3 @Localizer["late"]
                            //4 @Localizer["underReview"]
                            //5 @Localizer["rejected"]
                            //6 @Localizer["finished"]
                            switch (reg.JobObservation.Status)
                            {
                                case 1:
                                    newReg.sosplanned++;
                                    break;
                                case 2:
                                    newReg.sosinProgress++;
                                    break;
                                case 3:
                                    newReg.soslate++;
                                    break;
                                case 4:
                                    newReg.sosunderReview++;
                                    break;
                                case 5:
                                    newReg.sosrejected++;
                                    break;
                                case 6:
                                    newReg.sosfinished++;
                                    break;

                                case 7:
                                    newReg.sosprogramed++;
                                    break;
                            }

                            OperationsInDistributionCount.Add(dist.DistributionId, newReg);
                        }
                        break;
                    }
                }

            }

            foreach (var reg in _AnotherJobs)
            {
                foreach (var dist in _distributions)
                {
                    if (dist.Operations.Any(o => o.OperationId == reg.OperationId))
                    {
                        if (OperationsInDistributionCount.ContainsKey(dist.DistributionId))
                        {
                            OperationsInDistributionCount[dist.DistributionId].TotalOutSosJobs++;
                            switch (reg.Status)
                            {
                                case 1:
                                    OperationsInDistributionCount[dist.DistributionId].planned++;
                                    break;
                                case 2:
                                    OperationsInDistributionCount[dist.DistributionId].inProgress++;
                                    break;
                                case 3:
                                    OperationsInDistributionCount[dist.DistributionId].late++;
                                    break;
                                case 4:
                                    OperationsInDistributionCount[dist.DistributionId].underReview++;
                                    break;
                                case 5:
                                    OperationsInDistributionCount[dist.DistributionId].rejected++;
                                    break;
                                case 6:
                                    OperationsInDistributionCount[dist.DistributionId].finished++;
                                    break;

                                case 7:
                                    OperationsInDistributionCount[dist.DistributionId].programed++;
                                    break;
                            }

                        }
                        else
                        {
                            SosJobCount newReg = new SosJobCount();

                            newReg.TotalJobs = dist.Operations.Count();

                            newReg.TotalOutSosJobs++;
                            //1 @Localizer["planned"]
                            //2 @Localizer["inProgress"]
                            //3 @Localizer["late"]
                            //4 @Localizer["underReview"]
                            //5 @Localizer["rejected"]
                            //6 @Localizer["finished"]
                            switch (reg.Status)
                            {
                                case 1:
                                    newReg.planned++;
                                    break;
                                case 2:
                                    newReg.inProgress++;
                                    break;
                                case 3:
                                    newReg.late++;
                                    break;
                                case 4:
                                    newReg.underReview++;
                                    break;
                                case 5:
                                    newReg.rejected++;
                                    break;
                                case 6:
                                    newReg.finished++;
                                    break;

                                case 7:
                                    newReg.programed++;
                                    break;
                            }

                            OperationsInDistributionCount.Add(dist.DistributionId, newReg);
                        }
                        break;
                    }
                }

            }

            Suggested_Registers_Matrix?.Clear();

            foreach (var dist in _distributions)
            {
                for (int i = 1; i < 13; i++)
                {
                    int currentindex = i;
                    var matchingRegisters = _All_Suggested_SOSJobobservation?
                       .Where(r => r.DistributionId == dist.DistributionId && r.StartDate.Value.Month == currentindex)
                       .ToList();

                    Suggested_Registers_Matrix.Add((dist.DistributionId, currentindex), matchingRegisters);
                }
            }

            Debug.WriteLine($"Debug: _Allsos{_All_SOSJobobservation.Count}");

            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> SetSugestionJobObservation(SOSReviewProgram _sos_plan, List<DistSelect>  Dist_Manager,
            List<User> SV_Manager,  int diasSeparate, DateTime Startday, int JobsPorDia)
        {
            this.diasSeparate = diasSeparate;
            this.Startday = Startday;
            this.JobsPorDia = JobsPorDia;
            Suggested_SOS_Registers_UserOperationRelationship?.Clear();
            try
            {
                TimeSpan? startHour = new TimeSpan(00, 00, 00);


                if (_All_Suggested_SOSJobobservation.Count == 0)
                {
                    foreach (var item in Dist_Manager)
                    {

                        if (item.isSelected)
                        {
                            foreach (var op in item.distribution.Operations)
                            {
                                //check if exist
                                if (!_All_SOSJobobservation.Any(j => j.OperationId == op.OperationId))
                                {

                                    JobObservationNulls _newSuggestion = new();

                                    int supervisorIndex = _All_Operations.IndexOf(op) % SV_Manager.Count;

                                    // Obtener el SupervisorId de SV_Manager usando el índice calculado
                                    int supervisorId = SV_Manager[supervisorIndex].UserId;

                                    // Asignar el SupervisorId a _newSuggestion
                                    _newSuggestion.SupervisorId = supervisorId;


                                    SOSRegUserOperationRelationship regAux = new();
                                    regAux.Register = new();
                                    regAux.Register.SOSReviewProgramid = _sos_plan.SOSid;
                                    regAux.Register.OperationId = op.OperationId;
                                    regAux.Register.SupervisorId = supervisorId;
                                    regAux.Register.Supervisor = SV_Manager.Find(u => u.UserId == supervisorId);


                                    regAux.Exist = false;
                                    regAux.StateUpdate = true;
                                    Suggested_SOS_Registers_UserOperationRelationship.Add(op.OperationId, regAux);


                                    _newSuggestion.PlantId = (int)_sos_plan.PlantId;
                                    _newSuggestion.Plant = _sos_plan.Plant;
                                    _newSuggestion.AreaId = (int)_sos_plan.AreaId;
                                    _newSuggestion.Area = _sos_plan.Area;

                                    _newSuggestion.Distribution = _distributions.Find(d => d.Operations.Any(o => o.OperationId == op.OperationId));
                                    _newSuggestion.DistributionId = _newSuggestion.Distribution.DistributionId;

                                    _newSuggestion.Operation = op;
                                    _newSuggestion.OperationId = op.OperationId;
                                    _newSuggestion.Option = 2;
                                    _newSuggestion.Type = 3;
                                    _newSuggestion.Status = 7;
                                    _newSuggestion.SectionIds = jobCategoryStructureIds;

                                    _newSuggestion.IsActive = true;

                                    DateTime parsedDate = Startday;
                                    parsedDate = await FindNextAvailableDate(parsedDate, true, supervisorId);

                                    _newSuggestion.StartDate = parsedDate;
                                    _newSuggestion.PlannedStartDate = parsedDate;


                                    _All_Suggested_SOSJobobservation.Add(_newSuggestion);
                                }

                            }

                        }

                    }
                    //var OperationsInDist = _All_Operations.Where(o => o.id)


                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error CreateSuggestion: {ex.Message}");

            }
            finally
            {
                //ShowLoading = false;
                //isButtonDisabled = false;
            }

            Suggested_Registers_Matrix?.Clear();


            foreach (var dist in _distributions)
            {
                for (int i = 1; i < 13; i++)
                {
                    int currentindex = i;
                    var matchingRegisters = _All_Suggested_SOSJobobservation?
                       .Where(r => r.DistributionId == dist.DistributionId && r.StartDate.Value.Month == currentindex)
                       .ToList();

                    Suggested_Registers_Matrix.Add((dist.DistributionId, currentindex), matchingRegisters);
                }
            }
            return new AsyncVoidMethodBuilder();
        }
        public async Task<AsyncVoidMethodBuilder> SetNewConfigSugestionJobObservation(SOSReviewProgram _sos_plan, List<DistSelect> Dist_Manager,
            List<User> SV_Manager, int diasSeparate, DateTime Startday, int JobsPorDia, int OptionRandom)
        {

            _All_Suggested_SOSJobobservation?.Clear();
            Suggested_SOS_Registers_UserOperationRelationship?.Clear();

            Random random = new Random();

            switch (OptionRandom)
            {
                case 0:
                    _All_Operations?.Clear();
                    foreach (var item in Dist_Manager.Where(item => item.isSelected).Select(item => item.distribution))
                    {
                        _All_Operations.AddRange(item.Operations);
                    }
                    break;
                case 1:
                    _All_Operations?.Clear();
                    foreach (var item in Dist_Manager.Where(item => item.isSelected).Select(item => item.distribution))
                    {
                        _All_Operations.AddRange(item.Operations);
                    }
                    _All_Operations = _All_Operations.OrderBy(x => random.Next()).ToList();
                    //OperationIterator = OperationIterator.OrderBy(x => random.Next()).ToList();

                    break;
                case 2:
                    var _distributionsRandom = Dist_Manager.Where(item => item.isSelected).Select(item => item.distribution).OrderBy(x => random.Next()).ToList();
                    _All_Operations?.Clear();
                    foreach (var item in _distributionsRandom)
                    {
                        _All_Operations.AddRange(item.Operations);
                    }
                    break;
            }

            TimeSpan? startHour = new TimeSpan(00, 00, 00);

            try
            {

                if (_All_Suggested_SOSJobobservation.Count == 0)
                {
                    //foreach (var op in _All_Operations)
                    foreach (var op in _All_Operations)
                    {
                        if (!_All_SOSJobobservation.Any(j => j.OperationId == op.OperationId))
                        {

                            JobObservationNulls _newSuggestion = new();

                            int supervisorIndex = _All_Operations.IndexOf(op) % SV_Manager.Count;

                            // Obtener el SupervisorId de SV_Manager usando el índice calculado
                            int supervisorId = SV_Manager[supervisorIndex].UserId;

                            // Asignar el SupervisorId a _newSuggestion
                            _newSuggestion.SupervisorId = supervisorId;


                            SOSRegUserOperationRelationship regAux = new();
                            regAux.Register = new();
                            regAux.Register.SOSReviewProgramid = _sos_plan.SOSid;
                            regAux.Register.OperationId = op.OperationId;
                            regAux.Register.SupervisorId = supervisorId;
                            regAux.Register.Supervisor = SV_Manager.Find(u => u.UserId == supervisorId);


                            regAux.Exist = false;
                            regAux.StateUpdate = true;
                            Suggested_SOS_Registers_UserOperationRelationship.Add(op.OperationId, regAux);



                            _newSuggestion.PlantId = (int)_sos_plan.PlantId;
                            _newSuggestion.Plant = _sos_plan.Plant;
                            _newSuggestion.AreaId = (int)_sos_plan.AreaId;
                            _newSuggestion.Area = _sos_plan.Area;

                            _newSuggestion.Distribution = _distributions.Find(d => d.Operations.Any(o => o.OperationId == op.OperationId));
                            _newSuggestion.DistributionId = _newSuggestion.Distribution.DistributionId;

                            _newSuggestion.Operation = op;
                            _newSuggestion.OperationId = op.OperationId;


                            _newSuggestion.Option = 2;
                            _newSuggestion.Type = 3;
                            _newSuggestion.Status = 7;
                            _newSuggestion.SectionIds = jobCategoryStructureIds;
                            _newSuggestion.IsActive = true;

                            DateTime parsedDate = Startday;
                            parsedDate = await FindNextAvailableDate(parsedDate, true, supervisorId);

                            _newSuggestion.StartDate = parsedDate;
                            _newSuggestion.PlannedStartDate = parsedDate;



                            _All_Suggested_SOSJobobservation.Add(_newSuggestion);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error CreateSuggestion: {ex.Message}");

            }
            finally
            {

               
            }

            return new AsyncVoidMethodBuilder();
        }


        public async Task<DateTime> FindNextAvailableDate(DateTime startAvailabeDate, bool isSuggest, int id_SV = 0)
        {
            int yearToCheck = startAvailabeDate.Year;
            int initialDayOfYear = startAvailabeDate.DayOfYear;

            while (startAvailabeDate.Year == yearToCheck || startAvailabeDate.DayOfYear < initialDayOfYear)
            {
                if (isSuggest)
                {
                    if (!(await IsDateSuggestAlreadyUsed(startAvailabeDate, id_SV)) && !(await IsWeekend(startAvailabeDate)))
                    {
                        return startAvailabeDate;
                    }
                }
                else
                {
                    if (!(await IsDateAlreadyUsed(startAvailabeDate)) && !(await IsWeekend(startAvailabeDate)))
                    {
                        return startAvailabeDate;
                    }
                }

                startAvailabeDate = startAvailabeDate.AddDays(diasSeparate);

                if (startAvailabeDate > DateTime.MaxValue)
                {
                    throw new InvalidOperationException("No se pudo encontrar una fecha disponible en el año actual.");
                }
            }



            return startAvailabeDate.AddDays(-1);
        }

        private async Task<bool> IsDateAlreadyUsed(DateTime dateToCheck)
        {
            int count = _All_SOSJobobservation.Count(o => o.PlannedStartDate.Value.Date == dateToCheck.Date);
            return count > 1;
        }

        private async Task<bool> IsDateSuggestAlreadyUsed(DateTime dateToCheck, int id_SV)
        {
            if (id_SV != 0)
            {
                int count = _All_Suggested_SOSJobobservation.Where(o => o.PlannedStartDate.Value.Date == dateToCheck.Date && o.SupervisorId == id_SV).Count();
                return count >= JobsPorDia;
            }

            return true;

        }

        private async Task<bool> IsWeekend(DateTime dateToCheck)
        {
            return dateToCheck.DayOfWeek == DayOfWeek.Saturday || dateToCheck.DayOfWeek == DayOfWeek.Sunday;
        }

        public List<JobObservationNulls> Get_AllSos_Month(int month)
        {
            return _All_SOSJobobservation.Where(j => j.StartDate.Value.Month == month || j.PlannedStartDate.Value.Month == month).ToList();
        }

        public List<JobObservationNulls> Get_AllSos_Month_Dist(int month, int dist_Id)
        {
            return _All_SOSJobobservation.Where(j => j.StartDate.Value.Month == month || j.PlannedStartDate.Value.Month == month && j.DistributionId == dist_Id).ToList();
        }

        public List<JobObservationNulls> Get_AllSos_Dist(int dist_Id)
        {
            return _All_SOSJobobservation.Where(j => j.DistributionId == dist_Id).ToList();
        }

        public Dictionary<(int, int), List<SOSRegisterJobObservation>?> Get_Registers_Matrix_Month(int month)
        {
            return SOS_Registers_Matrix.Where(kv => kv.Key.Item2 == month).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public Dictionary<(int, int), List<SOSRegisterJobObservation>?> Get_Registers_Matri_Month_Dist(int month, int dist_Id)
        {
            var filterbyMont = SOS_Registers_Matrix.Where(kv => kv.Key.Item2 == month).ToDictionary(kv => kv.Key, kv => kv.Value);

            var filteredDictionary = new Dictionary<(int, int), List<SOSRegisterJobObservation>?>();

            // Recorrer cada entrada en el diccionario SOS_Registers_Matrix
            foreach (var kvp in filterbyMont)
            {
                // Obtener la lista de observaciones asociada a la clave actual
                List<SOSRegisterJobObservation>? observations = kvp.Value;

                // Verificar si la lista de observaciones no es nula y contiene al menos una observación con el DistributionId deseado
                if (observations != null && observations.Any(obs => obs.JobObservation?.DistributionId == dist_Id))
                {
                    // Agregar la entrada al nuevo diccionario filtrado
                    filteredDictionary.Add(kvp.Key, observations);
                }
            }

            return filteredDictionary;

        }

        public Dictionary<(int, int), List<SOSRegisterJobObservation>?> Get_Registers_Matrix_Dist(int dist_Id)
        {
            var filteredDictionary = new Dictionary<(int, int), List<SOSRegisterJobObservation>?>();

            // Recorrer cada entrada en el diccionario SOS_Registers_Matrix
            foreach (var kvp in SOS_Registers_Matrix)
            {
                // Obtener la lista de observaciones asociada a la clave actual
                List<SOSRegisterJobObservation>? observations = kvp.Value;

                // Verificar si la lista de observaciones no es nula y contiene al menos una observación con el DistributionId deseado
                if (observations != null && observations.Any(obs => obs.JobObservation?.DistributionId == dist_Id))
                {
                    // Agregar la entrada al nuevo diccionario filtrado
                    filteredDictionary.Add(kvp.Key, observations);
                }
            }

            return filteredDictionary;
        }

        public Dictionary<int, SOSRegUserOperationRelationship?> Get_SOS_Registers_UserOperationRelationship(int dist_Id)
        {

            var targetDistribution = _distributions.FirstOrDefault(d => d.DistributionId == dist_Id);

            if (targetDistribution != null)
            {
                var operationsInDistribution = targetDistribution.Operations;

                var filteredRelationships = SOS_Registers_UserOperationRelationship
                  .Where(kvp => kvp.Value != null && operationsInDistribution
                      .Any(op => op.OperationId == kvp.Key))
                  .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                return filteredRelationships;
            }
            else
            {
                Console.WriteLine($"No se encontró la Distribution con ID: {dist_Id}");
            }


            return null;
        }

        public List<JobObservationNulls> Get_Suggest_AllSos_Month(int month)
        {
            return _All_Suggested_SOSJobobservation.Where(j => j.StartDate.Value.Month == month || j.PlannedStartDate.Value.Month == month).ToList();
        }

        public List<JobObservationNulls> Get_Suggest_AllSos_Month_Dist(int month, int dist_Id)
        {
            return _All_Suggested_SOSJobobservation.Where(j => j.StartDate.Value.Month == month || j.PlannedStartDate.Value.Month == month && j.DistributionId == dist_Id).ToList();
        }

        public List<JobObservationNulls> Get_Suggest_AllSos_Dist(int dist_Id)
        {
            return _All_Suggested_SOSJobobservation.Where(j => j.DistributionId == dist_Id).ToList();
        }

        public Dictionary<(int, int), List<JobObservationNulls>?> Get_Suggest_Registers_Matrix_Month(int month)
        {
            return Suggested_Registers_Matrix.Where(kv => kv.Key.Item2 == month).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public Dictionary<(int, int), List<JobObservationNulls>?> Get_Suggest_Registers_Matri_Month_Dist(int month, int dist_Id)
        {
            var filteredDictionary = Suggested_Registers_Matrix.Where(kv => kv.Key.Item1 == dist_Id && kv.Key.Item2 == month).ToDictionary(kv => kv.Key, kv => kv.Value);

            return filteredDictionary;
        }

        public Dictionary<(int, int), List<JobObservationNulls>?> Get_Suggest_Registers_Matrix_Dist(int dist_Id)
        {
            var filteredDictionary = Suggested_Registers_Matrix.Where(kv => kv.Key.Item1 == dist_Id).ToDictionary(kv => kv.Key, kv => kv.Value);

            return filteredDictionary;
        }
        public Dictionary<int, SOSRegUserOperationRelationship?> Get_Suggested_SOS_Registers_UserOperationRelationship(int dist_Id)
        {

            var targetDistribution = _distributions.FirstOrDefault(d => d.DistributionId == dist_Id);

            if (targetDistribution != null)
            {
                var operationsInDistribution = targetDistribution.Operations;

                var filteredRelationships = Suggested_SOS_Registers_UserOperationRelationship
                  .Where(kvp => kvp.Value != null && operationsInDistribution
                      .Any(op => op.OperationId == kvp.Key))
                  .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                return filteredRelationships;
            }
            else
            {
                Console.WriteLine($"No se encontró la Distribution con ID: {dist_Id}");
            }


            return null;
        }

        public void UpdateJobItem(JobObservation UpdatedItem)
        {
            JobObservationNulls itemInService = _All_SOSJobobservation.Find(j => j.JobObservationId == UpdatedItem.JobObservationId);

            _mapper.Map(UpdatedItem, itemInService);
        }


    }


}
