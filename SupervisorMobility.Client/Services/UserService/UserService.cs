using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.UserService
{
    public class UserService : IUserService
    {

        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public UserService(HttpClient http, IJSRuntime jSRuntime)
        {
            _http = http;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        //
        public async Task<UploadResult> UploadFileUsers(MultipartFormDataContent contentfile)
        {
            var response = await _http.PostAsync("Users/FileUpload", contentfile);

            var result = await response.Content.ReadFromJsonAsync<UploadResult>();

            return result;
        }
        public async Task<UsersUploadResult> ProccedToUploadUsers(UploadResult fileinfo)
        {
            var response = await _http.PostAsJsonAsync("Users/FileUpload/Data", fileinfo);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<UsersUploadResult>();
                return result;
            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error Upload Useres error: {response.Content.ReadAsStringAsync().Result}");
            }

            return null;
        }

        //Create User
        public async Task<User> CreateUser(User _newUser)
        {
            var response = await _http.PostAsJsonAsync($"Users", _newUser);
            var newUser = await response.Content.ReadFromJsonAsync<User>();
            return newUser;
        }
       
        //get User by id
        public async Task<User> GetUser(int UserId)
        {
            var response = await _http.GetAsync($"Users/{UserId}");
            var content = await response.Content.ReadFromJsonAsync<User>();
            return content;
        }

        public async Task<List<User>> GetUsers()
        {
            var response = await _http.GetAsync("Users");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var UsersList = JsonSerializer.Deserialize<List<User>>(content, _options);

            return UsersList;
        }
        //delete User
        public async Task DeleteUser(int UserId)
        {
            var response = await _http.DeleteAsync($"Users/{UserId}");
        }
        //update User
        public async Task<bool> UpdateUser(int UserId, User UserToUpdate)
        {
            var response = await _http.PutAsJsonAsync($"Users/{UserId}", UserToUpdate);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }
    }
}
