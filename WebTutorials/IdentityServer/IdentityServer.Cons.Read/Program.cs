// See https://aka.ms/new-console-template for more information

using IdentityModel;
using IdentityModel.Client;

using var httpClient = new HttpClient();
var discoveryDocumentResponse = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5110");

if (discoveryDocumentResponse.IsError)
{ 
    Console.Write(discoveryDocumentResponse.Error);
}
else
{
    var clientCredentialsTokenRequest = new ClientCredentialsTokenRequest
    {
        Address = discoveryDocumentResponse.TokenEndpoint,
        GrantType = OidcConstants.GrantTypes.ClientCredentials,
        ClientId = "console-read",
        ClientSecret = "console-read-secret",
        Scope = "api.read"
    };
    
    var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(clientCredentialsTokenRequest);
    if (tokenResponse.IsError)
    {
        Console.WriteLine(tokenResponse.Error);
    }
    else
    {
        Console.WriteLine(tokenResponse.AccessToken);
        httpClient.SetBearerToken(tokenResponse.AccessToken);
        
        var httpResponseMessage = await httpClient.GetAsync("https://localhost:7201/weatherforecast");
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            Console.WriteLine(httpResponseMessage.StatusCode);
        }
        else
        {
            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            Console.WriteLine(content);
        }
        
        var httpResponseMessage2 = await httpClient.GetAsync("https://localhost:7154/weatherforecast");
        if (!httpResponseMessage2.IsSuccessStatusCode)
        {
            Console.WriteLine(httpResponseMessage2.StatusCode);
        }
        else
        {
            var content = await httpResponseMessage2.Content.ReadAsStringAsync();
            Console.WriteLine(content);
        }
    }
}