using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Services.ILUService
{
    public interface IILUService
    {
        Task<List<ILULevel>> GetLevelsILU();

        Task<ILURegister> AddRegisterForUser(ILURegister iluToRegister, int UserId);
    }
}
