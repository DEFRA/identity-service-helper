// <copyright file="RoleEndpointsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Tests.Endpoints.Roles;

using Defra.Identity.Api.Endpoints.Roles;
using Defra.Identity.Models.Responses.Roles;
using Defra.Identity.Services.Roles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;

public class RoleEndpointsTests
{
    private readonly IRoleService service = Substitute.For<IRoleService>();

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        // Arrange
        var roles = new List<Role> { new() { Id = Guid.NewGuid(), Name = "Test Role", Description = "Test Role", }, };

        service.GetAll(Arg.Any<CancellationToken>()).Returns(roles);

        // Act
        var result = await (Task<IResult>)typeof(RoleEndpoints)
            .GetMethod("GetAllRoute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [service])!;

        // Assert
        result.ShouldBeOfType<Ok<List<Role>>>();
        ((Ok<List<Role>>)result).Value.ShouldBe(roles);
    }
}
