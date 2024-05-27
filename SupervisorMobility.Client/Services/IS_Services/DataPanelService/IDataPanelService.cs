using SupervisorMobility.Client.Data.Entities.IS;

namespace SupervisorMobility.Client.Services.IS_Services.DataPanelService
{
    public interface IDataPanelService
    {
        #region DataPanel
        Task<DataPanel> CreateDataPanel(DataPanel dataPaneltoCreate);
        Task<DataPanel> GetDataPanel(bool includeSpecifications = false);
        Task<List<DataPanel>> GetAllDataPanels(bool includeSpecifications = false);
        Task<DataPanel> DeleteDataPanel(int id);

        #endregion

        #region DataPanelSpecifications
        Task<List<DataPanelSpecification>> GetAllSpecificationsFromDataPanel(bool includeDataPanel = false);
        Task<DataPanelSpecification> GetDataPanelSpecification(int id_DataPanelSpecification, bool includeDataPanel = false);
        #endregion
    }
}
