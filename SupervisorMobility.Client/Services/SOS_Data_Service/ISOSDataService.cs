using System.Runtime.CompilerServices;
using static SupervisorMobility.Client.Pages.Inicio.SOSProgramPage.SOS_Details;

namespace SupervisorMobility.Client.Services.SOS_Data_Service
{
    public interface ISOSDataService
    {

        //Control de jobs Observations
        List<JobObservationNulls> _All_SOSJobobservation { get; set; }
        //relation User SV to Operation processed
        Dictionary<int, SOSRegUserOperationRelationship?> SOS_Registers_UserOperationRelationship { get; set; } 
        //matris de datos general
        Dictionary<(int, int), List<SOSRegisterJobObservation>?> SOS_Registers_Matrix { get; set; }
        //Control de jobs Observations suggestion
        List<JobObservationNulls> _All_Suggested_SOSJobobservation { get; set; } 
        //Registro de SV - Operacion suggest
        Dictionary<int, SOSRegUserOperationRelationship?> Suggested_SOS_Registers_UserOperationRelationship { get; set; }
        //matris de datos suggestion general
        Dictionary<(int, int), List<JobObservationNulls>?> Suggested_Registers_Matrix { get; set; }
        //Jobs Externas a SOS Program de los SV (Planeadas y Programadas)
        List<JobObservationNulls> _AnotherJobs { get; set; }
        //Resume Operations In Distribution
        Dictionary<int, SosJobCount> OperationsInDistributionCount { get; set; }
        //RAW Data registers
        //Register Jobs without processed
        List<SOSRegisterJobObservation> _SosRegisters { get; set; } 
        //Relation User SV to Operation without processed
        List<SOSRegUserOperation> _SosRegistersrUserOperation { get; set; }
        List<Distribution> _distributions { get; set; } 
        List<Operation> _All_Operations { get; set; }
        //Holiday JobObs
        List<JobObservation> JobObsInHolidays { get; set; }
        List<Holiday> holidays { get; set; }


        DateTime Startday { get; set; }
        int diasSeparate { get; set; }
        int JobsPorDia { get; set; }
        string jobCategoryStructureIds { get; set; }


        //FUNCTIONS
        //Set Jobs
        Task<AsyncVoidMethodBuilder> SetSosJobObservation(int sos, List<Distribution> _distributions, string jobCategoryStructureIds);
        //Crear sugerencias
        //Task<AsyncVoidMethodBuilder> SetSugestionJobObservation(SOSReviewProgram _sos_plan, List<DistSelect> Dist_Manager, List<User> SV_Manager,int diasSeparate, DateTime Startday,int JobsPorDia);

        //Task<AsyncVoidMethodBuilder> SetNewConfigSugestionJobObservation(SOSReviewProgram _sos_plan, List<DistSelect> Dist_Manager,
        //    List<User> SV_Manager, int diasSeparate, DateTime Startday, int JobsPorDia, int OptionRandom);
        Task<AsyncVoidMethodBuilder> SetSuggestion(SOSReviewProgram _sos_plan, List<DistSelect> Dist_Manager, List<User> SV_Manager, DateTime Startday, int diasSeparate = 1, int JobsPorDia = 1, int OptionRandom = 1);
        //Task<AsyncVoidMethodBuilder> SetSuggestionJobObservation(SOSReviewProgram _sos_plan, List<DistSelect> Dist_Manager, List<User> SV_Manager, int diasSeparate, DateTime Startday, int JobsPorDia, int OptionRandom, int DistribucionesPorMes);
        //Task<AsyncVoidMethodBuilder> SetSuggestionJobObservation_OptionsJob(SOSReviewProgram _sos_plan, List<DistSelect> Dist_Manager, List<User> SV_Manager, int diasSeparate, DateTime Startday, int JobsPorDia, int OptionRandom, int DistribucionesPorMes, int JobOption);


        Task<DateTime> FindNextAvailableDate(DateTime startAvailabeDate, bool isSuggest, int id_SV = 0);

        List<JobObservationNulls> Get_AllSos_Month(int month);
        List<JobObservationNulls> Get_AllSos_Month_Dist(int month, int dist_Id);
        List<JobObservationNulls> Get_AllSos_Dist(int dist_Id);
        Dictionary<(int, int), List<SOSRegisterJobObservation>?> Get_Registers_Matrix_Month(int month);
        Dictionary<(int, int), List<SOSRegisterJobObservation>?> Get_Registers_Matri_Month_Dist(int month, int dist_Id);
        Dictionary<(int, int), List<SOSRegisterJobObservation>?> Get_Registers_Matrix_Dist(int dist_Id);


        Dictionary<int, SOSRegUserOperationRelationship?> Get_SOS_Registers_UserOperationRelationship(int dist_Id);
        
        List<JobObservationNulls> Get_Suggest_AllSos_Month(int month);
        List<JobObservationNulls> Get_Suggest_AllSos_Month_Dist(int month, int dist_Id);
        List<JobObservationNulls> Get_Suggest_AllSos_Dist(int dist_Id);
        
        Dictionary<(int, int), List<JobObservationNulls>?> Get_Suggest_Registers_Matrix_Month(int month);
        Dictionary<(int, int), List<JobObservationNulls>?> Get_Suggest_Registers_Matri_Month_Dist(int month, int dist_Id);
        Dictionary<(int, int), List<JobObservationNulls>?> Get_Suggest_Registers_Matrix_Dist(int dist_Id);
        Dictionary<int, SOSRegUserOperationRelationship?> Get_Suggested_SOS_Registers_UserOperationRelationship(int dist_Id);

        void UpdateJobItem(JobObservation UpdatedItem);

    }
}
