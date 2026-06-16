// <copyright file="UserMapperTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Common.Mappers;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Services.Common.Mappers;

public class UserMapperTests
{
    [Fact]
    public void UserMapper_ReturnsValidInstance()
    {
        // Arrange
        var user = new UserAccounts
        {
            Id = Guid.NewGuid(),
            EmailAddress = "Test@exmaple.com",
            DisplayName = "Test User",
            FirstName = "Test",
            LastName = "User",
            KrdsId = Guid.NewGuid(),
            SamId = "C123455",
            CreatedById = Guid.NewGuid(),
            CreatedBy = new UserAccounts(),
            CreatedAt = DateTime.UtcNow,
            DeletedById = Guid.NewGuid(),
            DeletedBy = new UserAccounts(),
            DeletedAt = DateTime.UtcNow,
        };

        // Act
        var result = UserMapper.MapUserEntityToUser(user);

        // Assert
        result.ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(user));
    }
}
