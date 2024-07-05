using SupervisorMobility.Client.Data.Entities.IS;

namespace SupervisorMobility.Client.Services.IS_Services.LogbookAppearanceService
{
    public interface ILogbookAppearanceService
    {
        Task<LogbookAppearance?> CreateLogbookAppearance(LogbookAppearance logbookAppearanceToCreate);
        Task<List<LogbookAppearance>> GetAllLogbookAppearances(bool includePanelResults = false, bool includeProblemDefectResults = false);
        Task<LogbookAppearance?> GetLogbookAppearance(int logbookAppearance_id, bool includePanelResults = false, bool includeProblemDefectResults = false);
        Task<LogbookAppearance?> UpdateLogbookAppearance(LogbookAppearance logbookAppearanceToUpdate);
        Task<bool> DeleteLogbookAppearance(int logbookAppearance_id);
    }
}
