using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Services.UserService
{
    public interface IUserService
    {
        Task<UsersUploadResult> ProccedToUploadUsers(FileUpload fileinfo);
        Task<UsersUploadResult> UploadUsers(List<User> UsersToUpload);
        Task<UsersUploadResult> UploadUsersToSuperior(List<User> UsersToUpload, User Superior);

        //Create User
        Task<User> CreateUser(User _newUser);

        //READ
        //get  Users
  
        Task<List<User>> GetSubordinates(int SupervisorId);
        Task<User> GetUser(int UserId);
        Task<User> GetUserWhitObjectId(string ObjectId);
        Task<User> GetUserByObjectIdWithCollections(string ObjectId);
        Task<User> GetUserByEmailWithCollections(string email);
        Task<User> GetUserAndCollection(int UserId);

        Task<List<User>> GetUsers(bool includeCollections, bool includeSubordinates);
        Task<List<User>> GetUsersByType(int userType, bool includeCollections, bool includeSubordinates);
        Task<List<User>> GetUsersByUserTypeInPlantAndArea(int PlantId,int AreaId,int userType, bool includeCollections, bool includeSubordinates);
        Task<List<User>> GetUsersByUserTypeInPlant(int PlantId, int userType, bool includeCollections, bool includeSubordinates);

        //UPDATE
        Task<bool> UpdateUser(int UserId, User _newUser);
        Task<bool> PromoveUserAndAssignNewSuperior(int UserId, User _newUser, User _userCopy, int NewSuperiorId);
      
        //DELETE
        Task DeleteUser(int UserId);

        //UsersFormat
        Task DownloadAllUsersFormat();
        Task DownloadSSVFormat();
        Task DownloadSupervisorsFormat();
        Task DownloadOperatorsFormat();


        //Users Not Found
        Task<List<UserNotFound>> GetUsersNotFound();
        Task<UserNotFound> CreateUnregisteredUser(UserNotFound _newUser);
        Task<bool> UpdateUnregisteredUser(int UserId, UserNotFound _newUser);

    }
}
