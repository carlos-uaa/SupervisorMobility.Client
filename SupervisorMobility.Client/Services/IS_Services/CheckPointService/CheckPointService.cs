using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.IS;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.IS_Services.CheckpointService
{
    public class CheckpointService : ICheckPointService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public CheckpointService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        #region Checkpoint
        public  async Task<Checkpoint> CreateCheckpoint(Checkpoint CheckpointtoCreate)
        {
            var response = await _http.PostAsJsonAsync($"IS/Template/Checkpoints", CheckpointtoCreate);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var Checkpoint = JsonSerializer.Deserialize<Checkpoint>(content, _options);

            return Checkpoint;
        } 

       

        public async Task<List<Checkpoint>> GetAllCheckpoints(bool includeStandars = false, bool includeSketches = false)
        {
            var response = await _http.GetAsync($"IS/Template/Checkpoints?includeStandars={includeStandars}&includeSketches={includeSketches}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var Checkpoints = JsonSerializer.Deserialize<List<Checkpoint>>(content, _options);

            return Checkpoints;
        }

        public async Task<Checkpoint> GetCheckpoint(int id_Checkpoint, bool includeStandars = false, bool includeSketches = false)
        {
            var response = await _http.GetAsync($"IS/Template/Checkpoints/{id_Checkpoint}?includeStandars={includeStandars}&includeSketches={includeSketches}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var Checkpoint = JsonSerializer.Deserialize<Checkpoint>(content, _options);

            return Checkpoint;
        }

        public async Task<Checkpoint?> UpdateCheckpoint(Checkpoint CheckpointtoUpdate)
        {
            var response = await _http.PutAsJsonAsync($"IS/Template/Checkpoints/{CheckpointtoUpdate.CheckpointId}", CheckpointtoUpdate);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var Checkpoint = JsonSerializer.Deserialize<Checkpoint>(content, _options);

            return Checkpoint;
        }
        public async Task<bool> UpdatePanelSequence(int Checkpoint_Id, Checkpoint Checkpoint)
        {
            var response = await _http.PutAsJsonAsync($"IS/Template/Checkpoints/sequence/{Checkpoint_Id}", Checkpoint);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed");
                return false;
            }
            else
            {
                Console.WriteLine("Ok");
                return true;
            }
        }

        public async Task<Checkpoint> DeleteCheckpoint(int id)
        {
            var response = await _http.DeleteAsync($"IS/Template/Checkpoints/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var Checkpoint = JsonSerializer.Deserialize<Checkpoint>(content, _options);

            return Checkpoint;
        }

        #endregion

        #region CheckpointNorm
        public async Task<CheckpointNorm> CreatecheckpointNorm(CheckpointNorm checkpointNorm)
        {
            var response = await _http.PostAsJsonAsync($"IS/Template/Checkpoints/Norms", checkpointNorm);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var Checkpoint = JsonSerializer.Deserialize<CheckpointNorm>(content, _options);

            return Checkpoint;
        }

        public async Task<CheckpointNorm> GetCheckpointNorm(int id_CheckpointNorm, bool includeSketches = false)
        {
            var response = await _http.GetAsync($"IS/Template/Checkpoints/Norms/{id_CheckpointNorm}?includeSketches={includeSketches}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var CheckpointNorm = JsonSerializer.Deserialize<CheckpointNorm>(content, _options);

            return CheckpointNorm;
        }

        //public async Task<bool> UpdatecheckpointNormSequence(int Checkpoint_Id, CheckpointNorm datacheckpointNorm)
        //{
        //    var response = await _http.PutAsJsonAsync($"IS/Template/Checkpoints/Norms/sequence/{Checkpoint_Id}", datacheckpointNorm);

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        Console.WriteLine("Failed");
        //        return false;
        //    }
        //    else
        //    {
        //        Console.WriteLine("Ok");
        //        return true;
        //    }
        //}

        public async Task<CheckpointNorm> DeleteCheckpointNorm(int id)
        {
            var response = await _http.DeleteAsync($"IS/Template/Checkpoints/Norms/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var Checkpoint = JsonSerializer.Deserialize<CheckpointNorm>(content, _options);

            return Checkpoint;
        }
        #endregion
        public async Task<FileUpload> UploadSketchCheckpoint(MultipartFormDataContent? contentfiles, int Checkpoint_id)
        {
            var response = await _http.PostAsync($"IS/Template/Checkpoints/UploadSkecth/{Checkpoint_id}", contentfiles);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<FileUpload>(content, _options);

                return result;

            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error Upload Data error: {response.Content.ReadAsStringAsync().Result}");
            }

            return null;

        }

        public async Task<string> ShowImageCheckpoint(int idfile)
        {
            var response = await _http.GetAsync($"IS/Template/Checkpoints/Sketch/{idfile}");

            if (response.IsSuccessStatusCode)
            {
                var contentType = response.Content.Headers.ContentType.MediaType;
                var contentBytes = await response.Content.ReadAsByteArrayAsync();
                var base64Content = Convert.ToBase64String(contentBytes);

                return $"data:{contentType};base64,{base64Content}";
            }
            else
            {
                return "Error Loading Image";
            }
        }

        public async Task<FileUpload> UploadSketchCheckpointNorm(MultipartFormDataContent? contentfiles, int CheckpointNorm_id)
        {
            var response = await _http.PostAsync($"IS/Template/Checkpoints/Norms/UploadSkecth/{CheckpointNorm_id}", contentfiles);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<FileUpload>(content, _options);

                return result;

            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error Upload Data error: {response.Content.ReadAsStringAsync().Result}");
            }

            return null;

        }

        public async Task<string> ShowImageCheckpointNorm(int idfile)
        {
            var response = await _http.GetAsync($"IS/Template/Checkpoints/Norms/Sketch/{idfile}");

            if (response.IsSuccessStatusCode)
            {
                var contentType = response.Content.Headers.ContentType.MediaType;
                var contentBytes = await response.Content.ReadAsByteArrayAsync();
                var base64Content = Convert.ToBase64String(contentBytes);

                return $"data:{contentType};base64,{base64Content}";
            }
            else
            {
                return "Error Loading Image";
            }
        }


        public async Task<bool> RemoveSketchCheckPoint(int CheckpointId, int fileUploadId)
        {
            var response = await _http.GetAsync($"IS/Template/Checkpoints/{CheckpointId}/Sketch/{fileUploadId}/remove");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> RemoveSketchCheckPointNorm(int Checkpoint_NormId, int fileUploadId)
        {
            var response = await _http.GetAsync($"IS/Template/Checkpoints/Norms/{Checkpoint_NormId}/Sketch/{fileUploadId}/remove");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }


    }
}
