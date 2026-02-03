// <copyright file="UpdateUser.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Users.Commands.Update;

public class UpdateUser : User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string OperatorId { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}
