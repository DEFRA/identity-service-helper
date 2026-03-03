// <copyright file="RoleSeedData.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Fixtures.SeedData.Primary;

using Defra.Identity.Postgres.Database.Entities;

public static class RoleSeedData
{
    public static Roles[] GetRoleEntities()
        =>
        [
            new()
            {
                Id = new Guid("0c15ba2f-b4ba-406a-a0ae-213de64600a9"), Name = "test-role-1", Description = "Test Role 1",
            },
            new()
            {
                Id = new Guid("817647b3-d5d2-45e9-8833-df36d8264102"), Name = "test-role-2", Description = "Test Role 2",
            },
            new()
            {
                Id = new Guid("c63207ab-68f7-4613-b94a-492939eb6116"), Name = "test-role-3", Description = "Test Role 3",
            },
            new()
            {
                Id = new Guid("306fa0fc-bd1a-45d3-9fef-e6f11a85b601"), Name = "test-role-4", Description = "Test Role 4",
            },
        ];
}
