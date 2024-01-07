using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace OpenIddict.Web1.Services;

public class HttpContextUserBearerTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessorAccessor;

    public HttpContextUserBearerTokenHandler(
        IHttpContextAccessor httpContextAccessor
        )
    {
        _httpContextAccessorAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        //With OpenIddict
        // var token = await _httpContextAccessorAccessor.HttpContext.GetTokenAsync(
        //     scheme: CookieAuthenticationDefaults.AuthenticationScheme,
        //     tokenName: OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken);
        
        //With OpenIdConnect
        var token = await _httpContextAccessorAccessor.HttpContext
            .GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectParameterNames.AccessToken);
        //
        var expireAt = await _httpContextAccessorAccessor.HttpContext
            .GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme,"expires_at");

        if (DateTimeOffset.Parse(expireAt) < DateTimeOffset.UtcNow.AddSeconds(60))
        {
            var requestPath = _httpContextAccessorAccessor.HttpContext.Request.Path;
            _httpContextAccessorAccessor.HttpContext.Response.Redirect(
                $"/refresh-token?redirectUri={requestPath}");
        }

        request.SetBearerToken(token);
        return await base.SendAsync(request, cancellationToken);
    }
}