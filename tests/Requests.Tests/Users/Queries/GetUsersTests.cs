// <copyright file="GetUsersTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Tests.Users.Queries;

using Defra.Identity.Models.Requests.Users.Queries;

public class GetUsersTests
{
    [Fact]
    public void Default_Status_Should_Be_Active()
    {
        var query = new GetAllUsers();
        query.IncludeInactive.ShouldBeNull();
    }

    [Fact]
    public void Can_Set_Status()
    {
        var query = new GetAllUsers { IncludeInactive = "true" };
        query.IncludeInactive.ShouldBe("true");
    }
}
