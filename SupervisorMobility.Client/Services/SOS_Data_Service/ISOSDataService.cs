namespace SupervisorMobility.Client.Services.SOS_Data_Service
{
    public interface ISOSDataService
    {

        event Action<List<JobObservation>> JobsChanges;
        event Action<List<JobObservationNulls>> SuggestionJobsChanged;

        List<JobObservationNulls> SuggestionJobs { get; }
        List<JobObservationNulls> SosJobs { get; }

        void SetSosJobObservation(int sos);
        void SetSugestionJobObservation();


        List<JobObservationNulls> GetSosByMonth(int month);
        List<JobObservationNulls> GetSosSuggestionByMonth(int month);
        List<JobObservationNulls> GetSosByDistribution(int dis_Id);
        List<JobObservationNulls> GetSosSuggestionByDistribution(int dis_Id);


    }
}
