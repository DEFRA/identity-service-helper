// <copyright file="Application.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Applications;

using System.ComponentModel;

public class Application
{
    [Description(OpenApiMetadata.Applications.Id)]
    public Guid Id { get; set; } = Guid.Empty;

    [Description(OpenApiMetadata.Applications.Name)]
    public string Name { get; set; } = string.Empty;

    [Description(OpenApiMetadata.Applications.TenantName)]
    public string TenantName { get; set; } = string.Empty;

    [Description(OpenApiMetadata.Applications.Description)]
    public string Description { get; set; } = string.Empty;

    [Description(OpenApiMetadata.Applications.Scopes)]
    public List<string> Scopes { get; set; } = [];

    [Description(OpenApiMetadata.Applications.RedirectUris)]
    public List<string> RedirectUris { get; set; } = [];

    [Description(OpenApiMetadata.Applications.Secret)]
    public string Secret { get; set; } = string.Empty;
}
