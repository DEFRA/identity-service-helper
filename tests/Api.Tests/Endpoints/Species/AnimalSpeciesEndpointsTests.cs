// <copyright file="AnimalSpeciesEndpointsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Tests.Endpoints.Species;

using Defra.Identity.Api.Endpoints.Species;
using Defra.Identity.Models.Requests.Species.Commands;
using Defra.Identity.Models.Requests.Species.Queries;
using Defra.Identity.Models.Responses.Species;
using Defra.Identity.Services.Species;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;

public class AnimalSpeciesEndpointsTests
{
    private readonly IAnimalSpeciesService service = Substitute.For<IAnimalSpeciesService>();

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        // Arrange
        var species = new List<AnimalSpecies> { new() { Id = "SPC", Name = "Test Species", IsActive = true } };

        var request = new GetAllAnimalSpecies { IncludeInactive = "true" };

        service.GetAll(request, Arg.Any<CancellationToken>()).Returns(species);

        // Act
        var result = await (Task<IResult>)typeof(AnimalSpeciesEndpoints)
            .GetMethod("GetAllRoute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<Ok<List<AnimalSpecies>>>();
        ((Ok<List<AnimalSpecies>>)result).Value.ShouldBe(species);
    }

    [Fact]
    public async Task Get_ReturnsOk()
    {
        // Arrange
        var species = new AnimalSpecies { Id = "SPC", Name = "Test Species", IsActive = true };

        var request = new GetAnimalSpeciesById { Id = "SPC" };

        service.Get(request, Arg.Any<CancellationToken>()).Returns(species);

        // Act
        var result = await (Task<IResult>)typeof(AnimalSpeciesEndpoints)
            .GetMethod(
                "GetByIdRoute",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<Ok<AnimalSpecies>>();
        ((Ok<AnimalSpecies>)result).Value.ShouldBe(species);
    }

    [Fact]
    public async Task Toggle_ReturnsNoContent()
    {
        // Arrange
        var species = new AnimalSpecies { Id = "SPC", Name = "Test Species", IsActive = false };

        var request = new ToggleAnimalSpeciesById { Id = "SPC" };

        service.Toggle(request, Arg.Any<CancellationToken>()).Returns(species);

        // Act
        var result = await (Task<IResult>)typeof(AnimalSpeciesEndpoints)
            .GetMethod(
                "ToggleByIdRoute",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<NoContent>();
        await service.Received(1).Toggle(request, Arg.Any<CancellationToken>());
    }
}
