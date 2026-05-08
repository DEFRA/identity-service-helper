// <copyright file="CphDelegationsRepository.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Delegations;

using Microsoft.Extensions.Logging;

public partial class CphDelegationsRepository
{
    [LoggerMessage(LogLevel.Information, "Getting single delegation")]
    partial void LogGettingSingleDelegation();

    [LoggerMessage(LogLevel.Information, "Getting list of county parish holding delegations")]
    partial void LogGettingListOfCountyParishHoldingDelegations();

    [LoggerMessage(LogLevel.Information, "Creating delegation with id {Id}")]
    partial void LogCreatingDelegationWithId(Guid id);

    [LoggerMessage(LogLevel.Information, "Updating delegation with id {Id}")]
    partial void LogUpdatingDelegationWithId(Guid id);

    [LoggerMessage(LogLevel.Information, "Deleting delegation with operator id {OperatorId}")]
    partial void LogDeletingDelegationWithOperatorId(Guid operatorId);

    [LoggerMessage(LogLevel.Warning, "Delegation not found for deletion")]
    partial void LogDelegationNotFoundForDeletion();
}
