// <copyright file="PostgreContainerFixture.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Fixtures;

using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures.SeedData;
using Defra.Identity.Postgres.Database.Tests.Fixtures.SeedData.Associations;
using Defra.Identity.Postgres.Database.Tests.Fixtures.SeedData.Primary;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Testcontainers.PostgreSql;

public class PostgreContainerFixture
    : IAsyncLifetime
{
    private readonly PostgreSqlContainer container = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .Build();

    public string ConnectionString => container.GetConnectionString();

    public async ValueTask DisposeAsync()
    {
        await container.StopAsync();
    }

    public async ValueTask InitializeAsync()
    {
        await container.StartAsync();
        var options = new DbContextOptionsBuilder<PostgresDbContext>()
            .UseNpgsql(container.GetConnectionString())
            .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))
            .Options;

        var context = new PostgresDbContext(options);
        await context.Database.MigrateAsync();
        await TearDownData(context);
        await SeedData(context);
    }

    private static async Task TearDownData(PostgresDbContext context)
    {
        var adminUser = await CreateAdminUser(context);

        await DeleteCphUsers(adminUser, context);
        await DeleteApplicationRoles(context);

        await DeleteCphs(context);
        await DeleteRoles(context);
        await DeleteApplications(context);
        await DeleteUsers(context);
    }

    private static async Task SeedData(PostgresDbContext context)
    {
        var adminUser = await CreateAdminUser(context);

        await CreateStandardUsers(context);
        await CreateApplications(adminUser, context);
        await CreateRoles(context);
        await CreateCphs(adminUser, context);

        await CreateApplicationRoles(context);
        await CreateCphUsers(adminUser, context);
    }

    private static async Task<UserAccounts> CreateAdminUser(PostgresDbContext context)
    {
        if (!await context.UserAccounts.AnyAsync(user => user.EmailAddress == UserSeedData.AdminEmailAddress))
        {
            var adminUserEntity = UserSeedData.GetAdminUserEntity();

            await context.UserAccounts.AddAsync(adminUserEntity, TestContext.Current.CancellationToken);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        return await SeedDataQueryHelper.GetAdminUser(context);
    }

    private static async Task CreateStandardUsers(PostgresDbContext context)
    {
        var userAccountEntities = UserSeedData.GetStandardUserEntities();

        foreach (var entity in userAccountEntities)
        {
            await context.UserAccounts.AddAsync(entity, TestContext.Current.CancellationToken);
        }

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    private static async Task DeleteUsers(PostgresDbContext context)
    {
        var userAccountEntities = await context.UserAccounts.ToListAsync();
        context.UserAccounts.RemoveRange(userAccountEntities);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    private static async Task CreateApplications(UserAccounts adminUser, PostgresDbContext context)
    {
        var applicationEntities = ApplicationSeedData.GetApplicationEntities(adminUser.Id);

        foreach (var entity in applicationEntities)
        {
            await context.Applications.AddAsync(entity, TestContext.Current.CancellationToken);
        }

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    private static async Task DeleteApplications(PostgresDbContext context)
    {
        var applicationEntities = await context.Applications.ToListAsync();
        context.Applications.RemoveRange(applicationEntities);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    private static async Task CreateRoles(PostgresDbContext context)
    {
        var roleEntities = RoleSeedData.GetRoleEntities();

        foreach (var entity in roleEntities)
        {
            await context.Roles.AddAsync(entity, TestContext.Current.CancellationToken);
        }

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    private static async Task DeleteRoles(PostgresDbContext context)
    {
        var roleEntities = await context.Roles.ToListAsync();
        context.Roles.RemoveRange(roleEntities);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    private static async Task CreateCphs(UserAccounts adminUser, PostgresDbContext context)
    {
        var cphEntities = CphSeedData.GetCphEntities(adminUser.Id);

        foreach (var entity in cphEntities)
        {
            await context.CountyParishHoldings.AddAsync(entity, TestContext.Current.CancellationToken);
        }

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    private static async Task DeleteCphs(PostgresDbContext context)
    {
        var cphEntities = await context.CountyParishHoldings.ToListAsync();
        context.CountyParishHoldings.RemoveRange(cphEntities);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    private static async Task CreateApplicationRoles(PostgresDbContext context)
    {
        var applicationRoleEntities = ApplicationRoleSeedData.GetApplicationRoleEntities(context);

        foreach (var entity in applicationRoleEntities)
        {
            await context.ApplicationRoles.AddAsync(entity, TestContext.Current.CancellationToken);
        }

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    private static async Task DeleteApplicationRoles(PostgresDbContext context)
    {
        var applicationRoleEntities = await context.ApplicationRoles.ToListAsync();
        context.ApplicationRoles.RemoveRange(applicationRoleEntities);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    private static async Task CreateCphUsers(UserAccounts adminUser, PostgresDbContext context)
    {
        var cphUserEntities = CphUserSeedData.GetCphUserEntities(adminUser.Id);

        foreach (var entity in cphUserEntities)
        {
            var userAccount = await context.UserAccounts.FirstAsync(u => u.Id == entity.UserAccountId, TestContext.Current.CancellationToken);

            userAccount.ApplicationUserAccountHoldingAssignments.Add(entity);
        }

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    private static async Task DeleteCphUsers(UserAccounts adminUser, PostgresDbContext context)
    {
        var userEntities = await context.UserAccounts.Include(userAccounts => userAccounts.ApplicationUserAccountHoldingAssignments).ToListAsync();

        foreach (var userEntityToClear in userEntities)
        {
            foreach (var associationToRemove in userEntityToClear.ApplicationUserAccountHoldingAssignments.ToList())
            {
                userEntityToClear.ApplicationUserAccountHoldingAssignments.Remove(associationToRemove);
            }
        }

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }
}
