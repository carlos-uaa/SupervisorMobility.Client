using System.Text.Json.Serialization;
using System.Runtime.CompilerServices;
using AutoMapper;
using static SupervisorMobility.Client.Pages.Inicio.SOSProgramPage.SOS_Details;
using System.Diagnostics;
using System.Collections.Generic;

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


        //    public async Task<AsyncVoidMethodBuilder> SetSuggestionJobObservation(SOSReviewProgram _sos_plan, List<DistSelect> Dist_Manager,
        //List<User> SV_Manager, int diasSeparate, DateTime Startday, int JobsPorDia, int OptionRandom, int DistribucionesPorMes)
        //    {
        //        // Inicialización común
        //        List<JobObservation> availableJobs = await JobObsServices.GetAllNextYearJobsObservations(
        //            _sos_plan.PlantId, _sos_plan.AreaId, (int)_sos_plan.AplicationYear);

        //        this.diasSeparate = diasSeparate;
        //        this.Startday = Startday;
        //        this.JobsPorDia = JobsPorDia;
        //        this.Year = Startday.Year;
        //        this.YearLoop = 0;



        //        _All_Suggested_SOSJobobservation?.Clear();
        //        Suggested_SOS_Registers_UserOperationRelationship?.Clear();

        //        // Preparar distribuciones y operaciones según la opción seleccionada
        //        var selectedDistributions = Dist_Manager.Where(d => d.isSelected).Select(d => d.distribution).ToList();
        //        PrepareOperationsByOption(OptionRandom, selectedDistributions);

        //        try
        //        {
        //            if (_All_Suggested_SOSJobobservation.Count == 0)
        //            {
        //                switch (OptionRandom)
        //                {
        //                    case 3: // Opción especial 1 distribución por mes
        //                        await ProcessOption3(_sos_plan, selectedDistributions, SV_Manager, availableJobs, DistribucionesPorMes);
        //                        break;

        //                    default: // Opciones 0, 1 y 2
        //                        await ProcessStandardOptions(_sos_plan, selectedDistributions, SV_Manager, availableJobs, OptionRandom);
        //                        break;
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Error CreateSuggestion: {ex.Message}");
        //        }
        //        finally
        //        {
        //            GenerateSuggestedRegistersMatrix();
        //        }

        //        return new AsyncVoidMethodBuilder();
        //    }

        //    public async Task<AsyncVoidMethodBuilder> SetSuggestionJobObservation_OptionsJob(SOSReviewProgram _sos_plan, List<DistSelect> Dist_Manager,
        //        List<User> SV_Manager, int diasSeparate, DateTime Startday, int JobsPorDia, int OptionRandom, int DistribucionesPorMes, int JobOption)
        //    {
        //        // Inicialización común
        //        List<JobObservation> availableJobs = await JobObsServices.GetAllNextYearJobsObservations(
        //            _sos_plan.PlantId, _sos_plan.AreaId, (int)_sos_plan.AplicationYear);

        //        this.diasSeparate = diasSeparate;
        //        this.Startday = Startday;
        //        this.JobsPorDia = JobsPorDia;
        //        this.Year = Startday.Year;
        //        this.YearLoop = 0;



        //        _All_Suggested_SOSJobobservation?.Clear();
        //        Suggested_SOS_Registers_UserOperationRelationship?.Clear();

        //        // Preparar distribuciones y operaciones según la opción seleccionada
        //        var selectedDistributions = Dist_Manager.Where(d => d.isSelected).Select(d => d.distribution).ToList();

        //        if (OptionRandom == 2)
        //        {
        //            //Distribuciones mezcladas
        //            selectedDistributions = selectedDistributions.OrderBy(x => Guid.NewGuid()).ToList();
        //        }


        //        try
        //        {
        //            if (_All_Suggested_SOSJobobservation.Count == 0)
        //            {
        //                switch (JobOption)
        //                {
        //                    case 2: // Opción 1 - Jobs por operacion en el mismo dia de la distribucion
        //                        await ProcessMultipleJobsDistributionOption(_sos_plan, selectedDistributions, SV_Manager, availableJobs);
        //                        break;
        //                    case 3: // Opción 2 - Job por distribucion con todas las operaciones
        //                        await ProcessDistributionOption(_sos_plan, selectedDistributions, SV_Manager, availableJobs);
        //                        break;
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Error CreateSuggestion: {ex.Message}");
        //        }
        //        finally
        //        {
        //            GenerateSuggestedRegistersMatrix();
        //        }

        //        return new AsyncVoidMethodBuilder();
        //    }


        public async Task<AsyncVoidMethodBuilder> SetSuggestion(SOSReviewProgram _sos_plan, List<DistSelect> Dist_Manager, List<User> SV_Manager, DateTime Startday,
            int diasSeparate = 1, int JobsPorDia = 1, int OptionRandom = 1)
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

            var selectedDistributions = Dist_Manager.Where(d => d.isSelected).Select(d => d.distribution).ToList();


            try
            {
                if (_All_Suggested_SOSJobobservation.Count == 0)
                {
                    await ProcessSuggestion(_sos_plan, selectedDistributions, SV_Manager, availableJobs);
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

        class DistributionState
        {
            public Distribution Distribution { get; set; }
            public int CurrentOperationIndex { get; set; } = 0;
            public bool ObservedThisMonth { get; set; } = false;
        }

        // Métodos auxiliares
        private async Task ProcessSuggestion(SOSReviewProgram _sos_plan, List<Distribution> selectedDistributions,
     List<User> SV_Manager, List<JobObservation> availableJobs)
        {
            var distributionStates = selectedDistributions
                .Select(d => new DistributionState { Distribution = d })
                .ToList();

            DateTime today = DateTime.Today;
            DateTime currentDate = Startday >= today ? Startday : today;
            string lastDistributionAssigned = null;

            while (distributionStates.Any(d => d.CurrentOperationIndex < d.Distribution.Operations.Count))
            {
                foreach (var distState in distributionStates)
                {
                    // Saltar distribuciones sin operaciones pendientes
                    if (distState.CurrentOperationIndex >= distState.Distribution.Operations.Count)
                        continue;

                    if (await IsWeekend(currentDate))
                    {
                        currentDate = currentDate.AddDays(1);
                        continue;
                    }

                    var operation = distState.Distribution.Operations.ElementAtOrDefault(distState.CurrentOperationIndex);
                    if (operation == null)
                    {
                        distState.CurrentOperationIndex = distState.Distribution.Operations.Count;
                        continue;
                    }

                    int distId = distState.Distribution.DistributionId;
                    int supervisorIndex = selectedDistributions.IndexOf(distState.Distribution) % SV_Manager.Count;
                    int supervisorId = SV_Manager[supervisorIndex].UserId;

                    await ProcessSingleOperation(_sos_plan, operation, supervisorId, distId, SV_Manager, currentDate, availableJobs);

                    distState.CurrentOperationIndex++;
                    lastDistributionAssigned = distState.Distribution.Description;

                    // Después de asignar, avanzar días con separación real
                    currentDate = await AddWorkingDays(currentDate, diasSeparate);
                }
            }
        }

        private async Task<DateTime> AddWorkingDays(DateTime start, int workDays)
        {
            DateTime result = start;
            int added = 0;

            while (added < workDays)
            {
                result = result.AddDays(1);
                if (!await IsWeekend(result))
                    added++;
            }

            return result;
        }


        private async Task<bool> IsWeekend(DateTime date)
        {
            return await Task.FromResult(date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);
        }

        //   private async Task ProcessMultipleJobsDistributionOption(SOSReviewProgram _sos_plan, List<Distribution> selectedDistributions,
        //       List<User> SV_Manager, List<JobObservation> availableJobs)
        //   {
        //       foreach (var dist in selectedDistributions)
        //       {
        //           //optener fecha disponible o siguiente
        //           DateTime operationDate = new DateTime(year: 1, month: 1, day: 1);

        //           foreach (var op in dist.Operations)
        //           {
        //               if (!_All_SOSJobobservation.Any(j => j.Operations.Any(jo => jo.OperationId == op.OperationId)))
        //               {
        //                   int supervisorIndex = _All_Operations.IndexOf(op) % SV_Manager.Count;
        //                   int supervisorId = SV_Manager[supervisorIndex].UserId;
        //                   int distID = selectedDistributions.Find(d => d.Operations.Any(o => o.OperationId == op.OperationId)).DistributionId;

        //                   if(operationDate.Year == 1 && operationDate.Month == 1 && operationDate.Day == 1 )
        //                       operationDate = await FindNextAvailableDate(this.Startday, true, supervisorId); 

        //                   await ProcessSingleOperation(_sos_plan, op, supervisorId, distID, SV_Manager, operationDate);
        //               }
        //           }
        //       }
        //   }

        //   private async Task ProcessDistributionOption(SOSReviewProgram _sos_plan, List<Distribution> selectedDistributions,
        //      List<User> SV_Manager, List<JobObservation> availableJobs)
        //   {
        //       foreach (var dist in selectedDistributions)
        //       {
        //           //optener fecha disponible o siguiente
        //           int supervisorIndex = selectedDistributions.IndexOf(dist) % SV_Manager.Count;
        //           int supervisorId = SV_Manager[supervisorIndex].UserId;
        //           int distID = dist.DistributionId;

        //           DateTime operationDate = await FindNextAvailableDate(this.Startday, true, supervisorId);

        //           if (!_All_SOSJobobservation.Any(j => j.DistributionId == dist.DistributionId))
        //           {
        //               await ProcessSingleOperation(_sos_plan, dist.Operations, supervisorId, distID, SV_Manager, operationDate);
        //           }

        //       }
        //   }


        //   private void PrepareOperationsByOption(int option, List<Distribution> selectedDistributions)
        //   {
        //       _All_Operations?.Clear();
        //       var random = new Random();

        //       switch (option)
        //       {
        //           case 0:
        //           case 3: // Orden original por distribución
        //               foreach (var dist in selectedDistributions)
        //               {
        //                   _All_Operations.AddRange(dist.Operations);
        //               }
        //               break;

        //           case 1: // Operaciones mezcladas
        //               foreach (var dist in selectedDistributions)
        //               {
        //                   _All_Operations.AddRange(dist.Operations);
        //               }
        //               _All_Operations = _All_Operations.OrderBy(x => random.Next()).ToList();
        //               break;

        //           case 2: // Distribuciones mezcladas
        //               var shuffledDistributions = selectedDistributions.OrderBy(x => random.Next()).ToList();
        //               foreach (var dist in shuffledDistributions)
        //               {
        //                   _All_Operations.AddRange(dist.Operations);
        //               }
        //               break;


        //       }
        //   }

        //   private async Task ProcessStandardOptions(SOSReviewProgram _sos_plan, List<Distribution> selectedDistributions,
        //       List<User> SV_Manager, List<JobObservation> availableJobs, int option)
        //   {
        //       foreach (var op in _All_Operations)
        //       {
        //           if (!_All_SOSJobobservation.Any(j => j.Operations.Any(jo => jo.OperationId == op.OperationId)))
        //           {
        //               int supervisorIndex = _All_Operations.IndexOf(op) % SV_Manager.Count;
        //               int supervisorId = SV_Manager[supervisorIndex].UserId;
        //               int distID = selectedDistributions.Find(d => d.Operations.Any(o => o.OperationId == op.OperationId)).DistributionId;

        //               await ProcessSingleOperation(_sos_plan, op, supervisorId, distID, SV_Manager, availableJobs);
        //           }
        //       }
        //   }

        //   //aqui me quede ver como trabaja la asignacion por mes, actulizar el metodo que optiene las fechas disponibles
        //   private async Task ProcessOption3(SOSReviewProgram _sos_plan, List<Distribution> selectedDistributions,
        //List<User> SV_Manager, List<JobObservation> availableJobs, int DistribucionesPorMes = 1)
        //   {
        //       var remainingDistributions = new Queue<Distribution>(selectedDistributions);
        //       var auxSaveJobs = JobsPorDia;

        //       int year = Startday.Year;
        //       int currentLoop = 0; // similar a YearLoop pero local para esta función
        //       int distribucionesPorMesActual = DistribucionesPorMes;

        //       while (remainingDistributions.Count > 0)
        //       {
        //           var calendarMonths = Enumerable.Range(0, 12)
        //               .Select(i => new DateTime(year, Startday.Month, 1).AddMonths(i))
        //               .Where(c => c.Year == year)
        //               .ToList();

        //           foreach (var monthStart in calendarMonths)
        //           {
        //               int distribucionesEnEsteMes = 0;

        //               while (remainingDistributions.Count > 0 && distribucionesEnEsteMes < distribucionesPorMesActual)
        //               {
        //                   var currentDist = remainingDistributions.Dequeue();
        //                   distribucionesEnEsteMes++;

        //                   foreach (var op in currentDist.Operations)
        //                   {
        //                       int supervisorIndex = _All_Operations.FindIndex(o => o.OperationId == op.OperationId) % SV_Manager.Count;
        //                       int supervisorId = SV_Manager[supervisorIndex].UserId;

        //                       await ProcessSingleOperationMonth(_sos_plan, op, supervisorId, currentDist, SV_Manager, availableJobs, monthStart.Month);
        //                   }
        //               }

        //               JobsPorDia = auxSaveJobs; // Reset por mes
        //           }

        //           // Si aún quedan distribuciones → volver a recorrer el año con más capacidad
        //           if (remainingDistributions.Count > 0)
        //           {
        //               currentLoop++;
        //               distribucionesPorMesActual = DistribucionesPorMes + currentLoop;

        //               Console.WriteLine($"Reiniciando año, nueva capacidad por mes: {distribucionesPorMesActual}");
        //           }
        //       }
        //   }



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

        //private async Task ProcessSingleOperation(SOSReviewProgram _sos_plan, Operation op, int supervisorId, int distId, List<User> SV_Manager, List<JobObservation> availableJobs)
        //{
        //    //Buscamos si ya existe alguna job con esa operacion.
        //    JobObservation? existingJob = availableJobs?
        //        .Where(j => j.DistributionId == distId)
        //        .FirstOrDefault(j => j.Operations.Any(o => o.OperationId == op.OperationId));

        //    DateTime operationDate = Startday;


        //    var dist = _distributions.Find(d => d.DistributionId == distId);

        //    if (existingJob != null)
        //    {
        //        var job = _mapper.Map<JobObservationNulls>(existingJob);

        //        job.SupervisorId = supervisorId;
        //        job.Supervisor = SV_Manager.Find(u => u.UserId == supervisorId);

        //        //se asignan/actualizan los valores/objetos para evitar errores en la visualizacion previa
        //        job.Plant = _sos_plan.Plant;
        //        job.Area = _sos_plan.Area;
        //        job.Distribution = dist;
        //        job.DistributionId = distId;

        //        //Operaciones de la job
        //        var operationsList = job.Operations?.ToList() ?? new List<Operation>();
        //        if (!operationsList.Any(o => o.OperationId == op.OperationId))
        //        {
        //            operationsList.Add(_All_Operations.First(o => o.OperationId == op.OperationId));
        //        }
        //        job.Operations = operationsList;

        //        //actualizacion del status de la job, se cambia a planeada, se asigna al sos y se actualizan las categorias disponibles
        //        job.Option = 2;
        //        job.Type = 3;
        //        job.Status = 7;
        //        job.SectionIds = jobCategoryStructureIds;
        //        job.IsActive = true;

        //        //Se valida si el dia en que estaba registrada esta disponible, 
        //        operationDate = await FindNextAvailableDate((DateTime)job.StartDate, true, supervisorId);

        //        job.StartDate = operationDate;
        //        job.PlannedStartDate = operationDate;

        //        UpdateRegisterRelations(_sos_plan, op, supervisorId, SV_Manager, existing: true);
        //        availableJobs.Remove(existingJob);
        //        _All_Suggested_SOSJobobservation.Add(job);
        //    }
        //    else
        //    {
        //        var job = new JobObservationNulls
        //        {
        //            SupervisorId = supervisorId,
        //            Supervisor = SV_Manager.Find(u => u.UserId == supervisorId),
        //            PlantId = (int)_sos_plan.PlantId,
        //            AreaId = (int)_sos_plan.AreaId,
        //            Plant = _sos_plan.Plant,
        //            Area = _sos_plan.Area,
        //            DistributionId = distId,
        //            Distribution = dist,
        //            Option = 2,
        //            Type = 3,
        //            Status = 7,
        //            SectionIds = jobCategoryStructureIds,
        //            IsActive = true,
        //            Operations = new List<Operation> { op },
        //        };

        //        operationDate = await FindNextAvailableDate(operationDate, true, supervisorId);

        //        job.StartDate = operationDate;
        //        job.PlannedStartDate = operationDate;

        //        UpdateRegisterRelations(_sos_plan, op, supervisorId, SV_Manager, existing: false);
        //        _All_Suggested_SOSJobobservation.Add(job);
        //    }
        //}

        private async Task ProcessSingleOperation(SOSReviewProgram _sos_plan, Operation op, int supervisorId, int distId, List<User> SV_Manager, DateTime operationDate, List<JobObservation> availableJobs)
        {
            var dist = _distributions.Find(d => d.DistributionId == distId);

            JobObservation? existingJob = availableJobs?
                .Where(j => j.DistributionId == dist.DistributionId)
                .FirstOrDefault(j => j.Operations.Any(o => o.OperationId == op.OperationId));

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

             
                job.StartDate = existingJob.StartDate;
                job.PlannedStartDate = existingJob.PlannedStartDate;

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

            // var job = new JobObservationNulls
            // {
            //     SupervisorId = supervisorId,
            //     Supervisor = SV_Manager.Find(u => u.UserId == supervisorId),
            //     PlantId = (int)_sos_plan.PlantId,
            //     AreaId = (int)_sos_plan.AreaId,
            //     Plant = _sos_plan.Plant,
            //     Area = _sos_plan.Area,
            //     DistributionId = distId,
            //     Distribution = dist,
            //     Option = 2,
            //     Type = 3,
            //     Status = 7,
            //     SectionIds = jobCategoryStructureIds,
            //     IsActive = true,
            //     Operations = new List<Operation> { op },
            // };

            // job.StartDate = operationDate;
            // job.PlannedStartDate = operationDate;

            // UpdateRegisterRelations(_sos_plan, op, supervisorId, SV_Manager, existing: false);
            // _All_Suggested_SOSJobobservation.Add(job);

        }



        public async Task<DateTime> FindNextAvailableDateInMonth(DateTime startDate, bool isSuggest, int Month, int supervisorId = 0)
        {
            DateTime currentDate = startDate;
            int attempts = 0;
            int maxAttempts = 365;

            // Aplicar desplazamiento al inicio si estamos dando "vueltas" en el mes
            if (YearLoop > 0)
            {
                currentDate = await AddWorkingDays(startDate, YearLoop);
            }

            while (attempts < maxAttempts)
            {
                // Saltar fines de semana (ya los evita AddWorkingDays, pero extra seguro)
                if (await IsWeekend(currentDate))
                {
                    currentDate = currentDate.AddDays(1);
                    continue;
                }

                // Validar si la fecha está disponible y es del mes correcto
                if (currentDate.Month == Month)
                {
                    bool isUsed = isSuggest
                        ? await IsDateSuggestAlreadyUsed(currentDate, supervisorId)
                        : await IsDateAlreadyUsed(currentDate);

                    if (!isUsed && currentDate >= DateTime.Today && currentDate >= this.Startday)
                        return currentDate;
                }

                // Avanzar días hábiles reales
                currentDate = await AddWorkingDays(currentDate, diasSeparate);
                attempts++;

                // Si salimos del mes → nueva vuelta
                if (currentDate.Month != Month)
                {
                    if (YearLoop == diasSeparate)
                    {
                        YearLoop = 0;
                        JobsPorDia++;
                    }

                    currentDate = await AddWorkingDays(new DateTime(Year, Month, 1), YearLoop++);
                }
            }

            throw new InvalidOperationException("No se pudo encontrar una fecha disponible en este mes.");
        }
        public async Task<DateTime> FindNextAvailableDate(DateTime startDate, bool isSuggest, int supervisorId = 0)
        {
            int year = this.Year;
            DateTime today = DateTime.Today;

            if (year < today.Year)
                throw new InvalidOperationException("No se puede asignar tareas en años anteriores al actual.");

            // Punto de partida válido
            DateTime baseDate = (year == today.Year && startDate < today) ? today : new DateTime(year, 1, 1);

            // Desplazar baseDate por días hábiles (YearLoop veces)
            DateTime searchDate = await AddWorkingDays(baseDate, YearLoop);

            int searchAttempts = 0;
            const int maxSearchDays = 366;

            while (searchAttempts < maxSearchDays)
            {
                if (searchDate.Year != year)
                {
                    if (await AllWorkingDaysInYearAreFull(JobsPorDia, supervisorId))
                    {
                        JobsPorDia++;
                        YearLoop = 0;
                        return await FindNextAvailableDate(startDate, isSuggest, supervisorId); // reinicio con nueva carga
                    }

                    YearLoop++;
                    return await FindNextAvailableDate(startDate, isSuggest, supervisorId); // siguiente vuelta con desplazamiento
                }

                // Validar si es hábil y disponible
                if (!await IsWeekend(searchDate))
                {

                    if (year == today.Year && searchDate < today)
                    {
                        searchDate = today;
                        continue;
                    }

                    bool isUsed = isSuggest
                        ? await IsDateSuggestAlreadyUsed(searchDate, supervisorId)
                        : await IsDateAlreadyUsed(searchDate);

                    if (!isUsed && searchDate >= DateTime.Today && searchDate >= this.Startday)
                        return searchDate;
                }

                // Avanzar X días hábiles
                searchDate = await AddWorkingDays(searchDate, diasSeparate);
                searchAttempts++;
            }

            throw new InvalidOperationException("No se pudo encontrar una fecha disponible en el año.");
        }

     
       

        private async Task<bool> AllWorkingDaysInYearAreFull(int jobsPerDay, int supervisorId)
        {
            DateTime start = (Year == DateTime.Today.Year) ? DateTime.Today : new DateTime(Year, 1, 1);
            DateTime end = new DateTime(Year, 12, 31);

            for (DateTime date = start; date <= end; date = date.AddDays(1))
            {
                if (await IsWeekend(date)) continue;

                int count = _All_Suggested_SOSJobobservation
                    .Count(j => j.PlannedStartDate.Value.Date == date.Date && j.SupervisorId == supervisorId);

                if (count < jobsPerDay)
                    return false;
            }

            return true;
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
