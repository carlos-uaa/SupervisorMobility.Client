using DocumentFormat.OpenXml.Drawing;
using Microsoft.JSInterop;
using System.ComponentModel;
using System.IO;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace SupervisorMobility.Client.Services.UserService
{
    public class UserService : IUserService
    {

        private readonly HttpClient _http;
        private readonly HttpClient _httpBridge;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public UserService(CustomHttpClientService customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService.GetApiHttpClient();
            _httpBridge = customHttpClientService.GetBridgeHttpClient();
            _js = jSRuntime;
            _options = new JsonSerializerOptions { 
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
            };
            _options.Converters.Add(new IntToStringConverter());
        }

       
        public async Task<UsersUploadResult> ProccedToUploadUsers(FileUpload fileinfo)
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


        public async Task<UsersUploadResult> UploadUsers(List<User> UsersToUpload)
        {
            var response = await _http.PostAsJsonAsync("Users/MasiveUpload", UsersToUpload);

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

        public async Task<UsersUploadResult> UploadUsersToSuperior(List<User> UsersToUpload, User Superior)
        {
            var response = await _http.PostAsJsonAsync($"Users/MasiveUpload/Superior/{Superior.UserId}", UsersToUpload);

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

            if (response.IsSuccessStatusCode)
            {
                var newUser = await response.Content.ReadFromJsonAsync<User>();
                return newUser;
            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error Upload Data error: {response.Content.ReadAsStringAsync().Result}");

            }

            return null;
            

           
        }
       
        //get User by id
        public async Task<User> GetUser(int UserId)
        {
            var response = await _http.GetAsync($"Users/{UserId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<User>();
                return content;
            }

            return null;
        }  
        //get User by objectid
        public async Task<User> GetUserWhitObjectId(string ObjectId)
        {
            var response = await _http.GetAsync($"Users/ByObjectId?ObjectId={ObjectId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<User>();
                return content;
            }

            return null;
        }   
        //get User by objectid whitr collection
        public async Task<User> GetUserByObjectIdWithCollections(string ObjectId)
        {
            var response = await _http.GetAsync($"Users/ByObjectId?ObjectId={ObjectId}&collections=true");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<User>();
                return content;
            }

            return null;
        }

        //Get User by email with collections
        public async Task<User> GetUserByEmailWithCollections(string email)
        {
            var response = await _http.GetAsync($"Users/ByEmail?Email={email}&collections=true");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<User>();
                return content;
            }

            return null;
        }

        public async Task<User> GetUserAndCollection(int UserId)
        {
            var response = await _http.GetAsync($"Users/{UserId}?collections=true");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<User>();
                return content;
            }

            return null;


        }

        public async Task<List<User>> GetUsers()
        {
          
            try
            {
                var response = await _http.GetAsync("Users");

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var UsersList = JsonSerializer.Deserialize<List<User>>(content, _options);

                    response.Dispose();

                    return UsersList;
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error al obtener la lista de usuarios: {ex.Message}");
            }

            return null;
           
        } 
        public async Task<List<User>> GetUsersWhitCollections()
        {
            
            try
            {
                var response = await _http.GetAsync("Users?collections=true");

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var UsersList = JsonSerializer.Deserialize<List<User>>(content, _options);

                    response.Dispose();

                    return UsersList;
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error al obtener la lista de usuarios: {ex.Message}");
            }
            
            return null;
           
        } 
        public async Task<List<User>> GetSubordinates(int SupervisorId)
        {
            
            try
            {
                var response = await _http.GetAsync($"Users/{SupervisorId}/Subordinates?collections=true");

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var UsersList = JsonSerializer.Deserialize<List<User>>(content, _options);

                    response.Dispose();

                    return UsersList;
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error al obtener la lista de usuarios: {ex.Message}");
            }
            
            return null;
           
        }
        //delete User
        public async Task DeleteUser(int UserId)
        {
            var response = await _http.DeleteAsync($"Users/{UserId}");
        }

        //update User
        public async Task<bool> UpdateUser(int UserId, User UserToUpdate)
        {

            if(UserToUpdate.Areas?.Count > 0)
            {
                foreach(var area in UserToUpdate.Areas)
                {
                    Console.WriteLine(area.AreaId);
                }

            }

            var response = await _http.PutAsJsonAsync($"Users/{UserId}", UserToUpdate);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

        //USERS FORMATS
        public async Task DownloadAllUsersFormat()
        {
            var response = await _http.GetAsync($"Users/Bulk/DownloadAllUsersFormat");

            if (!response.IsSuccessStatusCode)
            {
                await _js.InvokeVoidAsync("alert", "Error File Download");
            }
            else
            {
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", "AllUsersFormat.xlsx", streamRef);
            }
        }

        public async Task DownloadSSVFormat()
        {
            var response = await _http.GetAsync($"Users/Bulk/DownloadSSVFormat");

            if (!response.IsSuccessStatusCode)
            {
                await _js.InvokeVoidAsync("alert", "Error File Download");
            }
            else
            {
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", "SSVFormat.xlsx", streamRef);
            }
        }

        public async Task DownloadSupervisorsFormat()
        {
            var response = await _http.GetAsync($"Users/Bulk/DownloadSupervisorFormat");

            if (!response.IsSuccessStatusCode)
            {
                await _js.InvokeVoidAsync("alert", "Error File Download");
            }
            else
            {
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", "SupervisorFormat.xlsx", streamRef);
            }
        }

        public async Task DownloadOperatorsFormat()
        {
            var response = await _http.GetAsync($"Users/Bulk/DownloadOperatorsFormat");

            if (!response.IsSuccessStatusCode)
            {
                await _js.InvokeVoidAsync("alert", "Error File Download");
            }
            else
            {
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", "OperatorsFormat.xlsx", streamRef);
            }
        }



        //Get Users not found
        public async Task<List<UserNotFound>> GetUsersNotFound()
        {

            try
            {
                var response = await _http.GetAsync("UserNotFound");

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var UsersList = JsonSerializer.Deserialize<List<UserNotFound>>(content, _options);

                    response.Dispose();

                    return UsersList;
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error al obtener la lista de usuarios: {ex.Message}");
            }

            return null;

        }
    }
}
