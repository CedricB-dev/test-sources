using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OpenIddict.Client.AspNetCore;

namespace OpenIddict.Web3.Services;

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
        var token = await _httpContextAccessorAccessor.HttpContext.GetTokenAsync(
            scheme: CookieAuthenticationDefaults.AuthenticationScheme,
            tokenName: OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }
}