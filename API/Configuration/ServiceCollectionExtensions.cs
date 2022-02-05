namespace WeatherApi.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection(nameof(IdentityServerSettings)).Get<IdentityServerSettings>();
            services.AddAuthentication(settings.AuthenticationScheme)
                .AddIdentityServerAuthentication(settings.AuthenticationScheme, options =>
                {
                    options.ApiName = settings.ApiName;
                    options.Authority = settings.AuthorityUrl;
                    options.RequireHttpsMetadata = settings.RequireHttpsMetadata;
                });

            return services;
        }                
    }
}
