using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
// using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityServerHost.Quickstart.UI
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly IOptions<IdentityServerSettings> _identityServerSettings;
        private readonly DiscoveryDocumentResponse _discoveryDocument;

        public TokenService(ILogger<TokenService> logger, IOptions<IdentityServerSettings> identityServerSettings)
        {
            //_logger = logger;
            _identityServerSettings = identityServerSettings;

            //using var httpClient = new HttpClient();
            //_discoveryDocument = httpClient.GetDiscoveryDocumentAsync(identityServerSettings.Value.DiscoveryUrl).Result;
            //if (_discoveryDocument.IsError)
            //{
            //    logger.LogError($"Unable to get discovery document. Error is: {_discoveryDocument.Error}");
            //    throw new Exception("Unable to get discovery document", _discoveryDocument.Exception);
            //}
        }
        // is the same way the curl command works to send reqesut to the token endpoint
        public async Task<string> GetToken(string scope)
        {
            using var client = new HttpClient();
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest // this method come from identity model package
                                  {
                                      Address = "https://localhost:5443/connect/token", // get TokenEndpoint from the discovery document  

                                      ClientId = _identityServerSettings.Value.ClientName,
                                      ClientSecret = _identityServerSettings.Value.ClientPassword,
                                      Scope = scope
                                  });


            if (tokenResponse.IsError)
            {
                _logger.LogError($"Unable to get token. Error is: {tokenResponse.Error}");
                throw new Exception("Unable to get token", tokenResponse.Exception);
            }

            return tokenResponse.AccessToken;
        }

    }
}