// <copyright file="ApplicationSeedData.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Fixtures.SeedData.Primary;

using Defra.Identity.Postgres.Database.Entities;

public static class ApplicationSeedData
{
    public static Applications[] GetApplicationEntities(Guid adminUserId)
        =>
        [
            new()
            {
                Id = new Guid("112788f5-4cb5-4acc-a3f5-d8b2b0e20945"),
                Name = "Test Livestock Service 1",
                ClientId = new Guid("df9ab2b8-1f01-4eda-bbdf-13814d91ebb6"),
                TenantName = "Test Tenant 1",
                Description = "Test Description 1",
                CreatedAt = DateTime.Parse("2026-03-01").ToUniversalTime(),
                CreatedById = adminUserId,
            },
            new()
            {
                Id = new Guid("5466ef9b-aa6b-4b7d-9aac-6c6e55a66ab9"),
                Name = "Test Livestock Service 2",
                ClientId = new Guid("543ebe7b-e4cd-4969-9cba-ca8223b0b3c4"),
                TenantName = "Test Tenant 2",
                Description = "Test Description 2",
                CreatedAt = DateTime.Parse("2026-03-02").ToUniversalTime(),
                CreatedById = adminUserId,
            },
        ];
}
