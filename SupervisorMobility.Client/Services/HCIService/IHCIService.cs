

namespace SupervisorMobility.Client.Services.HCIService
{
    public interface IHCIService
    {
        Task<List<HCI>> GetHCIs(bool includeNavigation = false, bool includePeople = false, bool includeComments = false, bool includeTransactions = false);
        Task<HCI> GetHCI(int id);
        Task<List<User>> GetUsersWithoutHCI();
        Task<bool> CreateHCI(HCI content);
        Task<bool> UpdateHCI(HCI content);
        Task<bool> DeleteHCI(int hciId);


        // Get all Categories
        Task<List<HCICategory>> GetCategories();

        // Get Categorie by Id
        Task<HCICategory> GetCategorieById(int id);

        // Create Categorie
        Task<HCICategory> CreateCategorie(HCICategory Categorie);

        // Update Categorie
        Task<bool> UpdateCategorie(HCICategory Categorie);

        // Delete Categorie
        Task DeleteCategorie(int id);
        Task<List<HCICategory>> GetHCICategories();
    }
}
