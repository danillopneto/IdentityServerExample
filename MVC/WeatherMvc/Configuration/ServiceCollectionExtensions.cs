using Microsoft.Extensions.Options;
using WeatherMvc.Configuration;
using WeatherMvc.Services;

namespace WeatherMvc.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration) =>
            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = "cookie";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("cookie").AddOpenIdConnect("oidc", options =>
                {
                    var interactiveSettings = configuration
                                     .GetSection(nameof(InteractiveServiceSettings))
                                     .Get<InteractiveServiceSettings>();
                    options.Authority = interactiveSettings.AuthorityUrl;
                    options.ClientId = interactiveSettings.ClientId;
                    options.ClientSecret = interactiveSettings.ClientSecret;

                    options.ResponseType = "code";
                    options.UsePkce = true;
                    options.ResponseMode = "query";

                    options.Scope.Add(interactiveSettings.Scopes[0]);
                    options.SaveTokens = true;
                }).Services;

        public static IServiceCollection ConfigureServices(this IServiceCollection services) =>
            services.AddScoped<ITokenService, TokenService>();

        public static IServiceCollection ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<IdentityServerSettings>(configuration.GetSection(nameof(IdentityServerSettings)));
            services.AddScoped(c => c.GetService<IOptionsSnapshot<IdentityServerSettings>>().Value);

            return services;
        }
    }
}
