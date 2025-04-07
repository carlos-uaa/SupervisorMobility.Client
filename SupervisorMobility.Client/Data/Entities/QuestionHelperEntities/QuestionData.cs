namespace SupervisorMobility.Client.Data.Entities.QuestionHelperEntities
{
    public class QuestionData
    {
        public int QuestionId { get; set; }
        public string Comparator { get; set; }
        public string QstOption { get; set; }
    }
    public class ActionData
    {
        public string Operation { get; set; }
        public string? Value { get; set; }
    }

    public class CategoryData
    {
        public int CategoryId { get; set; }
        public Dictionary<int,CategoryQuestionData> QuestionsInCategory { get; set; } = new();
    }
    public class CategoryQuestionData
    {
        public Dictionary<int, (Dictionary<int, QuestionData> Questions, Dictionary<int, ActionData> Actions)>
            QuestionContent
        { get; set; } = new();
    }
}
