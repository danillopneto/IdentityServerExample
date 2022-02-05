using IdentityModel.Client;
using WeatherMvc.Configuration;

namespace WeatherMvc.Services
{
    public class TokenService : ITokenService
    {
        private readonly DiscoveryDocumentResponse _discoveryDocument;

        private readonly IdentityServerSettings _identityServerSettings;

        private readonly ILogger<TokenService> _logger;

        public TokenService(ILogger<TokenService> logger, IdentityServerSettings identityServerSettings)
        {
            _logger = logger;
            _identityServerSettings = identityServerSettings;

            using var httpClient = new HttpClient();
            _discoveryDocument = httpClient.GetDiscoveryDocumentAsync(identityServerSettings.DiscoveryUrl).Result;
            if (_discoveryDocument.IsError)
            {
                logger.LogError("Unable to get discovery document. Error is: {Error}", _discoveryDocument.Error);
                throw new Exception("Unable to get discovery document", _discoveryDocument.Exception);
            }
        }

        public async Task<TokenResponse> GetToken(string scope)
        {
            using var client = new HttpClient();
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = _discoveryDocument.TokenEndpoint,

                ClientId = _identityServerSettings.ClientName,
                ClientSecret = _identityServerSettings.ClientPassword,
                Scope = scope
            });

            if (tokenResponse.IsError)
            {
                _logger.LogError("Unable to get token. Error is: {Error}", tokenResponse.Error);
                throw new Exception("Unable to get token", tokenResponse.Exception);
            }

            return tokenResponse;
        }
    }
}