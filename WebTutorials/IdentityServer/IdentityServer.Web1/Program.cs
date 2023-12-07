using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHttpContextAccessor();
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = _ => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

// Add services to the container.
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
        o.Authority = "https://localhost:5110";
        o.ClientId = "web-read";
        o.ClientSecret = "web-read-secret";
        o.ResponseType = "code";
        o.Scope.Add("openid");
        o.Scope.Add("profile");
        o.Scope.Add("roles");
        o.Scope.Add("api.read");
        o.Scope.Add("offline_access");
        o.ClaimActions.MapJsonKey("role", "role");
        //o.GetClaimsFromUserInfoEndpoint = true;
        o.SaveTokens = true;
    });

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();