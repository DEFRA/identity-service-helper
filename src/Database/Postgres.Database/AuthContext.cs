// <copyright file="AuthContext.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database;

using System.Reflection;
using Defra.Identity.Postgres.Database.Entities.Base;

/// <summary>
/// The Authorisation DbContext.
/// </summary>
/// <param name="options">options to apply to the context.</param>
public class AuthContext(DbContextOptions<AuthContext> options)
    : DbContext(options)
{
    /// <summary>
    /// The Applications DbSet.
    /// </summary>
    public virtual DbSet<Application> Applications { get; set; }

    /// <summary>
    /// The Federations DbSet.
    /// </summary>
    public virtual DbSet<Federation> Federations { get; set; }

    /// <summary>
    /// The Enrolments DbSet.
    /// </summary>
    public virtual DbSet<Delegation> Delegations{ get; set; }

    /// <summary>
    /// The KrdsSyncLogs DbSet.
    /// </summary>
    public virtual DbSet<KrdsSyncLog> KrdsSyncLogs { get; set; }

    /// <summary>
    /// The Users DbSet.
    /// </summary>
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
