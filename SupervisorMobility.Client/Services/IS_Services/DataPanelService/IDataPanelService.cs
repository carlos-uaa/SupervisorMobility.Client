using SupervisorMobility.Client.Data.Entities.IS;

namespace SupervisorMobility.Client.Services.IS_Services.DataPanelService
{
    public interface IDataPanelService
    {
        #region DataPanel
        Task<DataPanel> CreateDataPanel(DataPanel dataPaneltoCreate);
        Task<DataPanel> GetDataPanel(int id_datapanel, bool includeSpecifications = false);
        Task<List<DataPanel>> GetAllDataPanels(bool includeSpecifications = false);
        Task<DataPanel> DeleteDataPanel(int id);

        Task<bool> UpdatePanelSequence(int datapanel_Id, DataPanel dataPanel);

        #endregion

        #region DataPanelSpecifications
        Task<List<DataPanelSpecification>> GetAllSpecificationsFromDataPanel(bool includeDataPanel = false);
        Task<DataPanelSpecification> GetDataPanelSpecification(int id_DataPanelSpecification, bool includeDataPanel = false);
        #endregion
    }
}
