using IdentityServer.Auth.Infrastructure;using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging(b => b.AddConsole());
serviceCollection.AddDbContext<AuthDbContext>(opt =>
{
    opt.UseSqlServer(
        "Data Source=host.docker.internal,1433;Initial Catalog=Test;User ID=sa;Password=Cbl!123!Cbl;Encrypt=False;");
    opt.EnableSensitiveDataLogging();
});
serviceCollection.AddIdentity<ApplicationUser, ApplicationRole>(option =>
    {
        option.SignIn.RequireConfirmedEmail = false;
        option.SignIn.RequireConfirmedPhoneNumber = false;
        option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        option.Lockout.MaxFailedAccessAttempts = 5;
    }).AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

var buildServiceProvider = serviceCollection.BuildServiceProvider();

await CreateUser(buildServiceProvider);

Console.WriteLine("Identity created");



async Task CreateUser(ServiceProvider serviceProvider)
{
    var adminRole = "Admin";
    var cblUser = "cedric.blouin.dev@gmail.com";
    var password = "Cbl!123!Cbl";
    
    var serviceScope = serviceProvider.CreateScope();
    var dbContext = serviceScope.ServiceProvider.GetService<AuthDbContext>()!;
    var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<ApplicationRole>>()!;
    var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>()!;

    var roleOrNothing = dbContext.Roles.FirstOrDefault(x => x.Name == adminRole);

    if (roleOrNothing is null)
    {
        var roleResult = await roleManager.CreateAsync(new ApplicationRole
        {
            Name = adminRole
        });
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
    }
}