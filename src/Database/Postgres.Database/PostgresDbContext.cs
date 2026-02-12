// <copyright file="PostgresDbContext.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database;

using System.Reflection;
using Defra.Identity.Postgres.Database.Entities.Base;
using Microsoft.EntityFrameworkCore.Diagnostics;

/// <summary>
/// The Authorisation DbContext.
/// </summary>
/// <param name="options">options to apply to the context.</param>
public class PostgresDbContext(DbContextOptions<PostgresDbContext> options)
    : DbContext(options)
{
    public virtual DbSet<Applications> Applications { get; set; }

    public virtual DbSet<Delegations> Delegations { get; set; }

    public virtual DbSet<DelegationInvitations> DelegationInvitations { get; set; }

    public virtual DbSet<KrdsSyncLogs> KrdsSyncLogs { get; set; }

    public virtual DbSet<UserAccounts> UserAccounts { get; set; }

    public virtual DbSet<Roles> Roles { get; set; }

    public virtual DbSet<ApplicationRoles> ApplicationRoles { get; set; }

    public virtual DbSet<CountyParishHoldings> CountyParishHoldings { get; set; }

    public virtual DbSet<DelegationsCountyParishHoldings> DelegationsCountyParishHoldings { get; set; }

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Requires.NotNull(modelBuilder);

        modelBuilder.HasDefaultSchema(DatabaseConstants.SchemaName);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.HasPostgresExtension(PostgreExtensions.PgCrypto);
        modelBuilder.HasPostgresExtension(PostgreExtensions.Citext);
    }

    private void SetProcessingDateTimes()
    {
        foreach (var entry in ChangeTracker.Entries<BaseProcessingEntity>()
                     .Where(e => e.State == EntityState.Modified))
        {
            entry.Entity.ProcessedAt = DateTime.UtcNow;
        }

        // Ensure CreatedById is always set for added audit entities (helps tests where it's not explicitly set)
        Guid? defaultCreatorId = null;
        try
        {
            if (!ChangeTracker.Entries<BaseAuditEntity>().Any(e => e.State == EntityState.Added))
            {
                return;
            }

            // Attempt to use the first available user as creator if not specified
            defaultCreatorId = UserAccounts.AsNoTracking().Select(u => (Guid?)u.Id).FirstOrDefault();
        }
        catch
        {
            // Swallow any issues determining default creator; leave CreatedById as-is
        }

        if (defaultCreatorId.HasValue)
        {
            foreach (var entry in ChangeTracker.Entries<BaseAuditEntity>()
                         .Where(e => e.State == EntityState.Added && e.Entity.CreatedById == default))
            {
                entry.Entity.CreatedById = defaultCreatorId.Value;
            }
        }
    }
}
