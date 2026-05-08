// <copyright file="PermissionsService.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Permissions;

using Microsoft.Extensions.Logging;

public partial class PermissionsService
{
    [LoggerMessage(LogLevel.Information, "Getting all county parish holding users for id {Id} by page")]
    partial void LogGettingAllCountyParishHoldingUsersForIdByPage(Guid id);

    [LoggerMessage(LogLevel.Warning, "County parish holding with id {Id} not found")]
    partial void LogCountyParishHoldingWithIdNotFound(Guid id);
}
