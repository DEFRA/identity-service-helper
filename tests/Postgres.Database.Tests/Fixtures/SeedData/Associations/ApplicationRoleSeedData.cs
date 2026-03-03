// <copyright file="ApplicationRoleSeedData.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Fixtures.SeedData.Associations;

using Defra.Identity.Postgres.Database.Entities;

public static class ApplicationRoleSeedData
{
    public static ApplicationRoles[] GetApplicationRoleEntities(PostgresDbContext context)
        =>
        [
            GetApplicationRoleAssociation(new Guid("112788f5-4cb5-4acc-a3f5-d8b2b0e20945"), new Guid("0c15ba2f-b4ba-406a-a0ae-213de64600a9"), context),
            GetApplicationRoleAssociation(new Guid("112788f5-4cb5-4acc-a3f5-d8b2b0e20945"), new Guid("817647b3-d5d2-45e9-8833-df36d8264102"), context),
            GetApplicationRoleAssociation(new Guid("5466ef9b-aa6b-4b7d-9aac-6c6e55a66ab9"), new Guid("c63207ab-68f7-4613-b94a-492939eb6116"), context),
            GetApplicationRoleAssociation(new Guid("5466ef9b-aa6b-4b7d-9aac-6c6e55a66ab9"), new Guid("306fa0fc-bd1a-45d3-9fef-e6f11a85b601"), context),
        ];

    private static ApplicationRoles GetApplicationRoleAssociation(Guid applicationId, Guid roleId, PostgresDbContext context)
        => new()
        {
            ApplicationId = applicationId,
            RoleId = roleId,
            Applications = context.Applications.Single(app => app.Id == applicationId),
            Roles = context.Roles.Single(role => role.Id == roleId),
        };
}
