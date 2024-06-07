using SupervisorMobility.Client.Data.Entities.IS;

namespace SupervisorMobility.Client.Services.IS_Services.AppearanceService
{
    public interface IAppearanceService
    {
        Task<Appearance?> CreateAppearance(Appearance appearanceToCreate);
        Task<List<Appearance>> GetAllAppearances(bool includeDataPanelItems = false, bool includeProblemDefectItems = false, bool includeLogBookAppearance = false);
        Task<Appearance?> GetAppearance(int appearance_id, bool includeDataPanelItems = false, bool includeProblemDefectItems = false, bool includeLogBookAppearance = false);
        Task<Appearance?> UpdateAppearance(Appearance appearanceToUpdate);
        Task<bool> DeleteAppearance(int appearance_id);
    }
}
