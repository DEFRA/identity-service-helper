// <copyright file="RequestHeaderNamesTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Tests;

public class RequestHeaderNamesTests
{
    [Fact]
    public void HeaderNames_Should_Have_Correct_Values()
    {
        RequestHeaderNames.CorrelationId.ShouldBe("x-correlation-id");
        RequestHeaderNames.OperatorId.ShouldBe("x-operator-id");
        RequestHeaderNames.ApiKey.ShouldBe("x-api-key");
    }
}
