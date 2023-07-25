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
        Task<List<User>> GetUsers();
        Task<List<User>> GetUsersWhitCollections();
        Task<List<User>> GetSubordinates(int SupervisorId);
        Task<User> GetUser(int UserId);
        Task<User> GetUserWhitObjectId(string ObjectId);
        Task<User> GetUserByObjectIdWithCollections(string ObjectId);
        Task<User> GetUserByEmailWithCollections(string email);
        Task<User> GetUserAndCollection(int UserId);
        Task<List<User>> GetUserByTypeAndCollection(int userType);
        Task<List<User>> GetUserByType(int userType);
        Task<List<User>> GetUserByUserTypeInPlantAndArea(int PlantId,int AreaId,int userType, bool includeCollections);

        //UPDATE
        Task<bool> UpdateUser(int UserId, User _newUser);

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
