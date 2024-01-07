using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenIddict.Client.AspNetCore;

namespace OpenIddict.Web3.Pages;

public class Login : PageModel
{
    public IActionResult OnGet(string returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            returnUrl = Url.Content("~/");
        }

        // If user is already logged in, we can redirect directly...
        if (HttpContext.User.Identity.IsAuthenticated)
        {
            Response.Redirect(returnUrl);
        }

        return Challenge(
            new AuthenticationProperties
            {
                RedirectUri = returnUrl
            }, OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
    }
}