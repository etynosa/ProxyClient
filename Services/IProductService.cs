using Microsoft.Extensions.Caching.Memory;
using Practice.Models;
using Practice.Models.Dtos;

namespace Practice.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetProductsAsync(string? name, int page, int pageSize);
        Task<Product> CreateProductAsync(ProductCreateRequestDto request);
        Task<bool> DeleteProductAsync(string id);
        Task<Product?> GetProductByIdAsync(string id);
        Task<Product?> UpdateProductAsync(string id, ProductCreateRequestDto request);
        Task<Product?> PatchProductAsync(string id, Dictionary<string, object> patchData);

    }

    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ProductService> _logger;

        private const string BaseUrl = "https://api.restful-api.dev/objects";

        public ProductService(HttpClient httpClient, IMemoryCache cache, ILogger<ProductService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(string? name, int page, int pageSize)
        {
            var cacheKey = $"products_{name}_{page}_{pageSize}";
            if (_cache.TryGetValue(cacheKey, out IEnumerable<Product> cached))
                return cached;

            var response = await _httpClient.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();

            var products = await response.Content.ReadFromJsonAsync<List<Product>>() ?? new();

            if (!string.IsNullOrWhiteSpace(name))
            {
                products = products
                    .Where(p => p.Name != null && p.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var paged = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            _cache.Set(cacheKey, paged, TimeSpan.FromMinutes(2));

            return paged;
        }

        public async Task<Product> CreateProductAsync(ProductCreateRequestDto request)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Product>();
        }

        public async Task<bool> DeleteProductAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<Product?> GetProductByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<Product>();
        }

        public async Task<Product?> UpdateProductAsync(string id, ProductCreateRequestDto request)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request);
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<Product>();
        }

        public async Task<Product?> PatchProductAsync(string id, Dictionary<string, object> patchData)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, $"{BaseUrl}/{id}")
            {
                Content = JsonContent.Create(patchData)
            };

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<Product>();
        }
    }
}