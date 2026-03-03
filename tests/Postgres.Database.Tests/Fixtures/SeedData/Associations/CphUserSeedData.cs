// <copyright file="CphUserSeedData.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Fixtures.SeedData.Associations;

using Defra.Identity.Postgres.Database.Entities;

public static class CphUserSeedData
{
    public static ApplicationUserAccountHoldingAssignments[] GetCphUserEntities(Guid adminUserId)
        =>
        [
            new()
            {
                CountyParishHoldingId = new Guid("4435a146-d0ac-4260-8a27-c550e0ed9563"),
                UserAccountId = new Guid("42bde7a0-9efe-402a-a7c3-9161be7b00ba"),
                ApplicationId = new Guid("112788f5-4cb5-4acc-a3f5-d8b2b0e20945"),
                RoleId = new Guid("0c15ba2f-b4ba-406a-a0ae-213de64600a9"),
                CreatedById = adminUserId,
                CreatedAt = DateTime.Parse("2026-03-01").ToUniversalTime(),
            },
            new()
            {
                CountyParishHoldingId = new Guid("4435a146-d0ac-4260-8a27-c550e0ed9563"),
                UserAccountId = new Guid("1e21b685-2247-4d96-bf39-f7dc30f356c2"),
                ApplicationId = new Guid("112788f5-4cb5-4acc-a3f5-d8b2b0e20945"),
                RoleId = new Guid("817647b3-d5d2-45e9-8833-df36d8264102"),
                CreatedById = adminUserId,
                CreatedAt = DateTime.Parse("2026-03-02").ToUniversalTime(),
            },
            new()
            {
                CountyParishHoldingId = new Guid("4435a146-d0ac-4260-8a27-c550e0ed9563"),
                UserAccountId = new Guid("83bf35f9-fd59-4c8a-b70a-7d95a1aab2b6"),
                ApplicationId = new Guid("5466ef9b-aa6b-4b7d-9aac-6c6e55a66ab9"),
                RoleId = new Guid("c63207ab-68f7-4613-b94a-492939eb6116"),
                CreatedById = adminUserId,
                CreatedAt = DateTime.Parse("2026-03-03").ToUniversalTime(),
                DeletedAt = DateTime.Parse("2026-03-05").ToUniversalTime(),
            },
            new()
            {
                CountyParishHoldingId = new Guid("4435a146-d0ac-4260-8a27-c550e0ed9563"),
                UserAccountId = new Guid("d1354eb1-dd1c-471e-bd0e-2626e2e21366"),
                ApplicationId = new Guid("5466ef9b-aa6b-4b7d-9aac-6c6e55a66ab9"),
                RoleId = new Guid("306fa0fc-bd1a-45d3-9fef-e6f11a85b601"),
                CreatedById = adminUserId,
                CreatedAt = DateTime.Parse("2026-03-04").ToUniversalTime(),
            },
            new()
            {
                CountyParishHoldingId = new Guid("204459b1-3a07-4e65-9122-91c1699e3d3f"),
                UserAccountId = new Guid("42bde7a0-9efe-402a-a7c3-9161be7b00ba"),
                ApplicationId = new Guid("112788f5-4cb5-4acc-a3f5-d8b2b0e20945"),
                RoleId = new Guid("0c15ba2f-b4ba-406a-a0ae-213de64600a9"),
                CreatedById = adminUserId,
                CreatedAt = DateTime.Parse("2026-03-01").ToUniversalTime(),
            },
            new()
            {
                CountyParishHoldingId = new Guid("204459b1-3a07-4e65-9122-91c1699e3d3f"),
                UserAccountId = new Guid("1e21b685-2247-4d96-bf39-f7dc30f356c2"),
                ApplicationId = new Guid("112788f5-4cb5-4acc-a3f5-d8b2b0e20945"),
                RoleId = new Guid("817647b3-d5d2-45e9-8833-df36d8264102"),
                CreatedById = adminUserId,
                CreatedAt = DateTime.Parse("2026-03-02").ToUniversalTime(),
            },
        ];
}
