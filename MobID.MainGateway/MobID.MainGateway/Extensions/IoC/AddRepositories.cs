using MobID.MainGateway.Repo;
using MobID.MainGateway.Repo.Interfaces;
using System.Reflection;

namespace MobID.MainGateway.Extensions.IoC
{
    public static class AddRepositories
    {
        public static IServiceCollection AddRepositoriesExtension(this IServiceCollection appService)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            appService.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            types.Where(type => type.IsInterface).ToList()
                .ForEach(interfac => types.Where(type => type.GetInterfaces().Contains(interfac)).ToList()
                .ForEach(implementation => appService.AddScoped(interfac, implementation)));

            return appService;
        }
    }
}
