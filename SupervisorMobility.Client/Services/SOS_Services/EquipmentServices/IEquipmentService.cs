namespace SupervisorMobility.Client.Services.SOS_Services.EquipmentServices
{
    public interface IEquipmentService
    {
        Task<List<Equipment>> GetEquipments();

        Task<Equipment> GetEquipmentById(int id);

        Task<Equipment> CreateEquipment(Equipment Equipment);

        Task<bool> UpdateEquipment(Equipment Equipment);

        Task DeleteEquipment(int id);
    }
}
