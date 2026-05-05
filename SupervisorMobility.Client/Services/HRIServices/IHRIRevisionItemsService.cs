using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entites.Dtos.HRIRevisionItemsDtos;
using SupervisorMobility.Client.Data.Entities.Hri;

namespace SupervisorMobility.Client.Services.HRIServices
{
    public interface IHRIRevisionItemsService
    {
        // Revision Items
        Task<ServiceResponse<List<GetHRIRevisionItemDto>>> GetAllHRIRevisionItems();
        Task<ServiceResponse<GetHRIRevisionItemDto>> GetHRIRevisionItemById(int id);
        Task<ServiceResponse<List<GetHRIRevisionItemDto>>> GetHRIRevisionItemsByHRIId(int id);
        Task<ServiceResponse<GetHRIRevisionItemDto>> CreateHRIRevisionItem(CreateHRIRevisionItemDto dto);
        Task<ServiceResponse<GetHRIRevisionItemDto>> UpdateHRIRevisionItem(int id, UpdateHRIRevisionItemDto dto);
        Task<ServiceResponse<bool>> DeleteHRIRevisionItem(int id);

        // Frequencies
        Task<ServiceResponse<List<Frequency>>> GetAllFrequencies();
        Task<ServiceResponse<Frequency>> GetFrequencyById(int id);
        Task<ServiceResponse<Frequency>> CreateFrequency(Frequency dto);
        Task<ServiceResponse<Frequency>> UpdateFrequency(int id, Frequency dto);
        Task<ServiceResponse<bool>> DeleteFrequency(int id);

        // Veredicts
        Task<ServiceResponse<List<Veredict>>> GetAllVeredicts();
        Task<ServiceResponse<Veredict>> GetVeredictById(int id);
        Task<ServiceResponse<Veredict>> CreateVeredict(Veredict dto);
        Task<ServiceResponse<Veredict>> UpdateVeredict(int id, Veredict dto);
        Task<ServiceResponse<bool>> DeleteVeredict(int id);

        // Revision Methods
        Task<ServiceResponse<List<RevisionMethod>>> GetAllRevisionMethods();
        Task<ServiceResponse<RevisionMethod>> GetRevisionMethodById(int id);
        Task<ServiceResponse<RevisionMethod>> CreateRevisionMethod(RevisionMethod dto);
        Task<ServiceResponse<RevisionMethod>> UpdateRevisionMethod(int id, RevisionMethod dto);
        Task<ServiceResponse<bool>> DeleteRevisionMethod(int id);

    }
}
