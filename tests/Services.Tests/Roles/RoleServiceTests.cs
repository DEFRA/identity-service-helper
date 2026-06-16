// <copyright file="RoleServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Roles;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Roles;
using Defra.Identity.Services.Common.Context;
using Defra.Identity.Services.Common.Strategy.Factories;
using Defra.Identity.Services.Roles;
using Defra.Identity.Test.Utilities.Repository;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class RoleServiceTests
{
    private readonly IRoleRepository repository = Substitute.For<IRoleRepository>();

    private readonly IStrategyBuilderFactory<RoleService> strategyBuilderFactory =
        new StrategyBuilderFactory<RoleService>();

    private readonly ILogger<RoleService> logger =
        DefraLoggerExtensions.CreateNSubstituteLogger<RoleService>();

    private readonly IOperatorContext? operatorContext = Substitute.For<IOperatorContext>();

    private readonly SutProvider<RoleService> sut;

    public RoleServiceTests()
    {
        sut = SutProvider<RoleService>.CreateFor(
            _ => new RoleService(
                repository,
                strategyBuilderFactory,
                logger),
            operatorContext);
    }

    [Fact]
    [Description("GetAll returns all roles")]
    public async Task GetAll_Returns_All_Roles()
    {
        // Arrange
        var role1 = TestData.Role.Role1;
        var role2 = TestData.Role.Role2;

        MockRepositoryContext<Roles>.CreateFor(repository).WithData(
        [
            role1,
            role2,
        ]);

        // Act
        var result = await sut.WithoutOperatorId.GetAll(TestContext.Current.CancellationToken);

        // Assert
        result.Count.ShouldBe(2);

        result[0].ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(role1));
        result[1].ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(role2));

        logger.VerifyLogContainsOne(LogLevel.Information, "Executing get all roles [role]");
        logger.VerifyLogContainsOne(LogLevel.Information, "Successfully executed get all roles [role]");
    }
}
