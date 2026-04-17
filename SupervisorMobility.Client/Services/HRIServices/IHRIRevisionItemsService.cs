using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entites.Dtos.HRIRevisionItemsDtos;

namespace SupervisorMobility.Client.Services.HRIServices
{
    public interface IHRIRevisionItemsService
    {
        // Revision Items
        Task<ServiceResponse<List<GetHRIRevisionItemDto>>> GetAllHRIRevisionItems();
        Task<ServiceResponse<GetHRIRevisionItemDto>> GetHRIRevisionItemById(int id);
        Task<ServiceResponse<GetHRIRevisionItemDto>> CreateHRIRevisionItem(CreateHRIRevisionItemDto dto);
        Task<ServiceResponse<GetHRIRevisionItemDto>> UpdateHRIRevisionItem(int id, UpdateHRIRevisionItemDto dto);
        Task<ServiceResponse<bool>> DeleteHRIRevisionItem(int id);

        // Frequencies
        Task<ServiceResponse<List<GetFrequencyDto>>> GetAllFrequencies();
        Task<ServiceResponse<GetFrequencyDto>> GetFrequencyById(int id);
        Task<ServiceResponse<GetFrequencyDto>> CreateFrequency(CreateFrequencyDto dto);
        Task<ServiceResponse<GetFrequencyDto>> UpdateFrequency(int id, UpdateFrequencyDto dto);
        Task<ServiceResponse<bool>> DeleteFrequency(int id);

        // Veredicts
        Task<ServiceResponse<List<GetVeredictDto>>> GetAllVeredicts();
        Task<ServiceResponse<GetVeredictDto>> GetVeredictById(int id);
        Task<ServiceResponse<GetVeredictDto>> CreateVeredict(CreateVeredictDto dto);
        Task<ServiceResponse<GetVeredictDto>> UpdateVeredict(int id, UpdateVeredictDto dto);
        Task<ServiceResponse<bool>> DeleteVeredict(int id);

        // Revision Methods
        Task<ServiceResponse<List<GetRevisionMethodDto>>> GetAllRevisionMethods();
        Task<ServiceResponse<GetRevisionMethodDto>> GetRevisionMethodById(int id);
        Task<ServiceResponse<GetRevisionMethodDto>> CreateRevisionMethod(CreateRevisionMethodDto dto);
        Task<ServiceResponse<GetRevisionMethodDto>> UpdateRevisionMethod(int id, UpdateRevisionMethodDto dto);
        Task<ServiceResponse<bool>> DeleteRevisionMethod(int id);

    }
}
