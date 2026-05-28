// <copyright file="PostgresDbContextBase.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database;

public abstract class PostgresDbContextBase<T>(DbContextOptions<T> options) : DbContext(options)
    where T : DbContext
{
    public virtual DbSet<AnimalSpecies> AnimalSpecies { get; set; }

    public virtual DbSet<Applications> Applications { get; set; }

    public virtual DbSet<KrdsSyncLogs> KrdsSyncLogs { get; set; }

    public virtual DbSet<UserAccounts> UserAccounts { get; set; }

    public virtual DbSet<Roles> Roles { get; set; }

    public virtual DbSet<CountyParishHoldings> CountyParishHoldings { get; set; }

    public virtual DbSet<CountyParishHoldingDelegations> CountyParishHoldingDelegations { get; set; }

    public virtual DbSet<UserAccountCountyParishHoldingAssignments> UserAccountCountyParishHoldingAssignments { get; set; }

    public virtual DbSet<CountyParishHoldingDelegationsNotifications> CountyParishHoldingDelegationsNotifications { get; set; }

    public virtual DbSet<CountyParishHoldingAnimalSpecies> CountyParishHoldingAnimalSpecies { get; set; }

    public virtual DbSet<ExternalMessaging> ExternalMessaging { get; set; }
}
