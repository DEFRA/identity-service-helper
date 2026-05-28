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
            CreatedByUser = new UserAccounts(),
            DeletedByUser = new UserAccounts(),
        };

        var role = new Roles
        {
            Id = Guid.NewGuid(),
            Name = "Test role",
            Description = "Test description",
        };

        var user = new UserAccounts
        {
            Id = Guid.NewGuid(),
            EmailAddress = "test@example.com",
            DisplayName = "Test User",
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
            DeletedByUser = new UserAccounts(),
            CreatedByUser = new UserAccounts(),
        };

        // Act
        var result = AssignmentMapper.MapCphAssignmentEntityToCphAssignment(assignment);

        // Assert
        result.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.Id.ShouldBe(assignment.Id),
            x => x.CountyParishHoldingId.ShouldBe(assignment.CountyParishHoldingId),
            x => x.CountyParishHoldingNumber.ShouldBe(assignment.CountyParishHolding.Identifier),
            x => x.UserId.ShouldBe(assignment.UserAccountId),
            x => x.RoleId.ShouldBe(assignment.RoleId),
            x => x.RoleName.ShouldBe(assignment.Role.Name),
            x => x.Email.ShouldBe(assignment.UserAccount.EmailAddress),
            x => x.DisplayName.ShouldBe(assignment.UserAccount.DisplayName));
    }
}
