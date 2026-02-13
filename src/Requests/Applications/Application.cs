// <copyright file="Application.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Applications;

public abstract class Application
{
    public string Name { get; set; } = string.Empty;

    public Guid ClientId { get; set; }

    public string TenantName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
