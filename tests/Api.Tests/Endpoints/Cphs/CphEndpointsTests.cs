// <copyright file="CphEndpointsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Tests.Endpoints.Cphs;

using Defra.Identity.Api.Endpoints.Cphs;
using Defra.Identity.Models.Requests.Cphs.Commands;
using Defra.Identity.Models.Requests.Cphs.Queries;
using Defra.Identity.Models.Responses.Common;
using Defra.Identity.Models.Responses.Cphs;
using Defra.Identity.Services.Cphs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;

public class CphEndpointsTests
{
    private readonly ICphService service = Substitute.For<ICphService>();

    [Fact]
    public async Task GetAllPaged_ReturnsOk()
    {
        // Arrange
        var pagedCphs =
            new PagedResults<Cph>(
                new List<Cph>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        CountyParishHoldingNumber = "22/001/0001",
                        Expired = false,
                        ExpiredAt = null,
                    },
                },
                1,
                1,
                1,
                10);

        var request = new GetCphs
        {
            Expired = "true",
            PageNumber = 1,
            PageSize = 10,
            OrderBy = "CountyParishHoldingNumber",
            OrderByDescending = true,
        };

        service.GetAllPaged(request, Arg.Any<CancellationToken>()).Returns(pagedCphs);

        // Act
        var result = await (Task<IResult>)typeof(CphEndpoints)
            .GetMethod(
                "GetAllPagedRoute",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<Ok<PagedResults<Cph>>>();
        ((Ok<PagedResults<Cph>>)result).Value.ShouldBe(pagedCphs);
    }

    [Fact]
    public async Task Get_ReturnsOk()
    {
        // Arrange
        var cph = new Cph
        {
            Id = Guid.NewGuid(), CountyParishHoldingNumber = "22/001/0001", Expired = false, ExpiredAt = null,
        };

        var request = new GetCphByCphId() { Id = cph.Id, };

        service.Get(request, Arg.Any<CancellationToken>()).Returns(cph);

        // Act
        var result = await (Task<IResult>)typeof(CphEndpoints)
            .GetMethod(
                "GetByIdRoute",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<Ok<Cph>>();
        ((Ok<Cph>)result).Value.ShouldBe(cph);
    }

    [Fact]
    public async Task Expire_ReturnsNoContent()
    {
        // Arrange
        var request = new ExpireCphByCphId() { Id = Guid.NewGuid(), };

        // Act
        var result = await (Task<IResult>)typeof(CphEndpoints)
            .GetMethod(
                "ExpireByIdRoute",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<NoContent>();
        await service.Received(1).Expire(Arg.Any<ExpireCphByCphId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        // Arrange
        var request = new DeleteCphByCphId() { Id = Guid.NewGuid(), };

        // Act
        var result = await (Task<IResult>)typeof(CphEndpoints)
            .GetMethod(
                "DeleteByIdRoute",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<NoContent>();
        await service.Received(1).Delete(Arg.Any<DeleteCphByCphId>(), Arg.Any<CancellationToken>());
    }
}
