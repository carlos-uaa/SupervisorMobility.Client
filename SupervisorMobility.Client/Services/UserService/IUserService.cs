using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Services.UserService
{
    public interface IUserService
    {
        Task<UsersUploadResult> ProccedToUploadUsers(FileUpload fileinfo);

        //Create User
        Task<User> CreateUser(User _newUser);

        //READ
        //get  Users
        Task<List<User>> GetUsers();
        Task<List<User>> GetUsersWhitCollections();
        Task<User> GetUser(int UserId);
        Task<User> GetUserWhitObjectId(string ObjectId);
        Task<User> GetUserWhitObjectIdAndCollections(string ObjectId);
        Task<User> GetUserAndCollection(int UserId);

        //UPDATE
        Task<bool> UpdateUser(int UserId, User _newUser);

        //DELETE
        Task DeleteUser(int UserId);

    }
}
