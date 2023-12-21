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
        var cblUser = "cedric.blouin.dev@gmail.com";
        var password = "Cbl!123!Cbl";

        var dbContext = serviceScope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var roleOrNothing = dbContext.Roles.FirstOrDefault(x => x.Name == adminRole);

        if (roleOrNothing is null)
        {
            var roleResult = await roleManager.CreateAsync(new ApplicationRole
            {
                Name = adminRole
            });
            Log.Debug("Roles being populated");
        }
        else
        {
            Log.Debug("Roles already populated");
        }

        var userOrNothing = dbContext.Users.FirstOrDefault(x => x.UserName == cblUser);

        if (userOrNothing is null)
        {
            var user = new ApplicationUser
            {
                UserName = cblUser
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

        if (await applicationManager.FindByClientIdAsync("console-reader") is null)
        {
            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = "console-reader",
                ClientSecret = "console-read-secret",
                ClientType = OpenIddictConstants.ClientTypes.Confidential,
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.Endpoints.Introspection,
                    OpenIddictConstants.Permissions.Endpoints.Revocation,

                    OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                    OpenIddictConstants.Permissions.Prefixes.Scope + "api.read"
                }
            };

            await applicationManager.CreateAsync(descriptor);
        }
    }

    private static async Task InitScopes(IServiceScope scope)
    {
        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        if (await scopeManager.FindByNameAsync("api") is null)
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