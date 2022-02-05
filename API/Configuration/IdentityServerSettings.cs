namespace WeatherApi.Configuration
{
    public class IdentityServerSettings
    {
        public string ApiName { get; set; }

        public string AuthenticationScheme { get; set; }
        
        public string AuthorityUrl { get; set; }

        public bool RequireHttpsMetadata { get; set; }
    }
}
