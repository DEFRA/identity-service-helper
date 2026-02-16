// <copyright file="GetUserByIdTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Tests.Users.Queries;

using Defra.Identity.Requests.Users.Queries;

public class GetUserByIdTests
{
    [Fact]
    public void Can_Set_Id_And_Status()
    {
        var id = Guid.NewGuid();
        var query = new GetUserById { Id = id };
        query.Id.ShouldBe(id);
    }
}
