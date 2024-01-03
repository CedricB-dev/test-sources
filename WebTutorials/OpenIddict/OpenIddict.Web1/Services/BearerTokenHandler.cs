using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OpenIddict.Client.AspNetCore;

namespace OpenIddict.Web1.Services;

public class HttpContextUserBearerTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessorAccessor;

    public HttpContextUserBearerTokenHandler(IHttpContextAccessor httpContextAccessor)
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
            .GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme,"access_token");
        
        request.SetBearerToken(token);
        return await base.SendAsync(request, cancellationToken);
    }
}