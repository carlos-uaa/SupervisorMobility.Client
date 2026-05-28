using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.Dtos;
using SupervisorMobility.Client.Data.SPModels;
using SupervisorMobility.Client.Services.UserService;

namespace TestSupervisorMobility.Client.Services
{
    public sealed class FakeUserService : IUserService
    {
        public Task<UsersUploadResult> ProccedToUploadUsers(FileUpload fileinfo) => throw new NotImplementedException();
        public Task<UsersUploadResult> UploadUsers(List<User> UsersToUpload) => throw new NotImplementedException();
        public Task<UsersUploadResult> UploadUsersToSuperior(List<User> UsersToUpload, User Superior) => throw new NotImplementedException();
        public Task<User> CreateUser(User _newUser) => throw new NotImplementedException();
        public Task<List<User>> GetSubordinates(int SupervisorId, bool includeCollections = true) => throw new NotImplementedException();
        public Task<User> GetUser(int UserId) => throw new NotImplementedException();
        public Task<User> GetUserWhitObjectId(string ObjectId) => throw new NotImplementedException();
        public Task<User> GetUserByObjectIdWithCollections(string ObjectId) => throw new NotImplementedException();
        public Task<User> GetUserByEmailWithCollections(string email) => throw new NotImplementedException();
        public Task<User> GetUserAndCollection(int UserId) => throw new NotImplementedException();
        public Task<List<User>> GetUsers(bool includeCollections, bool includeSubordinates) => Task.FromResult(new List<User>
        {
            new() { UserId = 10, Name = "Supervisor de prueba", UserType = 3, IsActive = true, PlantId = 1, Areas = new List<Area> { new() { AreaId = 1 } } },
            new() { UserId = 20, Name = "Senior supervisor de prueba", UserType = 2, IsActive = true, PlantId = 1, Areas = new List<Area> { new() { AreaId = 1 } } },
            new() { UserId = 30, Name = "Operador de prueba", UserType = 4, IsActive = true, PlantId = 1, Areas = new List<Area> { new() { AreaId = 1 } } }
        });
        public Task<List<User>> GetUsersByType(int userType, bool includeCollections, bool includeSubordinates) => throw new NotImplementedException();
        public Task<List<User>> GetUsersByUserTypeInPlantAndArea(int PlantId, int AreaId, int userType, bool includeCollections, bool includeSubordinates) => throw new NotImplementedException();
        public Task<List<User>> GetUsersByUserTypeInPlant(int PlantId, int userType, bool includeCollections, bool includeSubordinates) => throw new NotImplementedException();
        public Task<bool> UpdateUser(int UserId, User _newUser, int areaTypeUpdate) => throw new NotImplementedException();
        public Task<bool> ReassignNewSuperior(List<User> UsersReassign, int reasignType) => throw new NotImplementedException();
        public Task<bool> CleanUsers(List<User> CleanUsers) => throw new NotImplementedException();
        public Task DeleteUser(int UserId) => throw new NotImplementedException();
        public Task DownloadAllUsersFormat() => throw new NotImplementedException();
        public Task DownloadSSVFormat() => throw new NotImplementedException();
        public Task DownloadSupervisorsFormat() => throw new NotImplementedException();
        public Task DownloadOperatorsFormat() => throw new NotImplementedException();
        public Task<ServiceResponse<WFMInfoSP>> GetPersonalInfoByPersonalNumber(int personalNumber) => throw new NotImplementedException();
        public Task<List<UserNotFound>> GetUsersNotFound() => throw new NotImplementedException();
        public Task<UserNotFound> CreateUnregisteredUser(UserNotFound _newUser) => throw new NotImplementedException();
        public Task<bool> UpdateUnregisteredUser(int UserId, UserNotFound _newUser) => throw new NotImplementedException();
        public Task<ServiceResponse<UpdateUsersAreasResult>> UpdateUsersAreas(List<UpdateAreasForSuperiorDto> _usersList) => throw new NotImplementedException();
    }
}