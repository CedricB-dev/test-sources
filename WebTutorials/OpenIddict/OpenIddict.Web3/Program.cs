using Microsoft.Net.Http.Headers;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using OpenIddict.Web3.Components;
using OpenIddict.Web3.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<HttpContextUserBearerTokenHandler>();
builder.Services.AddTransient<WeatherService>();

builder.Services.AddHttpClient("weather", client =>
{
    client.BaseAddress = new Uri("https://localhost:7136/");
    client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
}).AddHttpMessageHandler<HttpContextUserBearerTokenHandler>();


builder.Services.AddAuthentication(o =>
{
    o.DefaultScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(opt =>
{
    opt.LoginPath = "/login";
    // opt.LogoutPath = "/logout";
    // opt.ExpireTimeSpan = TimeSpan.FromMinutes(50);
    // opt.SlidingExpiration = false;
});

builder.Services.AddAuthorization();

builder.Services.AddOpenIddict()
    .AddClient(opt =>
    {
        opt.DisableTokenStorage();
        opt.AllowAuthorizationCodeFlow()
            .AllowRefreshTokenFlow();
        
        opt.AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();

        // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
        opt.UseAspNetCore()
            .EnableStatusCodePagesIntegration()
            .EnableRedirectionEndpointPassthrough()
            .EnablePostLogoutRedirectionEndpointPassthrough();

        opt.UseSystemNetHttp();
            //.SetProductInformation(typeof(Startup).Assembly);
        
        opt.AddRegistration(new OpenIddictClientRegistration
        {
            Issuer = new Uri("https://localhost:7279"),
            ClientId = "web-read",
            //ClientSecret = "web-read-secret",
            Scopes = 
            { 
                OpenIddictConstants.Scopes.OpenId,
                OpenIddictConstants.Scopes.Profile,
                OpenIddictConstants.Scopes.Roles,
                "api.read" 
            },
            //ResponseTypes = { ResponseTypes.Code },
            //GrantTypes = { GrantTypes.AuthorizationCode },
            RedirectUri = new Uri("https://localhost:7298/callback/login"),
            PostLogoutRedirectUri = new Uri("https://localhost:7298/callback/logout"),
        });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorPages();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();