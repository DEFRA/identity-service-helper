// <copyright file="CphDelegationsService.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Delegations;

using Microsoft.Extensions.Logging;

public partial class CphDelegationsService
{
    [LoggerMessage(LogLevel.Information, "Getting all delegations")]
    partial void LogGettingAllDelegations();

    [LoggerMessage(LogLevel.Information, "Getting delegation by id {Id}")]
    partial void LogGettingDelegationById(Guid id);

    [LoggerMessage(LogLevel.Warning, "Delegation with id {Id} not found")]
    partial void LogDelegationWithIdNotFound(Guid id);

    [LoggerMessage(LogLevel.Information, "Deleting delegation with id {Id} by operator {OperatorId}")]
    partial void LogDeletingDelegationWithIdByOperatorId(Guid id, Guid operatorId);
}
