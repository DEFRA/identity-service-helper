// <copyright file="AnimalSpeciesServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Species;

using System.ComponentModel;
using Defra.Identity.Models.Requests.Species.Commands;
using Defra.Identity.Models.Requests.Species.Queries;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Repositories.Species;
using Defra.Identity.Services.Common.Context;
using Defra.Identity.Services.Common.Strategy.Factories;
using Defra.Identity.Services.Species;
using Defra.Identity.Test.Utilities.Assertions;
using Defra.Identity.Test.Utilities.Comparison;
using Defra.Identity.Test.Utilities.Repository;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class AnimalSpeciesServiceTests
{
    private readonly IAnimalSpeciesRepository repository = Substitute.For<IAnimalSpeciesRepository>();

    private readonly IStrategyBuilderFactory<AnimalSpeciesService> strategyBuilderFactory =
        new StrategyBuilderFactory<AnimalSpeciesService>();

    private readonly ILogger<AnimalSpeciesService> logger =
        DefraLoggerExtensions.CreateNSubstituteLogger<AnimalSpeciesService>();

    private readonly IOperatorContext? operatorContext = Substitute.For<IOperatorContext>();

    private readonly SutProvider<AnimalSpeciesService> sut;

    public AnimalSpeciesServiceTests()
    {
        sut = SutProvider<AnimalSpeciesService>.CreateFor(
            context => new AnimalSpeciesService(
                repository,
                context!,
                strategyBuilderFactory,
                logger),
            operatorContext);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("False")]
    [InlineData("false")]
    [InlineData("FALSE")]
    [InlineData("InvalidValue")]
    [Description("GetAll returns all animal species excluding inactive with given IncludeInactive property")]
    public async Task GetAll_Returns_All_AnimalSpecies_Excluding_Inactive_Given_IncludeInactive(string? includeInactive)
    {
        // Arrange
        var request = new GetAllAnimalSpecies() { IncludeInactive = includeInactive };

        MockRepositoryContext<AnimalSpecies>.CreateFor(repository).WithData(
        [
            TestData.Species.AnimalSpecies1Active,
            TestData.Species.AnimalSpecies2Active,
            TestData.Species.AnimalSpecies3Inactive,
        ]);

        // Act
        var result = await sut.WithoutOperatorContext.GetAll(request, TestContext.Current.CancellationToken);

        // Assert
        result.Count.ShouldBe(2);

        result[0].ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(TestData.Species.AnimalSpecies1Active));
        result[1].ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(TestData.Species.AnimalSpecies2Active));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Executing get all animal species, includehidden: false [animal species]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Successfully executed get all animal species, includehidden: false [animal species]");
    }

    [Theory]
    [InlineData("")]
    [InlineData("True")]
    [InlineData("true")]
    [InlineData("TRUE")]
    [Description("GetAll returns all users including inactive with given IncludeInactive property")]
    public async Task GetAll_Returns_All_AnimalSpecies_Including_Inactive_Given_IncludeInactive(string? includeInactive)
    {
        // Arrange
        var request = new GetAllAnimalSpecies() { IncludeInactive = includeInactive };

        MockRepositoryContext<AnimalSpecies>.CreateFor(repository).WithData(
        [
            TestData.Species.AnimalSpecies1Active,
            TestData.Species.AnimalSpecies2Active,
            TestData.Species.AnimalSpecies3Inactive,
        ]);

        // Act
        var result = await sut.WithoutOperatorContext.GetAll(request, TestContext.Current.CancellationToken);

        // Assert
        result.Count.ShouldBe(3);

        result[0].ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(TestData.Species.AnimalSpecies1Active));
        result[1].ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(TestData.Species.AnimalSpecies2Active));
        result[2].ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(TestData.Species.AnimalSpecies3Inactive));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Executing get all animal species, includehidden: true [animal species]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Successfully executed get all animal species, includehidden: true [animal species]");
    }

    [Fact]
    [Description("Get returns the requested animal species")]
    public async Task Get_Returns_Requested_AnimalSpecies()
    {
        // Arrange
        var request = new GetAnimalSpeciesById() { Id = TestData.Species.AnimalSpecies2Active.Id };

        MockRepositoryContext<AnimalSpecies>.CreateFor(repository).WithData(
        [
            TestData.Species.AnimalSpecies1Active,
            TestData.Species.AnimalSpecies2Active,
            TestData.Species.AnimalSpecies3Inactive,
        ]);

        // Act
        var result = await sut.WithoutOperatorId.Get(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(TestData.Species.AnimalSpecies2Active));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get animal species [animal species] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed get animal species [animal species] with id {request.Id}");
    }

    [Fact]
    [Description("Get returns the requested inactive animal species")]
    public async Task Get_Returns_Requested_Inactive_AnimalSpecies()
    {
        // Arrange
        var request = new GetAnimalSpeciesById() { Id = TestData.Species.AnimalSpecies3Inactive.Id };

        MockRepositoryContext<AnimalSpecies>.CreateFor(repository).WithData(
        [
            TestData.Species.AnimalSpecies3Inactive,
        ]);

        // Act
        var result = await sut.WithoutOperatorId.Get(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(TestData.Species.AnimalSpecies3Inactive));

        result.IsActive.ShouldBeFalse();

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get animal species [animal species] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed get animal species [animal species] with id {request.Id}");
    }

    [Fact]
    [Description("Get throws not found exception when requested animal species does not exist")]
    public async Task Get_Throws_NotFound_Exception_When_Requested_AnimalSpecies_Does_Not_Exist()
    {
        // Arrange
        var request = new GetAnimalSpeciesById() { Id = "INVALID", };
        var repositoryContext = MockRepositoryContext<AnimalSpecies>.CreateFor(repository);

        // Act
        Func<Task> act = async () =>
            await sut.WithoutOperatorId.Get(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("Animal species not found");

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBeNull();

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get animal species [animal species] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Animal species with id {request.Id} not found");
    }

    [Fact]
    [Description("Toggle updates the IsActive flag to true when currently false")]
    public async Task Toggle_Updates_IsActive_Flag_To_True_When_Currently_False()
    {
        // Arrange
        var animalSpecies3Inactive = TestData.Species.AnimalSpecies3Inactive;
        var originalAnimalSpecies3Inactive = TestData.Species.AnimalSpecies3Inactive;

        var request = new ToggleAnimalSpeciesById() { Id = animalSpecies3Inactive.Id, IsActive = true };

        var repositoryContext = MockRepositoryContext<AnimalSpecies>.CreateFor(repository)
            .WithData(
            [
                animalSpecies3Inactive,
            ]);

        // Act
        await sut.WithOperatorId.Toggle(request, TestContext.Current.CancellationToken);

        // Assert
        repositoryContext.Calls.UpdateCallCount.ShouldBe(1);
        repositoryContext.Calls.LastUpdateResult.ShouldNotBeNull();
        repositoryContext.Calls.LastUpdateResult.ShouldBe(animalSpecies3Inactive);

        EntityComparer.CreateFor(originalAnimalSpecies3Inactive, repositoryContext.Calls.LastUpdateResult)
            .VariancesAtTopLevelWithoutObjectsOrEnumerables
            .ShouldOnlyHaveChanged(nameof(AnimalSpecies.IsActive));

        repositoryContext.Calls.LastUpdateResult.IsActive.ShouldBe(request.IsActive);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing toggle animal species [animal species] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed toggle animal species [animal species] with id {request.Id} by operator {operatorContext!.OperatorId}");
    }

    [Fact]
    [Description("Toggle updates the IsActive flag to true when currently true")]
    public async Task Toggle_Updates_IsActive_Flag_To_True_When_Currently_True()
    {
        // Arrange
        var animalSpecies1Active = TestData.Species.AnimalSpecies1Active;
        var originalAnimalSpecies1Active = TestData.Species.AnimalSpecies1Active;

        var request = new ToggleAnimalSpeciesById() { Id = animalSpecies1Active.Id, IsActive = true };

        var repositoryContext = MockRepositoryContext<AnimalSpecies>.CreateFor(repository)
            .WithData(
            [
                animalSpecies1Active,
            ]);

        // Act
        await sut.WithOperatorId.Toggle(request, TestContext.Current.CancellationToken);

        // Assert
        repositoryContext.Calls.UpdateCallCount.ShouldBe(1);
        repositoryContext.Calls.LastUpdateResult.ShouldNotBeNull();
        repositoryContext.Calls.LastUpdateResult.ShouldBe(animalSpecies1Active);

        EntityComparer.CreateFor(originalAnimalSpecies1Active, repositoryContext.Calls.LastUpdateResult)
            .VariancesAtTopLevelWithoutObjectsOrEnumerables
            .ShouldHaveNoChanges();

        repositoryContext.Calls.LastUpdateResult.IsActive.ShouldBe(request.IsActive);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing toggle animal species [animal species] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed toggle animal species [animal species] with id {request.Id} by operator {operatorContext!.OperatorId}");
    }

    [Fact]
    [Description("Toggle updates the IsActive flag to false when currently true")]
    public async Task Toggle_Updates_IsActive_Flag_To_False_When_Currently_True()
    {
        // Arrange
        var animalSpecies1Active = TestData.Species.AnimalSpecies1Active;
        var originalAnimalSpecies1Active = TestData.Species.AnimalSpecies1Active;

        var request = new ToggleAnimalSpeciesById() { Id = animalSpecies1Active.Id, IsActive = false };

        var repositoryContext = MockRepositoryContext<AnimalSpecies>.CreateFor(repository)
            .WithData(
            [
                animalSpecies1Active,
            ]);

        // Act
        await sut.WithOperatorId.Toggle(request, TestContext.Current.CancellationToken);

        // Assert
        repositoryContext.Calls.UpdateCallCount.ShouldBe(1);
        repositoryContext.Calls.LastUpdateResult.ShouldNotBeNull();
        repositoryContext.Calls.LastUpdateResult.ShouldBe(animalSpecies1Active);

        EntityComparer.CreateFor(originalAnimalSpecies1Active, repositoryContext.Calls.LastUpdateResult)
            .VariancesAtTopLevelWithoutObjectsOrEnumerables
            .ShouldOnlyHaveChanged(nameof(AnimalSpecies.IsActive));

        repositoryContext.Calls.LastUpdateResult.IsActive.ShouldBe(request.IsActive);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing toggle animal species [animal species] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed toggle animal species [animal species] with id {request.Id} by operator {operatorContext!.OperatorId}");
    }

    [Fact]
    [Description("Toggle updates the IsActive flag to false when currently false")]
    public async Task Toggle_Updates_IsActive_Flag_To_False_When_Currently_False()
    {
        // Arrange
        var animalSpecies3Inactive = TestData.Species.AnimalSpecies3Inactive;
        var originalAnimalSpecies3Inactive = TestData.Species.AnimalSpecies3Inactive;

        var request = new ToggleAnimalSpeciesById() { Id = animalSpecies3Inactive.Id, IsActive = false };

        var repositoryContext = MockRepositoryContext<AnimalSpecies>.CreateFor(repository)
            .WithData(
            [
                animalSpecies3Inactive,
            ]);

        // Act
        await sut.WithOperatorId.Toggle(request, TestContext.Current.CancellationToken);

        // Assert
        repositoryContext.Calls.UpdateCallCount.ShouldBe(1);
        repositoryContext.Calls.LastUpdateResult.ShouldNotBeNull();
        repositoryContext.Calls.LastUpdateResult.ShouldBe(animalSpecies3Inactive);

        EntityComparer.CreateFor(originalAnimalSpecies3Inactive, repositoryContext.Calls.LastUpdateResult)
            .VariancesAtTopLevelWithoutObjectsOrEnumerables
            .ShouldHaveNoChanges();

        repositoryContext.Calls.LastUpdateResult.IsActive.ShouldBe(request.IsActive);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing toggle animal species [animal species] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed toggle animal species [animal species] with id {request.Id} by operator {operatorContext!.OperatorId}");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    [Description("Toggle throws not found exception when animal species does not exist")]
    public async Task Toggle_Throws_NotFound_Exception_When_AnimalSpecies_Does_Not_Exist(bool isActive)
    {
        // Arrange
        var request = new ToggleAnimalSpeciesById() { Id = "INVALID", IsActive = isActive };
        var repositoryContext = MockRepositoryContext<AnimalSpecies>.CreateFor(repository);

        // Act
        var act = async () => await sut.WithOperatorId.Toggle(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("Animal species not found");

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBeNull();
        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing toggle animal species [animal species] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Animal species with id {request.Id} not found");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    [Description("Toggle throws invalid operation exception when operator context is not provided")]
    public async Task Toggle_Throws_Invalid_Operation_Exception_When_Operator_Context_Not_Provided(bool isActive)
    {
        // Arrange
        var animalSpecies1Active = TestData.Species.AnimalSpecies1Active;

        var request = new ToggleAnimalSpeciesById() { Id = animalSpecies1Active.Id, IsActive = isActive };

        var repositoryContext = MockRepositoryContext<AnimalSpecies>.CreateFor(repository);

        // Act
        var act = async () =>
            await sut.WithoutOperatorContext.Toggle(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<InvalidOperationException>();

        exception.Message.ShouldContain("Operator context must be provided for this operation");

        repositoryContext.Calls.ShouldHaveNoCalls();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    [Description("Toggle throws unauthorised access exception when operator id is not provided")]
    public async Task Toggle_Throws_Unauthorised_Access_Exception_When_Operator_Id_Not_Provided(bool isActive)
    {
        // Arrange
        var animalSpecies1Active = TestData.Species.AnimalSpecies1Active;

        var request = new ToggleAnimalSpeciesById() { Id = animalSpecies1Active.Id, IsActive = isActive };

        var repositoryContext = MockRepositoryContext<AnimalSpecies>.CreateFor(repository);

        // Act
        var act = async () =>
            await sut.WithoutOperatorId.Toggle(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<UnauthorizedAccessException>();

        exception.Message.ShouldContain("Operator id must be provided for this operation");

        repositoryContext.Calls.ShouldHaveNoCalls();
    }
}
