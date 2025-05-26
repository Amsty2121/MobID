using MobID.MainGateway.Extensions.IoC;

namespace MobID.MainGateway.Extensions
{
    public static class AddDependencyExtension
    {
        public static IServiceCollection AddDependencyServiceExtension(this IServiceCollection services)
        {
            services.AddAppServiceExtension();
            services.AddRepositoriesExtension();
            return services;
        }
    }
}
