// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using IdentityServer.Auth.Infrastructure;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.EntityFrameworkCore.Migrations;
using Serilog;

namespace IdentityServer.Auth
{
    public class SeedData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();

            // services.AddDbContext<AuthDbContext>(opt =>
            // {
            //     opt.UseSqlServer(connectionString,
            //         sql =>
            //         {
            //             sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName);
            //             sql.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "identityUsers");
            //         });
            // });
            
            // services.AddDbContext<KeysContext>(options =>
            // {
            //     options.UseSqlServer(connectionString, sql =>
            //     {
            //         sql.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "identityKeys");
            //     });
            // });
            
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

            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            //scope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();
            //scope.ServiceProvider.GetService<AuthDbContext>().Database.Migrate();
            //scope.ServiceProvider.GetService<KeysContext>().Database.Migrate();
            
            var configurationDbContext = scope.ServiceProvider.GetService<ConfigurationDbContext>();
            //configurationDbContext.Database.Migrate();
            EnsureSeedData(configurationDbContext);
                
            
            //EnsureSeedData(authDbContext);
        }

        // private static void EnsureSeedData(AuthDbContext context)
        // {
        //     if (!context.Users.Any())
        //     {
        //         Log.Debug("Users being populated");
        //         foreach (var VARIABLE in Config.Users)
        //         {
        //             
        //         }
        //     }
        // }
        
        private static void EnsureSeedData(ConfigurationDbContext context)
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
