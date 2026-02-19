// <copyright file="CphSeedData.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Fixtures.SeedData.Cphs;

using Defra.Identity.Postgres.Database.Entities;

public static class CphSeedData
{
    public static CountyParishHoldings[] GetCphEntities(Guid adminUserId)
        =>
        [
            new()
            {
                Id = new Guid("088967e7-71b8-457a-9001-5b71f24798fd"),
                Identifier = $"44/000/0007",
                CreatedAt = DateTime.Parse("2026-02-07").ToUniversalTime(),
                CreatedById = adminUserId,
                ExpiredAt = DateTime.Parse("2026-02-12").ToUniversalTime(),
                DeletedAt = DateTime.Parse("2026-02-13").ToUniversalTime(),
                DeletedById = adminUserId,
            },
            new()
            {
                Id = new Guid("4435a146-d0ac-4260-8a27-c550e0ed9563"),
                Identifier = $"44/000/0001",
                CreatedAt = DateTime.Parse("2026-02-01").ToUniversalTime(),
                CreatedById = adminUserId,
                ExpiredAt = null,
                DeletedAt = null,
                DeletedById = null,
            },
            new()
            {
                Id = new Guid("1eb0f2fb-a332-4cd5-8a20-02d7adfd7156"),
                Identifier = $"44/000/0003",
                CreatedAt = DateTime.Parse("2026-02-03").ToUniversalTime(),
                CreatedById = adminUserId,
                ExpiredAt = null,
                DeletedAt = null,
                DeletedById = null,
            },
            new()
            {
                Id = new Guid("204459b1-3a07-4e65-9122-91c1699e3d3f"),
                Identifier = $"44/000/0002",
                CreatedAt = DateTime.Parse("2026-02-02").ToUniversalTime(),
                CreatedById = adminUserId,
                ExpiredAt = null,
                DeletedAt = null,
                DeletedById = null,
            },
            new()
            {
                Id = new Guid("82181a8b-7f7f-470c-9263-2b94675599df"),
                Identifier = $"44/000/0006",
                CreatedAt = DateTime.Parse("2026-02-06").ToUniversalTime(),
                CreatedById = adminUserId,
                ExpiredAt = null,
                DeletedAt = DateTime.Parse("2026-02-11").ToUniversalTime(),
                DeletedById = adminUserId,
            },
            new()
            {
                Id = new Guid("02f8043f-510a-41aa-8012-db316ae7fefa"),
                Identifier = $"44/000/0004",
                CreatedAt = DateTime.Parse("2026-02-04").ToUniversalTime(),
                CreatedById = adminUserId,
                ExpiredAt = null,
                DeletedAt = null,
                DeletedById = null,
            },
            new()
            {
                Id = new Guid("7973060a-d483-4ad4-9716-c70415ed620a"),
                Identifier = $"44/000/0005",
                CreatedAt = DateTime.Parse("2026-02-05").ToUniversalTime(),
                CreatedById = adminUserId,
                ExpiredAt = DateTime.Parse("2026-02-10").ToUniversalTime(),
                DeletedAt = null,
                DeletedById = null,
            },
        ];
}
