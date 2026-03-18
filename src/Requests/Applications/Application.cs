// <copyright file="Application.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Applications;

public abstract class Application
{
    public Guid Id { get; set; } = Guid.Empty;

    public string Name { get; set; } = string.Empty;

    public string TenantName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<string> Scopes { get; set; } = [];

    public List<string> RedirectUris { get; set; } = [];

    public string Secret { get; set; } = string.Empty;
}
