// <copyright file="GetUsers.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Users.Queries;

public class GetUsers
{
    public string? Status { get; set; } = "Active";
}
