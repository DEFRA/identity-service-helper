// <copyright file="CphRepositoryMockingHelper.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Cphs.TestData;

using Defra.Identity.Postgres.Database.Entities;

public static class CphRepositoryMockingHelper
{
    public static CountyParishHoldings[] GetCphEntitiesForSimpleFilterChecks() =>
    [
        new()
        {
            Id = new Guid("68625a5c-7999-4394-836f-9ee55cac0a21"), Identifier = $"01/028/0001", ExpiredAt = null, DeletedAt = null,
        },
        new()
        {
            Id = new Guid("335c9de9-a516-4fc6-98ab-8ccfa9f015de"), Identifier = $"01/028/0002", ExpiredAt = null, DeletedAt = DateTime.Parse("2026-02-12").ToUniversalTime(),
        },
    ];

    public static CountyParishHoldings[] GetCphEntities() =>
    [
        new()
        {
            Id = new Guid("7140056b-b2ee-40d6-9be1-882bdff30cc2"), Identifier = "44/100/0007", ExpiredAt = null, DeletedAt = null,
        },
        new()
        {
            Id = new Guid("1cd09a5b-6b00-4f30-b03e-8de45130cad6"), Identifier = "44/100/0001", ExpiredAt = null, DeletedAt = null,
        },
        new()
        {
            Id = new Guid("a4343f59-011c-46dc-a9fe-553923338e0a"), Identifier = "44/100/0003", ExpiredAt = null, DeletedAt = DateTime.Parse("2026-02-13").ToUniversalTime(),
        },
        new()
        {
            Id = new Guid("e7009d6d-0a29-4e3f-ac0b-7bf0c7497f46"), Identifier = "44/100/0002", ExpiredAt = DateTime.Parse("2026-02-12").ToUniversalTime(), DeletedAt = null,
        },
        new()
        {
            Id = new Guid("5bc8f1a5-2d44-40b5-93e4-52b613bf099f"), Identifier = "44/100/0006", ExpiredAt = null, DeletedAt = DateTime.Parse("2026-02-11").ToUniversalTime(),
        },
        new()
        {
            Id = new Guid("77b9c956-2780-4b48-9abc-71bf505466f9"), Identifier = "44/100/0004", ExpiredAt = null, DeletedAt = null,
        },
        new()
        {
            Id = new Guid("802428bd-0411-451b-b75c-2fb6c037f271"), Identifier = "44/100/0005", ExpiredAt = DateTime.Parse("2026-02-10").ToUniversalTime(), DeletedAt = null,
        },
    ];

    public static ApplicationUserAccountHoldingAssignments[] GetCphUserEntities() =>
    [
        new()
        {
            Id = new Guid("560ce019-2e6e-4f76-8b86-de302bbceb2e"),
            CountyParishHoldingId = new Guid("7140056b-b2ee-40d6-9be1-882bdff30cc2"),
            CountyParishHolding = new CountyParishHoldings()
            {
                Identifier = "44/100/0007",
            },
            UserAccountId = new Guid("95bdde08-b510-40e3-a09d-6d4c48f122b2"),
            UserAccount = new UserAccounts()
            {
                DisplayName = "Test 101", EmailAddress = "test101@test.com",
            },
            ApplicationId = new Guid("0bcd7934-4e18-414a-a6a8-d94d6a45c148"),
            RoleId = new Guid("81b11eb8-2ac7-468f-a80a-cfeb24f70585"),
            Role = new Roles()
            {
                Name = "Role 1",
            },
            DeletedAt = null,
        },
        new()
        {
            Id = new Guid("5d04e4fb-cbf7-4ed3-8bfc-38da192ea4ce"),
            CountyParishHoldingId = new Guid("7140056b-b2ee-40d6-9be1-882bdff30cc2"),
            CountyParishHolding = new CountyParishHoldings()
            {
                Identifier = "44/100/0007",
            },
            UserAccountId = new Guid("d686d63e-a9a0-469a-a864-a2c33436f9a7"),
            UserAccount = new UserAccounts()
            {
                DisplayName = "Test 102", EmailAddress = "test102@test.com",
            },
            ApplicationId = new Guid("0bcd7934-4e18-414a-a6a8-d94d6a45c148"),
            RoleId = new Guid("81b11eb8-2ac7-468f-a80a-cfeb24f70585"),
            Role = new Roles()
            {
                Name = "Role 1",
            },
            DeletedAt = null,
        },
        new()
        {
            Id = new Guid("63b0cf7d-ea81-4789-b024-5d8f2c372e2d"),
            CountyParishHoldingId = new Guid("7140056b-b2ee-40d6-9be1-882bdff30cc2"),
            CountyParishHolding = new CountyParishHoldings()
            {
                Identifier = "44/100/0007",
            },
            UserAccountId = new Guid("cfa84a29-9442-4b06-aea5-73295b5975c5"),
            UserAccount = new UserAccounts()
            {
                DisplayName = "Test 103", EmailAddress = "test103@test.com",
            },
            ApplicationId = new Guid("0bcd7934-4e18-414a-a6a8-d94d6a45c148"),
            RoleId = new Guid("81b11eb8-2ac7-468f-a80a-cfeb24f70585"),
            Role = new Roles()
            {
                Name = "Role 1",
            },
            DeletedAt = DateTime.Parse("2026-03-03").ToUniversalTime(),
        },
        new()
        {
            Id = new Guid("05425759-8e3c-4800-abba-bc1d77a97a92"),
            CountyParishHoldingId = new Guid("7140056b-b2ee-40d6-9be1-882bdff30cc2"),
            CountyParishHolding = new CountyParishHoldings()
            {
                Identifier = "44/100/0007",
            },
            UserAccountId = new Guid("a2b746a7-e733-40d3-a7e8-5f9522deae2b"),
            UserAccount = new UserAccounts()
            {
                DisplayName = "Test 104", EmailAddress = "test104@test.com",
            },
            ApplicationId = new Guid("f81bbbe9-8eba-4a86-8e65-a08348219f06"),
            RoleId = new Guid("42452ec5-8393-4674-8968-f4929be60099"),
            Role = new Roles()
            {
                Name = "Role 2",
            },
            DeletedAt = null,
        },
        new()
        {
            Id = new Guid("439c56e2-7521-4d6b-9106-b10a91805e9f"),
            CountyParishHoldingId = new Guid("1cd09a5b-6b00-4f30-b03e-8de45130cad6"),
            CountyParishHolding = new CountyParishHoldings()
            {
                Identifier = "44/100/0001",
            },
            UserAccountId = new Guid("43426677-8dba-46d0-b429-d7192dfeb6f5"),
            UserAccount = new UserAccounts()
            {
                DisplayName = "Test 105", EmailAddress = "test105@test.com",
            },
            ApplicationId = new Guid("97193f21-877d-4806-9f1b-7ba0730245e4"),
            RoleId = new Guid("b49960ce-5c27-451b-b0f0-bdd297a933ef"),
            Role = new Roles()
            {
                Name = "Role 3",
            },
            DeletedAt = null,
        },
        new()
        {
            Id = new Guid("81b3624b-4c2b-4247-be3a-82ae5b76573e"),
            CountyParishHoldingId = new Guid("1cd09a5b-6b00-4f30-b03e-8de45130cad6"),
            CountyParishHolding = new CountyParishHoldings()
            {
                Identifier = "44/100/0001",
            },
            UserAccountId = new Guid("75db555e-b686-40ff-abdb-e2683b91feb1"),
            UserAccount = new UserAccounts()
            {
                DisplayName = "Test 106", EmailAddress = "test106@test.com",
            },
            ApplicationId = new Guid("97193f21-877d-4806-9f1b-7ba0730245e4"),
            RoleId = new Guid("b49960ce-5c27-451b-b0f0-bdd297a933ef"),
            Role = new Roles()
            {
                Name = "Role 3",
            },
            DeletedAt = null,
        },
    ];
}
