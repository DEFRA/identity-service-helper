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
        var delegatedUser = new UserAccounts()
        {
            Id = Guid.NewGuid(), DisplayName = "Delegated user", EmailAddress = "test@example.com",
        };
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
            InvitationToken = string.Empty,
            RevokedAt = DateTime.UtcNow,
            RevokedById = revokeUser.Id,
            RevokedByUser = revokeUser,
            ExpiresAt = DateTime.UtcNow,
            CreatedById = Guid.NewGuid(),
            CreatedByUser = new UserAccounts(),
            CreatedAt = DateTime.UtcNow,
            DeletedById = Guid.NewGuid(),
            DeletedByUser = new UserAccounts(),
            DeletedAt = DateTime.UtcNow,
        };

        // Act
        var result = DelegationMapper.MapCphDelegationEntityToCphDelegation(delegation);

        // Assert
        result.ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(delegation));
    }
}
