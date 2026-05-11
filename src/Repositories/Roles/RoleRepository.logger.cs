// <copyright file="RoleRepository.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Roles;

using Microsoft.Extensions.Logging;

public partial class RoleRepository
{
    [LoggerMessage(LogLevel.Information, "Validating county parish holding reference with id {Id}")]
    static partial void LogValidatingCountyParishHoldingReferenceWithId(ILogger logger, Guid id);

    [LoggerMessage(LogLevel.Information, "Getting single role")]
    partial void LogGettingSingleRole();
}
