using OpenIddict.Validation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("all", builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});

builder.Services.AddOpenIddict()
    .AddValidation(opt =>
    {
        opt.SetIssuer("https://localhost:7279");
        //opt.AddAudiences("api1");

        opt.UseSystemNetHttp();
        opt.UseAspNetCore();
    });

//.AddCookie
// .AddOpenIdConnect(opt =>
// {
//     opt.Authority = "https://localhost:7279";
//     opt.RequireHttpsMetadata = false;
//     opt.Resource = "ap1";
// });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiReader", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "api.read");
    });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("all");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();