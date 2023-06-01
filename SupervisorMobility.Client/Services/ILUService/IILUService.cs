using SupervisorMobility.Client.Data;

namespace SupervisorMobility.Client.Services.ILUService
{
    public interface IILUService
    {
        Task<List<ILULevel>> GetLevelsILU();
    }
}
