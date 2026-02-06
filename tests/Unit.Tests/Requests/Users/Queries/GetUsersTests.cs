// <copyright file="GetUsersTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Unit.Tests.Requests.Users.Queries;

using Defra.Identity.Models.Requests.Users.Queries;
using Shouldly;
using Xunit;

public class GetUsersTests
{
    [Fact]
    public void Default_Status_Should_Be_Active()
    {
        var query = new GetUsers();
        query.Status.ShouldBe("Active");
    }

    [Fact]
    public void Can_Set_Status()
    {
        var query = new GetUsers { Status = "Inactive" };
        query.Status.ShouldBe("Inactive");
    }
}
