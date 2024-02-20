using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.SignatureImageService
{
    public class SignatureImageService
    {
        private string _imageBase64 = "";

        public void SetImage(string imageBase64)
        {
            _imageBase64 = imageBase64;
        }

        public string GetImage()
        {
            return _imageBase64;
        }
    }
}
