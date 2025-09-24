namespace SupervisorMobility.Client.Data.Entities.SOS_Process.STRO.Collection.Skill.Dtos
{
    public class SkillDynamicDTO
    {
        public int SOSHubId { get; set; }
        public string SelectedSkillName { get; set; } = string.Empty;

        public List<Skill> FilteredSkill { get; set; } = new();

        public List<int> SkillIds { get; set; } = new();
    }
}
