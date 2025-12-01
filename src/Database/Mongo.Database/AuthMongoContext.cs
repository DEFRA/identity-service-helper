// <copyright file="AuthMongoContext.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Mongo.Database;

using Defra.Identity.Extensions;
using Defra.Identity.Mongo.Database.Documents;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

public class AuthMongoContext : DbContext
{
    public AuthMongoContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Applications> Applications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Applications>().ToCollection(nameof(Applications).ToSnakeCase());
    }
}
