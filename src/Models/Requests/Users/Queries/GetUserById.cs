// <copyright file="GetUserById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Users.Queries;

public class GetUserById
{
    public Guid Id { get; set; }

    public string? Status { get; set; } = "Active";
}
