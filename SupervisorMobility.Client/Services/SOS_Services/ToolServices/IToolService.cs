namespace SupervisorMobility.Client.Services.SOS_Services.ToolServices
{
    public interface IToolService
    {
        Task<List<Tool>> GetTools();

        Task<Tool> GetToolById(int id);

        Task<Tool> CreateTool(Tool Tool);

        Task<bool> UpdateTool(Tool Tool);

        Task DeleteTool(int id);
    }
}
