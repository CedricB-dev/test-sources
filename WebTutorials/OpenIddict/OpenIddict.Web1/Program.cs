using Microsoft.AspNetCore.Authentication;
using OpenIddict.Client;
using OpenIddict.Client.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using OpenIddict.Web1.Components;
using OpenIddict.Web1.Services;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();
//builder.Services.AddAccessTokenManagement();
builder.Services.AddTransient<HttpContextUserBearerTokenHandler>();
builder.Services.AddTransient<WeatherService>();
builder.Services.AddHttpClient("weather", client =>
{
    client.BaseAddress = new Uri("https://localhost:7136/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})//.AddUserAccessTokenHandler();
.AddHttpMessageHandler<HttpContextUserBearerTokenHandler>();

//With OpenIdConnect
builder.Services.AddAuthentication(o =>
{
    o.DefaultScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectDefaults.AuthenticationScheme;
}).AddCookie(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectDefaults.AuthenticationScheme, o =>
{
    o.RequireHttpsMetadata = false;
    o.SignInScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
    o.SignOutScheme = Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectDefaults.AuthenticationScheme;
    o.Authority = "https://localhost:7279";
    o.ClientId = "web-read";
    //o.ClientSecret = "web-read-secret";
    o.ResponseType = Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectResponseType.Code;
    o.Scope.Add(Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectScope.OpenIdProfile);
    o.Scope.Add( "roles");
    o.Scope.Add("api.read");
    o.Scope.Add(Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectScope.OfflineAccess);
    o.ClaimActions.MapJsonKey("role", "role");
    o.GetClaimsFromUserInfoEndpoint = true;
    o.SaveTokens = true;
});


//With OpenIddict Client
// builder.Services.AddAuthentication(o =>
// {
//     o.DefaultScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
// }).AddCookie(opt =>
// {
//     opt.LoginPath = "/login";
//     // opt.LogoutPath = "/logout";
//     // opt.ExpireTimeSpan = TimeSpan.FromMinutes(50);
//     // opt.SlidingExpiration = false;
// });
//
// builder.Services.AddAuthorization();
//
// builder.Services.AddOpenIddict()
//     .AddClient(opt =>
//     {
//         opt.DisableTokenStorage();
//         opt.AllowAuthorizationCodeFlow()
//             .AllowRefreshTokenFlow();
//         
//         opt.AddDevelopmentEncryptionCertificate()
//             .AddDevelopmentSigningCertificate();
//
//         // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
//         opt.UseAspNetCore()
//             .EnableStatusCodePagesIntegration()
//             .EnableRedirectionEndpointPassthrough()
//             .EnablePostLogoutRedirectionEndpointPassthrough();
//
//         opt.UseSystemNetHttp();
//             //.SetProductInformation(typeof(Startup).Assembly);
//         
//         opt.AddRegistration(new OpenIddictClientRegistration
//         {
//             Issuer = new Uri("https://localhost:7279"),
//             ClientId = "web-read",
//             //ClientSecret = "web-read-secret",
//             Scopes = { Scopes.OpenId, Scopes.Profile, Scopes.Roles, "api.read" },
//             //ResponseTypes = { ResponseTypes.Code },
//             //GrantTypes = { GrantTypes.AuthorizationCode },
//             RedirectUri = new Uri("https://localhost:7170/callback/login"),
//             PostLogoutRedirectUri = new Uri("https://localhost:7170/signout-callback-oidc")
//         });
//     });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();