using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Auth.Infrastructure;

public class KeysContext : DbContext, IDataProtectionKeyContext
{
    public KeysContext(DbContextOptions<KeysContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("identityKeys");
    }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = default!;
}