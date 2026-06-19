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
        var operatorIdService = new OperatorIdService();

        var operatorId = Guid.NewGuid();

        operatorIdService.OperatorId = operatorId;

        var operatorContext = new OperatorContext(operatorIdService);

        operatorContext.OperatorId.ShouldBe(operatorId);
    }

    [Fact]
    public void Should_Propagate_Invalid_Operation_Exception_When_Operator_Id_Not_Set()
    {
        var operatorIdService = new OperatorIdService();

        var operatorContext = new OperatorContext(operatorIdService);

        var act = () =>
        {
            var x = operatorContext.OperatorId;

            x.ShouldNotBe(Guid.Empty);
        };

        var exception = act.ShouldThrow<InvalidOperationException>();

        exception.Message.ShouldBe("Operator id has not been set");
    }
}
