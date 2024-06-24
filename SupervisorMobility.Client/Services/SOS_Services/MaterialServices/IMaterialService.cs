namespace SupervisorMobility.Client.Services.SOS_Services.MaterialServices
{
    public interface IMaterialService
    {
        Task<List<Material>> GetMaterials();

        Task<Material> GetMaterialById(int id);

        Task<Material> CreateMaterial(Material Material);

        Task<bool> UpdateMaterial(Material Material);

        Task DeleteMaterial(int id);
    }
}
