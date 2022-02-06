namespace IdentityServerAuthentication.Configuration
{
    public class IdentityServerSettings
    {
        public string RedirectUris { get; set; }

        public string FrontChannelLogoutUri { get; set; }

        public string PostLogoutRedirectUris { get; set; }

        public string[] ApiScopes { get; set; }

        public string Secret { get; set; }
    }
}