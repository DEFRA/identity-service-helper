// <copyright file="CphRepositoryMockingHelper.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Cphs.TestData;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common;
using NSubstitute.Core;

public static class CphRepositoryMockingHelper
{
    public static PagedEntities<CountyParishHoldings> MockGetAllPagedEntitiesResultFromCallInfo(CallInfo callInfo)
    {
        var actualFilter = (Expression<Func<CountyParishHoldings, bool>>)callInfo.Args()[0];
        var actualOrderBy = (Expression<Func<CountyParishHoldings, string>>)callInfo.Args()[3];

        var pageNumber = (int)callInfo.Args()[1];
        var pageSize = (int)callInfo.Args()[2];
        var orderByDescending = (bool)callInfo.Args()[4];

        var compiledFilter = actualFilter.Compile();
        var compiledOrderBy = actualOrderBy.Compile();

        var filteredEntities = GetCphEntities().Where(compiledFilter);
        var orderedEntities = (orderByDescending ? filteredEntities.OrderByDescending(compiledOrderBy) : filteredEntities.OrderBy(compiledOrderBy)).ToList();

        var totalCount = orderedEntities.Count;
        var totalPages = (totalCount + pageSize - 1) / pageSize;

        var pagedEntities = orderedEntities
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedEntities<CountyParishHoldings>(pagedEntities, totalCount, totalPages, pageNumber, pageSize);
    }

    public static PagedEntities<ApplicationUserAccountHoldingAssignments> MockGetAllCphUsersPagedEntitiesResultFromCallInfo(CallInfo callInfo)
    {
        var actualPrimaryFilter = (Expression<Func<CountyParishHoldings, bool>>)callInfo.Args()[0];
        var actualAssociationFilter = (Expression<Func<ApplicationUserAccountHoldingAssignments, bool>>)callInfo.Args()[1];
        var actualOrderBy = (Expression<Func<ApplicationUserAccountHoldingAssignments, string>>)callInfo.Args()[4];

        var pageNumber = (int)callInfo.Args()[2];
        var pageSize = (int)callInfo.Args()[3];
        var orderByDescending = (bool)callInfo.Args()[5];

        var compiledPrimaryFilter = actualPrimaryFilter.Compile();
        var compiledAssociationFilter = actualAssociationFilter.Compile();
        var compiledOrderBy = actualOrderBy.Compile();

        var filteredPrimaryEntity = GetCphEntities().FirstOrDefault(compiledPrimaryFilter);

        if (filteredPrimaryEntity == null)
        {
            return new PagedEntities<ApplicationUserAccountHoldingAssignments>([], 0, 0, pageNumber, pageSize);
        }

        var queryableAssociations = GetCphUserEntities().Where(entity => entity.CountyParishHoldingId == filteredPrimaryEntity.Id);
        var filteredAssociations = queryableAssociations.Where(compiledAssociationFilter);
        var orderedEntities = (orderByDescending ? filteredAssociations.OrderByDescending(compiledOrderBy) : filteredAssociations.OrderBy(compiledOrderBy)).ToList();

        var totalCount = orderedEntities.Count;
        var totalPages = (totalCount + pageSize - 1) / pageSize;

        var pagedEntities = orderedEntities
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedEntities<ApplicationUserAccountHoldingAssignments>(pagedEntities, totalCount, totalPages, pageNumber, pageSize);
    }

    public static CountyParishHoldings? GetSingleMockEntityResultFromCallInfo(CallInfo callInfo)
    {
        var actualFilter = (Expression<Func<CountyParishHoldings, bool>>)callInfo.Args()[0];
        var compiledFilter = actualFilter.Compile();

        var entity = GetCphEntities().FirstOrDefault(compiledFilter);

        return entity;
    }

    private static CountyParishHoldings[] GetCphEntities() =>
    [
        new()
        {
            Id = new Guid("7140056b-b2ee-40d6-9be1-882bdff30cc2"), Identifier = $"44/100/0007", ExpiredAt = null, DeletedAt = null,
        },
        new()
        {
            Id = new Guid("1cd09a5b-6b00-4f30-b03e-8de45130cad6"), Identifier = $"44/100/0001", ExpiredAt = null, DeletedAt = null,
        },
        new()
        {
            Id = new Guid("a4343f59-011c-46dc-a9fe-553923338e0a"), Identifier = $"44/100/0003", ExpiredAt = null, DeletedAt = DateTime.Parse("2026-02-13").ToUniversalTime(),
        },
        new()
        {
            Id = new Guid("e7009d6d-0a29-4e3f-ac0b-7bf0c7497f46"), Identifier = $"44/100/0002", ExpiredAt = DateTime.Parse("2026-02-12").ToUniversalTime(), DeletedAt = null,
        },
        new()
        {
            Id = new Guid("5bc8f1a5-2d44-40b5-93e4-52b613bf099f"), Identifier = $"44/100/0006", ExpiredAt = null, DeletedAt = DateTime.Parse("2026-02-11").ToUniversalTime(),
        },
        new()
        {
            Id = new Guid("77b9c956-2780-4b48-9abc-71bf505466f9"), Identifier = $"44/100/0004", ExpiredAt = null, DeletedAt = null,
        },
        new()
        {
            Id = new Guid("802428bd-0411-451b-b75c-2fb6c037f271"), Identifier = $"44/100/0005", ExpiredAt = DateTime.Parse("2026-02-10").ToUniversalTime(), DeletedAt = null,
        },
    ];

    private static ApplicationUserAccountHoldingAssignments[] GetCphUserEntities() =>
    [
        new()
        {
            Id = new Guid("560ce019-2e6e-4f76-8b86-de302bbceb2e"),
            CountyParishHoldingId = new Guid("7140056b-b2ee-40d6-9be1-882bdff30cc2"),
            UserAccountId = new Guid("95bdde08-b510-40e3-a09d-6d4c48f122b2"),
            UserAccount = new UserAccounts()
            {
                DisplayName = "Test 101", EmailAddress = "test101@test.com",
            },
            ApplicationId = new Guid("0bcd7934-4e18-414a-a6a8-d94d6a45c148"),
            RoleId = new Guid("81b11eb8-2ac7-468f-a80a-cfeb24f70585"),
            DeletedAt = null,
        },
        new()
        {
            Id = new Guid("5d04e4fb-cbf7-4ed3-8bfc-38da192ea4ce"),
            CountyParishHoldingId = new Guid("7140056b-b2ee-40d6-9be1-882bdff30cc2"),
            UserAccountId = new Guid("d686d63e-a9a0-469a-a864-a2c33436f9a7"),
            UserAccount = new UserAccounts()
            {
                DisplayName = "Test 102", EmailAddress = "test102@test.com",
            },
            ApplicationId = new Guid("0bcd7934-4e18-414a-a6a8-d94d6a45c148"),
            RoleId = new Guid("81b11eb8-2ac7-468f-a80a-cfeb24f70585"),
            DeletedAt = null,
        },
        new()
        {
            Id = new Guid("63b0cf7d-ea81-4789-b024-5d8f2c372e2d"),
            CountyParishHoldingId = new Guid("7140056b-b2ee-40d6-9be1-882bdff30cc2"),
            UserAccountId = new Guid("cfa84a29-9442-4b06-aea5-73295b5975c5"),
            UserAccount = new UserAccounts()
            {
                DisplayName = "Test 103", EmailAddress = "test103@test.com",
            },
            ApplicationId = new Guid("0bcd7934-4e18-414a-a6a8-d94d6a45c148"),
            RoleId = new Guid("81b11eb8-2ac7-468f-a80a-cfeb24f70585"),
            DeletedAt = DateTime.Parse("2026-03-03").ToUniversalTime(),
        },
        new()
        {
            Id = new Guid("05425759-8e3c-4800-abba-bc1d77a97a92"),
            CountyParishHoldingId = new Guid("7140056b-b2ee-40d6-9be1-882bdff30cc2"),
            UserAccountId = new Guid("a2b746a7-e733-40d3-a7e8-5f9522deae2b"),
            UserAccount = new UserAccounts()
            {
                DisplayName = "Test 104", EmailAddress = "test104@test.com",
            },
            ApplicationId = new Guid("f81bbbe9-8eba-4a86-8e65-a08348219f06"),
            RoleId = new Guid("42452ec5-8393-4674-8968-f4929be60099"),
            DeletedAt = null,
        },
        new()
        {
            Id = new Guid("439c56e2-7521-4d6b-9106-b10a91805e9f"),
            CountyParishHoldingId = new Guid("1cd09a5b-6b00-4f30-b03e-8de45130cad6"),
            UserAccountId = new Guid("43426677-8dba-46d0-b429-d7192dfeb6f5"),
            UserAccount = new UserAccounts()
            {
                DisplayName = "Test 105", EmailAddress = "test105@test.com",
            },
            ApplicationId = new Guid("97193f21-877d-4806-9f1b-7ba0730245e4"),
            RoleId = new Guid("b49960ce-5c27-451b-b0f0-bdd297a933ef"),
            DeletedAt = null,
        },
        new()
        {
            Id = new Guid("81b3624b-4c2b-4247-be3a-82ae5b76573e"),
            CountyParishHoldingId = new Guid("1cd09a5b-6b00-4f30-b03e-8de45130cad6"),
            UserAccountId = new Guid("75db555e-b686-40ff-abdb-e2683b91feb1"),
            UserAccount = new UserAccounts()
            {
                DisplayName = "Test 106", EmailAddress = "test106@test.com",
            },
            ApplicationId = new Guid("97193f21-877d-4806-9f1b-7ba0730245e4"),
            RoleId = new Guid("b49960ce-5c27-451b-b0f0-bdd297a933ef"),
            DeletedAt = null,
        },
    ];
}
