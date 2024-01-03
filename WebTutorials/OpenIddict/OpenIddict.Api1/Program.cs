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

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiReader", policy =>
    {
        policy.RequireAuthenticatedUser();
        //policy.RequireClaim("scope", "api.read");
    });
});


builder.Services.AddOpenIddict()
    .AddValidation(opt =>
    {
        opt.SetIssuer("https://localhost:7279");
        opt.AddAudiences("api1");
        
        opt.UseSystemNetHttp();
        opt.UseAspNetCore();
    });


var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("all");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();