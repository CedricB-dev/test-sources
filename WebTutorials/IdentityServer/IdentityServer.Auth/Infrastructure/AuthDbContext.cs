using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Auth.Infrastructure
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("identityUsers");
        }
    }
}