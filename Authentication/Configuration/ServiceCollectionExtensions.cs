using IdentityServerAuthentication.Data;
using Microsoft.AspNetCore.Identity;
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

            services.AddLogging();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationAssembly));
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            services.AddIdentityServer()
                    .AddAspNetIdentity<IdentityUser>()
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