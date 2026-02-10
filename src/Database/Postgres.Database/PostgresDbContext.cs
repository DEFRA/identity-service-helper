// <copyright file="PostgresDbContext.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database;

using System.Reflection;
using Defra.Identity.Postgres.Database.Entities.Base;

/// <summary>
/// The Authorisation DbContext.
/// </summary>
/// <param name="options">options to apply to the context.</param>
public class PostgresDbContext(DbContextOptions<PostgresDbContext> options)
    : DbContext(options)
{
    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<Federation> Federations { get; set; }

    public virtual DbSet<Delegation> Delegations { get; set; }

    public virtual DbSet<KrdsSyncLog> KrdsSyncLogs { get; set; }

    public virtual DbSet<UserAccount> Users { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ApplicationRole> ApplicationRoles { get; set; }

    public virtual DbSet<CountyParishHolding> CountyParishHoldings { get; set; }

    public virtual DbSet<StatusType> StatusTypes { get; set; }

    public override int SaveChanges()
    {
        SetProcessingDateTimes();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetProcessingDateTimes();

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Requires.NotNull(modelBuilder);

        modelBuilder.HasDefaultSchema(DatabaseConstants.SchemaName);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.HasPostgresExtension(PostgreExtensions.UuidGenerator);
        modelBuilder.HasPostgresExtension(PostgreExtensions.Citext);
    }

    private void SetProcessingDateTimes()
    {
        foreach (var entry in ChangeTracker.Entries<BaseProcessingEntity>()
                     .Where(e => e.State == EntityState.Modified))
        {
            entry.Entity.ProcessedAt = DateTime.UtcNow;
        }

        foreach (var entry in ChangeTracker.Entries<BaseUpdateEntity>()
                     .Where(e => e.State == EntityState.Modified))
        {
            entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}
