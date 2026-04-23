// <copyright file="ValidateUser.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Users.Commands;

public class ValidateUser
{
    public string Email { get; set; } = string.Empty;
}
