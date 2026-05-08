// <copyright file="CphDelegationsForDelegateRepository.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Delegations;

using Microsoft.Extensions.Logging;

public partial class CphDelegationsForDelegateRepository
{
    [LoggerMessage(LogLevel.Information, "Getting list of delegations for delegate")]
    partial void LogGettingListOfDelegationsForDelegate();
}
