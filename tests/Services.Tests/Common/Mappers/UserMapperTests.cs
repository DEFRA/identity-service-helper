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
            DeletedBy = new UserAccounts(),
            CreatedBy = new UserAccounts(),
        };

        // Act
        var result = UserMapper.MapUserEntityToUser(user);

        // Assert
        result.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.Id.ShouldBe(user.Id),
            x => x.Email.ShouldBe(user.EmailAddress),
            x => x.DisplayName.ShouldBe(user.DisplayName),
            x => x.FirstName.ShouldBe(user.FirstName),
            x => x.LastName.ShouldBe(user.LastName),
            x => x.Active.ShouldBe(user.DeletedBy != null));
    }
}
