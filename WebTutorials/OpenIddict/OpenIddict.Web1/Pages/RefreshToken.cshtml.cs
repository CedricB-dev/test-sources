using System.Globalization;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace OpenIddict.Web1.Pages;

public class RefreshToken : PageModel
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientFactory _httpClientFactory;

    public RefreshToken(
        IHttpContextAccessor httpContextAccessor,
        IHttpClientFactory httpClientFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _httpClientFactory = httpClientFactory;
    }


    public async Task OnGet(string redirectUri)
    {
        var httpClient = _httpClientFactory.CreateClient("identity");

        var discoveryDocumentAsync = await httpClient.GetDiscoveryDocumentAsync();

        var refreshToken = await _httpContextAccessor.HttpContext
            .GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectParameterNames.RefreshToken);

        var requestRefreshTokenAsync = await httpClient.RequestRefreshTokenAsync(
            new RefreshTokenRequest
            {
                Address = discoveryDocumentAsync.TokenEndpoint,
                ClientId = "web-read",
                RefreshToken = refreshToken
            });

        var authenticationTokens = new List<AuthenticationToken>();
        authenticationTokens.Add(new AuthenticationToken
        {
            Name = OpenIdConnectParameterNames.AccessToken,
            Value = requestRefreshTokenAsync.AccessToken
        });
        authenticationTokens.Add(new AuthenticationToken
        {
            Name = OpenIdConnectParameterNames.RefreshToken,
            Value = requestRefreshTokenAsync.RefreshToken
        });
        authenticationTokens.Add(new AuthenticationToken
        {
            Name = "expires_at",
            Value = (DateTime.UtcNow + TimeSpan.FromSeconds(requestRefreshTokenAsync.ExpiresIn))
                .ToString("o", CultureInfo.InvariantCulture)
        });

        var authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        authenticateResult.Properties.StoreTokens(authenticationTokens);

        await _httpContextAccessor.HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            authenticateResult.Principal!,
            authenticateResult.Properties);

        Response.Redirect(redirectUri);
    }
}