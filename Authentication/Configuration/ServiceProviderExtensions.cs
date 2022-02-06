using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IdentityServerAuthentication.Configuration
{
    public static class ServiceProviderExtensions
    {
        public static void InitializeDatabase(this IServiceProvider services, Config configuration)
        {
            using var scope = services.CreateScope();

            var persistedContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
            persistedContext.Database.Migrate();

            var configurationContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            configurationContext.Database.Migrate();

            EnsureSeedData(configurationContext, configuration);
        }

        private static void EnsureSeedData(ConfigurationDbContext context, Config configuration)
        {
            if (!context.Clients.Any())
            {
                Log.Debug("Clients being populated");
                foreach (var client in configuration.GetClients().ToList())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Log.Debug("Clients already populated");
            }

            if (!context.IdentityResources.Any())
            {
                Log.Debug("IdentityResources being populated");
                foreach (var resource in configuration.IdentityResources.ToList())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Log.Debug("IdentityResources already populated");
            }

            if (!context.ApiScopes.Any())
            {
                Log.Debug("ApiScopes being populated");
                foreach (var resource in configuration.ApiScopes.ToList())
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Log.Debug("ApiScopes already populated");
            }

            if (!context.ApiResources.Any())
            {
                Log.Debug("ApiResources being populated");
                foreach (var resource in configuration.ApiResources.ToList())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Log.Debug("ApiScopes already populated");
            }
        }
    }
}