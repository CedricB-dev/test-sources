using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OpenIddict.Web1.Components.Pages;

public class Login : PageModel
{
    public async Task<IActionResult> OnGetAsync(string redirectUri)
    {
        await Task.CompletedTask;

        if (string.IsNullOrWhiteSpace(redirectUri))
        {
            redirectUri = Url.Content("~/");
        }

        // If user is already logged in, we can redirect directly...
        if (HttpContext.User.Identity.IsAuthenticated)
        {
            Response.Redirect(redirectUri);
        }

        return Challenge(
            new AuthenticationProperties
            {
                RedirectUri = redirectUri
            },
            OpenIdConnectDefaults.AuthenticationScheme);
    }
}