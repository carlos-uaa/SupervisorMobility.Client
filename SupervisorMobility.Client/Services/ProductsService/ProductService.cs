using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.ProductsService
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public ProductService(HttpClient http)
        {
            _http = http;
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
        
        public async Task DeleteDistribution(int productId, int distributionId)
        {
            var response = await _http.DeleteAsync($"products/{productId}/distributions/{distributionId}");
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
    }
}
