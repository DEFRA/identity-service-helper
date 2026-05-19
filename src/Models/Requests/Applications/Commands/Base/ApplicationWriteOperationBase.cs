// <copyright file="ApplicationWriteOperationBase.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Applications.Commands.Base;

using System.ComponentModel;

public abstract class ApplicationWriteOperationBase
{
    [Description(OpenApiMetadata.Applications.Name)]
    public string Name { get; init; } = string.Empty;

    [Description(OpenApiMetadata.Applications.TenantName)]
    public string TenantName { get; init; } = string.Empty;

    [Description(OpenApiMetadata.Applications.Description)]
    public string Description { get; init; } = string.Empty;

    [Description(OpenApiMetadata.Applications.Scopes)]
    public List<string> Scopes { get; init; } = [];

    [Description(OpenApiMetadata.Applications.RedirectUris)]
    public List<string> RedirectUris { get; init; } = [];

    [Description(OpenApiMetadata.Applications.Secret)]
    public string Secret { get; init; } = string.Empty;
}
