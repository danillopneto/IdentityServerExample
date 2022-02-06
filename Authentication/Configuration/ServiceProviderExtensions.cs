using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServerAuthentication.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;

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

            var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            applicationDbContext.Database.Migrate();
            EnsureUsers(scope);
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

        private static void EnsureUsers(IServiceScope scope)
        {
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var alice = userMgr.FindByNameAsync("alice").Result;
            if (alice == null)
            {
                alice = new IdentityUser
                {
                    UserName = "alice",
                    Email = "AliceSmith@email.com",
                    EmailConfirmed = true,
                };
                var result = userMgr.CreateAsync(alice, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(alice, new Claim[]
                {
                  new Claim(JwtClaimTypes.Name, "Alice Smith"),
                  new Claim(JwtClaimTypes.GivenName, "Alice"),
                  new Claim(JwtClaimTypes.FamilyName, "Smith"),
                  new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                Log.Debug("alice created");
            }
            else
            {
                Log.Debug("alice already exists");
            }

            var bob = userMgr.FindByNameAsync("bob").Result;
            if (bob == null)
            {
                bob = new IdentityUser
                {
                    UserName = "bob",
                    Email = "BobSmith@email.com",
                    EmailConfirmed = true
                };
                var result = userMgr.CreateAsync(bob, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(bob, new Claim[]
                {
                  new Claim(JwtClaimTypes.Name, "Bob Smith"),
                  new Claim(JwtClaimTypes.GivenName, "Bob"),
                  new Claim(JwtClaimTypes.FamilyName, "Smith"),
                  new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                  new Claim("location", "somewhere")
                }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                Log.Debug("bob created");
            }
            else
            {
                Log.Debug("bob already exists");
            }
        }
    }
}