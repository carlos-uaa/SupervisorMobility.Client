using DocumentFormat.OpenXml.Drawing;
using Microsoft.JSInterop;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using KellermanSoftware.CompareNetObjects;

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
            _options = new JsonSerializerOptions
            {
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

        //Upload Users
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
        //Upload User To Superior
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

        public async Task<List<User>> GetUsers(bool includeCollections, bool includeSubordinates)
        {

            try
            {
                var response = await _http.GetAsync($"Users?includeCollections={includeCollections}&includeSubordinates={includeSubordinates}");

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

        public async Task<List<User>> GetUsersByUserTypeInPlantAndArea(int PlantId, int AreaId, int userType, bool includeCollections, bool includeSubordinates)
        {
            try
            {
                var response = await _http.GetAsync($"Users/ByUserTypeInPlantAndArea?&plantid={PlantId}&areaid={AreaId}&typeUser={userType}&includeCollections={includeCollections}&includeSubordinates={includeSubordinates}");

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

        public async Task<List<User>> GetUsersByUserTypeInPlant(int PlantId, int userType, bool includeCollections, bool includeSubordinates)
        {
            try
            {
                var response = await _http.GetAsync($"Users/ByUserTypeInPlant?&plantid={PlantId}&typeUser={userType}&includeCollections={includeCollections}&includeSubordinates={includeSubordinates}");

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


        public async Task<List<User>> GetUsersByType(int userType, bool includeCollections, bool includeSubordinates)
        {
            try
            {
                var response = await _http.GetAsync($"Users/ByUserType?typeUser={userType}&includeCollections={includeCollections}&includeSubordinates={includeSubordinates}");

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

            if (UserToUpdate.Areas?.Count > 0)
            {
                foreach (var area in UserToUpdate.Areas)
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
        public async Task<bool> PromoveUserAndAssignNewSuperior(int UserId, User _newUser, User _userCopy, int NewSuperiorId)
        {
            Console.WriteLine("PromoveUsers Function");

            if (_newUser.UserType == _userCopy.UserType)
            {
                Console.WriteLine("Userttype igual");

                foreach (User Sub in _newUser.Subordinates)
                {
                    Console.WriteLine("Foreache");

                    //Verifico que no sean listados de reasignacion
                    if (_newUser.Subordinates?.Any(x => x.AreaId == -2) == true)
                    {
                        Console.WriteLine("Existe alguno con area -2");

                        //Incluye usuarios con reasignacion
                        if (Sub.AreaId != -2)
                        {
                            //identifico a los usuarios que se quedan
                            Sub.SuperiorId = -2;
                        }
                        //si tiene un area == -2 significa que debe ser reasignado

                    }
                    else if (_userCopy.Subordinates?.Any(x => x.UserId == Sub.UserId) == true)
                    {
                        Console.WriteLine("No exis existe -2");

                        //esta es una reasignacion, el usuario permanece en el mismo nivel 
                        //los usuarios que no pertenecen al area son reasignados a otro
                        switch (_newUser.UserType)
                        {
                            //saber si los subordinados seran operadores o SV
                            case 2:
                                //tratamos con sV
                                var matchedSubordinate = _newUser.Subordinates.FirstOrDefault(u => u.UserId == Sub.UserId);
                                if (matchedSubordinate != null && matchedSubordinate.AreaId != 0)
                                {
                                    ComparisonResult result = new CompareLogic().Compare(Sub, matchedSubordinate);

                                    if (result.AreEqual)
                                    {
                                        // Los objetos son iguales
                                        if (_newUser.Areas?.Any(x => x.AreaId == Sub.AreaId) == true)
                                        {
                                            Sub.SuperiorId = -2;
                                        }
                                    }
                                }
                                break;
                            case 3:
                                //tratamos con OPeradores/
                                var matchedSubordinateSV = _newUser.Subordinates.FirstOrDefault(u => u.UserId == Sub.UserId);
                                if (matchedSubordinateSV != null && matchedSubordinateSV.AreaId != 0)
                                {
                                    ComparisonResult result = new CompareLogic().Compare(Sub, matchedSubordinateSV);
                                    if (result.AreEqual)
                                    {
                                        // el usuario pertenece a la jerarquia
                                        if (_newUser.AreaId == Sub.AreaId)
                                        {
                                            Sub.SuperiorId = -2;
                                        }
                                    }
                                }
                                break;
                            case 5:
                                var matchedSubordinateM = _newUser.Subordinates.FirstOrDefault(u => u.UserId == Sub.UserId);
                                if (matchedSubordinateM != null && matchedSubordinateM.PlantId != 0)
                                {
                                    ComparisonResult result = new CompareLogic().Compare(Sub, matchedSubordinateM);

                                    if (result.AreEqual)
                                    {
                                        // el usuario pertenece a la jerarquia
                                        if (_newUser.PlantId == Sub.PlantId)
                                        {
                                            Sub.SuperiorId = -2;
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            else
            {
                // es una promocion 
                //este for es para los nuevos subordinados

                if (_newUser.Subordinates?.Count > 0)
                {
                    foreach (User Sub in _newUser.Subordinates)
                    {
                        Sub.SuperiorId = -2;
                    }
                }
                else
                {
                    _newUser.Subordinates = new List<User>();
                }

                if (_userCopy.Subordinates?.Count > 0)
                {
                    foreach (User Sub in _userCopy.Subordinates)
                    {
                        Sub.AreaId = -2;
                        _newUser.Subordinates?.Add(Sub);
                    }
                }



            }



            var response = await _http.PutAsJsonAsync($"Users/ReassingToNewSuperior/{UserId}?NewSuperiorId={NewSuperiorId}", _newUser);

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



        //Get Unregistered Users
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

        // Create UnregisteredUser User
        public async Task<UserNotFound> CreateUnregisteredUser(UserNotFound _newUser)
        {
            var response = await _http.PostAsJsonAsync($"UserNotFound", _newUser);

            if (response.IsSuccessStatusCode)
            {
                var newUser = await response.Content.ReadFromJsonAsync<UserNotFound>();
                return newUser;
            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error Upload Data error: {response.Content.ReadAsStringAsync().Result}");

            }

            return null;

        }

        //update User
        public async Task<bool> UpdateUnregisteredUser(int UserId, UserNotFound UserToUpdate)
        {

            var response = await _http.PutAsJsonAsync($"UserNotFound/{UserId}", UserToUpdate);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }
    }
}
