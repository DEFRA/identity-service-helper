// <copyright file="AuthContext.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Database;

using System.Reflection;
using Defra.Identity.Database.Entities;
using Defra.Identity.Database.Entities.Base;

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
    public virtual DbSet<Enrolment> Enrolments { get; set; }

    /// <summary>
    /// The KrdsSyncLogs DbSet.
    /// </summary>
    public virtual DbSet<KrdsSyncLog> KrdsSyncLogs { get; set; }

    /// <summary>
    /// The Users DbSet.
    /// </summary>
    public virtual DbSet<UserAccount> Users { get; set; }

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
        var processingEntities = ChangeTracker.Entries()
            .Where(e => e is { Entity: BaseProcessingEntity, State: EntityState.Modified })
            .Select(x => x.Entity).Cast<BaseProcessingEntity>().ToList();

        processingEntities.ForEach(entity => { entity.ProcessedAt = DateTime.UtcNow; });

        var updateEntities = ChangeTracker.Entries()
            .Where(e => e is { Entity: BaseUpdateEntity, State: EntityState.Modified })
            .Select(x => x.Entity).Cast<BaseUpdateEntity>().ToList();

        updateEntities.ForEach(entity => { entity.UpdatedAt = DateTime.UtcNow; });
    }
}
