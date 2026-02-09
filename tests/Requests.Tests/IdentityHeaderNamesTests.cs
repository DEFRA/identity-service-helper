// <copyright file="IdentityHeaderNamesTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Tests;

public class IdentityHeaderNamesTests
{
    [Fact]
    public void HeaderNames_Should_Have_Correct_Values()
    {
        IdentityHeaderNames.CorrelationId.ShouldBe("x-correlation-id");
        IdentityHeaderNames.OperatorId.ShouldBe("x-operator-id");
        IdentityHeaderNames.ApiKey.ShouldBe("x-api-key");
    }
}
