namespace SupervisorMobility.Client.Services.SOS_Services.KnowledgeServices
{
    public interface IKnowledgeService
    {
        Task<List<Knowledge>> GetKnowledges();

        Task<Knowledge> GetKnowledgeById(int id);

        Task<Knowledge> CreateKnowledge(CreateKnowledgeDto Knowledge);
    }
}
