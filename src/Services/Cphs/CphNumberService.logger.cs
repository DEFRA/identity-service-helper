// <copyright file="CphNumberService.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Cphs;

using Microsoft.Extensions.Logging;

public partial class CphNumberService
{
    [LoggerMessage(LogLevel.Information, "Getting county parish holding id by cph number {FormattedCphNumber}")]
    partial void LogGettingCountyParishHoldingIdByCphNumber(string formattedCphNumber);

    [LoggerMessage(LogLevel.Warning, "County parish holding with cph number {FormattedCphNumber} not found")]
    partial void LogCountyParishHoldingWithCphNumberNotFound(string formattedCphNumber);
}
