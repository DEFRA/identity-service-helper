// <copyright file="Application.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Responses.Applications;

public class Application
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string TenantName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<string> Scopes { get; set; } = [];

    public List<string> RedirectUri { get; set; } = [];
}
