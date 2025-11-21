// <copyright file="AuthContext.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Database;

using System.Reflection;
using Defra.Identity.Database.Entities;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Requires.NotNull(modelBuilder);

        modelBuilder.HasDefaultSchema(DatabaseConstants.SchemaName);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.HasPostgresExtension(PostgreExtensions.UuidGenerator);
        modelBuilder.HasPostgresExtension(PostgreExtensions.Citext);
    }
}
