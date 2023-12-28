using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;


namespace OpenIddict.Auth.Controllers;

public class AuthorizationController : Controller
{
    private readonly IOpenIddictScopeManager _scopeManager;

    public AuthorizationController(IOpenIddictScopeManager scopeManager)
    {
        _scopeManager = scopeManager;
    }
    
    [HttpPost("~/connect/token")]
    public async Task<IActionResult> Index()
    {
        var request = HttpContext.GetOpenIddictServerRequest();

        if (request is not null && request.IsClientCredentialsGrantType())
        {
            var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            identity.AddClaim(OpenIddictConstants.Claims.Subject, request.ClientId ?? throw new InvalidOperationException());
            identity.AddClaim("test", "abc", OpenIddictConstants.Destinations.AccessToken);
            
            identity.SetScopes(request.GetScopes());
            identity.SetResources(await _scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());
            var claimsPrincipal = new ClaimsPrincipal(identity);
            
            return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        throw new InvalidOperationException("The specified grant type is not supported.");
    }
}