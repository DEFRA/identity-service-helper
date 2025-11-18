// <copyright file="AuthContext.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Livestock.Auth.Database;

using System.Reflection;
using Livestock.Auth.Database.Entities;

public class AuthContext(DbContextOptions<AuthContext> options) : DbContext(options)
{
    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<Federation> Federations { get; set; }

    public virtual DbSet<Enrolment> Enrolments { get; set; }

    public virtual DbSet<KrdsSyncLog> KrdsSyncLogs { get; set; }

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
