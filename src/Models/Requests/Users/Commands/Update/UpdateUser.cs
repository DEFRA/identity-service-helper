// <copyright file="UpdateUser.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Users.Commands.Update;

public class UpdateUser : BaseUser
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string OperatorId { get; set; } = string.Empty;
}
