// <copyright file="CphDelegationsRepositoryMockingHelper.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Delegations.TestData;

using Defra.Identity.Postgres.Database.Entities;

public class CphDelegationsRepositoryMockingHelper
{
    public static CountyParishHoldingDelegations[] GetDelegationEntities()
    {
        return
        [
            new CountyParishHoldingDelegations()
            {
                Id = new Guid("219de60c-9d5e-4f5c-a88c-c435cd8423b4"),
                CountyParishHoldingId = new Guid("e9d9b904-0a2f-4c57-9c7f-4db26b4780f0"),
                CountyParishHolding = new CountyParishHoldings()
                {
                    Id = new Guid("e9d9b904-0a2f-4c57-9c7f-4db26b4780f0"), Identifier = "22/001/0001",
                },
                DelegatingUserId = new Guid("9ba5fda3-eca6-4411-b757-ddc298c5b5c5"),
                DelegatingUser = new UserAccounts()
                {
                    Id = new Guid("9ba5fda3-eca6-4411-b757-ddc298c5b5c5"), DisplayName = "Test User 100",
                },
                DelegatedUserId = new Guid("9bb1ab98-f667-4bab-a1d2-707e4f86eb07"),
                DelegatedUser = new UserAccounts()
                {
                    Id = new Guid("9bb1ab98-f667-4bab-a1d2-707e4f86eb07"), DisplayName = "Test User 200",
                },
                DelegatedUserRoleId = new Guid("6ea62ec2-52e8-455c-ae71-667c7be485e1"),
                DelegatedUserRole = new Roles()
                {
                    Id = new Guid("6ea62ec2-52e8-455c-ae71-667c7be485e1"), Name = "Test Role 100",
                },
                DelegatedUserEmail = "test200@test.com",
            },
            new CountyParishHoldingDelegations()
            {
                Id = new Guid("c37048b5-0ccc-4f7f-ac0d-deffb795ad7d"),
                CountyParishHoldingId = new Guid("a8f0c4b2-7614-4a65-93e8-82d9fb9d1caf"),
                CountyParishHolding = new CountyParishHoldings()
                {
                    Id = new Guid("a8f0c4b2-7614-4a65-93e8-82d9fb9d1caf"), Identifier = "22/001/0002",
                },
                DelegatingUserId = new Guid("3ab6cc55-fe03-4f04-81ea-99cca77b3024"),
                DelegatingUser = new UserAccounts()
                {
                    Id = new Guid("3ab6cc55-fe03-4f04-81ea-99cca77b3024"), DisplayName = "Test User 300",
                },
                DelegatedUserId = new Guid("bd150867-5c1f-44ed-aae1-68bcc90a7b65"),
                DelegatedUser = new UserAccounts()
                {
                    Id = new Guid("bd150867-5c1f-44ed-aae1-68bcc90a7b65"), DisplayName = "Test User 400",
                },
                DelegatedUserRoleId = new Guid("34fa4c82-9d68-42fa-9343-d001f684efd2"),
                DelegatedUserRole = new Roles()
                {
                    Id = new Guid("34fa4c82-9d68-42fa-9343-d001f684efd2"), Name = "Test Role 200",
                },
                DelegatedUserEmail = "test400@test.com",
            },
        ];
    }
}
