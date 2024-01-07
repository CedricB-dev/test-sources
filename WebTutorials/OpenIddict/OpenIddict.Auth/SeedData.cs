using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using OpenIddict.Abstractions;
using OpenIddict.Auth.Infrastructure;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace OpenIddict.Auth;

public class SeedData
{
    public static async Task EnsureSeedData(string connectionString)
    {
        var services = new ServiceCollection();
        services.AddLogging(b => b.AddConsole());
        services.AddDbContext<AuthDbContext>(opt =>
        {
            opt.UseOpenIddict();
            opt.EnableSensitiveDataLogging();
            opt.UseSqlServer(connectionString,
                sql =>
                {
                    sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName);
                    sql.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "identityUsers");
                });
        });

        services.AddIdentity<ApplicationUser, ApplicationRole>(option =>
            {
                option.SignIn.RequireConfirmedEmail = false;
                option.SignIn.RequireConfirmedPhoneNumber = false;
                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                option.Lockout.MaxFailedAccessAttempts = 5;
            }).AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();

        services.AddOpenIddict().AddCore(x => { x.UseEntityFrameworkCore().UseDbContext<AuthDbContext>(); });

        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        scope.ServiceProvider.GetService<ILogger>();
        await scope.ServiceProvider.GetRequiredService<AuthDbContext>().Database.MigrateAsync();
        await EnsureApiSeedData(scope);
        await EnsureUserSeedData(scope);
    }

    private static async Task EnsureUserSeedData(IServiceScope serviceScope)
    {
        var adminRole = "Admin";
        var testUser = "test@test.com";
        var password = "Test!123!Test";

        var dbContext = serviceScope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var roleOrNothing = dbContext.Roles.FirstOrDefault(x => x.Name == adminRole);

        if (roleOrNothing is null)
        {
            var roleResult = await roleManager.CreateAsync(new ApplicationRole
            {
                Name = adminRole,
            });
            Log.Debug("Roles being populated");
        }
        else
        {
            Log.Debug("Roles already populated");
        }

        var userOrNothing = dbContext.Users.FirstOrDefault(x => x.UserName == testUser);

        if (userOrNothing is null)
        {
            var user = new ApplicationUser
            {
                UserName = testUser,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var userResult = await userManager.CreateAsync(user, password);
            var roleResult = await userManager.AddToRoleAsync(user, adminRole);
            Log.Debug("Users being populated");
        }
        else
        {
            Log.Debug("Users already populated");
        }
    }

    private static async Task EnsureApiSeedData(IServiceScope scope)
    {
        await InitScopes(scope);
        await InitClients(scope);
    }

    private static async Task InitClients(IServiceScope scope)
    {
        var applicationManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        if (await applicationManager.FindByClientIdAsync("console-read") is null)
        {
            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = "console-read",
                ClientSecret = "console-read-secret",
                ClientType = OpenIddictConstants.ClientTypes.Confidential,
                ConsentType = OpenIddictConstants.ConsentTypes.Implicit,
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.Endpoints.Logout,
                    OpenIddictConstants.Permissions.Endpoints.Introspection,
                    OpenIddictConstants.Permissions.Endpoints.Revocation,

                    OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                    OpenIddictConstants.Permissions.Prefixes.Scope + "api.read"
                }
            };

            await applicationManager.CreateAsync(descriptor);
        }

        if (await applicationManager.FindByClientIdAsync("web-read") is null)
        {
            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = "web-read",
                //ClientSecret = "web-read-secret",
                ConsentType = OpenIddictConstants.ConsentTypes.Implicit,
                ClientType = OpenIddictConstants.ClientTypes.Public,
                RedirectUris = 
                { 
                    new Uri("https://localhost:7298/callback/login") ,
                    new Uri("https://localhost:7170/signin-oidc"),
                    new Uri("https://localhost:7138/signin-oidc")
                },
                PostLogoutRedirectUris =
                {
                    new Uri("https://localhost:7298/callback/logout"),
                    new Uri("https://localhost:7170/signout-callback-oidc"),
                    new Uri("https://localhost:7138/signout-callback-oidc")
                },
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Logout,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.Endpoints.Introspection,
                    OpenIddictConstants.Permissions.Endpoints.Revocation,
                    
                    OpenIddictConstants.Permissions.ResponseTypes.Code,
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                    
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Roles,
                    
                    OpenIddictConstants.Permissions.Prefixes.Scope + "api.read"
                }
            };

            await applicationManager.CreateAsync(descriptor);
        }
        
        if (await applicationManager.FindByClientIdAsync("react-app") is null)
        {
            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = "react-app",
                ClientSecret = "react-app-secret",
                ConsentType = OpenIddictConstants.ConsentTypes.Implicit,
                ClientType = OpenIddictConstants.ClientTypes.Confidential,
                RedirectUris = { new Uri("http://localhost:5173/signin-oidc") },
                PostLogoutRedirectUris = { new Uri("http://localhost:5173/signout-oidc") },
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Logout,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.Endpoints.Introspection,
                    OpenIddictConstants.Permissions.Endpoints.Revocation,
                    
                    OpenIddictConstants.Permissions.ResponseTypes.Code,
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                    
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Roles,
                    
                    OpenIddictConstants.Permissions.Prefixes.Scope + "api.read"
                }
            };

            await applicationManager.CreateAsync(descriptor);
        }
        
        if (await applicationManager.FindByClientIdAsync("insomnia") is null)
        {
            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = "insomnia",
                ClientSecret = "insomnia-secret",
                ConsentType = OpenIddictConstants.ConsentTypes.Implicit,
                ClientType = OpenIddictConstants.ClientTypes.Confidential,
                RedirectUris = { new Uri("https://insomnia.rest") },
                //PostLogoutRedirectUris = { new Uri("https://localhost:7170/signout-callback-oidc") },
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Logout,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.Endpoints.Introspection,
                    OpenIddictConstants.Permissions.Endpoints.Revocation,
                    
                    OpenIddictConstants.Permissions.ResponseTypes.Code,
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                    
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Roles,
                    OpenIddictConstants.Permissions.Prefixes.Scope + "api.read"
                }
            };

            await applicationManager.CreateAsync(descriptor);
        }
    }

    private static async Task InitScopes(IServiceScope scope)
    {
        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        if (await scopeManager.FindByNameAsync("api.read") is null)
        {
            var descriptor = new OpenIddictScopeDescriptor
            {
                Name = "api.read",
                Resources =
                {
                    "api1", "api2"
                }
            };

            await scopeManager.CreateAsync(descriptor);
        }

        if (await scopeManager.FindByNameAsync("api.write") is null)
        {
            var descriptor = new OpenIddictScopeDescriptor
            {
                Name = "api.write",
                Resources =
                {
                    "api1", "api2"
                }
            };

            await scopeManager.CreateAsync(descriptor);
        }

        if (await scopeManager.FindByNameAsync("api.delete") is null)
        {
            var descriptor = new OpenIddictScopeDescriptor
            {
                Name = "api.delete",
                Resources =
                {
                    "api1", "api2"
                }
            };

            await scopeManager.CreateAsync(descriptor);
        }
    }
}