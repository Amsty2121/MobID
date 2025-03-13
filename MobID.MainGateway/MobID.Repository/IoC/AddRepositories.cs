using Microsoft.Extensions.DependencyInjection;
using SmartPass.Repository.Implementations;
using SmartPass.Repository.Interfaces;

namespace SmartPass.Repository.IoC
{
    public static class AddRepositories
    {
        public static IServiceCollection AddRepositoriesExtension(this IServiceCollection services)
        {

            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepositoryq<,>));
            return services;
        }
    }
}
