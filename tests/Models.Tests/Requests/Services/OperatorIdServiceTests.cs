// <copyright file="OperatorIdServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Tests.Requests.Services;

using Defra.Identity.Models.Requests.Services;

public class OperatorIdServiceTests
{
    [Fact]
    public void Set_Should_Throw_Invalid_Operation_Exception_When_OperatorId_Is_Not_Set()
    {
        // Arrange
        var service = new OperatorIdService();

        // Act
        var act = () =>
        {
            var x = service.OperatorId;
            x.ShouldBeNull();
        };

        // Assert
        var exception = act.ShouldThrow<InvalidOperationException>();

        exception.Message.ShouldBe("Operator id has not been set");
    }

    [Fact]
    public void Set_Should_Store_And_Get_Should_Return_OperatorId()
    {
        // Arrange
        var operatorId = Guid.NewGuid();
        var service = new OperatorIdService { OperatorId = operatorId };

        // Act
        var result = service.OperatorId;

        // Assert
        result.ShouldBe(operatorId);
    }

    [Fact]
    public void Set_Should_Throw_Invalid_Operation_Exception_When_OperatorId_Is_Already_Set()
    {
        // Arrange
        var operatorId = Guid.NewGuid();
        var service = new OperatorIdService { OperatorId = operatorId };

        // Act
        Action act = () => service.OperatorId = Guid.NewGuid();

        // Assert
        var exception = act.ShouldThrow<InvalidOperationException>();

        exception.Message.ShouldBe("Operator id is already set");
    }
}
