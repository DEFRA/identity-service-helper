// <copyright file="BaseTests.Consts.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests;

public abstract partial class BaseTests
{
    /// <summary>
    /// The admin email address as defined in the changelog/schema/testcontainer.postgresql.sql script.
    /// </summary>
    protected readonly string AdminEmailAddress = "system.admin@defra.gov.uk";

    /// <summary>
    /// The admin user id as defined in the changelog/schema/testcontainer.postgresql.sql script.
    /// </summary>
    protected readonly Guid AdminUserId = Guid.Parse("df6990d6-e61a-4e14-aa79-5479dc7b3569");

    /// <summary>
    /// The admin email address as defined in the changelog/schema/testcontainer.postgresql.sql script.
    /// </summary>
    protected readonly string DelegatedEmailAddress = "delegated@defra.gov.uk";

    /// <summary>
    /// The admin user id as defined in the changelog/schema/testcontainer.postgresql.sql script.
    /// </summary>
    protected readonly Guid DelegatedUserId = Guid.Parse("DABCA904-5843-4739-AAF6-EC61E07BE78");
}
