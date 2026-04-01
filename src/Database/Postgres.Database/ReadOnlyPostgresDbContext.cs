// <copyright file="ReadOnlyPostgresDbContext.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database;

using System.Reflection;

/// <summary>
/// The Read-Only Authorisation DbContext.
/// </summary>
/// <param name="options">options to apply to the context.</param>
public class ReadOnlyPostgresDbContext(DbContextOptions<ReadOnlyPostgresDbContext> options)
    : DbContext(options)
{
    public virtual DbSet<AnimalSpecies> AnimalSpecies { get; set; }

    public virtual DbSet<Applications> Applications { get; set; }

    public virtual DbSet<KrdsSyncLogs> KrdsSyncLogs { get; set; }

    public virtual DbSet<UserAccounts> UserAccounts { get; set; }

    public virtual DbSet<Roles> Roles { get; set; }

    public virtual DbSet<ApplicationRoles> ApplicationRoles { get; set; }

    public virtual DbSet<CountyParishHoldings> CountyParishHoldings { get; set; }

    public virtual DbSet<CountyParishHoldingDelegations> CountyParishHoldingDelegations { get; set; }

    public override int SaveChanges()
    {
        throw new InvalidOperationException("This context is read-only.");
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("This context is read-only.");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Requires.NotNull(modelBuilder);

        modelBuilder.HasDefaultSchema(DatabaseConstants.SchemaName);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.HasPostgresExtension(PostgreExtensions.PgCrypto);
        modelBuilder.HasPostgresExtension(PostgreExtensions.Citext);
    }
}
