using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using MobID.MainGateway.Configuration;
using MobID.MainGateway.Constants;
using MobID.MainGateway.Models.Entities;

namespace MobID.MainGateway.Extensions
{
    public static class AuthServiceExtensions
    {
        public static void AddJwtAuthentication(this IServiceCollection services, AuthOptions authOptions)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authOptions.Issuer,

                    ValidateAudience = true,
                    ValidAudience = authOptions.Audience,

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = authOptions.GetSymmetricSecurityKey(),

                    RoleClaimType = "Roles"
                };
            });
        }

        public static void AddJwtAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                                        .RequireAuthenticatedUser()
                                        .RequireClaim(nameof(User.Id))
                                        .Build();

                //options.AddPolicy(Policies.SupervizerPolicy, pb => pb.RequireAuthenticatedUser().RequireClaim("iss", Policies.SupervizerPolicy).Build());

                //options.AddPolicy(Policies.AdminService, pb => pb.RequireAuthenticatedUser().RequireClaim("iss", Policies.AdminService).Build());
            });
        }

        public static AuthOptions ConfigureAuthOptions(this IServiceCollection services, IConfiguration configuration)
        {
            var authOptionsConfigurationSection = configuration.GetSection(MobIDConstants.AuthOptions);
            services.Configure<AuthOptions>(authOptionsConfigurationSection);
            var authOptions = authOptionsConfigurationSection.Get<AuthOptions>();

            return authOptions;
        }

        /*public static SyncServiceSettings ConfigureSyncServiceOptions(this IServiceCollection services, IConfiguration configuration)
        {
            var syncServiceConfigurationSection = configuration.GetSection("SyncServiceRoute");
            services.Configure<SyncServiceSettings>(syncServiceConfigurationSection);
            var syncServiceSettings = syncServiceConfigurationSection.Get<SyncServiceSettings>();

            return syncServiceSettings;
        }*/
    }
}
