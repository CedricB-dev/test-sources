using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using OpenIddict.Auth.Infrastructure;

namespace OpenIddict.Auth;

public class Startup
{
    public IWebHostEnvironment Environment { get; }
    public IConfiguration Configuration { get; }

    public Startup(IWebHostEnvironment environment, IConfiguration configuration)
    {
        Environment = environment;
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();
        
        services.AddCors(options =>
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
        // services.AddEndpointsApiExplorer();
        // services.AddSwaggerGen();

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie();
        
        var connectionString = Configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AuthDbContext>(opt =>
        {
            opt.UseOpenIddict();
            opt.UseSqlServer(connectionString);
            opt.EnableSensitiveDataLogging();
        });

        services.AddIdentity<ApplicationUser, ApplicationRole>(option =>
            {
                option.SignIn.RequireConfirmedEmail = false;
                option.SignIn.RequireConfirmedPhoneNumber = false;
                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                option.Lockout.MaxFailedAccessAttempts = 5;
            }).AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();
        
        // byte[] secretKey = new byte[128]; // 128 octets = 1024 bits
        // using (var random = RandomNumberGenerator.Create())
        // {
        //     random.GetBytes(secretKey);
        // }
        
        services.AddOpenIddict()
            .AddCore(x => { x.UseEntityFrameworkCore().UseDbContext<AuthDbContext>(); })
            // .AddValidation(x =>
            // {
            //     //x.UseLocalServer();
            // })
            .AddServer(x =>
            {
                x.DisableAccessTokenEncryption();
                
                // x.AddSigningKey(new SymmetricSecurityKey(secretKey))
                //     .DisableAccessTokenEncryption();
                
                x.AllowClientCredentialsFlow();
                x.AllowAuthorizationCodeFlow();

                x.SetAuthorizationEndpointUris("connect/authorize");
                x.SetTokenEndpointUris("connect/token");
                x.SetUserinfoEndpointUris("connect/userinfo");
                x.SetIntrospectionEndpointUris("connect/introspect");

                x.AddDevelopmentEncryptionCertificate();
                x.AddDevelopmentSigningCertificate();

                // x.AddEphemeralEncryptionKey()
                //     .AddEphemeralSigningKey();
                
                x.RegisterScopes(
                    OpenIddictConstants.Scopes.OpenId,
                    OpenIddictConstants.Scopes.Profile,
                    OpenIddictConstants.Scopes.Roles,
                    "api.read",
                    "api.write",
                    "api.delete",
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
    }

    public void Configure(IApplicationBuilder app)
    {
        // if (Environment.IsDevelopment())
        // {
        //     app.UseSwagger();
        //     app.UseSwaggerUI();
        // }
        
        app.UseStaticFiles();
        app.UseRouting();
        
        app.UseCors("all");
        
        //app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
        });
    }
}