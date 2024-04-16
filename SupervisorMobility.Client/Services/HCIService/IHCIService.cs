

namespace SupervisorMobility.Client.Services.HCIService
{
    public interface IHCIService
    {
        Task<List<HCI>> GetHCIs(bool includeNavigation = false, bool includePeople = false, bool includeEvidences = false, bool includeTransactions = false);
        Task<List<HCI>> GetHCI(int id);
        Task<bool> CreateHCI(HCI content);
        Task<bool> UpdateHCI(HCI content);
        Task<bool> DeleteHCI(int hciId);
    }
}
