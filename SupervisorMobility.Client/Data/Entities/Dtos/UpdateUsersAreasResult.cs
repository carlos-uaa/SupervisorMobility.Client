namespace SupervisorMobility.Client.Data.Entities.Dtos
{
    public class UpdateUsersAreasResult
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public int TotalUsers { get; set; }
        public int SuccessfullyUpdated { get; set; }
        public int Failed { get; set; }
        public List<UserUpdateError> Errors { get; set; } = new List<UserUpdateError>();
    }

    public class UserUpdateError
    {
        public int UserId { get; set; }
        public string ErrorType { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public List<int> MissingAreaIds { get; set; } = new List<int>();
    }
}
