// <copyright file="AssignmentMapperTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Common.Mappers;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Services.Common.Mappers;

public class AssignmentMapperTests
{
    [Fact]
    public void AssignmentMapper_ReturnsValidInstance()
    {
        // Arrange
        var cph = new CountyParishHoldings
        {
            Id = Guid.NewGuid(),
            Identifier = "Test identifier",
            CreatedById = Guid.NewGuid(),
            CreatedByUser = new UserAccounts(),
            CreatedAt = DateTime.UtcNow,
            DeletedById = Guid.NewGuid(),
            DeletedByUser = new UserAccounts(),
            DeletedAt = DateTime.UtcNow,
        };

        var role = new Roles { Id = Guid.NewGuid(), Name = "Test role", Description = "Test description", };

        var user = new UserAccounts
        {
            Id = Guid.NewGuid(),
            DisplayName = "Test User",
            FirstName = "Test",
            LastName = "User",
            EmailAddress = "test@example.com",
            CreatedById = Guid.NewGuid(),
            CreatedBy = new UserAccounts(),
            CreatedAt = DateTime.UtcNow,
            DeletedById = Guid.NewGuid(),
            DeletedBy = new UserAccounts(),
            DeletedAt = DateTime.UtcNow,
        };

        var assignment = new UserAccountCountyParishHoldingAssignments
        {
            Id = Guid.NewGuid(),
            CountyParishHoldingId = cph.Id,
            CountyParishHolding = cph,
            UserAccountId = user.Id,
            RoleId = role.Id,
            Role = role,
            UserAccount = user,
            CreatedById = Guid.NewGuid(),
            CreatedByUser = new UserAccounts(),
            CreatedAt = DateTime.UtcNow,
            DeletedById = Guid.NewGuid(),
            DeletedByUser = new UserAccounts(),
            DeletedAt = DateTime.UtcNow,
        };

        // Act
        var result = AssignmentMapper.MapCphAssignmentEntityToCphAssignment(assignment);

        // Assert
        result.ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(assignment));
    }
}
