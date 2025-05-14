using Practice.Services;

namespace Practice.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddHttpClient<IProductService, ProductService>();
            services.AddScoped<IProductService, ProductService>();
        }
    }
}
