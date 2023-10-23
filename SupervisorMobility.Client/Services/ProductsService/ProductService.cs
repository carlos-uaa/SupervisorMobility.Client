using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.ProductsService
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;


        // Constructor
        public ProductService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _js = jSRuntime;

            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // Create product
        public async Task<Product> CreateProduct(Product product)
        {
            var response = await _http.PostAsJsonAsync("products", product);
            var newProduct = await response.Content.ReadFromJsonAsync<Product>();

            return newProduct;
        }

        // Delete product
        public async Task DeleteProduct(int id)
        {
            var response = await _http.DeleteAsync($"products/{id}");
        }
      
       

        // Get product by Id
        public async Task<Product> GetProductById(int id)
        {
            var response = await _http.GetAsync($"products/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var product = JsonSerializer.Deserialize<Product>(content, _options);

            return product;
        }

        public async Task<Product> GetProductAndCollection(int id)
        {
            var response = await _http.GetAsync($"products/{id}?collections=true");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var product = JsonSerializer.Deserialize<Product>(content, _options);

            return product;
        }

        // Get all products
        public async Task<List<Product>> GetProducts()
        {
            var response = await _http.GetAsync("products");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var products = JsonSerializer.Deserialize<List<Product>>(content, _options);

            return products;
        }

        // Update product
        public async Task<bool> UpdateProduct(Product product)
        {
            var response = await _http.PutAsJsonAsync($"products/{product.ProductId}", product);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

        //get distribution
        public async Task<Distribution> GetDistributionOnlyById(int productId, int distributionId)
        {
            var response = await _http.GetAsync($"products/{productId}/distributions/{distributionId}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var distribution = JsonSerializer.Deserialize<Distribution>(content, _options);

            return distribution;
        }
        //Create Distribution 
        public async Task<Distribution> CreateDistribution(int productId, int plantId, int areaId, Distribution distribution)
        {
            var response = await _http.PostAsJsonAsync<Distribution>($"products/{productId}/distributions/create?plantId={plantId}&areaId={areaId}", distribution);

            if (response.IsSuccessStatusCode)
            {
                var newDistribution = await response.Content.ReadFromJsonAsync<Distribution>();
                return newDistribution;
            }
            await _js.InvokeVoidAsync("alert", $"Error : {response.Content.ReadAsStringAsync().Result}");

            return null;
        }

        public async Task<Distribution> AddDistribution(int productId, int plantId, int areaId, Distribution distribution)
        {
            var response = await _http.PostAsJsonAsync<Distribution>($"products/{productId}/distributions/add?plantId={plantId}&areaId={areaId}", distribution);


            if (response.IsSuccessStatusCode)
            {
                var newDistribution = await response.Content.ReadFromJsonAsync<Distribution>();
                return newDistribution;
            }
            await _js.InvokeVoidAsync("alert", $"Error : {response.Content.ReadAsStringAsync().Result}");

            return null;
        }


        public async Task<bool> UpdateDistributionForProduct(int productId, Distribution distribution)
        {
            var response = await _http.PutAsJsonAsync($"products/{productId}/distributions/{distribution.DistributionId}/update", distribution);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }


        public async Task DeleteDistribution(int productId, int distributionId)
        {
            var response = await _http.DeleteAsync($"products/{productId}/distributions/{distributionId}/remove");
        }
    }
}
