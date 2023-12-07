// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Auth.Infrastructure;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace IdentityServer.Auth
{
    public class SeedData
    {
        public static async Task EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();

            services.AddLogging(b => b.AddConsole());
            
            services.AddDbContext<AuthDbContext>(opt =>
            {
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
            
            services.AddDbContext<KeysContext>(options =>
            {
                options.UseSqlServer(connectionString, sql =>
                {
                    sql.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "identityKeys");
                });
            });
            
            services.AddConfigurationDbContext(options =>
            {
                options.ConfigureDbContext = db => db.UseSqlServer(connectionString, sql =>
                {
                    sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName);
                    sql.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "identityConfiguration");
                });
                options.DefaultSchema = "identityConfiguration";
            });
            
            services.AddOperationalDbContext(options =>
            {
                options.ConfigureDbContext = db => db.UseSqlServer(connectionString, sql =>
                {
                    sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName);
                    sql.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "identityOperational");
                });
                options.DefaultSchema = "identityOperational";
            });


            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            scope.ServiceProvider.GetService<ILogger>();
            await scope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.MigrateAsync();
            await scope.ServiceProvider.GetService<AuthDbContext>().Database.MigrateAsync();
            await scope.ServiceProvider.GetService<KeysContext>().Database.MigrateAsync();
            
            var configurationDbContext = scope.ServiceProvider.GetService<ConfigurationDbContext>();
            await configurationDbContext.Database.MigrateAsync();
            EnsureApiSeedData(configurationDbContext);
            await EnsureUserSeedData(scope);
        }

        private static async Task EnsureUserSeedData(IServiceScope serviceScope)
        {
            var adminRole = "Admin";
            var cblUser = "cedric.blouin.dev@gmail.com";
            var password = "Cbl!123!Cbl";
            
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
        
        private static void EnsureApiSeedData(ConfigurationDbContext context)
        {
            if (!context.ApiScopes.Any())
            {
                Log.Debug("ApiScopes being populated");
                foreach (var apiScope in Config.ApiScopes.ToList())
                {
                    context.ApiScopes.Add(apiScope.ToEntity());
                }
            }
            else
            {
                Log.Debug("ApiScopes already populated");
            }
            
            if (!context.ApiResources.Any())
            {
                Log.Debug("ApiResources being populated");
                foreach (var resource in Config.ApiResources.ToList())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Log.Debug("ApiResources already populated");
            }
            
            if (!context.IdentityResources.Any())
            {
                Log.Debug("IdentityResources being populated");
                foreach (var resource in Config.IdentityResources.ToList())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Log.Debug("IdentityResources already populated");
            }
            
            if (!context.Clients.Any())
            {
                Log.Debug("Clients being populated");
                foreach (var client in Config.Clients.ToList())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Log.Debug("Clients already populated");
            }
        }
    }
}
