// <copyright file="ApplicationMapperTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Common.Mappers;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Services.Common.Mappers;

public class ApplicationMapperTests
{
    [Fact]
    public void ApplicationMapper_ReturnsValidInstance()
    {
        // Arrange
        var application = new Applications
        {
            Id = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
            Name = "Test name",
            TenantName = "Test tenant",
            Description = "Test Description",
            Scopes = "Scope1;Scope2;Scope3",
            Secret = "Test secret",
            RedirectUris = "https://example.com;https://example1.com;https://example2.com",
            CreatedById = Guid.NewGuid(),
            CreatedByUser = new UserAccounts(),
            CreatedAt = DateTime.UtcNow,
            DeletedById = Guid.NewGuid(),
            DeletedByUser = new UserAccounts(),
            DeletedAt = DateTime.UtcNow,
        };

        // Act
        var result = ApplicationMapper.MapApplicationEntityToApplication(application);

        // Assert
        result.ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(application));
    }
}
