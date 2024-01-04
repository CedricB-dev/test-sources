// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.RazorPages;
// using OpenIddict.Client.AspNetCore;
//
// namespace OpenIddict.Web1.Pages;
//
// public class Login : PageModel
// {
//     public IActionResult OnGet(string redirectUri)
//     {
//         if (string.IsNullOrWhiteSpace(redirectUri))
//         {
//             redirectUri = Url.Content("~/");
//         }
//
//         // If user is already logged in, we can redirect directly...
//         if (HttpContext.User.Identity.IsAuthenticated)
//         {
//             Response.Redirect(redirectUri);
//         }
//
//         return Challenge(
//             new AuthenticationProperties
//             {
//                 RedirectUri = redirectUri
//             }, OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
//     }
// }