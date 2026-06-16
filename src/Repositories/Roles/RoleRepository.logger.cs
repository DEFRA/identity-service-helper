// <copyright file="RoleRepository.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Roles;

using Microsoft.Extensions.Logging;

public partial class RoleRepository
{
    [LoggerMessage(LogLevel.Information, "Getting list of roles")]
    static partial void LogGettingAllRoles(ILogger logger);

    [LoggerMessage(LogLevel.Information, "Getting single role")]
    static partial void LogGettingSingleRole(ILogger logger);

    [LoggerMessage(LogLevel.Information, "Creating role")]
    static partial void LogCreatingRole(ILogger logger);
}
