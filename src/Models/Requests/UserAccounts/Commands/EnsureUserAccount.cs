// <copyright file="EnsureUserAccount.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.UserAccounts.Commands;

using System.ComponentModel;

public class EnsureUserAccount
{
    [Description(OpenApiMetadata.UserAccounts.Subject)]
    public string Sub { get; init; } = string.Empty;

    [Description(OpenApiMetadata.Users.Email)]
    public string Email { get; init; } = string.Empty;

    [Description(OpenApiMetadata.UserAccounts.GivenName)]
    public string GivenName { get; init; } = string.Empty;

    [Description(OpenApiMetadata.UserAccounts.FamilyName)]
    public string FamilyName { get; init; } = string.Empty;
}
