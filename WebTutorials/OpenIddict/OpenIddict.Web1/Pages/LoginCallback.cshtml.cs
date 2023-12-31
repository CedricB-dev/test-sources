using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenIddict.Abstractions;
using OpenIddict.Client.AspNetCore;

namespace OpenIddict.Web1.Components.Pages;

public class LoginCallback : PageModel
{
    public async Task OnGet()
    {
        var result = await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
        
        if (result.Principal is not ClaimsPrincipal { Identity.IsAuthenticated: true })
        {
            throw new InvalidOperationException("The external authorization data cannot be used for authentication.");
        }
        
        var identity = new ClaimsIdentity(result.Principal.Claims ,authenticationType: "ExternalLogin");
        
        var properties = new AuthenticationProperties(result.Properties.Items)
        {
            RedirectUri = result.Properties.RedirectUri ?? "/"
        };
        
        await HttpContext.SignInAsync(new ClaimsPrincipal(identity), properties);
    }
    
    // public async Task OnGet()
    // {
    //     var result = await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
    //     
    //     if (result.Principal is not ClaimsPrincipal { Identity.IsAuthenticated: true })
    //     {
    //         throw new InvalidOperationException("The external authorization data cannot be used for authentication.");
    //     }
    //     
    //      // Build an identity based on the external claims and that will be used to create the authentication cookie.
    //     var identity = new ClaimsIdentity(authenticationType: "ExternalLogin");
    //
    //     
    //     // By default, OpenIddict will automatically try to map the email/name and name identifier claims from
    //     // their standard OpenID Connect or provider-specific equivalent, if available. If needed, additional
    //     // claims can be resolved from the external identity and copied to the final authentication cookie.
    //     identity.SetClaim(ClaimTypes.Email, result.Principal.GetClaim(ClaimTypes.Email))
    //             .SetClaim(ClaimTypes.Name, result.Principal.GetClaim(ClaimTypes.Name))
    //             .SetClaim(ClaimTypes.NameIdentifier, result.Principal.GetClaim(ClaimTypes.NameIdentifier));
    //
    //     // Preserve the registration details to be able to resolve them later.
    //     identity.SetClaim(OpenIddictConstants.Claims.Private.RegistrationId, result.Principal.GetClaim(OpenIddictConstants.Claims.Private.RegistrationId))
    //             .SetClaim(OpenIddictConstants.Claims.Private.ProviderName, result.Principal.GetClaim(OpenIddictConstants.Claims.Private.ProviderName));
    //
    //     // Build the authentication properties based on the properties that were added when the challenge was triggered.
    //     var properties = new AuthenticationProperties(result.Properties.Items)
    //     {
    //         RedirectUri = result.Properties.RedirectUri ?? "/"
    //     };
    //
    //     // If needed, the tokens returned by the authorization server can be stored in the authentication cookie.
    //     //
    //     // To make cookies less heavy, tokens that are not used are filtered out before creating the cookie.
    //     properties.StoreTokens(result.Properties.GetTokens().Where(token => token switch
    //     {
    //         // Preserve the access, identity and refresh tokens returned in the token response, if available.
    //         {
    //             Name: OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken   or
    //                   OpenIddictClientAspNetCoreConstants.Tokens.BackchannelIdentityToken or
    //                   OpenIddictClientAspNetCoreConstants.Tokens.RefreshToken
    //         } => true,
    //
    //         // Ignore the other tokens.
    //         _ => false
    //     }));
    //
    //     // Ask the default sign-in handler to return a new cookie and redirect the
    //     // user agent to the return URL stored in the authentication properties.
    //     //
    //     // For scenarios where the default sign-in handler configured in the ASP.NET Core
    //     // authentication options shouldn't be used, a specific scheme can be specified here.
    //     await HttpContext.SignInAsync(new ClaimsPrincipal(identity), properties);
    // }
}