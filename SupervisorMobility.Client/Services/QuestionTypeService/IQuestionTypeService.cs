namespace SupervisorMobility.Client.Services.QuestionTypeService
{
    public interface IQuestionTypeService
    {
        // Get all question types
        Task<List<QuestionType>> GetQuestionTypes();
    }
}
