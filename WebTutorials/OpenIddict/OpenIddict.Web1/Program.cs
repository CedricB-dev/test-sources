using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using OpenIddict.Web1.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddAuthentication(o =>
    {
        o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, o =>
    {
        o.RequireHttpsMetadata = false;
        o.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        o.SignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
        o.Authority = "https://localhost:7279";
        o.ClientId = "web-read";
        o.ClientSecret = "web-read-secret";
        o.ResponseType = OpenIdConnectResponseType.Code;
        //o.Scope.Add( OpenIdConnectScope.OpenId);
        o.Scope.Add(OpenIdConnectScope.OpenIdProfile);
        o.Scope.Add( "roles");
        o.Scope.Add("api.read");
        //o.Scope.Add(OpenIdConnectScope.OfflineAccess);
        o.ClaimActions.MapJsonKey("role", "role");
        //o.GetClaimsFromUserInfoEndpoint = true;
        o.SaveTokens = true;
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

app.UseRouting();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();