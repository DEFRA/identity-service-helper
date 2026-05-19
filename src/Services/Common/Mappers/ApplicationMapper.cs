// <copyright file="ApplicationMapper.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Mappers;

using Defra.Identity.Models.Responses.Applications;
using Defra.Identity.Postgres.Database.Entities;

public static class ApplicationMapper
{
    public const string ScopeSeparator = ";";
    public const string RedirectUriSeparator = ";";

    public static Application MapApplicationEntityToApplication(Applications applicationEntity)
    {
        return new Application
        {
            Id = applicationEntity.ClientId,
            Name = applicationEntity.Name,
            TenantName = applicationEntity.TenantName,
            Description = applicationEntity.Description,
            Secret = applicationEntity.Secret,
            Scopes = applicationEntity.Scopes.Split(ScopeSeparator, StringSplitOptions.RemoveEmptyEntries).ToList(),
            RedirectUris = applicationEntity.RedirectUris.Split(RedirectUriSeparator, StringSplitOptions.RemoveEmptyEntries).ToList(),
        };
    }
}
