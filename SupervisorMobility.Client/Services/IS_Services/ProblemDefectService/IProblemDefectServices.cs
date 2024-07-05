
namespace SupervisorMobility.Client.Services.IS_Services.DataPanelService
{
    public interface IProblemDefectServices
    {
        Task<ProblemDefect?> CreateProblemDefect(ProblemDefect ProblemDefectToCreate);
        Task<List<ProblemDefect>> GetAllProblemDefects();
        Task<ProblemDefect?> GetProblemDefect(int ProblemDefect_id);
        Task<ProblemDefect?> UpdateProblemDefect(ProblemDefect ProblemDefectToUpdate);
        Task<bool> DeleteProblemDefect(int ProblemDefect_id);
     
    }
}
