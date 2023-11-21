// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using IdentityServer.Auth.Infrastructure;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityServer.Auth
{
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

            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AuthDbContext>(opt =>
            {
                opt.UseSqlServer(connectionString);
                opt.EnableSensitiveDataLogging();
            });

            services.AddDbContext<KeysContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            
            services.AddIdentity<ApplicationUser, ApplicationRole>(option =>
                {
                    option.SignIn.RequireConfirmedEmail = false;
                    option.SignIn.RequireConfirmedPhoneNumber = false;
                    option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    option.Lockout.MaxFailedAccessAttempts = 5;
                }).AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();

            var builder = services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                    options.EmitStaticAudienceClaim = true;
                })
                //.AddTestUsers(TestUsers.Users)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName));
                    options.DefaultSchema = "identityConfiguration";
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName));
                    options.DefaultSchema = "identityOperational";
                    options.EnableTokenCleanup = true;
                })
                .AddAspNetIdentity<ApplicationUser>();

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();

            // services.AddAuthentication()
            //     .AddGoogle(options =>
            //     {
            //         options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            //
            //         // register your IdentityServer with Google at https://console.developers.google.com
            //         // enable the Google+ API
            //         // set the redirect URI to https://localhost:5001/signin-google
            //         options.ClientId = "copy client ID from Google here";
            //         options.ClientSecret = "copy client secret from Google here";
            //     });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
        }
    }
}