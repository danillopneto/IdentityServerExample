using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using IdentityServerAuthentication.Configuration;
using System.Security.Claims;
using System.Text.Json;

namespace IdentityServerAuthentication
{
    public class Config
    {
        private readonly IdentityServerSettings _identityServerSettings;

        public Config(IdentityServerSettings identityServerSettings)
        {
            _identityServerSettings = identityServerSettings ?? throw new ArgumentNullException(nameof(identityServerSettings));
        }

        public List<TestUser> Users
        {
            get
            {
                var address = new
                {
                    street_address = "One Hacker Way",
                    locality = "Heidelberg",
                    postal_code = 69118,
                    country = "Germany"
                };

                return new List<TestUser>
        {
          new TestUser
          {
            SubjectId = "818727",
            Username = "alice",
            Password = "alice",
            Claims =
            {
              new Claim(JwtClaimTypes.Name, "Alice Smith"),
              new Claim(JwtClaimTypes.GivenName, "Alice"),
              new Claim(JwtClaimTypes.FamilyName, "Smith"),
              new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
              new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
              new Claim(JwtClaimTypes.Role, "admin"),
              new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
              new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address),
                IdentityServerConstants.ClaimValueTypes.Json)
            }
          },
          new TestUser
          {
            SubjectId = "88421113",
            Username = "bob",
            Password = "bob",
            Claims =
            {
              new Claim(JwtClaimTypes.Name, "Bob Smith"),
              new Claim(JwtClaimTypes.GivenName, "Bob"),
              new Claim(JwtClaimTypes.FamilyName, "Smith"),
              new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
              new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
              new Claim(JwtClaimTypes.Role, "user"),
              new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
              new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address),
                IdentityServerConstants.ClaimValueTypes.Json)
            }
          }
        };
            }
        }

        public IEnumerable<IdentityResource> IdentityResources =>
          new[]
          {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource
            {
              Name = "role",
              UserClaims = new List<string> {"role"}
            }
          };

        public IEnumerable<ApiScope> ApiScopes => _identityServerSettings.ApiScopes.Select(x => new ApiScope(x));

        public IEnumerable<ApiResource> ApiResources => new[]
        {
          new ApiResource("weatherapi")
          {
            Scopes = _identityServerSettings.ApiScopes,
            ApiSecrets = new List<Secret> {new Secret("ScopeSecret".Sha256())},
            UserClaims = new List<string> {"role"}
          }
        };

        public IEnumerable<Client> GetClients()
        {
            var interactiveScopes = new List<string>
            { 
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile
            };
            interactiveScopes.AddRange(_identityServerSettings.ApiScopes);

            return new[]
              {
                // m2m client credentials flow client
                new Client
                {
                  ClientId = "m2m.client",
                  ClientName = "Client Credentials Client",

                  AllowedGrantTypes = GrantTypes.ClientCredentials,
                  ClientSecrets = {new Secret(_identityServerSettings.Secret.Sha256())},

                  AllowedScopes = _identityServerSettings.ApiScopes
                },

                // interactive client using code flow + pkce
                new Client
                {
                  ClientId = "interactive",
                  ClientSecrets = {new Secret(_identityServerSettings.Secret.Sha256())},

                  AllowedGrantTypes = GrantTypes.Code,

                  RedirectUris = { _identityServerSettings.RedirectUris },
                  FrontChannelLogoutUri = _identityServerSettings.FrontChannelLogoutUri,
                  PostLogoutRedirectUris = { _identityServerSettings.PostLogoutRedirectUris },

                  AllowOfflineAccess = true,
                  AllowedScopes = interactiveScopes,
                  RequirePkce = true,
                  RequireConsent = false,
                  AllowPlainTextPkce = false
                }
              };
        }
    }
}