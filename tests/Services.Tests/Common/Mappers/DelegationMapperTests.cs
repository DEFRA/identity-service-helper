// <copyright file="DelegationMapperTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Common.Mappers;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Services.Common.Mappers;

public class DelegationMapperTests
{
    [Fact]
    public void DelegationMapper_ReturnsValidInstance()
    {
        // Arrange
        var cph = new CountyParishHoldings() { Id = Guid.NewGuid(), Identifier = "12/123/1234", };
        var role = new Roles() { Id = Guid.NewGuid(), Name = "Delegated role", };
        var delegatingUser = new UserAccounts() { Id = Guid.NewGuid(), DisplayName = "Delegating user", };
        var delegatedUser = new UserAccounts() { Id = Guid.NewGuid(), DisplayName = "Delegated user", EmailAddress = "test@example.com", };
        var revokeUser = new UserAccounts() { Id = Guid.NewGuid(), DisplayName = "Revoking user", };

        var delegation = new CountyParishHoldingDelegations
        {
            Id = Guid.NewGuid(),
            CountyParishHoldingId = cph.Id,
            CountyParishHolding = cph,
            DelegatingUserId = delegatingUser.Id,
            DelegatingUser = delegatingUser,
            DelegatedUserId = delegatedUser.Id,
            DelegatedUser = delegatedUser,
            DelegatedUserEmail = delegatedUser.EmailAddress,
            DelegatedUserRoleId = role.Id,
            DelegatedUserRole = role,
            InvitationExpiresAt = DateTime.UtcNow,
            InvitationAcceptedAt = DateTime.UtcNow,
            InvitationRejectedAt = DateTime.UtcNow,
            RevokedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow,
            RevokedById = revokeUser.Id,
            RevokedByUser = revokeUser,
            DeletedByUser = new UserAccounts(),
            CreatedByUser = new UserAccounts(),
        };

        // Act
        var result = DelegationMapper.MapCphDelegationEntityToCphDelegation(delegation);

        // Assert
        result.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.Id.ShouldBe(delegation.Id),
            x => x.CountyParishHoldingId.ShouldBe(delegation.CountyParishHolding.Id),
            x => x.CountyParishHoldingNumber.ShouldBe(delegation.CountyParishHolding.Identifier),
            x => x.DelegatingUserId.ShouldBe(delegation.DelegatingUserId),
            x => x.DelegatingUserName.ShouldBe(delegation.DelegatingUser.DisplayName),
            x => x.DelegatedUserId.ShouldBe(delegation.DelegatedUserId),
            x => x.DelegatedUserName.ShouldBe(delegation.DelegatedUser?.DisplayName),
            x => x.DelegatedUserEmail.ShouldBe(delegation.DelegatedUserEmail),
            x => x.DelegatedUserRoleId.ShouldBe(delegation.DelegatedUserRoleId),
            x => x.DelegatedUserRoleName.ShouldBe(delegation.DelegatedUserRole.Name),
            x => x.InvitationExpiresAt.ShouldBe(delegation.InvitationExpiresAt),
            x => x.InvitationAcceptedAt.ShouldBe(delegation.InvitationAcceptedAt),
            x => x.InvitationRejectedAt.ShouldBe(delegation.InvitationRejectedAt),
            x => x.RevokedAt.ShouldBe(delegation.RevokedAt),
            x => x.ExpiresAt.ShouldBe(delegation.ExpiresAt),
            x => x.RevokedById.ShouldBe(delegation.RevokedById),
            x => x.RevokedByName.ShouldBe(delegation.RevokedByUser?.DisplayName));
    }
}
