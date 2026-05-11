// <copyright file="GetAllUsers.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Users.Queries;

using System.ComponentModel;

public class GetAllUsers
{
    [Description(OpenApiMetadata.IncludeInactive)]
    public string? IncludeInactive { get; set; } = null;
}
