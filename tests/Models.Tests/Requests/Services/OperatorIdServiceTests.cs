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
        var service = new OperatorIdService();

        var act = () =>
        {
            var x = service.OperatorId;

            x.ShouldBeNull();
        };

        var exception = act.ShouldThrow<InvalidOperationException>();

        exception.Message.ShouldBe("Operator id has not been set");
    }

    [Fact]
    public void Set_Should_Store_And_Get_Should_Return_OperatorId()
    {
        var operatorId = Guid.NewGuid();

        var service = new OperatorIdService { OperatorId = operatorId };

        var result = service.OperatorId;

        result.ShouldBe(operatorId);
    }

    [Fact]
    public void Set_Should_Throw_Invalid_Operation_Exception_When_OperatorId_Is_Already_Set()
    {
        var operatorId = Guid.NewGuid();

        var service = new OperatorIdService { OperatorId = operatorId };

        Action act = () => service.OperatorId = Guid.NewGuid();

        var exception = act.ShouldThrow<InvalidOperationException>();

        exception.Message.ShouldBe("Operator id is already set");
    }
}
