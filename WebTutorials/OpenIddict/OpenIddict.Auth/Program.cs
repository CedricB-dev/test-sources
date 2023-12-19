using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using OpenIddict.Auth.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("all", builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AuthDbContext>(opt =>
{
    opt.UseOpenIddict();
    opt.UseSqlServer(connectionString);
    opt.EnableSensitiveDataLogging();
});


builder.Services.AddOpenIddict()
    .AddCore(x =>
    {
        x.UseEntityFrameworkCore().UseDbContext<AuthDbContext>();
    })
    .AddServer(x =>
    {
        x.AllowClientCredentialsFlow();
        x.AllowAuthorizationCodeFlow();
        
        x.SetAuthorizationEndpointUris("connect/authorize");
        x.SetTokenEndpointUris("connect/token");
        x.SetUserinfoEndpointUris("connect/userinfo");
        x.SetIntrospectionEndpointUris("connect/introspect");
        
        x.AddDevelopmentEncryptionCertificate();
        x.AddDevelopmentSigningCertificate();

        x.RegisterScopes(
            OpenIddictConstants.Scopes.OpenId,
            OpenIddictConstants.Scopes.Profile,
            OpenIddictConstants.Scopes.Roles,
            // "api.read",
            // "api.write",
            // "api.delete",
            OpenIddictConstants.Scopes.OfflineAccess);
        
        x.RegisterClaims(
            OpenIddictConstants.Claims.Subject,
            OpenIddictConstants.Claims.Name,
            OpenIddictConstants.Claims.FamilyName,
            OpenIddictConstants.Claims.GivenName,
            OpenIddictConstants.Claims.MiddleName,
            OpenIddictConstants.Claims.Nickname,
            OpenIddictConstants.Claims.PreferredUsername,
            OpenIddictConstants.Claims.Profile,
            OpenIddictConstants.Claims.Picture,
            OpenIddictConstants.Claims.Website,
            OpenIddictConstants.Claims.Gender,
            OpenIddictConstants.Claims.Birthdate,
            OpenIddictConstants.Claims.Zoneinfo,
            OpenIddictConstants.Claims.Locale,
            OpenIddictConstants.Claims.UpdatedAt,
            OpenIddictConstants.Claims.Role);
        
        x.UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough()
            .EnableTokenEndpointPassthrough()
            .EnableUserinfoEndpointPassthrough();
    });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("all");

//app.UseHttpsRedirection();
//app.UseAuthorization();
app.UseAuthentication();

app.Run();
