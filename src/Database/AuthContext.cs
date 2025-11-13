using System.Reflection;
using Livestock.Auth.Database.Entities;

namespace Livestock.Auth.Database;

using System.Diagnostics.CodeAnalysis;

public class AuthContext(DbContextOptions<AuthContext> options): DbContext(options)
{
    public virtual DbSet<UserAccount> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        Requires.NotNull(modelBuilder);

        modelBuilder.HasDefaultSchema(Constants.SchemaName);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.HasPostgresExtension(PostgreExtensions.UuidGenerator);
        modelBuilder.HasPostgresExtension(PostgreExtensions.Citext);
    }
}