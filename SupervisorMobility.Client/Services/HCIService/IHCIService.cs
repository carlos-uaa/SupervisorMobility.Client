

namespace SupervisorMobility.Client.Services.HCIService
{
    public interface IHCIService
    {
        Task<List<HCI>> GetHCIs();
        Task<HCI> GetHCI(int id);
        Task<bool> CreateHCI(HCI content);
        Task<bool> UpdateHCI(HCI content);

    }
}
