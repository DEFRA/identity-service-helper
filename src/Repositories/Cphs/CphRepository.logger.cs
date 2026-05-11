// <copyright file="CphRepository.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Cphs;

using Microsoft.Extensions.Logging;

public partial class CphRepository
{
    [LoggerMessage(LogLevel.Information, "Validating county parish holding reference with id {Id}")]
    static partial void LogValidatingCountyParishHoldingReferenceWithId(ILogger logger, Guid id);

    [LoggerMessage(LogLevel.Information, "Getting single county parish holding")]
    partial void LogGettingSingleCountyParishHolding();

    [LoggerMessage(LogLevel.Information, "Getting list of county parish holdings")]
    partial void LogGettingListOfCountyParishHoldings();

    [LoggerMessage(LogLevel.Information, "Updating county parish holding with id {Id}")]
    partial void LogUpdatingCountyParishHoldingWithId(Guid id);
}
