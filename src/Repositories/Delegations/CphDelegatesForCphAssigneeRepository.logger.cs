// <copyright file="CphDelegatesForCphAssigneeRepository.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Delegations;

using Microsoft.Extensions.Logging;

public partial class CphDelegatesForCphAssigneeRepository
{
    [LoggerMessage(LogLevel.Information, "Getting list of unique delegates for delegator")]
    partial void LogGettingListOfUniqueDelegatesForDelegator();
}
