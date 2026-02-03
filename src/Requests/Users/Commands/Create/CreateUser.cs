// <copyright file="CreateUser.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Users.Commands.Create;

public class CreateUser : User
{
    public string OperatorId { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}
