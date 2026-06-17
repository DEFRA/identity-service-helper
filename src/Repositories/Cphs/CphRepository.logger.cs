// <copyright file="CphRepository.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Cphs;

using Microsoft.Extensions.Logging;

public partial class CphRepository
{
    [LoggerMessage(LogLevel.Information, "Getting single county parish holding")]
    partial void LogGettingSingleCountyParishHolding();

    [LoggerMessage(LogLevel.Information, "Getting paged list of county parish holdings")]
    partial void LogGettingPagedListOfCountyParishHoldings();

    [LoggerMessage(LogLevel.Information, "Creating county parish holding")]
    partial void LogCreatingCountyParishHolding();

    [LoggerMessage(LogLevel.Information, "Updating county parish holding with id {Id}")]
    partial void LogUpdatingCountyParishHoldingWithId(Guid id);
}
