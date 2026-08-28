// <copyright file="UserAccount.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Responses.UserAccounts;

using System.ComponentModel;

public class UserAccount
{
    [Description(OpenApiMetadata.Users.Id)]
    public required string Id { get; init; }

    [Description(OpenApiMetadata.UserAccounts.Subject)]
    public string? Subject { get; init; }

    [Description(OpenApiMetadata.Users.Email)]
    public required string Email { get; init; }

    [Description(OpenApiMetadata.Users.FirstName)]
    public string? FirstName { get; init; }

    [Description(OpenApiMetadata.Users.LastName)]
    public string? LastName { get; init; }

    [Description(OpenApiMetadata.Users.DisplayName)]
    public string? DisplayName { get; init; }

    [Description(OpenApiMetadata.UserAccounts.CphAssociations)]
    public IReadOnlyCollection<CphAssociation> CphAssociations { get; init; } = [];

    [Description(OpenApiMetadata.UserAccounts.AssociationsRefreshedAt)]
    public DateTimeOffset? AssociationsRefreshedAt { get; init; }
}
