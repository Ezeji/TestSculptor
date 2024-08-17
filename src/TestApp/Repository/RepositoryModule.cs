using TestApp.Models;
using TestApp.Repository.Interfaces;

namespace TestApp.Repository
{
    public static class RepositoryModule
    {
        public static void AddCoreRepository(this IServiceCollection services)
        {
            services.AddScoped<IGenericRepository<Product>, GenericRepository<Product>>();
            services.AddScoped<IGenericRepository<Order>, GenericRepository<Order>>();
        }
    }
}
