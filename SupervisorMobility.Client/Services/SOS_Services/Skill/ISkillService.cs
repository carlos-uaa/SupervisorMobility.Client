namespace SupervisorMobility.Client.Services.SOS_Services.SkillServices
{
    public interface ISkillService
    {
        Task<List<Skill>> GetSkills();

        Task<Skill> GetSkillById(int id);

        Task<Skill> CreateSkill(CreateSkillDto Skill);

    }
}
