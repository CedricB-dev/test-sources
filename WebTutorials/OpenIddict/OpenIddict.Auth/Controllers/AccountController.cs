using Microsoft.AspNetCore.Mvc;

namespace OpenIddict.Auth.Controllers;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        returnUrl ??= Url.Content("~/");
        var viewModel = new LoginViewModel
        {
            ReturnUrl = returnUrl
        };

        return View(viewModel);
    }
}

public class LoginViewModel
{
    public string Username { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
    public string ReturnUrl { get; set; }
}