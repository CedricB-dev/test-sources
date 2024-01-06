// using IdentityServer4.AccessTokenValidation;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

// builder.Services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
//     .AddIdentityServerAuthentication(options =>
//     {
//         options.Authority = "https://localhost:5110";
//         options.RequireHttpsMetadata = false;
//         options.ApiName = "api1";
//     });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.Authority = "https://localhost:5110";
        opt.RequireHttpsMetadata = false;
        opt.Audience = "api1";
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ClockSkew = TimeSpan.Zero
        };
    });

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