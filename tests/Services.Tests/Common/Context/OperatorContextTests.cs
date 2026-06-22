// <copyright file="OperatorContextTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Common.Context;

using Defra.Identity.Models.Requests.Services;
using Defra.Identity.Services.Common.Context;

public class OperatorContextTests
{
    [Fact]
    public void Should_Return_Operator_Id_From_Operator_Id_Service()
    {
        // Arrange
        var operatorIdService = new OperatorIdService();
        var operatorId = Guid.NewGuid();

        operatorIdService.OperatorId = operatorId;

        // Act
        var operatorContext = new OperatorContext(operatorIdService);

        // Assert
        operatorContext.OperatorId.ShouldBe(operatorId);
    }

    [Fact]
    public void Should_Propagate_Invalid_Operation_Exception_When_Operator_Id_Not_Set()
    {
        // Arrange
        var operatorIdService = new OperatorIdService();
        var operatorContext = new OperatorContext(operatorIdService);

        // Act
        var act = () =>
        {
            var x = operatorContext.OperatorId;
            x.ShouldNotBe(Guid.Empty);
        };

        // Assert
        var exception = act.ShouldThrow<InvalidOperationException>();

        exception.Message.ShouldBe("Operator id has not been set");
    }
}
