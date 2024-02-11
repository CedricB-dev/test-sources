using IdentityModel.AspNetCore.AccessTokenManagement;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Net.Http.Headers;
using OpenIddict.Web2.Authentications;
using OpenIddict.Web2.Components;
using OpenIddict.Web2.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//builder.Services.AddTransient<CookieEvents>();
//builder.Services.AddTransient<OidcEvents>();
builder.Services.AddAccessTokenManagement();
//builder.Services.AddSingleton<IUserAccessTokenStore, ServerSideTokenStore>();

// builder.Services.AddCascadingAuthenticationState();

builder.Services.AddTransient<WeatherService>();
builder.Services.AddUserAccessTokenHttpClient("weather", configureClient: client =>
{
    client.BaseAddress = new Uri("https://localhost:7136/");
    client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
});//.AddHttpMessageHandler<UserAccessTokenHandler>();

builder.Services.AddAuthentication(o =>
    {
        o.DefaultScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
        o.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectDefaults.AuthenticationScheme;
    }).AddCookie(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, opt =>
    {
        //opt.EventsType = typeof(CookieEvents);
    })
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
        //o.ClaimActions.MapJsonKey("sub", ClaimTypes.NameIdentifier);
        o.GetClaimsFromUserInfoEndpoint = true;
        o.SaveTokens = true;
        
        //o.EventsType = typeof(OidcEvents);
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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();