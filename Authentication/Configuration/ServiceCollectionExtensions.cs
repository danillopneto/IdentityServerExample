using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace IdentityServerAuthentication.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureIdentityServer(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var migrationAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

            services
                   .AddIdentityServer()
                   .AddTestUsers(Config.Users)
                   .AddConfigurationStore(options =>
                   {
                       options.ConfigureDbContext = builder => builder.UseSqlServer(
                                                                                    connectionString,
                                                                                    opt => opt.MigrationsAssembly(migrationAssembly));
                   })
                   .AddOperationalStore(options =>
                   {
                       options.ConfigureDbContext = builder => builder.UseSqlServer(
                                                                                    connectionString,
                                                                                    opt => opt.MigrationsAssembly(migrationAssembly));
                   })
                   .AddDeveloperSigningCredential();

            return services;
        }
    }
}