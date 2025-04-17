using System.Text.Json.Serialization;
using System.Runtime.CompilerServices;
using AutoMapper;
using static SupervisorMobility.Client.Pages.Inicio.SOSProgramPage.SOS_Details;
using System.Diagnostics;
using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;
using SupervisorMobility.Client.Data.Entities;

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
        public int Year { get; set; } = 1;
        public int YearLoop { get; set; } = 0;
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
                    if (dist.Operations != null && dist.Operations.Any(o => o.OperationId == reg.Operations?.FirstOrDefault()?.OperationId))
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

        //public async Task<AsyncVoidMethodBuilder> SetSugestionJobObservation(SOSReviewProgram _sos_plan, List<DistSelect> Dist_Manager,
        //    List<User> SV_Manager, int diasSeparate, DateTime Startday, int JobsPorDia)
        //{
        //    List<JobObservation> availableJobs = await JobObsServices.GetAllNextYearJobsObservations(_sos_plan.PlantId, _sos_plan.AreaId, (int)_sos_plan.AplicationYear);

        //    this.diasSeparate = diasSeparate;
        //    this.Startday = Startday;
        //    this.JobsPorDia = JobsPorDia;
        //    this.Year = Startday.Year;
        //    this.YearLoop = 0;

        //    Suggested_SOS_Registers_UserOperationRelationship?.Clear();
        //    try
        //    {
        //        TimeSpan? startHour = new TimeSpan(00, 00, 00);


        //        if (_All_Suggested_SOSJobobservation.Count == 0)
        //        {
        //            foreach (var item in Dist_Manager)
        //            {

        //                if (item.isSelected)
        //                {
        //                    foreach (var op in item.distribution.Operations)
        //                    {
        //                        // Obtener el SupervisorId de SV_Manager usando el índice calculado
        //                        int supervisorIndex = _All_Operations.IndexOf(op) % SV_Manager.Count;
        //                        int supervisorId = SV_Manager[supervisorIndex].UserId;

        //                        if (!_All_SOSJobobservation.Any(j => j.Operations.Any(jo => jo.OperationId == op.OperationId)))
        //                        {

        //                            List<JobObservation> findedJobs = availableJobs.Where(j => j.DistributionId == item.distribution.DistributionId).ToList();

        //                            JobObservation? findedJob = findedJobs.Find(j => j.Operations.Any(jo => jo.OperationId == op.OperationId));

        //                            if (findedJob != null)
        //                            {
        //                                JobObservationNulls _newSuggestionExist = _mapper.Map<JobObservationNulls>(findedJob);

        //                                // Asignar el SupervisorId a _newSuggestion
        //                                if (findedJob.SupervisorId == null)
        //                                {
        //                                    _newSuggestionExist.SupervisorId = supervisorId;
        //                                }


        //                                SOSRegUserOperationRelationship regAuxExist = new();
        //                                regAuxExist.Register = new();
        //                                regAuxExist.Register.SOSReviewProgramid = _sos_plan.SOSid;
        //                                regAuxExist.Register.OperationId = op.OperationId;
        //                                regAuxExist.Register.SupervisorId = supervisorId;
        //                                regAuxExist.Register.Supervisor = SV_Manager.Find(u => u.UserId == supervisorId);


        //                                regAuxExist.Exist = false;
        //                                regAuxExist.StateUpdate = true;
        //                                Suggested_SOS_Registers_UserOperationRelationship.Add(op.OperationId, regAuxExist);


        //                                _newSuggestionExist.Plant = _sos_plan.Plant;
        //                                _newSuggestionExist.Area = _sos_plan.Area;
        //                                _newSuggestionExist.Distribution = _distributions.Find(d => d.Operations.Any(o => o.OperationId == op.OperationId));

        //                                _newSuggestionExist.Supervisor = SV_Manager.Find(u => u.UserId == supervisorId);

        //                                var operationsList = _newSuggestionExist.Operations?.ToList() ?? new List<Operation>();
        //                                if (!operationsList.Any(o => o.OperationId == op.OperationId))
        //                                {
        //                                    operationsList.Add(_All_Operations.First(o => o.OperationId == op.OperationId));
        //                                }
        //                                _newSuggestionExist.Operations = operationsList;

        //                                //_newSuggestionExist.Operation = op;
        //                                //if (_newSuggestionExist.OperationId == 0)
        //                                //{
        //                                //    _newSuggestionExist.OperationId = op.OperationId;
        //                                //}

        //                                _newSuggestionExist.Option = 2;
        //                                _newSuggestionExist.Type = 3;
        //                                _newSuggestionExist.Status = 7;
        //                                _newSuggestionExist.SectionIds = jobCategoryStructureIds;

        //                                _newSuggestionExist.IsActive = true;

        //                                DateTime parsedDate = Startday;
        //                                parsedDate = await FindNextAvailableDate((DateTime)_newSuggestionExist.StartDate, true, supervisorId);

        //                                _newSuggestionExist.StartDate = parsedDate;
        //                                _newSuggestionExist.PlannedStartDate = parsedDate;

        //                                //se elimina la job del siguiene año para evitar duplicados
        //                                availableJobs.RemoveAll(j => j.JobObservationId == findedJob.JobObservationId);

        //                                _All_Suggested_SOSJobobservation.Add(_newSuggestionExist);
        //                            }
        //                            else
        //                            {
        //                                JobObservationNulls _newSuggestion = new();


        //                                // Asignar el SupervisorId a _newSuggestion
        //                                _newSuggestion.SupervisorId = supervisorId;


        //                                SOSRegUserOperationRelationship regAux = new();
        //                                regAux.Register = new();
        //                                regAux.Register.SOSReviewProgramid = _sos_plan.SOSid;
        //                                regAux.Register.OperationId = op.OperationId;
        //                                regAux.Register.SupervisorId = supervisorId;
        //                                regAux.Register.Supervisor = SV_Manager.Find(u => u.UserId == supervisorId);


        //                                regAux.Exist = false;
        //                                regAux.StateUpdate = true;
        //                                Suggested_SOS_Registers_UserOperationRelationship.Add(op.OperationId, regAux);


        //                                _newSuggestion.PlantId = (int)_sos_plan.PlantId;
        //                                _newSuggestion.Plant = _sos_plan.Plant;
        //                                _newSuggestion.AreaId = (int)_sos_plan.AreaId;
        //                                _newSuggestion.Area = _sos_plan.Area;

        //                                _newSuggestion.Distribution = _distributions.Find(d => d.Operations.Any(o => o.OperationId == op.OperationId));
        //                                _newSuggestion.DistributionId = _newSuggestion.Distribution.DistributionId;

        //                                //_newSuggestion.Operation = op;
        //                                //_newSuggestion.OperationId = op.OperationId;


        //                                _newSuggestion.Supervisor = SV_Manager.Find(u => u.UserId == supervisorId);

        //                                var operationsList = _newSuggestion.Operations?.ToList() ?? new List<Operation>();
        //                                if (!operationsList.Any(o => o.OperationId == op.OperationId))
        //                                {
        //                                    operationsList.Add(_All_Operations.First(o => o.OperationId == op.OperationId));
        //                                }
        //                                _newSuggestion.Operations = operationsList;

        //                                _newSuggestion.Option = 2;
        //                                _newSuggestion.Type = 3;
        //                                _newSuggestion.Status = 7;
        //                                _newSuggestion.SectionIds = jobCategoryStructureIds;

        //                                _newSuggestion.IsActive = true;

        //                                DateTime parsedDate = Startday;
        //                                parsedDate = await FindNextAvailableDate(parsedDate, true, supervisorId);

        //                                _newSuggestion.StartDate = parsedDate;
        //                                _newSuggestion.PlannedStartDate = parsedDate;


        //                                _All_Suggested_SOSJobobservation.Add(_newSuggestion);
        //                            }

        //                        }

        //                    }
        //                }

        //            }
        //            //var OperationsInDist = _All_Operations.Where(o => o.id)


        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error CreateSuggestion: {ex.Message}");

        //    }
        //    finally
        //    {
        //        //ShowLoading = false;
        //        //isButtonDisabled = false;
        //    }

        //    Suggested_Registers_Matrix?.Clear();


        //    foreach (var dist in _distributions)
        //    {
        //        for (int i = 1; i < 13; i++)
        //        {
        //            int currentindex = i;
        //            var matchingRegisters = _All_Suggested_SOSJobobservation?
        //               .Where(r => r.DistributionId == dist.DistributionId && r.StartDate.Value.Month == currentindex)
        //               .ToList();

        //            Suggested_Registers_Matrix.Add((dist.DistributionId, currentindex), matchingRegisters);
        //        }
        //    }
        //    return new AsyncVoidMethodBuilder();
        //}



        //public async Task<AsyncVoidMethodBuilder> SetNewConfigSugestionJobObservation(SOSReviewProgram _sos_plan, List<DistSelect> Dist_Manager,
        //    List<User> SV_Manager, int diasSeparate, DateTime Startday, int JobsPorDia, int OptionRandom)
        //{

        //    List<JobObservation>? availableJobs = await JobObsServices.GetAllNextYearJobsObservations(_sos_plan.PlantId, _sos_plan.AreaId, (int)_sos_plan.AplicationYear);

        //    this.diasSeparate = diasSeparate;
        //    this.Startday = Startday;
        //    this.JobsPorDia = JobsPorDia;
        //    this.Year = Startday.Year;
        //    this.YearLoop = 0;

        //    _All_Suggested_SOSJobobservation?.Clear();
        //    Suggested_SOS_Registers_UserOperationRelationship?.Clear();

        //    Random random = new Random();

        //    switch (OptionRandom)
        //    {
        //        case 0:
        //            _All_Operations?.Clear();
        //            foreach (var item in Dist_Manager.Where(item => item.isSelected).Select(item => item.distribution))
        //            {
        //                _All_Operations.AddRange(item.Operations);
        //            }
        //            break;
        //        case 1:
        //            _All_Operations?.Clear();
        //            foreach (var item in Dist_Manager.Where(item => item.isSelected).Select(item => item.distribution))
        //            {
        //                _All_Operations.AddRange(item.Operations);
        //            }
        //            _All_Operations = _All_Operations.OrderBy(x => random.Next()).ToList();
        //            //OperationIterator = OperationIterator.OrderBy(x => random.Next()).ToList();

        //            break;
        //        case 2:
        //            var _distributionsRandom = Dist_Manager.Where(item => item.isSelected).Select(item => item.distribution).OrderBy(x => random.Next()).ToList();
        //            _All_Operations?.Clear();
        //            foreach (var item in _distributionsRandom)
        //            {
        //                _All_Operations.AddRange(item.Operations);
        //            }
        //            break;
        //    }

        //    TimeSpan? startHour = new TimeSpan(00, 00, 00);

        //    List<Distribution> selectedDistributions = Dist_Manager.Where(item => item.isSelected).Select(item => item.distribution).ToList();
        //    try
        //    {

        //        if (_All_Suggested_SOSJobobservation.Count == 0)
        //        {

        //            foreach (var op in _All_Operations)
        //            {
        //                if (!_All_SOSJobobservation.Any(j => j.Operations.Any(jo => jo.OperationId == op.OperationId)))
        //                {

        //                    int supervisorIndex = _All_Operations.IndexOf(op) % SV_Manager.Count;


        //                    int supervisorId = SV_Manager[supervisorIndex].UserId;


        //                    //Buscamos el id de distribucion al que pertenece la operacion
        //                    int distID = selectedDistributions.Find(d => d.Operations.Any(o => o.OperationId == op.OperationId)).DistributionId;

        //                    //Filtrar las jobs siguiente año disponibles por distribucion para evitar usar Jobs de alguna otra distribucion
        //                    List<JobObservation> findedJobs = availableJobs?.Where(j => j.DistributionId == distID).ToList();
        //                    //(En caso de que esten incompletas)
        //                    //Buscar alguna que tenga la operacion
        //                    //o en su defecto usar la primera job disponible sin operacion para no tener registros en bdd
        //                    JobObservation? findedJob = findedJobs.Find(j => j.Operations.Any(jo => jo.OperationId == op.OperationId));



        //                    if (findedJob != null)
        //                    {
        //                        JobObservationNulls _newSuggestionExist = _mapper.Map<JobObservationNulls>(findedJob);


        //                        if (findedJob.SupervisorId == null)
        //                        {
        //                            _newSuggestionExist.SupervisorId = supervisorId;
        //                        }


        //                        SOSRegUserOperationRelationship regAuxExist = new();
        //                        regAuxExist.Register = new();
        //                        regAuxExist.Register.SOSReviewProgramid = _sos_plan.SOSid;
        //                        regAuxExist.Register.OperationId = op.OperationId;
        //                        regAuxExist.Register.SupervisorId = supervisorId;
        //                        regAuxExist.Register.Supervisor = SV_Manager.Find(u => u.UserId == supervisorId);


        //                        regAuxExist.Exist = false;
        //                        regAuxExist.StateUpdate = true;
        //                        Suggested_SOS_Registers_UserOperationRelationship.Add(op.OperationId, regAuxExist);

        //                        _newSuggestionExist.Plant = _sos_plan.Plant;
        //                        _newSuggestionExist.Area = _sos_plan.Area;
        //                        _newSuggestionExist.Distribution = _distributions.Find(d => d.Operations.Any(o => o.OperationId == op.OperationId));

        


        //                        _newSuggestionExist.Supervisor = SV_Manager.Find(u => u.UserId == supervisorId);

        //                        var operationsList = _newSuggestionExist.Operations?.ToList() ?? new List<Operation>();
        //                        if (!operationsList.Any(o => o.OperationId == op.OperationId))
        //                        {
        //                            operationsList.Add(_All_Operations.First(o => o.OperationId == op.OperationId));
        //                        }
        //                        _newSuggestionExist.Operations = operationsList;

        //                        _newSuggestionExist.Option = 2;
        //                        _newSuggestionExist.Type = 3;
        //                        _newSuggestionExist.Status = 7;
        //                        _newSuggestionExist.SectionIds = jobCategoryStructureIds;
        //                        _newSuggestionExist.IsActive = true;

        //                        DateTime parsedDate = Startday;
        //                        parsedDate = await FindNextAvailableDate((DateTime)_newSuggestionExist.StartDate, true, supervisorId);

        //                        _newSuggestionExist.StartDate = parsedDate;
        //                        _newSuggestionExist.PlannedStartDate = parsedDate;


        //                        availableJobs.RemoveAll(j => j.JobObservationId == findedJob.JobObservationId);
        //                        _All_Suggested_SOSJobobservation.Add(_newSuggestionExist);
        //                    }
        //                    else
        //                    {
        //                        JobObservationNulls _newSuggestion = new();
        //                        // Asignar el SupervisorId a _newSuggestion
        //                        _newSuggestion.SupervisorId = supervisorId;


        //                        SOSRegUserOperationRelationship regAux = new();
        //                        regAux.Register = new();
        //                        regAux.Register.SOSReviewProgramid = _sos_plan.SOSid;
        //                        regAux.Register.OperationId = op.OperationId;
        //                        regAux.Register.SupervisorId = supervisorId;
        //                        regAux.Register.Supervisor = SV_Manager.Find(u => u.UserId == supervisorId);


        //                        regAux.Exist = false;
        //                        regAux.StateUpdate = true;
        //                        Suggested_SOS_Registers_UserOperationRelationship.Add(op.OperationId, regAux);



        //                        _newSuggestion.PlantId = (int)_sos_plan.PlantId;
        //                        _newSuggestion.Plant = _sos_plan.Plant;
        //                        _newSuggestion.AreaId = (int)_sos_plan.AreaId;
        //                        _newSuggestion.Area = _sos_plan.Area;

        //                        _newSuggestion.Distribution = _distributions.Find(d => d.Operations.Any(o => o.OperationId == op.OperationId));
        //                        _newSuggestion.DistributionId = _newSuggestion.Distribution.DistributionId;

        //                        //_newSuggestion.Operation = op;
        //                        //_newSuggestion.OperationId = op.OperationId;

        //                        var operationsList = _newSuggestion.Operations?.ToList() ?? new List<Operation>();
        //                        if (!operationsList.Any(o => o.OperationId == op.OperationId))
        //                        {
        //                            operationsList.Add(_All_Operations.First(o => o.OperationId == op.OperationId));
        //                        }
        //                        _newSuggestion.Operations = operationsList;


        //                        _newSuggestion.Supervisor = SV_Manager.Find(u => u.UserId == supervisorId);

        //                        _newSuggestion.Option = 2;
        //                        _newSuggestion.Type = 3;
        //                        _newSuggestion.Status = 7;
        //                        _newSuggestion.SectionIds = jobCategoryStructureIds;
        //                        _newSuggestion.IsActive = true;

        //                        DateTime parsedDate = Startday;
        //                        parsedDate = await FindNextAvailableDate(parsedDate, true, supervisorId);

        //                        _newSuggestion.StartDate = parsedDate;
        //                        _newSuggestion.PlannedStartDate = parsedDate;


        //                        _All_Suggested_SOSJobobservation.Add(_newSuggestion);
        //                    }
        //                }
        //            }
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error CreateSuggestion: {ex.Message}");

        //    }
        //    finally
        //    {
        //        Suggested_Registers_Matrix?.Clear();


        //        foreach (var dist in _distributions)
        //        {
        //            for (int i = 1; i < 13; i++)
        //            {
        //                int currentindex = i;
        //                var matchingRegisters = _All_Suggested_SOSJobobservation?
        //                   .Where(r => r.DistributionId == dist.DistributionId && r.StartDate.Value.Month == currentindex)
        //                   .ToList();

        //                Suggested_Registers_Matrix.Add((dist.DistributionId, currentindex), matchingRegisters);
        //            }
        //        }

        //    }

        //    return new AsyncVoidMethodBuilder();
        //}

        public async Task<AsyncVoidMethodBuilder> SetSuggestionJobObservation(SOSReviewProgram _sos_plan, List<DistSelect> Dist_Manager,
    List<User> SV_Manager, int diasSeparate, DateTime Startday, int JobsPorDia, int OptionRandom, int DistribucionesPorMes)
        {
            // Inicialización común
            List<JobObservation> availableJobs = await JobObsServices.GetAllNextYearJobsObservations(
                _sos_plan.PlantId, _sos_plan.AreaId, (int)_sos_plan.AplicationYear);

            this.diasSeparate = diasSeparate;
            this.Startday = Startday;
            this.JobsPorDia = JobsPorDia;
            this.Year = Startday.Year;
            this.YearLoop = 0;

            

            _All_Suggested_SOSJobobservation?.Clear();
            Suggested_SOS_Registers_UserOperationRelationship?.Clear();

            // Preparar distribuciones y operaciones según la opción seleccionada
            var selectedDistributions = Dist_Manager.Where(d => d.isSelected).Select(d => d.distribution).ToList();
            PrepareOperationsByOption(OptionRandom, selectedDistributions);

            try
            {
                if (_All_Suggested_SOSJobobservation.Count == 0)
                {
                    switch (OptionRandom)
                    {
                        case 3: // Opción especial 1 distribución por mes
                            await ProcessOption3(_sos_plan, selectedDistributions, SV_Manager, availableJobs);
                            break;

                        default: // Opciones 0, 1 y 2
                            await ProcessStandardOptions(_sos_plan, selectedDistributions, SV_Manager, availableJobs, OptionRandom);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error CreateSuggestion: {ex.Message}");
            }
            finally
            {
                GenerateSuggestedRegistersMatrix();
            }

            return new AsyncVoidMethodBuilder();
        }

        // Métodos auxiliares

        private void PrepareOperationsByOption(int option, List<Distribution> selectedDistributions)
        {
            _All_Operations?.Clear();
            var random = new Random();

            switch (option)
            {
                case 0:
                case 3: // Orden original por distribución
                    foreach (var dist in selectedDistributions)
                    {
                        _All_Operations.AddRange(dist.Operations);
                    }
                    break;

                case 1: // Operaciones mezcladas
                    foreach (var dist in selectedDistributions)
                    {
                        _All_Operations.AddRange(dist.Operations);
                    }
                    _All_Operations = _All_Operations.OrderBy(x => random.Next()).ToList();
                    break;

                case 2: // Distribuciones mezcladas
                    var shuffledDistributions = selectedDistributions.OrderBy(x => random.Next()).ToList();
                    foreach (var dist in shuffledDistributions)
                    {
                        _All_Operations.AddRange(dist.Operations);
                    }
                    break;

                
            }
        }

        private async Task ProcessStandardOptions(SOSReviewProgram _sos_plan, List<Distribution> selectedDistributions,
            List<User> SV_Manager, List<JobObservation> availableJobs, int option)
        {
            foreach (var op in _All_Operations)
            {
                if (!_All_SOSJobobservation.Any(j => j.Operations.Any(jo => jo.OperationId == op.OperationId)))
                {
                    int supervisorIndex = _All_Operations.IndexOf(op) % SV_Manager.Count;
                    int supervisorId = SV_Manager[supervisorIndex].UserId;
                    int distID = selectedDistributions.Find(d => d.Operations.Any(o => o.OperationId == op.OperationId)).DistributionId;

                    await ProcessSingleOperation(_sos_plan, op, supervisorId, distID, SV_Manager, availableJobs);
                }
            }
        }

        //aqui me quede ver como trabaja la asignacion por mes, actulizar el metodo que optiene las fechas disponibles
        private async Task ProcessOption3(SOSReviewProgram _sos_plan, List<Distribution> selectedDistributions,
            List<User> SV_Manager, List<JobObservation> availableJobs)
        {
            var currentDate = Startday;
            var remainingDistributions = new Queue<Distribution>(selectedDistributions);
            var calendarMonths = new List<DateTime>();

            // Crear lista de meses del año
            for (int i = 0; i < 12; i++)
            {
                calendarMonths.Add(new DateTime(Startday.Year, Startday.Month, 1).AddMonths(i));
            }

            //Filtrar meses al año
            calendarMonths = calendarMonths.Where(c => c.Year == Startday.Year).ToList();

                int auxSaveJobs = JobsPorDia;
            //do while en un for (Experimentando limites)
            for (int monthIndex = 0; remainingDistributions.Count > 0; monthIndex++)
            {
                var currentDist = remainingDistributions.Dequeue();
                var monthStart = calendarMonths[monthIndex];
                // Procesar operaciones de la distribución actual
                foreach (var op in currentDist.Operations)
                {
                    Console.WriteLine($"Start {auxSaveJobs}");
                    int supervisorIndex = _All_Operations.FindIndex(o => o.OperationId == op.OperationId) % SV_Manager.Count;
                    int supervisorId = SV_Manager[supervisorIndex].UserId;

                    await ProcessSingleOperationMonth(_sos_plan, op, supervisorId, currentDist, SV_Manager, availableJobs, monthStart.Month);
                    Console.WriteLine($"End {JobsPorDia}");
      
                }

                JobsPorDia = auxSaveJobs;
            }

        }

        private void UpdateRegisterRelations(SOSReviewProgram _sos_plan, Operation op, int supervisorId, List<User> SV_Manager, bool existing)
        {
            var relationship = new SOSRegUserOperationRelationship
            {
                Register = new()
                {
                    SOSReviewProgramid = _sos_plan.SOSid,
                    OperationId = op.OperationId,
                    SupervisorId = supervisorId,
                    Supervisor = SV_Manager.Find(u => u.UserId == supervisorId)
                },
                Exist = existing,
                StateUpdate = true
            };

            Suggested_SOS_Registers_UserOperationRelationship.Add(op.OperationId, relationship);
        }

        private void GenerateSuggestedRegistersMatrix()
        {
            Suggested_Registers_Matrix?.Clear();

            foreach (var dist in _distributions)
            {
                for (int month = 1; month <= 12; month++)
                {
                    var registers = _All_Suggested_SOSJobobservation?
                       .Where(r => r.DistributionId == dist.DistributionId && r.StartDate?.Month == month)
                       .ToList();

                    Suggested_Registers_Matrix.Add((dist.DistributionId, month), registers);
                }
            }
        }

        private async Task ProcessSingleOperation(SOSReviewProgram _sos_plan, Operation op, int supervisorId, int distId, List<User> SV_Manager, List<JobObservation> availableJobs)
        {
            //Buscamos si ya existe alguna job con esa operacion.
            JobObservation? existingJob = availableJobs?
                .Where(j => j.DistributionId == distId)
                .FirstOrDefault(j => j.Operations.Any(o => o.OperationId == op.OperationId));

            DateTime operationDate = Startday;

                
            var dist = _distributions.Find(d => d.DistributionId == distId);

            if (existingJob != null)
            {
                var job = _mapper.Map<JobObservationNulls>(existingJob);

                job.SupervisorId = supervisorId;
                job.Supervisor = SV_Manager.Find(u => u.UserId == supervisorId);

                //se asignan/actualizan los valores/objetos para evitar errores en la visualizacion previa
                job.Plant = _sos_plan.Plant;
                job.Area = _sos_plan.Area;
                job.Distribution = dist;
                job.DistributionId = distId;

                //Operaciones de la job
                var operationsList = job.Operations?.ToList() ?? new List<Operation>();
                if (!operationsList.Any(o => o.OperationId == op.OperationId))
                {
                    operationsList.Add(_All_Operations.First(o => o.OperationId == op.OperationId));
                }
                job.Operations = operationsList;

                //actualizacion del status de la job, se cambia a planeada, se asigna al sos y se actualizan las categorias disponibles
                job.Option = 2;
                job.Type = 3;
                job.Status = 7;
                job.SectionIds = jobCategoryStructureIds;
                job.IsActive = true;

                //Se valida si el dia en que estaba registrada esta disponible, 
                operationDate = await FindNextAvailableDate((DateTime)job.StartDate, true, supervisorId);
           
                job.StartDate = operationDate;
                job.PlannedStartDate = operationDate;

                UpdateRegisterRelations(_sos_plan, op, supervisorId, SV_Manager, existing: true);
                availableJobs.Remove(existingJob);
                _All_Suggested_SOSJobobservation.Add(job);
            }
            else
            {
                var job = new JobObservationNulls
                {
                    SupervisorId = supervisorId,
                    Supervisor = SV_Manager.Find(u => u.UserId == supervisorId),
                    PlantId = (int)_sos_plan.PlantId,
                    AreaId = (int)_sos_plan.AreaId,
                    Plant = _sos_plan.Plant,
                    Area = _sos_plan.Area,
                    DistributionId = distId,
                    Distribution = dist,
                    Option = 2,
                    Type = 3,
                    Status = 7,
                    SectionIds = jobCategoryStructureIds,
                    IsActive = true,
                    Operations = new List<Operation> { op },
                };

                operationDate = await FindNextAvailableDate(operationDate, true, supervisorId);

                job.StartDate = operationDate;
                job.PlannedStartDate = operationDate;

                UpdateRegisterRelations(_sos_plan, op, supervisorId, SV_Manager, existing: false);
                _All_Suggested_SOSJobobservation.Add(job);
            }
        }
        private async Task ProcessSingleOperationMonth(SOSReviewProgram _sos_plan, Operation op, int supervisorId, Distribution dist, List<User> SV_Manager, List<JobObservation> availableJobs, int Month)
        {
            //Buscamos si ya existe alguna job con esa operacion.
            JobObservation? existingJob = availableJobs?
                .Where(j => j.DistributionId == dist.DistributionId)
                .FirstOrDefault(j => j.Operations.Any(o => o.OperationId == op.OperationId));

            DateTime operationDate = new DateTime(year: Startday.Year, month: Month, day: Startday.Day);


            if (existingJob != null)
            {
                var job = _mapper.Map<JobObservationNulls>(existingJob);

                job.SupervisorId = supervisorId;
                job.Supervisor = SV_Manager.Find(u => u.UserId == supervisorId);

                //se asignan/actualizan los valores/objetos para evitar errores en la visualizacion previa
                job.Plant = _sos_plan.Plant;
                job.Area = _sos_plan.Area;
                job.Distribution = dist;
                job.DistributionId = dist.DistributionId;

                //Operaciones de la job
                var operationsList = job.Operations?.ToList() ?? new List<Operation>();
                if (!operationsList.Any(o => o.OperationId == op.OperationId))
                {
                    operationsList.Add(_All_Operations.First(o => o.OperationId == op.OperationId));
                }
                job.Operations = operationsList;

                //actualizacion del status de la job, se cambia a planeada, se asigna al sos y se actualizan las categorias disponibles
                job.Option = 2;
                job.Type = 3;
                job.Status = 7;
                job.SectionIds = jobCategoryStructureIds;
                job.IsActive = true;

                //Se valida si el dia en que estaba registrada esta disponible, 
                operationDate = await FindNextAvailableDate((DateTime)job.StartDate, true, supervisorId);

                job.StartDate = operationDate;
                job.PlannedStartDate = operationDate;

                UpdateRegisterRelations(_sos_plan, op, supervisorId, SV_Manager, existing: true);
                availableJobs.Remove(existingJob);
                _All_Suggested_SOSJobobservation.Add(job);
            }
            else
            {
                var job = new JobObservationNulls
                {
                    SupervisorId = supervisorId,
                    Supervisor = SV_Manager.Find(u => u.UserId == supervisorId),
                    PlantId = (int)_sos_plan.PlantId,
                    AreaId = (int)_sos_plan.AreaId,
                    Plant = _sos_plan.Plant,
                    Area = _sos_plan.Area,
                    DistributionId = dist.DistributionId,
                    Distribution = dist,
                    Option = 2,
                    Type = 3,
                    Status = 7,
                    SectionIds = jobCategoryStructureIds,
                    IsActive = true,
                    Operations = new List<Operation> { op },
                };

                operationDate = await FindNextAvailableDateInMonth(operationDate, true, Month, supervisorId);

                job.StartDate = operationDate;
                job.PlannedStartDate = operationDate;

                UpdateRegisterRelations(_sos_plan, op, supervisorId, SV_Manager, existing: false);
                _All_Suggested_SOSJobobservation.Add(job);
            }
        }

       
        public async Task<DateTime> FindNextAvailableDateInMonth(DateTime startDate, bool isSuggest, int Month, int supervisorId = 0)
        {
            DateTime currentDate = startDate;
            int attempts = 0;
            int maxAttempts = 365; // Máximo de días a verificar para evitar bucles infinitos


            if (YearLoop > 0)
            {
                Console.WriteLine($"Start Vueltas al mes: {YearLoop}");
                if (YearLoop == diasSeparate)
                {
                    YearLoop = 0;
                    JobsPorDia += 1;
                }

                currentDate = currentDate.AddDays(YearLoop);
            }

            while (attempts < maxAttempts)
            {
                // Saltar fines de semana
                if (currentDate.DayOfWeek == DayOfWeek.Saturday)
                {
                    currentDate = currentDate.AddDays(2);
                    attempts += 2;
                    continue;
                }
                else if (currentDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    currentDate = currentDate.AddDays(1);
                    attempts++;
                    continue;
                }

                // Verificar disponibilidad según el modo
                bool isAvailable;
                if (isSuggest)
                {
                    isAvailable = !(await IsDateSuggestAlreadyUsed(currentDate, supervisorId));
                }
                else
                {
                    isAvailable = !(await IsDateAlreadyUsed(currentDate));
                }

                if (isAvailable && currentDate.Month == Month)
                {
                    return currentDate;
                }

                // Avanzar al siguiente día hábil con separación
                currentDate = currentDate.AddDays(diasSeparate);
                attempts += diasSeparate;

                // Mantener dentro del año actual
                if (currentDate.Month != Month)
                {
                    Console.WriteLine($"Vueltas mes: {YearLoop}");


                    if (YearLoop == diasSeparate)
                    {
                        YearLoop = 0;
                        JobsPorDia += 1;
                    }

                    currentDate = new DateTime(Year, Month, startDate.Day).AddDays(YearLoop); // Reiniciar desde enero
                    YearLoop++;
                }
            }

            throw new InvalidOperationException("No se pudo encontrar una fecha disponible después de múltiples intentos.");
        }

        public async Task<DateTime> FindNextAvailableDate(DateTime startDate, bool isSuggest, int id_SV = 0)
        {
            int currentYear = DateTime.Today.Year;
            DateTime currentDate = AdjustStartDate(startDate, YearLoop);
            int maxAttempts = 366; // máximo días del año
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                currentDate = SkipWeekend(currentDate);

                bool isUsed = isSuggest
                    ? await IsDateSuggestAlreadyUsed(currentDate, id_SV)
                    : await IsDateAlreadyUsed(currentDate);

                if (!isUsed && currentDate.Year == Year)
                    return currentDate;

                currentDate = currentDate.AddDays(diasSeparate);
                attempts++;

                if (currentDate.Year != Year)
                {
                    // Verifica si todos los días del año ya tienen JobsPorDia
                    if (await AllWorkingDaysInYearAreFull(JobsPorDia, id_SV))
                    {
                        JobsPorDia++; // aumenta la carga
                        YearLoop = 0; // reinicia el loop
                    }

                    // Recalcula nueva fecha base desde donde reiniciar
                    DateTime restartDate = AdjustStartDate(startDate, YearLoop++);
                    return await FindNextAvailableDate(restartDate, isSuggest, id_SV);
                }
            }

            throw new InvalidOperationException("No se pudo encontrar una fecha disponible después de múltiples intentos.");
        }

        private DateTime SkipWeekend(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday) return date.AddDays(2);
            if (date.DayOfWeek == DayOfWeek.Sunday) return date.AddDays(1);
            return date;
        }

        private DateTime AdjustStartDate(DateTime start, int offset)
        {
            DateTime today = DateTime.Today;

            if (Year < today.Year)
                throw new InvalidOperationException("No se pueden asignar fechas en años anteriores al actual.");

            if (Year == today.Year)
            {
                DateTime baseDate = today.AddDays(offset);
                return baseDate.Year == Year ? baseDate : today;
            }

            return new DateTime(Year, 1, 1).AddDays(offset);
        }
        private async Task<bool> AllWorkingDaysInYearAreFull(int jobsPerDay, int supervisorId)
        {
            DateTime start = (Year == DateTime.Today.Year) ? DateTime.Today : new DateTime(Year, 1, 1);
            DateTime end = new DateTime(Year, 12, 31);

            for (DateTime date = start; date <= end; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                int count = _All_Suggested_SOSJobobservation
                    .Count(j => j.PlannedStartDate.Value.Date == date.Date && j.SupervisorId == supervisorId);

                if (count < jobsPerDay)
                    return false; // hay huecos
            }

            return true; // todos los días hábiles están llenos
        }


        //Primera version
        private async Task<bool> IsDateAlreadyUsed(DateTime dateToCheck)
        {
            int count = _All_SOSJobobservation.Count(o => o.PlannedStartDate.Value.Date == dateToCheck.Date);
            return count > JobsPorDia;
        }
        //version anterior
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
            //Console.WriteLine($"Get_Suggest_AllSos_Dist: {dist_Id} - {DateTime.Now}");
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
