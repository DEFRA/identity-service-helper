// <copyright file="BaseTests.Consts.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests;

/// <summary>
/// The base class for the database fixture driven tests.
/// </summary>
public abstract partial class BaseTests
{
    /// <summary>
    /// Gets the admin email address as defined in the changelog/schema/testcontainer.postgresql.sql script.
    /// </summary>
    protected string AdminEmailAddress { get; } = "system.admin@defra.gov.uk";

    /// <summary>
    /// Gets the admin user id as defined in the changelog/schema/testcontainer.postgresql.sql script.
    /// </summary>
    protected Guid AdminUserId { get; } = Guid.Parse("df6990d6-e61a-4e14-aa79-5479dc7b3569");

    /// <summary>
    /// Gets the admin email address as defined in the changelog/schema/testcontainer.postgresql.sql script.
    /// </summary>
    protected string DelegatedEmailAddress { get; } = "delegated@defra.gov.uk";

    /// <summary>
    /// Gets the admin user id as defined in the changelog/schema/testcontainer.postgresql.sql script.
    /// </summary>
    protected Guid DelegatedUserId { get; } = Guid.Parse("cefcc537-1b7b-48bb-9f40-546087e29712");
}
