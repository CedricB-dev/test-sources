using Microsoft.AspNetCore.Authentication;
using Microsoft.Net.Http.Headers;
using OpenIddict.Web1.Components;
using OpenIddict.Web1.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMemoryCache();
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

builder.Services.AddHttpClient("identity", client =>
{
    client.BaseAddress = new Uri("https://localhost:7279/");
    client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
});

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