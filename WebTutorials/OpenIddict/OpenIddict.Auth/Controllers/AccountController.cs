using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Auth.Infrastructure;

namespace OpenIddict.Auth.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    
    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }
    
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
    
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var user = await _userManager.FindByNameAsync(viewModel.Username);
        var result = await _signInManager.PasswordSignInAsync(viewModel.Username, viewModel.Password,
            viewModel.RememberMe, lockoutOnFailure: true);
        
        AuthenticationProperties props = null;
        if(user is not null && result.Succeeded)
        {
            if (viewModel.RememberMe)
            {
                props = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(30)),
                };
            }
            
            var claims = new List<Claim>();
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(OpenIddictConstants.Claims.Role, role)));
            claims.Add(new(OpenIddictConstants.Claims.Name, user.UserName));
            claims.Add(new(OpenIddictConstants.Claims.Subject, user.Id.ToString()));
            var claimsIdentity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme ,claimsPrincipal, props);
            
            return LocalRedirect(viewModel.ReturnUrl);
        }
        
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